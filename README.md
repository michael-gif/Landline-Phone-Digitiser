# Landline Phone Digitiser
This project converts an analog landline phone into usb landline device you can plug into your computer. A microcontroller detects keypad presses and transmitts them to a computer. Software can intepret these signals for various things such as opening applications, creating IVR systems etc.

This project targets the Vanguard 4001AR but can be adapted to any landline.
![Vanguard 4001AR Landline](https://www.modip.ac.uk/sites/default/files/styles/artefact_carousel_full/public/images/artefacts/000544_1.jpg?itok=sssZS4nU)
More images [here](https://www.modip.ac.uk/artefact/aibdc-000544).  
Vanguard user guide [here](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Vanguard%204001AR%20User%20Guide.pdf).

# How the phone lines work (UK)
In the UK we have BT sockets (British RJ11 variant) in the walls for connecting phones. They come in two types: BT431A (4 wires) and BT631A (6 wires). Learn more about the BT Socket [here](https://en.wikipedia.org/wiki/British_telephone_socket).  

BT431A is enough for the phone lines and carries the following signals:
| Pin | Purpose       |
| --- | ------------- |
| 2   | Leg B         |
| 3   | External Bell |
| 4   | Reserved      |
| 5   | Leg A         |

Uk landline phones have adapter cables that convert between BT plugs and RJ11 plugs. Wiring diagram below:  
![BT to RJ11 cable wiring](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/RJ11%20to%20BT%20wiring.png)  

Testing the phone lines with a multimeter yielded the following results:
- 48V DC between A and B when on-hook
- 8.2V DC between A and B when off-hook (when you pick up the handset)
- More than 70V AC on bell wire when the phone is ringing

Picking up the handset closes a loop between the A and B lines. The telephone exchange detects this and sends an audible dial tone (AC voltage) that is superimposed on the AB lines. Pressing a button on the keypad causes the phone to generate a dual frequency tone that is sent down the AB lines and is detected by the exchange. Dialling a full number tells the exchange to connect you with whoever you dialled.

# How the phone works
Knowing the wiring of the phone lines isn't enough to figure out how to digitise the phone, so I disassembled the phone, pulled out the PCB and stared at it for 3 hours straight.  

Here is an image of the PCB:  
![Vanguard PCB](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Vanguard%20PCB%20image.jpg)  
And the schematics I created from looking at the traces:  
![full circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/SCH_Vanguard%204001AR%20Circuit_1-Full%20Circuit_2025-06-02.png)  
![ringer circuit only](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/SCH_Vanguard%204001AR%20Circuit_2-Ringer%20Circuit_2025-06-02.png)  
I tried getting datasheets for the integrated circuits on the pcb but I couldn't find anything online.

Reverse engineering the PCB led to the discovery that the ringer circuitry is completely separate from the main circuit, and is powered exclusively through the external bell wire.

I then did some research on how these landline phones worked, and after watching an instructional video from the 80s on Youtube [(source)](https://www.youtube.com/watch?v=CMZU5NdSeIg), found that each row of the keypad has its own frequency and so does each column. Pressing a button combines two of these frequencies into a unique tone. These tones are called DTMF tones (Dual-Tone Multi-Frequency).

The Vanguard also has 5 special buttons:
| Button | Purpose |
| ------ | ------- |
| R | Recall button. Sends a pulse down the AB lines to the exchange |
| L | Last number redial. Redials the last number that was dialed. |
| S | Mutes the microphone |
| * | Unkown |
| # | Unknown |

# The plan
After figuring out how the phone lines work and how the Vanguard worked, I formulated a plan:  

Design my own telephone exchange circuit that can ring the phone, detect DTMF tones and send audio down back to the phone.

I estimated that this would take a week. It did not take a week. It took 7.

# Designing the telephone exchange circuit
### Ringing the phone
Started by trying to ring the phone using the bell wire since it seemed easiest. The first idea was to make an AC generator circuit that could output a sine wave and send it down the bell wire to the phone. I designed two circuits that take a PWM output from a microcontroller pin and turn it into AC. Both circuits were successfully tested using an ESP32 Feather as the PWM source, but the generated AC was not enough to ring the phone. I don't know if its the voltage, current, or both that are too low for the phone ringer circuit. The schematics for the circuits are below:  
![PWM to AC circuits](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/SCH_Schematic1_1-P1_2025-06-02.png)

### Isolating the DTMF tones
After abandoning the idea of ringing the phone, I instead went for DTMF detection. Powering the Vanguard was done by using the 12V wire (the yellow one) from an ATX power supply's [floppy drive connector](https://zoobab.wdfiles.com/local--files/atx-psu-breadboard-mod/atx-power-supply-fdd-connector.jpg) connected to Leg A of the phone cable. Connecting a 1uF capacitor to Leg B isolated the AC component generated by the phone from the 12V. The other side of the capacitor was connected to an 8Ω speaker. Pressing buttons on the keypad and listening to the speaker yielded great success, as I was able to hear the DTMF tones very faintly through the speaker.

### Amplifying the DTMF tones
The DTMF tones from the speaker were too quiet to detect with the ESP32, so I attempted to amplify them.

Failed attempts:
- TBA820M Audio Amplifier [(datasheet)](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Datasheets/TBA820M.PDF)
  - Didn't work because the circuit for this chip has too many components that I don't have. I was able to get some amplification from the chip, but the output was too noisy to use since I couldn't build the full circuit
- HA17393 Dual Op Amp [(datasheet)](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Datasheets/HA17903.PDF)
  - This didn't work because the chip was dead
- APA3541 Stereo Headphone Driver [(datasheet)](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Datasheets/APA3541-1.PDF)
  - This didn't work because the chip is just a pass through for audio and doesn't do any amplification

Successful attempts:
- Moving the 1uF capacitor from Leg B to Leg A
  - I don't know why, but the AC component is stronger on the positive side of the phone than it is on the negative
- M&M CD Case and Portable Speaker
  - Connected the output of the 1uF capacitor to a 3.5mm jack on a breadboard and plugged the M&M speaker into the jack. The DTMF tones were amplified and audible. Image of the M&M speaker:
  ![M&M speaker](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/M%26M%20CD%20Case%20and%20Portable%20Speaker.jpg)
- Single Transistor Amplifier
  - This is a custom circuit using an NPN transistor to amplify AC signals. The DTMF tones were amplified enough to be useable by the ESP32, and the M&M speaker wasn't needed anymore which simplified the circuit. Note that the potentiometer in this circuit needs to be set such that the transistor (Q1) voltage from collector to emitter is 2.5V. Circuit schematic:  
  ![Transistor amplifier circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/transistor%20amplifier%20circuit.png)

### Detecting the DTMF tones
Once the AC signals were amplified using the transistor amplifier circuit, detecting the tones themselves became a lot easier. An ESP32 Feather was used to receive the amplified signal, perform a Fast Fourier Transform (FFT) on the signal to extract all its frequencies, and display only the ones that are DTMF tones.

DTMF tones are made of the following 8 frequencies:  
697Hz, 770Hz, 852Hz, 941Hz, 1209Hz, 1336Hz, 1477Hz  

And they are arranged on the keypad like so:  
![DTMF Keypad](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/dtmf%20keypad.png)  

Pressing a button produces a tone made of its row frequency and column frequency played at the same time. A demo of this can be found [here](https://onlinetonegenerator.com/dtmf.html).  

The ESP32 was setup to do a fourier transform on the signal and output the magnitudes of all 8 DTMF frequencies to the serial port, which would then be read by a python script and displayed in a graph. Two python scripts were written for this: a data recorder and a graph generator. The data recorder connects to the serial port and saves the frequency magnitudes to a text file. The graph generator uses matplotlib to generate graphs based on those magnitudes and saves the graphs to pngs. Both scripts can be found [here](https://github.com/michael-gif/Landline-Phone-Digitiser/tree/main/Code/Python%20Scripts). The code used to do the fourier transform is based on an FFT library by Robin Scheibler which can be found [here](https://github.com/fakufaku/esp32-fft).

Here is one of the initial graphs showing the frequencies produced by the button sequence `11223344556677889900`:  
![Graph 1](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Labelled%20Graphs/2025_06_01_19_39_28.png)  
The X-Axis shows all the samples taken by the ESP32, and the Y-Axis shows the magnitude of the freqeuncies. Each frequency has a color associated with it. Check the key in the top right of the graphs for clarification.

Paying close attention to the graphs above, there are obvious spikes when a button is pressed, and there are obvious patterns of frequencies when certain buttons are pressed. The purple spikes in the middle are the buttons 4, 5 and 6 pressed in the sequence `445566`. The pink spikes are the buttons 7, 8 and 9 pressed in the sequence `778899` and the grey spikes are the 0 button pressed in the sequence `00`.

The frequencies being detected when the buttons are pressed are completely wrong. When 7, 8 and 9 are pressed, 1477Hz is detected (pink line), even though the numbers 7 and 8 don't have 1477Hz present in them. 1, 2 and 3 are detected as a complete mess of frequencies, the numbers 4, 5 and 6 are detected as 1209Hz (purple line) even though 5 and 6 don't have 1209Hz in them, and the number 0 is detected as 1633Hz (grey line) when it shouldn't be.

Whats going wrong here is the sample rate and number of samples. The ESP32 code used to generate those initial graphs is sampling at 40kHz, and each fourier transform is using 256 samples. After consulting ChatGPT, the sample rate was lowered to 8kHz, and the number of samples per fourier transform lowered from 256 to 205. The code using the fft library was also completely deleted and replaced with a function that performs the goertzel magnitude, which is the same thing as fft but can target specific frequencies instead of the whole spectrum. These 3 changes led to the errors being fixed. Here is a graph showing the results of these changes:  
![Graph 2](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Labelled%20Graphs/2025_06_02_10_36_53.png)  

As you can see, each button press looks much different with two obvious frequencies spiking during each press. Whats better is the obvious difference between each button press. No two buttons share the same combination of frequency spikes, meaning the DTMF detection works.

### Detecting when the phone is off hook
A 50 ohm resistor was placed in series with the phone in order to detect when the phone is off hook. When the phone is on hook the circuit is open and the resistor voltage is 0V. When the phone is off hook, the circuit is closed and the resistor voltage is >0V. From testing, the resistor voltage is 2.8~3.0V when the phone is off hook.

### Improving the DTMF detection code
After rewriting the ESP32 code for the DTMF detection, more code was written to convert pairs of frequencies into numbers. This was done by detecting when two frequencies were spiking, summing those frequencies together and using a switch statement to map the sum to its keypad number. Since each frequency is unique, their sums are also unique which makes this possible.

When testing this code by pressing buttons on the keypad, the following was produced:  
![Serial Output](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/example%20output.png)  

The circuit and code worked, but were extremely unstable. The circuit had a potentiometer to bias the isolated AC signal before it gets amplified and processed by the ESP32, and simply touching the breadboard could mess up that bias and break the detection. To fix this the potentiometer was replaced with 3 resistors which kept the bias constant.

As for the code, it used two thresholds to detect specific frequencies. When a frequency exceeded the first threshold, it was considered the start of a DTMF tone. When it dipped below the second threshold it was considered the end of a DTMF tone. This is an archaic, finnicky technique that wasn't reliable. If the maximum amplitude of the frequencies went below the first threshold then the detection no longer worked.

A new method of detection was developed such that only one threshold is needed, and that one threshold is for detecting the presence of a DTMF tone rather than the presence of specific frequencies. Once a tone is detected, the top two most prevalent frequencies are extracted and converted into a number. This new method is much more reliable as the top two frequencies are always guaranteed to be the correct ones, and the maximum amplitude can drop and still be detected since the DTMF frequencies will always exceed the noise level. The threshold can be set to just above the noise level and the DTMF tones will always be detected.

### Sending audio back to the phone
The new code works very well, but there is a problem. You can dial numbers, but you can't receive audio. To fix this an extra circuit is needed that can superimpose AC audio signals onto the phones 12V Leg A line. The phone will automatically isolate this AC audio signal from Leg A and send it to the speaker in the handset allowing you to hear the audio. Superimposing AC onto DC means combining the two together so that you end up with the same AC but offset by the DC.

My initial research on a circuit that can do this job led to [this article](https://masteringelectronicsdesign.com/injecting-ac-to-dc-power-supply/) which uses a capacitor and inductor to inject AC into DC. I didn't have any inductors so I modified the circuit to use a diode instead of an inductor. The new circuit was simulated in LTSpice to see if it could work:  
![LTSpice simulation](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/LTSpice%20ac%20superimpose%20simulation.png)  
On the top half of the image is a graph showing the voltages at different points in the circuit. The bottom half is the circuit itself. On the graph, the green line is a PWM signal that can be produced by a microcontroller. The pink line is the output of the RC filter formed by R2 and C2 that only allows frequencies less than 1591Hz through, and the red line is the right side of C1 which isolates the AC component. This AC component is then combined with the 5V DC outputed from D1, and the combined voltages are sent to R1 which simulates the Vanguard as a load.

The circuit was tested in real life on a breadboard, replacing the 5V part with the 12V from the power supply and the circuit worked just fine. The microcontroller was able to output a 500Hz PWM signal into the circuit, the circuit superimposed the signal onto the 12V line going into the phone and the phone was able to isolate the AC signal and send it to the speaker in the handset, resulting in an audible 500Hz tone when you put the handset speaker to your ear.

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

With the ESP32 code and the Python script, phone numbers and IVR systems can be created that allow the Vanguard to do many things. Such things involve opening websites, opening programs, sending emails, sending messages etc etc

Here is the full telephone exchange circuit:  
![Telephone Exchange Schematic](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/Schematics/Telephone%20Exchange%20Schematic.png)

And here is the exchange circuit on a breadboard:
![Breadboard Circuit](https://github.com/michael-gif/Landline-Phone-Digitiser/blob/main/Resources/breadboard_circuit.jpg)