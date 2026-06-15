#include <vector>

#define PIN_HOOK_DETECT A1
#define PIN_HOOK_STATUS 15
#define PIN_DTMF_DETECT A0
#define PIN_DAC_OUTPUT 14

#define MODE_DTMF_DETECT 0
#define MODE_DAC_OUTPUT 1

int phoneState = MODE_DTMF_DETECT;

int getKeypadButtonPressed(int freq1, int freq2) {
  switch (freq1 + freq2) {
    case 1906:
      return 1;
    case 2033:
      return 2;
    case 2174:
      return 3;
    case 1979:
      return 4;
    case 2106:
      return 5;
    case 2247:
      return 6;
    case 2061:
      return 7;
    case 2188:
      return 8;
    case 2329:
      return 9;
    case 2277:
      return 0;
    case 2150:
      return 10; // *
    case 2418:
      return 11; // #
    default:
      return 12; // unknown
  }
}

int goertzelMagnitude(const int* samples, int num_samples, int target_freq, int sample_rate) {
  float k = 0.5f + (((float)num_samples * target_freq) / sample_rate);
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
  return real * real + imag * imag;
}

float coeffs[7] = {
  1.7154572,
  1.6629392,
  1.5460207,
  1.4819024,
  1.1913984,
  1.0282056,
  0.7653668
};
int goertzelMagnitudeFast(const int* samples, int num_samples, int freqIndex) {
  float coeff = coeffs[freqIndex];
  float q0 = 0, q1 = 0, q2 = 0;
  for (int i = 0; i < num_samples; i++) {
    q0 = coeff * q1 - q2 + samples[i];
    q2 = q1;
    q1 = q0;
  }
  return q1*q1 + q2*q2 - q1*q2*coeff;
}

const int FFT_SAMPLE_COUNT = 128;
int fft_input[FFT_SAMPLE_COUNT];
const int dtmfFreqs[7] = {697, 770, 852, 941, 1209, 1336, 1477};
bool toneStarted = false;
int dtmf_freq1 = -1;
int dtmf_freq2 = -1;
int dtmf_mag1 = -1;
int dtmf_mag2 = -1;
unsigned long toneStart;
void dtmfDetection() {
  // sample the ADC 128 times at ~8kHz
  for (int i = 0; i < FFT_SAMPLE_COUNT; i++) {
    fft_input[i] = analogRead(PIN_DTMF_DETECT);
    delayMicroseconds(122);
  }

  // fast fourier transform
  String res = "4096,";
  int activeTones = 0;
  for (int i = 0; i < 7; i++) {
    int freq = dtmfFreqs[i];
    int freq_mag = goertzelMagnitudeFast(fft_input, FFT_SAMPLE_COUNT, i);
    res += (String)freq + ":" + (String)freq_mag + ",";
    continue;
    if (freq_mag > 4000) { // anything above 4000 means we have a dtmf tone being played
      activeTones++;
      if (!toneStarted) {
        toneStarted = true;
        toneStart = millis();
      }

      // update the two highest frequencies by tracking the 2 highest magnitudes
      if (freq_mag >= dtmf_mag1) {
        dtmf_mag1 = freq_mag;
        dtmf_freq1 = freq;
      } else if (freq != dtmf_freq1 && freq_mag >= dtmf_mag2) {
        dtmf_mag2 = freq_mag;
        dtmf_freq2 = freq;
      }
    }
  }
  Serial.println(res + "0");
  return;


  // dtmf decoding
  if (toneStarted && activeTones == 0) {
    toneStarted = false;
    unsigned long toneDuration = millis() - toneStart;
    if (toneDuration < 100) return;
    int button = getKeypadButtonPressed(dtmf_freq1, dtmf_freq2);
    if (button != 12)
      Serial.println(button);
    // if (button == "#") {
    //   delay(1000);
    //   phoneState = MODE_DAC_OUTPUT;
    //   uk_anthem();
    //   phoneState = MODE_DTMF_DETECT;
    // }
    dtmf_freq1 = -1;
    dtmf_freq2 = -1;
    dtmf_mag1 = -1;
    dtmf_mag2 = -1;
  }
}

