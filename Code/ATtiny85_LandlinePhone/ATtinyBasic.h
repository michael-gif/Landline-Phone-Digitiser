#ifndef ATtinyBasic_h
#define ATtinyBasic_h

#include <stdint.h>

void _pinMode(uint8_t pin, uint8_t direction);
void _digitalWrite(uint8_t pin, uint8_t value);
uint8_t _digitalRead(uint8_t pin);
void _adcSetup();
void _adcSetPin(uint8_t pin);
int _analogRead(uint8_t pin);

#endif