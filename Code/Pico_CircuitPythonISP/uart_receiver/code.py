import board
import busio
import struct

PIN_TX = board.GP16
PIN_RX = board.GP17
uart = busio.UART(PIN_TX, PIN_RX, baudrate=9600)
uart.reset_input_buffer()
in_message = False
buffer = b''
while True:
    if not uart.in_waiting:
        continue
    data = uart.read(1)
    if not in_message:
        if data == b'<':
            in_message = True
    else:
        if data == b'>':
            in_message = False
            if buffer[0] == 0:
                if buffer[1] == 10:
                    print('ON_HOOK')
                elif buffer[1] == 11:
                    print('OFF_HOOK')
            elif buffer[0] == 1:
                print(f'New Delay: {buffer[1]}us')
            else:
                # read the bytes a chunk at a time and turn them back into numbers
                buffer = buffer[1:]
                chunk_size = 2
                freqs = [697, 770, 852, 941, 1209, 1336, 1477]
                freqIndex = 0;
                for i in range(0, len(buffer), chunk_size):
                    val = int.from_bytes(buffer[i:i+chunk_size], byteorder='little')
#                    if val < 8 and (i == 0 or i == 8):
#                        print(f'F:{freqs[val]}', end=',')
#                    else:
#                        print(f'M:{val}', end=',')
                    print(f'{freqs[freqIndex]}:{val}', end=',')
                    freqIndex += 1
                print()
            buffer = b''
        else:
            buffer += data
            