#include "driver/dac.h"
#include <string>

#define FFT_SAMPLE_COUNT 205
#define SAMPLE_RATE 8000.0  // Hz

bool ledOn = false;
bool toneStarted = false;
bool toneEnded = false;
long toneStartTime = 0;
long toneEndTime = 0;
int maxMag1 = 0, maxMag2 = 0;
int freqIdx1 = -1, freqIdx2 = -1;

float fft_input[FFT_SAMPLE_COUNT];
const float dtmfFreqs[7] = {697, 770, 852, 941, 1209, 1336, 1477};

int activeDtmfFrequencies[7] = {0, 0, 0, 0, 0, 0, 0};
int detectedFrequencyCount = 0;
int detectedFrequencies[2] = {0, 0};

int goertzelMagnitude(const float* samples, int num_samples, float target_freq, float sample_rate) {
    float k = 0.5f + ((num_samples * target_freq) / sample_rate);
    int bin = (int)k;
    float omega = (2.0f * M_PI * bin) / num_samples;
    float sine = sinf(omega);
    float cosine = cosf(omega);
    float coeff = 2.0f * cosine;

    float q0 = 0, q1 = 0, q2 = 0;
    for (int i = 0; i < num_samples; i++) {
        q0 = coeff * q1 - q2 + samples[i];
        q2 = q1;
        q1 = q0;
    }

    float real = (q1 - q2 * cosine);
    float imag = (q2 * sine);
    return sqrt(real * real + imag * imag);
}

bool isPhoneOnHook() {
  bool offHook = analogRead(14) > 64; // if pin 14 is high then phone is off hook. if pin 14 is low then phone is on hook
  if (offHook) {
    if (!ledOn) {
      digitalWrite(21, HIGH);
      ledOn = true;
      delay(50); // debounce
      Serial.println("OFF_HOOK");
    }
  } else {
    if (ledOn) {
      digitalWrite(21, LOW);
      ledOn = false;
      detectedFrequencyCount = 0; // When the phone goes back on hook there is a spike in frequencies which increments detectedFrequencyCount. so reset it
      delay(50); // debounce
      Serial.println("ON_HOOK");
      gpio_pullup_dis(GPIO_NUM_14);
      gpio_pulldown_en(GPIO_NUM_14);
      esp_sleep_enable_ext0_wakeup(GPIO_NUM_14, 1); // wake on HIGH
      esp_deep_sleep_start();
    }
  }
  return !offHook;
}

String getKeypadButtonPressed(int freq1, int freq2) {
  int buttonIdentifier = freq1 + freq2;
  switch (buttonIdentifier) {
    case 1906:
      return "1";
    case 2033:
      return "2";
    case 2174:
      return "3";
    case 1979:
      return "4";
    case 2106:
      return "5";
    case 2247:
      return "6";
    case 2061:
      return "7";
    case 2188:
      return "8";
    case 2329:
      return "9";
    case 2277:
      return "0";
    case 2150:
      return "*";
    case 2418:
      return "#";
  }
}

void playToneSequence(std::vector<int> values) {
  for (int i = 0; i < values.size(); i+=2) {
    ledcAttach(15, 500, 8);
    ledcChangeFrequency(15, values[i], 8);
    ledcWrite(15, 128);
    delay(values[i+1]);
    ledcDetach(15);
  }
  delay(20);
  Serial.println("Sent tone sequence");
}

std::vector<String> splitString(std::string& s, char delimiter) {
  //s.trim();
  std::vector<String> tokens;
  String temp;
  for (int i = 0; i < s.length(); i++) {
    if (s[i] == delimiter) {
      tokens.emplace_back(temp);
      temp = "";
      continue;
    }
    temp += s[i];
  }
  tokens.emplace_back(temp);
  return tokens;
}

std::string serialInputBufferToString(std::vector<char> serialInputBuffer) {
  std::string str(serialInputBuffer.begin(), serialInputBuffer.end());
  return str;
}

