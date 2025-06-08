import serial
import threading
import sys

from datetime import datetime

ser = serial.Serial('COM3', 115200)  # adjust port and baud rate

lines= []
running = True
def read_serial():
    while running:
        data = ser.readline().decode().strip()
        lines.append(data)

thread = threading.Thread(target=read_serial)
thread.start()

input("Press Enter to stop recording...\n")
running = False
thread.join()
ser.close()

print("Saving data...")
filename = datetime.now().strftime('%Y_%m_%d_%H_%M_%S')
with open("raw_data/" + filename + ".txt", "a") as f:
    for line in lines:
        f.write(line + "\n")

print("Done.")
