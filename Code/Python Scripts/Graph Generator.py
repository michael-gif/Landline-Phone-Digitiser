import matplotlib.pyplot as plt
import os

folder_path = 'raw_data'
file_paths = [f.split(".")[0] for f in os.listdir(folder_path) if os.path.isfile(os.path.join(folder_path, f))]
file_paths = ["2025_06_02_10_45_57", "2025_06_02_10_46_06", "2025_06_02_10_46_16"]

def on_pick(event):
    legend = event.artist
    isVisible = legend.get_visible()

    graphs[legend].set_visible(not isVisible)
    legend.set_visible(not isVisible)

    fig.canvas.draw()

for file_path in file_paths:
    with open("raw_data/" + file_path + ".txt", 'r') as file:
        lines = file.readlines()

    data = [list(map(int, line.strip().split(' '))) for line in lines]
    transposed = list(zip(*data))

    fig, ax = plt.subplots(figsize=(14, 8))
    labels = ['697Hz', '770Hz', '852Hz', '941Hz', '1209Hz', '1336Hz', '1477Hz', '1633Hz']
    plotted_lines = []
    indices_to_skip = {}
    for i, freq_series in enumerate(transposed):
        if i in indices_to_skip:
            continue
        line, = ax.plot(range(len(freq_series)), freq_series, label=f'{labels[i]}')
        plotted_lines.append(line)

    plt.xlabel('Sample')
    plt.ylabel('Frequency')
    plt.title('DTMF Tones')
    #plt.legend()

    legend = plt.legend(loc='upper right')
    legend_lines = legend.get_lines()
    for leg_line in legend_lines:
        leg_line.set_picker(True)
        leg_line.set_pickradius(10)

    graphs = {}
    for i, leg_line in enumerate(legend_lines):
        graphs[leg_line] = plotted_lines[i]
    
    plt.grid(True)
    fig = plt.gcf()
    fig.set_size_inches(18.5, 10.5)
    figManager = plt.get_current_fig_manager()
    figManager.full_screen_toggle()
    ax.set_ylim(top=30000)

    plt.savefig(f'graphs/{file_path}.png')

    #plt.connect('pick_event', on_pick)
    #plt.show()
    
    #plt.clf()
    #plt.cla()
    #plt.close()