int dataLength = 0;
std::vector<char> serialInputBuffer;
void readSerial() {
  char startMarker = '<';
  bool receiving = false;
  String dataLengthString = "";
  int serialInputBufferIndex = 0;
  while (Serial.available() > 0) {
    char receivedChar = Serial.read();
    if (receiving) {
      if (!dataLength) {
        if (receivedChar == '_') {
          dataLength = atoi(dataLengthString.c_str()) - dataLengthString.length() - 1;
          serialInputBuffer.clear();
          serialInputBuffer.resize(dataLength);
          Serial.println("Read: " + String(dataLengthString.length()+1) + ", remaining: " + dataLength);
        } else {
          dataLengthString += receivedChar;
        }
      } else {
        serialInputBuffer[serialInputBufferIndex] = receivedChar;
        serialInputBufferIndex++;
        if (serialInputBufferIndex == dataLength) {
          Serial.println("Read " + String(dataLength) + " bytes");
          return;
        }
      }
    }

    if (receivedChar == startMarker) {
      receiving = true;
    }
  }
}

std::vector<char> dataBuffer;
bool readSerial2() {
  if (Serial.available() == 0) return false;

  // get number of bytes to recieve
  String dataLengthBuffer;
  int dataLength = 0;
  while (Serial.available() > 0) {
    char c = Serial.read();
    if (c == '_') {
      dataLength = atoi(dataLengthBuffer.c_str());
      break;
    } else {
      dataLengthBuffer += c;
    }
  }

  // prepare buffer
  dataBuffer.clear();
  dataBuffer.resize(dataLength);

  // recieve data in packets of 120 bytes
  int bytesReceived = 0;
  while (bytesReceived < dataLength) {
    int available = Serial.available();
    if (!available) continue;
    for (int i = 0; i < available; i++) {
      dataBuffer[bytesReceived] = Serial.read();
      bytesReceived++;
    }
    Serial.println("1"); // notify sender that the packet was received
  }
  return true;
}

String getSerialDataType() {
  String keyword;
  for (int i = 0; i < dataBuffer.size(); i++) {
    char c = dataBuffer[i];
    if (c == '_') return keyword;
    keyword += c;
  }
  return keyword;
}

uint8_t* pcmData;     // Pointer to your PCM buffer
volatile size_t pcmLen;     // Length of PCM buffer
int pcmIndex = 0;
hw_timer_t* timer = NULL;
void IRAM_ATTR onTimer() {
  dac_output_voltage(DAC_CHANNEL_1, pcmData[pcmIndex]);
  pcmIndex++;
  if (pcmIndex >= pcmLen) {
    dac_output_voltage(DAC_CHANNEL_1, 0);
    timerEnd(timer); // Stop when done
    dac_output_disable(DAC_CHANNEL_1);
    Serial.println("Played PCM data");
  }
}

void playPCMData(uint8_t* data, size_t length, uint32_t sampleRate) {
  pcmData = data;
  pcmLen = length;
  pcmIndex = 0;

  dac_output_enable(DAC_CHANNEL_1);

  int timerFrequency = 1000000;
  timer = timerBegin(timerFrequency); // 80 MHz / 80 = 1 MHz tick
  timerAttachInterrupt(timer, &onTimer);
  timerAlarm(timer, timerFrequency / sampleRate, true, 0); // sampleRate Hz
  Serial.println("Playing PCM data");
}

void checkForSerialData() {
  long start = millis();
  bool dataExists = readSerial2();
  long duration = millis() - start;
  if (!dataExists) return;

  Serial.println("Recieved " + String(dataBuffer.size()) + " bytes after " + String(duration) + "ms");
  String datatype = getSerialDataType();
  Serial.println(datatype);
  if (datatype == "TONESEQUENCE") {
    // create tone sequence as 2 vectors with the frequencies and durations
    std::string dataString = serialInputBufferToString(dataBuffer);
    std::vector<String> parts = splitString(dataString, '_');
    std::vector<int> values;
    for (int i = 1; i < parts.size(); i++) {
      int value = parts[i].toInt();
      values.emplace_back(value);
    }
    Serial.println("Sending tones");

    // send the sequence to the phone
    playToneSequence(values);
  }
  if (datatype == "AUDIO") {
    playPCMData((uint8_t*)dataBuffer.data() + 6, dataBuffer.size() - 6, 8000);
    //delayMicroseconds(dataBuffer.size() * 125);
  }
}

