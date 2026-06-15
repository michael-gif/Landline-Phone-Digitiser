// AVR High-voltage Serial Programmer
// Inspired by Paul Willoughby 03/20/2010
// http://www.rickety.us/2010/03/arduino-avr-high-voltage-serial-programmer/
// Inspired by Jeff Keyzer http://mightyohm.com
// Serial Programming routines from ATtiny25/45/85 datasheet

// Desired fuse configuration
#define LFUSE 0x62
#define HFUSE 0xDF
#define EFUSE 0xFE
#define LBITS 0xFF

#define RST    14 // Output to level shifter for !RESET from transistor to Pin 1
#define CLKOUT 13 // Connect to Serial Clock Input (SCI) Pin 2
#define SCK    18 // Connect to Serial Data Output (SDO) Pin 7
#define MISO   20 // Connect to Serial Instruction Input (SII) Pin 6
#define MOSI   19 // Connect to Serial Data Input (SDI) Pin 5 
#define VCC    15 // Connect to VCC Pin 8

int inByte = 0;    // incoming serial byte Computer
int inData = 0;    // incoming serial byte AVR

void serialFlush() {
  while(Serial.available() > 0) {
    char t = Serial.read();
  }
}

void eraseChip() {
  Serial.println("Erasing Chip...");
  shiftOut2(0x80, 0x4C);
  shiftOut2(0x00, 0x64);
  shiftOut2(0x00, 0x6C);
  delay(10); // Wait for erase to complete (datasheet says min 4.5ms, max 9ms)
  Serial.println("Chip has been erased\n");
  delay(100);
}

void readFuses() {
  // Read lfuse
  shiftOut2(0x04, 0x4C);
  shiftOut2(0x00, 0x68);
  inData = shiftOut2(0x00, 0x6C);
  Serial.print("lfuse reads as ");
  Serial.println(inData, HEX);
  
  // Read hfuse
  shiftOut2(0x04, 0x4C);
  shiftOut2(0x00, 0x7A);
  inData = shiftOut2(0x00, 0x7E);
  Serial.print("hfuse reads as ");
  Serial.println(inData, HEX);
  
  // Read efuse
  shiftOut2(0x04, 0x4C);
  shiftOut2(0x00, 0x6A);
  inData = shiftOut2(0x00, 0x6E);
  Serial.print("efuse reads as ");
  Serial.println(inData, HEX);

  shiftOut2(0x04, 0x4C);
  shiftOut2(0x00, 0x78);
  inData = shiftOut2(0x00, 0x7C);
  Serial.print("lbits reads as ");
  Serial.println(inData, HEX);

  Serial.println(); 
}

void writeFuses() {
  //Write lfuse
  Serial.println("Writing lfuse");
  shiftOut2(0x40, 0x4C);
  shiftOut2(LFUSE, 0x2C);
  shiftOut2(0x00, 0x64);
  shiftOut2(0x00, 0x6C);

  // Write hfuse
  Serial.println("Writing hfuse");
  shiftOut2(0x40, 0x4C);
  shiftOut2(HFUSE, 0x2C);
  shiftOut2(0x00, 0x74);
  shiftOut2(0x00, 0x7C);

  // Write Extended Fuse (efuse)
  Serial.println("Writing efuse");
  shiftOut2(0x40, 0x4C);
  shiftOut2(EFUSE, 0x2C);
  shiftOut2(0x00, 0x66);
  shiftOut2(0x00, 0x6E);

  // Write Lock Bits - must be written first or else the fuses can't be written
  Serial.println("Writing lock bits");
  shiftOut2(0x20, 0x4C);
  shiftOut2(LBITS, 0x2C);
  shiftOut2(0x00, 0x64);
  shiftOut2(0x00, 0x6C);

  Serial.println();
}

void establishContact() {
  while (Serial.available() <= 0) {
    Serial.println("Enter a character to continue"); // send an initial string
    delay(1000);
  }
}

int shiftOut2(byte val, byte val1) {
  uint8_t clockPin = CLKOUT;
  uint8_t bitOrder = MSBFIRST;

	int i;
  int inBits = 0;
  // Wait until SCK goes high
  while (!digitalRead(SCK));
  
  // Start bit
  digitalWrite(MOSI, LOW);
  digitalWrite(MISO, LOW);
  digitalWrite(clockPin, HIGH);
  digitalWrite(clockPin, LOW);

	for (i = 0; i < 8; i++)  {           
		if (bitOrder == LSBFIRST) {
			digitalWrite(MOSI, !!(val & (1 << i)));
      digitalWrite(MISO, !!(val1 & (1 << i)));
    }
		else {
			digitalWrite(MOSI, !!(val & (1 << (7 - i))));
      digitalWrite(MISO, !!(val1 & (1 << (7 - i))));
    }
    inBits <<= 1;
    inBits |= digitalRead(SCK);
    delayMicroseconds(5);
    digitalWrite(clockPin, HIGH);
    delayMicroseconds(5);
		digitalWrite(clockPin, LOW);
	}

  // End bits
  digitalWrite(MOSI, LOW);
  digitalWrite(MISO, LOW);
  digitalWrite(clockPin, HIGH);
  digitalWrite(clockPin, LOW);
  digitalWrite(clockPin, HIGH);
  digitalWrite(clockPin, LOW);
  
  return inBits;
}

void setup() {
  // Set up control lines for HV parallel programming
  pinMode(VCC, OUTPUT);
  pinMode(RST, OUTPUT);
  pinMode(MOSI, OUTPUT);
  pinMode(MISO, OUTPUT);
  pinMode(CLKOUT, OUTPUT);
  pinMode(SCK, OUTPUT); // configured as input when in programming mode
  
  // Initialize output pins as needed
  digitalWrite(RST, HIGH); // Level shifter is inverting, this shuts off 12V

  Serial.begin(9600);
  
  establishContact(); // Send a byte to establish contact until receiver responds 
}

void loop() {
  // If we get a valid byte, run:
  if (Serial.available() > 0) {
    // Get incoming byte:
    inByte = Serial.read();
    Serial.println(byte(inByte));
    Serial.println("Entering programming Mode\n");

    // Initialize pins to enter programming mode
    pinMode(SCK, OUTPUT); // Temporary
    digitalWrite(MOSI, LOW);
    digitalWrite(MISO, LOW);
    digitalWrite(SCK, LOW);
    digitalWrite(RST, HIGH); // Level shifter is inverting, this shuts off 12V
    
    // Enter High-voltage Serial programming mode
    digitalWrite(VCC, HIGH); // Apply VCC to start programming process
    delayMicroseconds(20);
    digitalWrite(RST, LOW); // Turn on 12v
    delayMicroseconds(10);
    pinMode(SCK, INPUT); // Release SCK
    delayMicroseconds(300);

    // Programming mode
    readFuses();
    eraseChip();
    readFuses();
    writeFuses();
    readFuses();
    
    Serial.println("Exiting programming Mode");
    digitalWrite(CLKOUT, LOW);
    digitalWrite(VCC, LOW);
    digitalWrite(RST, HIGH); // Turn off 12v

    serialFlush();
  }
}
