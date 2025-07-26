import serial
import threading
import sys
import subprocess
import webbrowser
import os
import math


ser = serial.Serial('COM3', 500000)

events = {}
def register_number(phone_number: str):
    """
    This is a decorator for functions that turns them into a dialable function.
    phone_number is a phone number as a string that when dialed, will call the function that is decorated.
    """
    def decorator(function):
        events[phone_number] = function
        print(f"Registered sequence '{phone_number}' to function '{function.__name__}'")
        return function # don't need to wrap the function
    return decorator

def send_tone_sequence(tones: list):
    """
    Takes a list of tuples in the format [(frequency, duration),...]
    and turns them into a string message that is sent to the ESP32
    """
    # send command to play sequence
    start = "TONESEQUENCE_"
    args = ""
    for tone in tones:
        args += str(tone[0]) + "_" + str(tone[1]) + "_"
    args = args[:-1]
    msg_len = len(start) + len(args)
    message = f"{msg_len}_{start}{args}"
    print("Sent: " + message)
    ser.write(message.encode("utf-8"))

    # wait for tone sequence to complete
    while True:
        data = ser.readline().decode().strip()
        print(data)
        if data.startswith("Sent tone sequence"):
            return

def send_pcm_audio(filename: str):
    print("Opening PCM")
    with open(filename, 'rb') as f:
        data = f.read()
    start = "AUDIO_"
    msg_len = len(start) + len(data)
    msg = str(msg_len).encode() + b"_" + start.encode() + data
    print("Writing " + str(msg_len) + " bytes")
    packets = math.ceil(len(msg) / 120)
    for i in range(packets):
        start = i * 120
        end = start + 120
        ser.write(msg[start:end])
        while True:
            try:
                data = ser.readline().decode().strip()
                if data == "1":
                    break
            except:
                print("Failed to decode byte")
                print(data)
    print("Sent pcm data")
    while True:
        try:
            data = ser.readline().decode().strip()
            if data == "Played PCM data":
                break
            if data == "ON_HOOK":
                print("ON_HOOK detected, playback cancelled")
                break
        except:
            print("Failed to decode byte")

def listen_for_button():
    """
    Listens for a keypad button press on the physical phone
    Returns -1 if the phone is put on hook while listening
    """
    while True:
        data = ser.readline().decode().strip()
        if data == "ON_HOOK":
            print("ON_HOOK detected, menu cancelled")
            return -1
        if not data.startswith("K"):
            continue
        if data == "KEY_?":
            continue
        detected_number = data.split("_")[1]
        return detected_number


def listen():
    print("Listening")
    while True:
        data = ser.readline()
        try:
            print(data.decode().strip())
        except:
            print("Failed to decode data")
            print(data)

@register_number("123")
def phone_number_1():
    webbrowser.open("https://www.youtube.com")

@register_number("456")
def phone_number_2():
    webbrowser.open("https://chatgpt.com/")

@register_number("#")
def ivr_system():
    send_tone_sequence([(440, 100), (494, 100), (523, 200)]) # notify user they have accessed the ivr system

    print("Waiting for button press")
    button = listen_for_button() # wait for a button press
    if button == -1:
        return
    
    print("Detected:", button)
    if button in ["1", "2", "3", "4", "5", "6"]:
        send_pcm_audio("valid_number.pcm")
        print("Button is valid")
    else: # if the button isn't in the menu then notify them with sad tones then exit the ivr system
        send_pcm_audio("invalid_number.pcm")
        print("Invalid button. Cancelling menu")
        return
    
    if button == "1":
        print("Option 1")
    if button == "2":
        print("Option 2")
    if button == "3":
        print("Option 3")
    if button == "4":
        print("Option 4")
    if button == "5":
        print("Option 5")
    if button == "7":
        print("Option 7")
    if button == "8":
        print("Option 8")
    if button == "9":
        print("Option 9")
    if button == "0":
        print("Option 0")

listening = False
current_sequence = ""
while True:
    try:
        data = ser.readline().decode().strip()
    except:
        print("Failed to decode byte")
        continue
    if data == "OFF_HOOK":
        listening = True
        print("OFF_HOOK")
    if data == "ON_HOOK":
        listening = False
        if current_sequence:
            print()
        print("ON_HOOK")
        current_sequence = ""
    if listening:
        if not data.startswith("K"):
            continue
        if data == "KEY_?":
            continue
        detected_number = data.split("_")[1]
        current_sequence += detected_number
        print(detected_number, end="", flush=True)
        if data == "KEY_*":
            print()
            current_sequence = current_sequence[:-1]
            found_sequence = False
            callback_to_execute = None
            for event in events:
                if event == current_sequence:
                    function_name = events[current_sequence].__name__
                    print(f"Triggered function: {function_name}")
                    found_sequence = True
                    send_pcm_audio("valid_number.pcm")
                    callback_to_execute = events[current_sequence]
            if not found_sequence:
                print("Sequence does not exist")
                send_pcm_audio("invalid_number.pcm")
            current_sequence = ""

            # execute the function registered to the phone number in a manner such that further functions can be executed without bloating the cpu stack.
            # this works well with a node system since every node can have a main function.
            # once a node's main function is executed, the node can return a pointer to another node that contains another main function.
            # thus you can make a calling tree out of nodes which directs the calling party from one node to another without bloating the cpu stack with function overhead
            if found_sequence:
                next_callback = None
                while True:
                    next_callback = callback_to_execute()
                    if next_callback is None:
                        break
