import serial
import math

ser = serial.Serial('COM3', 500000)

sentPCM = False
def audio():
    global sentPCM
    print("Opening PCM")
    with open('invalid_number.pcm', 'rb') as f:
        data = f.read()
    start = "AUDIO_"
    msg_len = len(start) + len(data)
    msg = str(msg_len).encode() + b"_" + start.encode() + data
    print("PY: Writing " + str(msg_len) + " bytes")
    packets = math.ceil(len(msg) / 120)
    for i in range(packets):
        start = i * 120
        end = start + 120
        ser.write(msg[start:end])
        while True:
            data = ser.readline().decode().strip()
            if data == "1":
                break
    print("PY: Sent pcm data")
    sentPCM = True

listening = False
current_sequence = ""
while True:
    data = ser.readline()
    try:
        data = data.decode().strip()
        print(data)
        if data == "Ready":
            audio()
    except:
        print("PY: Failed to decode byte")
        print(data)
