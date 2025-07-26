import matplotlib.pyplot as plt
import sys

def read_pcm(filename):
    with open(filename, "rb") as f:
        return list(f.read())

sine_table = [137, 166, 201, 222, 230, 222, 200, 167, 128, 89, 56, 34, 26, 34, 56, 89, 128, 167, 200, 222, 230, 222, 201, 167, 128, 89, 56, 33, 26, 33, 55, 89, 128, 167, 200, 222, 230, 223, 200, 167, 128, 89, 56, 33, 26, 34, 56, 89, 128, 167, 200, 223, 230, 222, 200, 167, 128, 89, 56, 34, 26, 33, 55, 88, 128, 167, 200, 223, 231, 223, 201, 168, 128, 89, 56, 33, 25, 33, 55, 88, 128, 167, 200, 223, 231, 223, 200, 167, 127, 88, 55, 33, 26, 33, 56, 89, 128, 167, 201, 223, 231, 224, 202, 168, 129, 89, 56, 33, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,]

def plot_pcm(data):
    plt.figure(figsize=(12, 4))
    plt.plot(data, color='red', linewidth=0.5)
    plt.title("PCM Data Plot (Unsigned 8-bit)")
    plt.xlabel("Sample Index")
    plt.ylabel("Amplitude")
    plt.ylim(0, 300)
    plt.tight_layout()
    plt.show()

if __name__ == "__main__":
    pcm_data = read_pcm("500.pcm")
    #plot_pcm(pcm_data[:100])
    plot_pcm(sine_table)
