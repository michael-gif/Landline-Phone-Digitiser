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

// float omegas[7] = {
//   0.539961,
//   0.589049,
//   0.687223,
//   0.736311,
//   0.93266,
//   1.03084,
//   1.1781
// };

// for some reason, voltaile lets you do 2 iterations of fft instead of 1
// float sines[7] = {
//   0.5141028f,
//   0.5555702f,
//   0.6343934f,
//   0.671559f,
//   0.8032074f,
//   0.8577286f,
//   0.9238794f
// };

// volatile const float cosines[7] = {
//   0.8577286,
//   0.8314696,
//   0.7730104,
//   0.7409512,
//   0.5956992,
//   0.5141028,
//   0.3826834
// };
int goertzelMagnitude(const int* samples, uint8_t num_samples, int target_freq) {
  //float k = 0.5f + (((float)num_samples * target_freq) / 8000);
  //int bin = (int)k;
  //float omega = (2.0f * M_PI * bin) / num_samples;
  float omega = (2.0f * M_PI * target_freq) / 8000;
  float sine = sin(omega);
  float cosine = cos(omega);
  float coeff = 2.0f * cosine;

  float q0 = 0, q1 = 0, q2 = 0;
  for (uint8_t i = 0; i < num_samples; i++) {
    q0 = coeff * q1 - q2 + samples[i];
    q2 = q1;
    q1 = q0;
  }

  //float real = (q0 - q2 * cosine);
  //float imag = (q2 * sine);
  //return sqrt(real * real + imag * imag) * 10;
  return sqrt(q1*q1 + q2*q2 - coeff*q1*q2);
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
  // sample the ADC 128 times at ~8kHz
  for (int i = 0; i < FFT_SAMPLE_COUNT; i++) {
    fft_input[i] = _analogRead(PIN_DTMF_DETECT);
    _delay_us(125);
  }

  // fast fourier transform
  Serial.write("<");
  int activeTones = 0;
  for (uint8_t i = 0; i < 7; i++) {
    int freq = dtmfFreqs[i];
    int freq_mag = goertzelMagnitude(fft_input, FFT_SAMPLE_COUNT, freq);
    Serial.write((byte*)&freq_mag, 2);
  }
  Serial.write(">");
}

bool phoneHookState = true; // true means the phone is on the hook
void checkHookState() {
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