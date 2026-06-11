// ---------- Required for TinySoftwareSerial.h ----------
#define USE_SOFTWARE_SERIAL 1
#define SOFT_TX_ONLY 1
#define ANALOG_COMP_DDR   DDRB
#define ANALOG_COMP_PORT  PORTB
#define ANALOG_COMP_PIN   PINB
#define ANALOG_COMP_AIN0_BIT  1 // PB0 = RX
#define ANALOG_COMP_AIN1_BIT  0 // PB1 = TX
// -------------------------------------------------------

#include "TinySoftwareSerial.cpp"
#include "ATtinyBasic.h"

#define PIN_HOOK_DETECT PB3
#define PIN_DTMF_DETECT PB4
#define PIN_HOOK_STATUS PB2

float samplesOverSampleRate = 0.016f;
float twoPI = 2 * M_PI;
int goertzelMagnitude(const int* samples, uint8_t num_samples, int target_freq, float sample_rate) {
    float k = 0.5f + (samplesOverSampleRate * target_freq);
    int bin = (int)k;
    float omega = (twoPI * bin) / num_samples;
    float sine = sinf(omega);
    float cosine = cosf(omega);
    float coeff = 2.0f * cosine;

    float q0 = 0, q1 = 0, q2 = 0;
    for (uint8_t i = 0; i < num_samples; i++) {
        q0 = coeff * q1 - q2 + samples[i];
        q2 = q1;
        q1 = q0;
    }

    float real = (q1 - q2 * cosine);
    float imag = (q2 * sine);
    return sqrt(real * real + imag * imag) * 10;
}

const uint8_t FFT_SAMPLE_COUNT = 128;
int fft_input[FFT_SAMPLE_COUNT];
const int dtmfFreqs[7] = {697, 770, 852, 941, 1209, 1336, 1477};
bool toneStarted = false;
int dtmf_freq1 = -1;
int dtmf_freq2 = -1;
int dtmf_mag1 = -1;
int dtmf_mag2 = -1;
void dtmfDetection() {
  // sample the ADC 128 times at 8kHz
  const int SAMPLE_RATE = 8000;
  for (int i = 0; i < FFT_SAMPLE_COUNT; i++) {
    fft_input[i] = _analogRead(PIN_DTMF_DETECT);
    _delay_us(125);
  }

  // fast fourier transform
  Serial.write("<");
  int activeTones = 0;
  for (uint8_t i = 0; i < 7; i++) {
    int freq = dtmfFreqs[i];
    int freq_mag = goertzelMagnitude(fft_input, FFT_SAMPLE_COUNT, freq, SAMPLE_RATE);
    Serial.write((byte*)&freq_mag, 2);
  }
  Serial.write(">");
}

bool phoneHookState = true; // true means the phone is on the hook
void checkHookState() {
  // Serial.write("<");
  // Serial.write(_analogRead(PIN_HOOK_DETECT));
  // Serial.write(">");
  // return;
  bool currentState = _analogRead(PIN_HOOK_DETECT) < 128;
  if (currentState != phoneHookState) {
    _digitalWrite(PIN_HOOK_STATUS, currentState ? LOW : HIGH);
    _delay_ms(50); // de-bounce (50ms)
    phoneHookState = currentState;
    Serial.write('<');
    Serial.write((currentState) ? (uint8_t)0x0A : (uint8_t)0x0B); // 0x0A means on hook, 0x0B means off hook
    Serial.write('>');

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

// void setup() {
//   pinMode(PIN_HOOK_DETECT, INPUT);
//   pinMode(PIN_DTMF_DETECT, INPUT);
//   pinMode(PIN_HOOK_STATUS, OUTPUT);
//   Serial.begin(9600);
// }

// void loop() {
//   checkHookState();
//   if (phoneHookState == true) return;
//   //dtmfDetection();
// }

int main() {
  _adcSetup();
  _pinMode(PIN_HOOK_STATUS, OUTPUT);
  Serial.begin(9600);

  while (1) {
    checkHookState();
    if (phoneHookState == true) continue;
    dtmfDetection();
  }
}