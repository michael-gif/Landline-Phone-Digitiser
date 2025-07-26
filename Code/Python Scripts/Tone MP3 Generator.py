from pydub import AudioSegment
from pydub.generators import Square

# List of (frequency in Hz, duration in ms)
tone_sequence = [
    (440, 300),
    (660, 200),
    (880, 400),
    (550, 150),
    (440, 500)
]

silence_duration_ms = 100
sequence = AudioSegment.silent(duration=0)

for freq, duration in tone_sequence:
    tone = Square(freq).to_audio_segment(duration=duration).set_frame_rate(44100).set_sample_width(2).set_channels(1)
    silence = AudioSegment.silent(duration=silence_duration_ms)
    sequence += tone + silence

sequence.export("custom_square_sequence.mp3", format="mp3")
