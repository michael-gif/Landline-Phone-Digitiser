import serial
import wave
import sys


ser = serial.Serial('COM4', 921600)

with wave.open('500hz_8.wav', 'rb') as wf:
    frame_count = wf.getnframes()
    print(f"Found {frame_count} frames")
    wf_bytes = wf.readframes(frame_count)
    wf_bytes_length = len(wf_bytes).to_bytes(4, byteorder='little')

print("Building bytearray...")
data = bytes([0x7e]) + wf_bytes_length + wf_bytes
print(f"Sending {len(wf_bytes)} bytes")
chunks = len(data) // 1024
for i in range(chunks):
    ser.write(data[i*1024:(i+1)*1024])
ser.write(data[chunks*1024:])
print(f"Sent {chunks + 1} chunks")

print("Reading...")
while True:
    data = ser.readline().decode().strip()
    if data:
        print(f"{data}")
        if data == "Finished playing wave file":
            break
ser.close()