void dtmfDetection() {
  // sample the ADC at 8kHz 205 times
  const uint32_t interval_us = 1000000.0 / SAMPLE_RATE;
  for (int i = 0; i < FFT_SAMPLE_COUNT; i++) {
    unsigned long t0 = micros();
    fft_input[i] = analogRead(A4);
    while (micros() - t0 < interval_us);
  }

  // fast fourier transform
  int mags[7] = {0, 0, 0, 0, 0, 0, 0};
  for (int i = 0; i < 7; i++) {
    mags[i] = goertzelMagnitude(fft_input, FFT_SAMPLE_COUNT, dtmfFreqs[i], SAMPLE_RATE);
  }

  int activityThreshold = 5000;

  // if any frequency exceeds the threshold then there is a dtmf tone
  for (int i = 0; i < 7; i++) {
    if (!toneStarted) {
      if (mags[i] > activityThreshold) {
        toneStarted = true;
        toneStartTime = millis();
        break;
      }
    }
  }

  // update max two frequencies
  if (toneStarted) {
    for (int i = 0; i < 7; i++) {
      if (mags[i] > maxMag1) {
        if (i != freqIdx1) {
          maxMag2 = maxMag1;
          freqIdx2 = freqIdx1;
        }
        maxMag1 = mags[i];
        freqIdx1 = i;
      } else if (mags[i] > maxMag2) {
        if (i == freqIdx1) continue;
        maxMag2 = mags[i];
        freqIdx2 = i;
      }
    }

    // count active frequencies
    int activeFrequencyCount = 0;
    for (int i = 0; i < 7; i++) {
      if (mags[i] > activityThreshold) activeFrequencyCount++;
    }

    // if a dtmf tone is active and there are no frequencies active then the tone has finished
    if (activeFrequencyCount == 0) {
      toneStarted = false;
      toneEnded = true;
      toneEndTime = millis();
    }
  }

  // print detected tones 2 at a time. this is because each number on the keypad is made up of 2 tones
  if (toneEnded) {
    toneEnded = false;
    if (toneEndTime - toneStartTime < 50) { // there are audible clicks after each keypad button press. they are 26ms while dtmf tones are 100ms.
      maxMag1 = 0;
      maxMag2 = 0;
      freqIdx1 = -1;
      freqIdx2 = -1;
      return;
    }
    String keypadButtonPressed = getKeypadButtonPressed(dtmfFreqs[freqIdx1], dtmfFreqs[freqIdx2]);
    maxMag1 = 0;
    maxMag2 = 0;
    freqIdx1 = -1;
    freqIdx2 = -1;

    Serial.println("KEY_" + keypadButtonPressed);
  }
}

void setup() {
  Serial.begin(500000);
  pinMode(A4, INPUT); // dtmf detection
  pinMode(14, INPUT); // off hook detection
  pinMode(21, OUTPUT); // indicator LED
  //pinMode(A1, OUTPUT);

  // Send the ESP32 into sleep mode when the phone is on hook
  if (esp_sleep_get_wakeup_cause() == ESP_SLEEP_WAKEUP_EXT0) {
    Serial.println("Woke from deep sleep (Phone off hook)");
  }
  if (analogRead(14) < 64) {
    gpio_pullup_dis(GPIO_NUM_14);
    gpio_pulldown_en(GPIO_NUM_14);
    esp_sleep_enable_ext0_wakeup(GPIO_NUM_14, 1); // wake on HIGH
    Serial.println("Sleep");
    esp_deep_sleep_start();
  }
}

void loop() {
  if (isPhoneOnHook()) return;
  checkForSerialData();
  dtmfDetection();
}