bool phoneHookState = true; // true means the phone is on the hook
void checkHookState() {
  //Serial.println(analogRead(PIN_HOOK_DETECT));
  //return;
  bool currentState = analogRead(PIN_HOOK_DETECT) < 128;
  if (currentState != phoneHookState) {
    digitalWrite(PIN_HOOK_STATUS, currentState ? LOW : HIGH);
    delay(50); // de-bounce (50ms)
    phoneHookState = currentState;
    Serial.println(currentState ? "ON_HOOK" : "OFF_HOOK");

    // reset dtmf detection when the phone is put back on the hook
    if (phoneHookState) {
      toneStarted = false;
      dtmf_freq1 = -1;
      dtmf_freq2 = -1;
      dtmf_mag1 = -1;
      dtmf_mag2 = -1;
    }
  }
}

void pwmWrite(int frequency, int duration_ms) {
  analogWriteFreq(frequency);
  analogWrite(PIN_DAC_OUTPUT, 128);
  delay(duration_ms);
  analogWrite(PIN_DAC_OUTPUT, 0);
}

void uk_anthem() {
  pwmWrite(310, 250);
  delay(200);
  pwmWrite(310, 250);
  delay(200);
  pwmWrite(350, 250);
  delay(200);
  pwmWrite(290, 750);
  delay(150);
  pwmWrite(310, 100);
  delay(150);
  pwmWrite(350, 250);
  delay(250);

  pwmWrite(390, 250);
  delay(200);
  pwmWrite(390, 250);
  delay(200);
  pwmWrite(415, 250);
  delay(200);
  pwmWrite(385, 750);
  delay(150);
  pwmWrite(350, 100);
  delay(150);
  pwmWrite(315, 250);
  delay(200);

  pwmWrite(350, 250);
  delay(200);
  pwmWrite(320, 250);
  delay(200);
  pwmWrite(300, 250);
  delay(200);
  pwmWrite(320, 500);
}

void dacInit() {
  analogWriteFreq(8000);
}

void dacWrite(unsigned char amplitude) {
  analogWrite(PIN_DAC_OUTPUT, amplitude);
}

void dacReset() {
  analogWrite(PIN_DAC_OUTPUT, 0);
}

void playWavFile(unsigned char* data, int length) {
  dacInit();
  for (int i = 0; i < length; i++) {
    dacWrite(*data);
    delayMicroseconds(125);
    data++;
  }
  dacReset();
}

enum SerialState {
  MESSAGE_START,
  MESSAGE_LENGTH,
  MESSAGE_CONTENT
};
SerialState serialState = MESSAGE_START;
void checkSerial() {
  int messageLength = 0;
  uint8_t messageLengthBytes[4];

  uint8_t startMarker = 0x7E;
  std::vector<uint8_t> receivedBytes;
  int numBytesReceived = 0;

  while (Serial.available() > 0) {
    uint8_t b = Serial.read();
    bool shouldBreak = false;

    switch (serialState) {
      case SerialState::MESSAGE_START:
        if (b == startMarker) {
          serialState = MESSAGE_LENGTH;
          Serial.println("Received message start");
        }
        break;
      case SerialState::MESSAGE_LENGTH:
        messageLengthBytes[messageLength] = b;
        messageLength++;
        if (messageLength == 4) {
          memcpy(&messageLength, messageLengthBytes, sizeof(int));
          serialState = MESSAGE_CONTENT;
          Serial.println("Received message length");
        }
        break;
      case SerialState::MESSAGE_CONTENT:
        receivedBytes.emplace_back(b);
        numBytesReceived++;
        if (numBytesReceived == messageLength) {
          serialState == MESSAGE_START;
          shouldBreak = true;
          Serial.println("Received message");
        }
        break;
      default:
        break;
    }

    if (shouldBreak) break;
  }

  if (numBytesReceived > 0) {
    Serial.println("Recieved: " + (String)numBytesReceived + " bytes");

    // just assume the message is audio
    phoneState = MODE_DAC_OUTPUT;
    playWavFile(receivedBytes.data(), numBytesReceived);
    Serial.println("Finished playing wave file");
    phoneState = MODE_DTMF_DETECT;

    numBytesReceived = 0;
    receivedBytes.clear();
  }
}

void setup() {
  pinMode(PIN_HOOK_DETECT, INPUT);
  pinMode(PIN_HOOK_STATUS, OUTPUT);
  pinMode(PIN_DTMF_DETECT, INPUT);
  pinMode(PIN_DAC_OUTPUT, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);

  analogReadResolution(10);
  analogWriteRange(2048);

  Serial.begin(921600);

  digitalWrite(LED_BUILTIN, HIGH);
  digitalWrite(PIN_HOOK_STATUS, LOW);
}

void loop() {
  checkHookState();
  if (phoneHookState == true) return;
  if (phoneState == MODE_DTMF_DETECT) dtmfDetection();
  //checkSerial();
}
