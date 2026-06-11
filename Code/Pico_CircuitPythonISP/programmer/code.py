# SPDX-FileCopyrightText: 2021 ladyada for Adafruit Industries
# SPDX-License-Identifier: MIT

"""
Trinket/Gemma (ATtiny85) programming example, be sure you have the '85 wired up so:
  Pin 8 (VCC) -> Pico 3.3V
  Pin 4 (GND) -> Pico GND
  Pin 5 (PB0) -> Pico MOSI
  Pin 6 (PB1) -> Pico MISO
  Pin 7 (PB2) -> Pico SCK
  Pin 1 (RST) -> Pico RESET (set below)
"""

import board
import busio
import os

import adafruit_avrprog

# ----- PICO GPIO PINS -----
PIN_RESET = board.GP10
PIN_SCK = board.GP18
PIN_MOSI = board.GP19
PIN_MISO = board.GP20
# --------------------------

spi = busio.SPI(PIN_SCK, PIN_MOSI, PIN_MISO)
avrprog = adafruit_avrprog.AVRprog()
avrprog.init(spi, PIN_RESET)

# Each chip has to have a definition so the script knows how to find it
attiny85 = avrprog.Boards.ATtiny85


def error(err):
    """Helper to print out errors for us and then halt"""
    print("ERROR: " + err)
    avrprog.end()
    while True:
        pass


# Uncomment this while loop to use manual upload
#while input("Ready to GO, type 'G' here to start> ") != "G":
#    pass

if not avrprog.verify_sig(attiny85, verbose=True):
    error("Signature read failure")
print("Found", attiny85["name"])

avrprog.write_fuses(attiny85, low=0xE2, high=0xD5, ext=0x06, lock=0x3F)
if not avrprog.verify_fuses(attiny85, low=0xE2, high=0xD5, ext=0x06, lock=0x3F):
    error("Failure verifying fuses!")

print("Programming flash from file")

# auto find the first hex file and use it as the program to write to the attiny85
hex_file_name = None
for file in os.listdir():
    if file.endswith(".hex"):
        hex_file_name = file
        break
print(f"Found hex file: {hex_file_name}")
avrprog.program_file(attiny85, hex_file_name, verbose=True, verify=True)

print("Done!")