import numpy as np
import wave
import struct
from scipy.signal import sawtooth

def generate_tone_sequence(tone_sequence, sample_rate=44100, amplitude=8000):
    audio_data = np.array([], dtype=np.int16)
    
    for freq, duration_ms in tone_sequence:
        duration_sec = duration_ms / 1000.0
        t = np.linspace(0, duration_sec, int(sample_rate * duration_sec), endpoint=False)
        waveform = amplitude * sawtooth(2 * np.pi * freq * t, width=0.5)
        audio_data = np.concatenate((audio_data, waveform.astype(np.int16)))
    
    return audio_data

def save_wave(filename, audio_data, sample_rate=44100):
    with wave.open(filename, 'w') as wf:
        wf.setnchannels(1)  # mono
        wf.setsampwidth(2)  # 16 bits
        wf.setframerate(sample_rate)
        for sample in audio_data:
            wf.writeframes(struct.pack('<h', int(sample)))

# Example usage
tone_sequence = [(950, 330), (1400, 330), (1800, 330)]
audio_data = generate_tone_sequence(tone_sequence)
save_wave("output.wav", audio_data)
