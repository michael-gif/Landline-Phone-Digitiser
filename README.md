# Landline Phone Digitiser
Turn a landline phone into a usb device

### Contents
- [Introduction](#introduction)
- [Theory](#theory)
  - [How the phone lines work](#phone-lines)
  - [How the phone works](#phones)
- [Designing my own telephone exchange circuit (Version 1)](#design1)
  - [The plan](#the-plan)
  - [Ringing the phone](#phone-ringing)
  - [Isolating the DTMF tones](#dtmf-isolation)
  - [Amplifying the DTMF tones](#dtmf-amplification)
  - [Detecting the DTMF tones](#dtmf-detection)
  - [Detecting when the phone is off hook](#off-hook-detection)
  - [Improving the DTMF detection code](#code-improvements)
  - [Sending audio back to the phone](#ac-superimposition)
- [Designing my own telephone exchange circuit (Version 2)](#design2)
  - [The plan](#the-plan-2)
  - [Detect when the phone is off the hook](#off-hook-detection-2)
  - [Detect and decode dtmf tones](#dtmf-detection-2)
  - [Superimpose AC signals onto the phone's input](#ac-superimposition-2)
- [Writing the IVR software](#ivr-software)
- [Conclusion](#conclusion)

## Introduction <a name="introduction"/>
This project converts an analog landline phone into usb landline device you can plug into your computer. A microcontroller is used to detects keypad presses, decode and send them to a computer, and send audio back into the phone. The decoded signals can be used with IVR software and other phone systems.

This project targets the Vanguard 4001AR but can be adapted to any landline.
![Vanguard 4001AR Landline](https://www.modip.ac.uk/sites/default/files/styles/artefact_carousel_full/public/images/artefacts/000544_1.jpg?itok=sssZS4nU)
More images [here](https://www.modip.ac.uk/artefact/aibdc-000544).  
Vanguard user guide [here](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Vanguard%204001AR%20User%20Guide.pdf).

## Theory <a name="theory"/>
### How the phone lines work (UK) <a name="phone-lines"/>
In the UK we have [BT sockets](https://en.wikipedia.org/wiki/British_telephone_socket) (British RJ11 variant) in the walls for connecting phones. There are two versions: BT431A (4 wires) and BT631A (6 wires).

BT431A is used most commonly for the phone lines and carries the following signals:
| Pin | Purpose       |
| --- | ------------- |
| 2   | Leg B         |
| 3   | External Bell |
| 4   | Reserved      |
| 5   | Leg A         |

There exist adapter cables that convert between BT and RJ11 connectors. Wiring diagram below:  
![BT to RJ11 cable wiring](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/RJ11_to_BT_wiring.png)  

Manual testing of the phone lines with a multimeter yielded the following results:
- 48V DC between A and B when on-hook
- 8.2V DC between A and B when off-hook (when you pick up the handset)
- 70V+ AC on bell wire when the phone is ringing. Could be up to 90V AC.

### How a call is made <a name="phone-calls"/>
Picking up the handset closes a loop between the A and B lines, allowing current to flow from Leg B to Leg A. The telephone exchange detects this and superimposes and AC voltage onto the AB lines. The phone isolates this AC voltage and sends it to the handset speaker. This is the dial tone. 

Pressing a button on the phone's keypad causes the phone to generate two frequencies at once. Both frequencies are combined together, sent down the A line and detected by the exchange. The exchange extracts the frequencies and decodes them to the number you pressed. When you dial a phone number, the exchange detects and parses each digit one by one. After enough digits are parsed, the exchange assumes you have dialled a full number and immediately tries to connect you to whoever has that number. Et voilà, you have a phone call.

### How the phone works <a name="phones"/>
Knowing the wiring of the phone lines isn't enough to figure out how to digitise a landline phone, so I disassembled the entire phone (the vanguard 4001AR), pulled out the PCB and reverse engineered it by following all the copper traces.  

Here is an image of the PCB:  
![Vanguard PCB](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Vanguard_PCB_image.jpg)  
And the schematics I created from following the traces:  
![full circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/SCH_Vanguard_4001AR_Circuit_1-Full_Circuit_2025-06-02.png)  
![ringer circuit only](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/SCH_Vanguard_4001AR_Circuit_2-Ringer_Circuit_2025-06-02.png)  
I tried getting datasheets for the integrated circuits on the pcb but unfortunately I couldn't find anything online. The chips are decades old and probably aren't documented on the internet.

During the reverse engineering, I discovered the ringer circuitry is completely separate from the main circuit, and is powered exclusively through the external bell wire. Modern phones don't do this as the ringing voltage is superimposed on the AB lines to save money.

Up until this point I didn't actually know how a landline phone worked. So I did some research and found this instructional video from the 80s on Youtube [(source)](https://www.youtube.com/watch?v=CMZU5NdSeIg). According to the video and many other sources from google, each row and column of a phone's keypad has a unique frequency. Pressing a button combines a row and column frequency into a unique sounding tone. These tones are called DTMF tones (Dual-Tone Multi-Frequency).

The Vanguard has a 4x4 keypad with 16 buttons. All but one of these buttons use DTMF tones. 10 of them are the number buttons (0-9), 5 are special buttons, and the last button doesn't exist. Here are the special buttons:
| Special Button | Purpose |
| ------ | ------- |
| R | Recall button - Sends a pulse down the AB lines to the exchange |
| L | Last number redial - Redials the last number that was dialed. |
| S | Mute - Mutes the microphone |
| * | Unkown - Depends on the phone service |
| # | Unknown - Depends on the phone service |

# Designing my own telephone exchange circuit (version 1) <a name="design1"/>
### The plan <a name="the-plan"/>
After figuring out how the phone lines work and how the Vanguard worked, I decided to make my own telephone exchange circuit that can ring the phone, detect DTMF tones from button presses and send audio back to the phone. I estimated that this would take a week. It did not take a week. It took 7.

### Ringing the phone <a name="phone-ringing"/>
To ring the phone you need an AC voltage. The vanguard is an old design that uses the bell wire, so this seemed to be the easiest option. The first idea was to make an AC generator circuit that could output a sine wave and send it down the bell wire to the phone. I designed two circuits that take a PWM output from a microcontroller pin and turn it into AC. Both circuits were successfully tested using an ESP32 Feather as the PWM source, but the voltage of the generated AC was not high enough to ring the phone. After some thinking, I decided to abandon the idea of ringing the phone and solve it at some point in the future. The schematics for the AC circuits are below:  
![PWM to AC circuits](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/SCH_Schematic1_1-P1_2025-06-02.png)

### Isolating the DTMF tones <a name="dtmf-isolation"/>
After abandoning the idea of ringing the phone, I instead went for DTMF detection. The Vanguard needed power which was supplied using the 12V wire from an ATX power supply's [floppy drive connector](https://zoobab.wdfiles.com/local--files/atx-psu-breadboard-mod/atx-power-supply-fdd-connector.jpg), connected to Leg B of the phone cable (Note that there's a full bridge rectifier inside the phone, so it doesn't matter which Leg you connect the 12V to). A 1uF capacitor was connected to Leg A to isolate the AC component generated by the phone. The other side of the capacitor was connected to an 8Ω speaker in series with a 1k resistor. Pressing buttons on the keypad and listening to the speaker yielded great success, as I was able to hear the DTMF tones very faintly through the speaker. Circuit diagram:  
![DTMF first detection](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/DTMF_first_detection.png)

### Amplifying the DTMF tones <a name="dtmf-amplification"/>
The DTMF tones from the speaker were too quiet to detect with the ESP32, so I attempted to amplify them.

#### Failed attempts
| Strategy | Reason for failure |
| -------- | ------------------ |
| TBA820M Audio Amplifier [(datasheet)](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Datasheets/TBA820M.PDF) | Didn't work because the circuit for this chip has too many components that I don't have. I was able to get some amplification from the chip, but the output was too noisy to use since I couldn't build the full circuit |
| HA17393 Dual Op Amp [(datasheet)](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Datasheets/HA17903.PDF) | This didn't work because the chip was dead |
| APA3541 Stereo Headphone Driver [(datasheet)](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Datasheets/APA3541-1.PDF) | This didn't work because the chip is just a pass through for audio and doesn't do any amplification |

#### Successful attempts
| Strategy | Notes |
| -------- | ----- |
| Moving the 1uF capacitor from Leg B to Leg A | I don't know why, but the AC component is stronger on the positive side of the phone than it is on the negative |
| M&M CD Case and Portable Speaker | Connected the output of the 1uF capacitor to a 3.5mm jack on a breadboard and plugged the M&M speaker into the jack. The DTMF tones were amplified and audible. Image of the M&M speaker: ![M&M speaker](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/M%26M%20CD%20Case%20and%20Portable%20Speaker.jpg) |
| Single Transistor Amplifier | This is a custom circuit using an NPN transistor to amplify AC signals. The DTMF tones were amplified enough to be useable by the ESP32, and the M&M speaker wasn't needed anymore which simplified the circuit. Note that the potentiometer in this circuit needs to be set such that the transistor (Q1) voltage from collector to emitter is 2.5V. Circuit schematic:  ![Transistor amplifier circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/transistor_amplifier_circuit.png) |

### Detecting the DTMF tones <a name="dtmf-detection"/>
Once the AC signals were amplified using the transistor amplifier circuit, detecting the tones themselves became a lot easier. An ESP32 Feather was used to receive the amplified signal, perform a Fast Fourier Transform (FFT) on the signal to extract all its frequencies, and display only the ones that are DTMF tones.

DTMF tones are combinations of 8 unique frequencies:  
697Hz, 770Hz, 852Hz, 941Hz, 1209Hz, 1336Hz, 1477Hz  

And they are arranged on the keypad like so:  
![DTMF Keypad](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/dtmf_keypad.png)  

Pressing a button produces a unique tone made of a mix of the row frequency and column frequency. A demo of this can be found [here](https://onlinetonegenerator.com/dtmf.html).  

The ESP32 was setup to do a fourier transform on the signal and output the magnitudes of all 8 DTMF frequencies to the serial port, which would then be read by a python script and displayed in a graph. Two python scripts were written for this: a data recorder and a graph generator. The data recorder connects to the serial port and saves the frequency magnitudes to a text file. The graph generator uses matplotlib to generate graphs based on the magnitudes and save the graphs to pngs. Both scripts can be found [here](https://github.com/michael-gif/Landline-Phone-Digitiser/tree/main/Code/Python%20Scripts). The code used to do the fourier transform is based on an FFT library by Robin Scheibler which can be found [here](https://github.com/fakufaku/esp32-fft).

Here is one of the initial graphs showing the frequencies produced by the button sequence `11223344556677889900`:  
![Graph 1](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Labelled%20Graphs/2025_06_01_19_39_28.png)  
The X-Axis shows all the samples taken by the ESP32, and the Y-Axis shows the magnitude of the frequencies. Each frequency has a color associated with it. Check the key in the top right of the graph for clarification.

Paying close attention to the graph above, there are obvious spikes when a button is pressed, and there are obvious patterns of frequencies when specific buttons are pressed. The purple spikes in the middle are the buttons 4, 5 and 6 pressed in the sequence `445566`. The pink spikes are the buttons 7, 8 and 9 pressed in the sequence `778899` and the grey spikes are the 0 button pressed in the sequence `00`.

Note that the frequencies being detected when the buttons are pressed are completely wrong. When 7, 8 and 9 are pressed, 1477Hz is detected (pink line), even though the numbers 7 and 8 don't have 1477Hz present in them. 1, 2 and 3 are detected as a complete mess of frequencies, the numbers 4, 5 and 6 are detected as 1209Hz (purple line) even though 5 and 6 don't have 1209Hz in them, and the number 0 is detected as 1633Hz (grey line) when it shouldn't be.

Whats going wrong here is the sample rate and number of samples. The code used to generate those initial graphs is sampling at 40kHz, and each fourier transform is using 256 samples. After consulting ChatGPT, the sample rate was lowered to 8kHz, and the number of samples per fourier transform lowered from 256 to 205. The code using the fft library was also completely deleted and replaced with a function that performs the goertzel magnitude, which is the same thing as fft but can target specific frequencies instead of the whole spectrum. These 3 changes led to the errors being fixed. Here is a graph showing the results of these changes:  
![Graph 2](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Labelled%20Graphs/2025_06_02_10_36_53.png)  

Now each button press looks unique, with two obvious frequencies spiking during each press. No two buttons share the same 2 frequencies, meaning the DTMF detection works.

### Detecting when the phone is off hook <a name="off-hook-detection"/>
A 50 ohm resistor was placed in series with the phone in order to detect when the phone is off hook. When the phone is on hook the circuit is open, the current through the resistor is 0mA and so the resistor voltage is 0V. When the phone is off hook, the circuit is closed, the current through it is 50-60mA and so the resistor voltage is ~2V. Using a threshold detection in the code, an LED is turned on if the resistor voltage is higher than 1V.

### Improving the DTMF detection code <a name="code-improvements"/>
After fixing the ESP32 code for the DTMF detection, more code was written to convert pairs of frequencies into numbers. This was done by detecting when two frequencies were spiking, summing those frequencies together to get a unique number, and using a switch statement to map the sum to its keypad number.

When testing this code by pressing buttons on the keypad, the following was produced which showed that the switch statement was correctly decoding the frequencies:  
![Serial Output](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/example_output.png)  

The circuit and code worked, but were extremely unstable. The transistor amplifier has a potentiometer to bias the inputted AC signal before it gets amplified, and simply touching the breadboard could mess up that bias and break the amplification. To fix this the potentiometer was replaced with fixed value resistors to keep the bias stable.

As for the code, it uses two thresholds to detect specific frequencies. When a frequency exceeded the first threshold, it was considered the start of a DTMF tone. When it dipped below the second threshold it was considered the end of a DTMF tone. This is a primitive, unstable technique that isn't reliable. If the maximum amplitude of the frequencies went below the first threshold then the detection no longer worked.

A new method of detection was developed such that only one threshold is needed, and that one threshold is for detecting the presence of a DTMF tone rather than the presence of specific frequencies. If a tone is detected, the top two most prevalent frequencies are extracted and converted into a number. This new method is much more reliable as the top two frequencies are always guaranteed to be the correct ones, and the maximum amplitude can drop and still be detected since the DTMF frequencies will always exceed the noise level. The threshold can be set to just above the noise level and the DTMF tones will always be detected.

### Sending audio back to the phone <a name="ac-superimposition"/>
After fixing the code I noticed a new problem. You can dial numbers and send signal, but you can't receive signals or audio. To fix this an extra circuit is needed that can superimpose AC signals onto the phones 12V line. The phone will automatically isolate this AC audio signal from the 12V and send it to the handset's speaker, allowing you to hear the signal. Superimposing AC onto DC means combining the two together so that you end up with the same AC signal, but offset by the DC.

My initial research on a circuit that can do this job led to [this article](https://masteringelectronicsdesign.com/injecting-ac-to-dc-power-supply/) which uses a capacitor and inductor to inject AC into DC. I didn't have any inductors so I modified the circuit to use a diode instead of an inductor. The new circuit was simulated in LTSpice to see if it could work:  
![LTSpice simulation](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/ac_superimposer_circuit_v1.png)  
On the top half of the image is a graph showing the voltages at different points in the circuit. The bottom half is the circuit itself. On the graph, the green line is a PWM signal that can be produced by a microcontroller. The pink line is the output of the RC filter formed by R2 and C2 that only allows frequencies less than 1591Hz through, and the red line is the right side of C1 which isolates the AC component. This AC component is then combined with the 5V DC outputed from D1, and the combined voltages are sent to R1 which simulates the Vanguard as a load.

The circuit was tested in real life on a breadboard, replacing the 5V part with the 12V from the power supply and the circuit worked just fine. The microcontroller was able to output a 500Hz PWM signal into the circuit, the circuit superimposed the signal onto the 12V line going into the phone and the phone was able to isolate the AC signal and send it to the speaker in the handset, resulting in an audible 500Hz tone when you put the handset speaker to your ear.

# Designing my own telephone exchange circuit (Version 2) <a name="design2"/>
### The plan <a name="the-plan-2">
I decided to revisit this project a year later, and man was I stupid a year ago. I'll skip all the theory since its the same. I wanted to design a new circuit that wasn't as awful as the first. I fried the ESP32 a while back, so I bought a Raspberry Pi Pico for a fraction of the price and used for the new circuit.

There are 3 goals for the new circuit:
- Detect when the phone is off the hook
- Detect and decode DTMF tones
- Superimpose AC signals onto the phone's input (the 12V line)

### Detect when the phone is off the hook <a name="off-hook-detection-2"/>
This is pretty simple. The phone has a full bridge rectifier inside, so you can connect 12V and ground to pins 2 and 5 of the phone in either direction and the phone will turn on regardless. I decided on 12V going to pin 2, and GND going to pin 5. This made the breadboarding a bit easier.

A 47 ohm capacitor was connected in series between the output of the phone (pin 5) and GND. If you measure the voltage around the resistor, it is 0V when the phone is on-hook, and ~2V when the phone is off-hook. This can be read by the ADC of a microcontroller, with a 1V threshold set in the code. When the voltage exceeds the threshold, the microcontroller turns on an LED. Schematic:  
![Hook Detection circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/hook_detection_circuit_v2.png)

### Detect and decode dtmf tones <a name="dtmf-detection-2"/>
This is also pretty simple. The DTMF tones are AC signals that are superimposed onto the output of the phone. Since we have 12V connected to pin 2, the AC signals are avaiable on pin 5. Connecting a 1uF capacitor to pin 5 isolates the AC signals which can be sent to an op amp for amplification. The amplified signals can then be sent to a microcontroller for software decoding. Isolation and amplification schematic:  
![DTMF Detection circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/dtmf_detection_circuit_v2.png)

Note that the amplifier circuit is optional. If you program the microcontroller's ADC to have a higher resolution, its possible to detect the AC signals without any amplification circuitry. I tested this with the Pico by changing its ADC resolution from 10 to 12 bits with `analogReadResolution(12);`. The Pico was able to pick up the signals without using the op amp.

To decode the DTMF tones, the AC signals are read by the microcontroller and turned into frequency amplitudes using the Goertzel algorithm [(wiki link)](https://en.wikipedia.org/wiki/Goertzel_algorithm). The code used is the same that was used last year. It is a function that when given samples of a signal and a target frequency, will output the magnitude of that frequency. If the target frequency is present in the samples, the magnitude will be high, otherwise it will be low. It's a surprisingly fast function for something written by ChatGPT a year ago. The function:  
```c++
int goertzelMagnitude(const float* samples, int num_samples, float target_freq, float sample_rate) {
    float k = 0.5f + ((num_samples * target_freq) / sample_rate);
    int bin = (int)k;
    float omega = (2.0f * M_PI * bin) / num_samples;
    float sine = sinf(omega);
    float cosine = cosf(omega);
    float coeff = 2.0f * cosine;

    float q0 = 0, q1 = 0, q2 = 0;
    for (int i = 0; i < num_samples; i++) {
        q0 = coeff * q1 - q2 + samples[i];
        q2 = q1;
        q1 = q0;
    }

    float real = (q1 - q2 * cosine);
    float imag = (q2 * sine);
    return sqrt(real * real + imag * imag);
}
```

There is a loop that runs this function on all the 8 DTMF frequencies. Every time it is run, the two highest frequencies detected are updated. Once all 8 frequencies have been checked, you are left with the two with the highest magnitudes, which when combined produce your DTMF tone. If you sum the frequencies you get a unique number. This unique number is given to a switch statement that maps the frequency sums to the keypad numbers. And thus, we have DTMF decoding. Here is the switch statement:  
```c++
String getKeypadButtonPressed(int freq1, int freq2) {
  switch (freq1 + freq2) {
    case 1906:
      return "1";
    case 2033:
      return "2";
    case 2174:
      return "3";
    case 1979:
      return "4";
    case 2106:
      return "5";
    case 2247:
      return "6";
    case 2061:
      return "7";
    case 2188:
      return "8";
    case 2329:
      return "9";
    case 2277:
      return "0";
    case 2150:
      return "*";
    case 2418:
      return "#";
    default:
      return "?";
  }
}
```

### Superimpose AC signals onto the phone's input <a name="ac-superimposition-2"/>
This part was tricky. I needed a circuit that could take 3.3V output from a microcontroller's gpio and combine it with the 12V input line into the phone. After some research I discovered you could bias the 3.3V output to 12V, but it I still couldn't figure out how to combine it with the phone's 12V input. I tried several combinations of resistors, capacitors and transistors to try and make this work. I was able to get it working with a series resistor and capacitor which connected to a 12V bias that the phone was in series with:  
![AC superimposer circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/ac_superimposer_circuit_v2a.png)

The pink line is a 3.3V PWM signal. The green line is the the junction between R7 and C3, and the red line is the 12V line with the PWM signal superimposed onto it.

Although I wasn't happy with this. Having a single tiny capacitor between a sensitive 3.3V gpio pin and a beefy 12V didn't seem safe. I opted not to use this circuit and to design something better that could isolate the 12V line from the 3.3V gpio. After several days of thinking, testing and researching, I finally found my solution: the optocoupler.

An optocoupler is a device that can isolate high voltage from low voltage. And if you use it right, it can pass AC signals from one side to the other. Here is the new circuit using an optocoupler:  
![AC superimposer circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/ac_superimposer_circuit_v2b.png)

The pink line is the 3.3V PWM signal. The green line is the optocoupler's output at R3, and the red line is the 12V line with the signal superimposed onto it. Note that the signal is more sine wave like. This is because of the 1uF capacitor (C2) connected to the optocoupler output at R3. I thought it would be nice if the PWM was less sharp on the ears.

Testing of the optocoupler circuit showed it working perfectly. PWM signals from the Pico were being turned into superimposed signals on the phone's 12V input which were being isolated by the phone and sent to the handset speaker, making them audible when you put the handset to your ear. With some simple coding I was able to play the UK national anthem on the handset speaker by changing the frequency of the Pico's PWM output.

# Writing the IVR software <a name="ivr-software"/>
Once the AC superimposing circuit was tested, work began on writing code for the ESP32 to allow it to receive commands from a Python script that acts as IVR software. IVR stands for Interactive Voice Response and is the system used for automating phone systems between businesses and customers. Instead of being directed to a person, a customer is directed to a robotic menu that reads out options and listens for button presses.

Getting the phone to work with a computer requires this kind of software, so I wrote a Python script that can read the ESP32's output from the serial port, and send a sequence of tones back to the ESP32 which can be sent to the handset. This way you can dial a number, have the script detect it and play back a nice sounding sequence of tones into your ear. You can also dial a number and have it open a website.

Each tone sequence follows the format `[(frequency, durationMillis),...]`. So a sequence consisting of `[(500,100),(750,250)]` would play a 500Hz tone for 100ms, immediately followed by a 750Hz tone for 250ms.  
The following tone sequences were chosen for playback:
| Sequence | Description |
| -------- | ----------- |
| `[(950, 330), (1400, 330), (1800, 330)]` | Number doesn't exist|
| `[(440, 100), (494, 100), (523, 200)]` | User has entered the IVR menu|
| `[(440, 150), (660, 150), (880, 330)]` | IVR menu item is valid |
| `[(950, 330), (1400, 330), (1800, 330)]` | IVR menu item is invalid |

However I wasn't happy that you couldn't have the phone do what it was designed to do, which is to play audio messages, so I added support for PCM file playback. Now you can send a PCM file with a voice message to the speaker of the phone rather than a bland tone sequence. The quality of the audio is abysmal when you hear it, but it's just good enough to make out words and understand what is being said.

# Conclusion <a name="conclusion"/>
With the ESP32 code and the Python script, phone numbers and IVR systems can be created that allow the Vanguard to do many things. Such things involve opening websites, opening programs, sending emails, sending messages etc etc

# Full circuit <a name="full-circuit"/>
### Version 1 (outdated, don't use)
Schematic
![Telephone Exchange Schematic](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/telephone_exchange_schematic_v1.png)

Breadbord implementation
![Breadboard Circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/breadboard_circuit_v1.jpg)

### Version 2 (latest version, use this instead)
Schematic  
![Telephone Exchange Schematic](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/telephone_exchange_schematic_v2.png)

Breadboard implementation  
![Breadboard Circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/breadboard_circuit_v2.jpg)

# TODO
- Write new software to send commands to and from the microcontroller using the new circuit