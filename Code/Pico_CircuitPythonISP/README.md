# How to program an Attiny85 using a Raspberry Pi Pico

1. Install circuit python on the Pico
- Download the latest version of circuit python
- Hold the reset button on the Pico down and plug it into a computer so it shows up as a drive
- Copy and paste the downloaded circuit python file into the pico drive

2. Install the AVR programmer script
- Copy and paste code.py from this folder to the pico. It should replace the code.py already on it

3. Create AVR hex file
- Open arduino IDE
- Set the board type to ATtiny25/45/85
- Set the Clock speed to 8MHz
- Write some code for the attiny
- Go to Sketch > Export Compiled Binary
- Open the folder with the sketch, open the 'build' folder and find the .hex file
- Copy and paste the hex file onto the pico
- Wait for it to auto-upload

# EXTREMELY IMPORTANT
- The code.py script sets the Attiny85 to run at 8MHz by default
- Your first program should be a simple Blink. If it blinks once per second, you're golden.
  If it blinks super fast or not at all, your clock fuses are wrong.
  You'll need to re-run the AVR programmer script with a different fuse setting. Change the
  fuse settings in code.py near the end of the script.
- If you ever set the fuses so the Attiny expects an external crystal (and you don't have one),
  you've "bricked" it for ISP programming. You'll need a high-voltage programmer to resurrect it.
  Don't ever use use an external crystal. Always use the internal clock.

# VERY IMPORTANT
- The code.py script will find the first .hex file and upload it to the attiny.
  If there are more than 1 hex files, the first found will be uploaded and the rest ignored.
  To be safe, delete any extra hex files from the pico and only paste the one you want uploaded.

# IMPORTANT
- To open a serial monitor to the Pico, install PuTTY and open a serial connection
  to the pico's COM port.
  Note: A serial monitor is not required to upload programs to the attiny.