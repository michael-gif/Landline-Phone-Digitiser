#include "ATtinyBasic.h"
#include <avr/io.h>

void _pinMode(uint8_t pin, uint8_t direction) {
  if (direction) {
    DDRB |= (1 << pin);
  } else {
    DDRB |= ~(1 << pin);
  }
}

void _digitalWrite(uint8_t pin, uint8_t value) {
  if (value == 1) {
    PORTB |= (1 << pin);
  } else {
    PORTB &= ~(1 << pin);
  }
}

void _adcSetup() {
  ADMUX = (0 << REFS1) | (0 << REFS0); // Set reference voltage to VCC
  ADCSRA = (1 << ADEN); // Enable ADC
  ADCSRA |= (1 << ADPS2) | (1 << ADPS1) | (0 << ADPS0); // Set prescaler to 64
}

void _adcSetPin(uint8_t pin) {
  ADMUX &= ~((1 << MUX3) | (1 << MUX2) | (1 << MUX1) | (1 << MUX0)); // clear mux pins

  switch (pin) { // Set ADC channel
    case PB2:
      ADMUX |= (0 << MUX3) | (0 << MUX2) | (0 << MUX1) | (1 << MUX0);
      break;
    case PB3:
      ADMUX |= (0 << MUX3) | (0 << MUX2) | (1 << MUX1) | (1 << MUX0);
      break;
    case PB4:
      ADMUX |= (0 << MUX3) | (0 << MUX2) | (1 << MUX1) | (0 << MUX0);
      break;
    case PB5:
      ADMUX |= (0 << MUX3) | (0 << MUX2) | (0 << MUX1) | (0 << MUX0);
      break;
  }
}

int _analogRead(uint8_t pin) {
  _adcSetPin(pin);
  ADCSRA |= (1 << ADSC); // start conversion
  while (ADCSRA & (1 << ADSC)); // wait for conversion to complete
  uint8_t l = ADCL;
  uint8_t h = ADCH;
  return (h << 8) | l;
}