import board
import busio

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
            if buffer == b'\x0A':
                print('ON_HOOK')
            elif buffer == b'\x0B':
                print('OFF_HOOK')
            else:
                # read the bytes two at a time and turn them back into numbers
                for i in range(0, len(buffer), 2):
                    print(int.from_bytes(buffer[i:i+2], byteorder='little'), end="")
                    print(',', end="")
                print()
            buffer = b''
        else:
            buffer += data
            