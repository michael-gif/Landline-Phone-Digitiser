using Northwoods.Go;
using Northwoods.Go.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telephone_IVR
{
    class SerialListener
    {
        static Diagram diagram;
        static SerialPort ser = new SerialPort("COM3", 115200);
        public static bool running = false;
        public static Thread listenerThread;
        static Dictionary<string, Node> registeredNumbers = new Dictionary<string, Node>();
        static volatile bool listening = false;
        static string currentSequence = "";

        static void RegisterNumber(string phoneNumber, Node nextNode)
        {
            registeredNumbers[phoneNumber] = nextNode;
            Console.WriteLine($"Registered sequence '{phoneNumber}' to function '{nextNode.Category}'");
        }

        static void SendToneSequence(List<(int, int)> tones)
        {
            string message = "TONESEQUENCE_";
            foreach (var tone in tones)
            {
                message += tone.Item1.ToString() + "_" + tone.Item2.ToString() + "_";
            }
            message = message.TrimEnd('_');
            ser.Write(message);

            while (true)
            {
                string data = ser.ReadLine().Trim();
                if (data.StartsWith("Sent tone sequence"))
                    return;
            }
        }

        static string ListenForButton()
        {
            Console.Write ("Listening for button: ");
            while (true)
            {
                string data = ser.ReadLine().Trim();
                if (data == "ON_HOOK")
                {
                    Console.WriteLine("ON_HOOK detected, menu cancelled");
                    return "-1";
                }
                if (!data.StartsWith("K")) continue;
                if (data == "KEY_?") continue;

                string detectedNumber = data.Split('_')[1];
                Console.WriteLine(detectedNumber);
                return detectedNumber;
            }
        }

        static Node GetNextNode(Node node)
        {
            int numLinks = node.LinksConnected.Count();
            if (numLinks == 0) return null;
            foreach (var link in node.LinksConnected)
            {
                if ((int)link.ToNode.Key != (int)node.Key) return link.ToNode;
            }
            return null;
        }

        static Node ExecuteNode(Node node)
        {
            NodeData data = (NodeData)diagram.Model.FindNodeDataForKey(node.Key);
            switch (node.Category)
            {
                case "MENU":
                    List<string> options = data.Options.Select(item => item.Text).ToList();
                    string button = ListenForButton();
                    if (button == "-1") return null;
                    if (options.Contains(button))
                    {
                        List<Link> outputLinks = node.LinksConnected.Where(link => (int)link.FromNode.Key == (int)node.Key).ToList();
                        Node nextNode = outputLinks.Where(link => link.FromPortId.Equals(button)).First().ToNode;
                        Console.WriteLine("MENU - Selected Item " + button);
                        return nextNode;
                    }
                    break;
                case "OPEN_WEBSITE":
                    string url = data.Text;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                    Console.WriteLine("OPEN_WEBSITE: " + url);
                    return GetNextNode(node);
                case "OPEN_APPLICATION":
                    string path = data.Text;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                    Console.WriteLine("OPEN_APPLICATION: " + path);
                    return GetNextNode(node);
                case "PLAY_TONE_SEQUENCE":
                    List<string> rawSequence = data.Options.Select(item => item.Text).ToList();
                    List<(int, int)> preparedSequence = new List<(int, int)>();
                    foreach (var toneString in rawSequence)
                    {
                        string[] parts = toneString.Split(" - ");
                        preparedSequence.Add(( Int32.Parse(parts[0].Replace("Hz", "")), Int32.Parse(parts[1].Replace("ms", "")) ));
                    }
                    SendToneSequence(preparedSequence);
                    Console.WriteLine("PLAY_TONE_SEQUENCE");
                    return GetNextNode(node);

            }
            return null;
        }

        static void Listen()
        {
            while (running)
            {
                string data;
                try {
                    data = ser.ReadLine().Trim();
                } catch (OperationCanceledException e)
                {
                    Console.WriteLine(e.StackTrace);
                    break;
                }

                // only listen for numbers if the phone is off hook
                if (data == "OFF_HOOK")
                {
                    listening = true;
                    Console.WriteLine("OFF_HOOK");
                }
                else if (data == "ON_HOOK")
                {
                    listening = false;
                    if (!string.IsNullOrEmpty(currentSequence)) Console.WriteLine();
                    Console.WriteLine("ON_HOOK");
                    currentSequence = "";
                }

                // listen for numbers
                if (listening)
                {
                    if (!data.StartsWith("K") || data == "KEY_?") continue;

                    string detectedNumber = data.Split('_')[1];
                    currentSequence += detectedNumber;
                    Console.Write(detectedNumber);

                    if (data == "KEY_*")
                    {
                        Console.WriteLine();
                        currentSequence = currentSequence[..^1]; // remove asterisk from end of sequency
                        bool foundSequence = false;
                        Node nodeToExecute = null;

                        if (registeredNumbers.TryGetValue(currentSequence, out nodeToExecute))
                        {
                            Console.WriteLine($"Found function: {nodeToExecute.Category}");
                            foundSequence = true;
                            SendToneSequence(new List<(int, int)> { (440, 150), (660, 150), (880, 330) });
                        }
                        else
                        {
                            Console.WriteLine("Sequence does not exist");
                            SendToneSequence(new List<(int, int)> { (950, 330), (1400, 330), (1800, 330) });
                        }

                        currentSequence = "";
                        if (foundSequence)
                        {
                            Node nextNodeToExecute = nodeToExecute;
                            while (true)
                            {
                                nextNodeToExecute = ExecuteNode(nextNodeToExecute);
                                if (nextNodeToExecute == null) break;
                            }
                        }
                    }
                }
            }
        }

        public static void Start(Diagram _diagram)
        {
            diagram = _diagram;
            ser.Open();

            foreach (NodeData nodeData in diagram.Model.NodeDataSource)
            {
                Node node = diagram.FindNodeForKey(nodeData.Key);
                if (nodeData.Category.Equals("REGISTERED_NUMBER"))
                {
                    string number = nodeData.Text;
                    if (node.LinksConnected.Count() == 0) return;
                    Node nextNode = node.LinksConnected.First().ToNode;
                    RegisterNumber(number, nextNode);
                }
            }

            running = true;
            listenerThread = new Thread(Listen);
            listenerThread.Start();
        }

        public static void Stop()
        {
            running = false;
            ser.Close();
            Console.WriteLine("Port closed");
        }
    }
}
