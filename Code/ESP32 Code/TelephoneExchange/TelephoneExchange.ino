#include <sstream>

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
  bool offHook = digitalRead(14); // if pin 14 is high then phone is off hook. if pin 14 is low then phone is on hook
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

void sendToneSequence(std::vector<int> values) {
  for (int i = 0; i < values.size(); i+=2) {
    ledcChangeFrequency(15, values[i], 8);
    ledcWrite(15, 128);
    delay(values[i+1]);
    ledcWrite(15, 0);
  }
  delay(20);
  Serial.println("Sent tone sequence");
}

std::vector<String> splitString(String& s, char delimiter) {
  s.trim();
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

String readSerial() {
  char startMarker = '<';
  char endMarker = '>';
  bool receiving = false;
  String data = "";
  while (Serial.available() > 0) {
    char receivedChar = Serial.read();

    if (receiving) {
      if (receivedChar == endMarker) {
        receiving = false;
        break;
      } else {
        data += receivedChar;
      }
    }

    if (receivedChar == startMarker) {
      receiving = true;
    }
  }
  return data;
}

void checkForIVRCommands() {
  if (Serial.available())
  {
    String ivr_command = readSerial();
    std::vector<String> parts = splitString(ivr_command, '_');
    if (parts[0] == "TONESEQUENCE") {
      // create tone sequence as 2 vectors with the frequencies and durations
      std::vector<int> values;
      for (int i = 1; i < parts.size(); i++) {
        int value = parts[i].toInt();
        values.emplace_back(value);
      }

      // send the sequence to the phone
      sendToneSequence(values);
    }
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
  Serial.begin(115200);
  pinMode(A4, INPUT); // dtmf detection
  pinMode(14, INPUT); // off hook detection
  pinMode(21, OUTPUT); // indicator LED

  ledcAttach(15, 500, 8); // PWM for tone sequences
}

void loop() {
  if (isPhoneOnHook()) return;
  checkForIVRCommands();
  dtmfDetection();
}