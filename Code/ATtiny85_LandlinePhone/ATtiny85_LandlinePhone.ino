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

// float coeffs[7] = {
//   1.7154572,
//   1.6629392,
//   1.5460207,
//   1.4819024,
//   1.1913984,
//   1.0282056,
//   0.7653668
// };
float coeffs[7] = {
  1.7154572,
  1.6629392,
  1.5460210,
  1.4819022,
  1.1913986,
  1.0282056,
  0.7653669
};
uint32_t goertzelMagnitude(const int* samples, uint8_t num_samples, uint8_t freqIndex) {
  float coeff = coeffs[freqIndex];
  float q0 = 0, q1 = 0, q2 = 0;
  for (uint8_t i = 0; i < num_samples; i++) {
    q0 = coeff * q1 - q2 + samples[i];
    q2 = q1;
    q1 = q0;
  }
  float mag = sqrt(q1*q1 + q2*q2 - q1*q2*coeff);
  return mag;
}

const uint8_t FFT_SAMPLE_COUNT = 128;
int fft_input[FFT_SAMPLE_COUNT];
const int dtmfFreqs[7] = {697, 770, 852, 941, 1209, 1336, 1477};
bool toneStarted = false;
uint32_t dtmf_freq1 = 0;
uint32_t dtmf_freq2 = 0;
uint32_t dtmf_mag1 = 0;
uint32_t dtmf_mag2 = 0;
uint8_t dtmfDelay = 122;
void dtmfDetection() {
  // sample the ADC 128 times at ~8kHz
  for (int i = 0; i < FFT_SAMPLE_COUNT; i++) {
    fft_input[i] = _analogRead(PIN_DTMF_DETECT);
    _delay_us(122);
    // switch(dtmfDelay) {
    //   case 120:
    //     _delay_us(120);
    //     break;
    //   case 121:
    //     _delay_us(121);
    //     break;
    //   case 122:
    //     _delay_us(122);
    //     break;
    //   case 123:
    //     _delay_us(123);
    //     break;
    //   case 124:
    //     _delay_us(124);
    //     break;
    //   case 125:
    //     _delay_us(125);
    //     break;
    //   case 126:
    //     _delay_us(126);
    //     break;
    //   case 127:
    //     _delay_us(127);
    //     break;
    //   case 128:
    //     _delay_us(128);
    //     break;
    //   case 129:
    //     _delay_us(129);
    //     break;
    //   case 130:
    //     _delay_us(130);
    //     break;
    //   default:
    //     _delay_us(125);
    //     break;
    // }
  }

  // fast fourier transform
  Serial.write("<");
  Serial.write((byte)2);
  int activeTones = 0;
  for (uint8_t i = 0; i < 7; i++) {
    uint32_t mag = goertzelMagnitude(fft_input, FFT_SAMPLE_COUNT, i);
    Serial.write((byte*)&mag, 2);
    continue;
    if (mag > 100) { // anything above 2048 means we have a dtmf tone being played
      activeTones++;
      toneStarted = true;

      // update the two highest frequencies by tracking the 2 highest magnitudes
      if (mag >= dtmf_mag1) {
        dtmf_mag1 = mag;
        dtmf_freq1 = i;
      } else if (i != dtmf_freq1 && mag >= dtmf_mag2) {
        dtmf_mag2 = mag;
        dtmf_freq2 = i;
      }
    }
  }
  Serial.write(">");
  return;

  // dtmf decoding
  if (toneStarted && activeTones == 0) {
    toneStarted = false;
    Serial.write("<");
    Serial.write((byte)2);
    Serial.write((byte*)&dtmf_freq1, 2);
    Serial.write((byte*)&dtmf_mag1, 2);
    Serial.write((byte*)&dtmf_freq2, 2);
    Serial.write((byte*)&dtmf_mag2, 2);
    Serial.write(">");
    dtmf_freq1 = 0;
    dtmf_freq2 = 0;
    dtmf_mag1 = 0;
    dtmf_mag2 = 0;
  }
}

bool phoneHookState = true; // true means the phone is on the hook
void checkHookState() {
  bool currentState = _analogRead(PIN_HOOK_DETECT) < 128;
  if (currentState != phoneHookState) {
    //_digitalWrite(PIN_HOOK_STATUS, currentState ? LOW : HIGH);
    _delay_ms(50); // de-bounce (50ms)
    phoneHookState = currentState;
    Serial.write("<");
    Serial.write((byte)0);
    Serial.write((currentState) ? (uint8_t)0x0A : (uint8_t)0x0B); // 0x0A means on hook, 0x0B means off hook
    Serial.write(">");

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

bool buttonState = false;
void checkDelayButton() {
  if (_digitalRead(PB2) == 0) {
    if (!buttonState) {
      buttonState = true;
      _delay_ms(100); // debounce

      dtmfDelay++;
      if (dtmfDelay > 130) dtmfDelay = 120;
      Serial.write("<");
      Serial.write((byte)1);
      Serial.write(dtmfDelay);
      Serial.write(">");
    }
  } else {
    if (buttonState) {
      buttonState = false;
      _delay_ms(100); // debounce
    }
  }
}

int main() {
  _adcSetup();
  _pinMode(PIN_HOOK_STATUS, INPUT_PULLUP);
  Serial.begin(9600);

  while (1) {
    checkDelayButton();
    checkHookState();
    if (phoneHookState == true) continue;
    dtmfDetection();
  }
}