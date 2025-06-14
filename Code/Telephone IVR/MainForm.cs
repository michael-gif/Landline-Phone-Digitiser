using Northwoods.Go;
using Northwoods.Go.Layouts;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Font = Northwoods.Go.Font;
using Panel = Northwoods.Go.Panel;

namespace Telephone_IVR
{
    public partial class MainForm : Form
    {
        Diagram diagram;
        int nodeCount = 0;
        Dictionary<int, Form> nodeForms = new Dictionary<int, Form>();
        string[] portColors =
            {
                "#FF0000", // red
                "#00FF00", // green
                "#0000FF", // blue
                "#FFFF00", // yellow
                "#00FFFF", // cyan
                "#FF00FF", // magenta
                "#FFA500", // orange
                "#800080", // purple
                "#BFFF00", // lime
                "#40E0D0"  // turquoise
            };

        public MainForm()
        {
            InitializeComponent();

            Console.SetOut(new MultiTextWriter(new ControlWriter(consoleTextBox), Console.Out));

            KeyPreview = true;
            KeyDown += new KeyEventHandler(MainForm_KeyDown);

            contextMenuStrip1.Items.Insert(0, new ToolStripLabel("Add node") { Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold) });
            contextMenuStrip1.Items.Insert(1, new ToolStripSeparator());
            diagramControl1.ContextMenuStrip = contextMenuStrip1;

            diagram = diagramControl1.Diagram;
            diagram.ElementDoubleClicked += nodeDoubleClicked;

            SetupGraph();
            CreateNodes();
        }

        private void SetupGraph()
        {
            diagram.ToolManager.MouseWheelBehavior = WheelMode.Zoom;
            diagram.AllowCopy = false;
            diagram.ToolManager.DraggingTool.DragsTree = false;
            diagram.CommandHandler.DeletesTree = true;
            diagram.UndoManager.IsEnabled = true;

            // each action is represented by a shape and some text
            var actionTemplate = new Panel(PanelType.Horizontal).Add(
                new Shape
                {
                    Width = 10,
                    Height = 10,
                    Margin = 5
                }.Bind("Fill", "Color"),
                new TextBlock
                {
                    Font = new Font("Segoe UI", 10),
                    Stroke = "black",
                }.Bind("Text", "Text")
            );

            var toneTemplate = new Panel(PanelType.Horizontal).Add(
                new TextBlock
                {
                    Font = new Font("Segoe UI", 10),
                    Stroke = "black",
                }.Bind("Text", "Text")
            );

            var menuPortTemplate = new Panel(PanelType.Spot).Add(
                new Shape
                {
                    Width = 8,
                    Height = 8,
                    Margin = new Margin(0, 5, 0, 5),
                    FromSpot = Spot.Bottom,
                    FromLinkable = true
                }.Bind("Fill", "Color").Bind("PortId", "Index")
            );

            diagram.NodeTemplateMap.Add("REGISTERED_NUMBER", new Node(PanelType.Vertical)
            {
                SelectionElementName = "Body"
            }.Bind(new Northwoods.Go.Models.Binding("Location", "Location").MakeTwoWay())
            .Add(
                new Panel(PanelType.Auto)
                {
                    Name = "BODY"
                }.Add(
                    new Shape("Rectangle")
                    {
                        Fill = "#fcba03",
                        Stroke = null
                    },
                    new Panel(PanelType.Vertical)
                    {
                        Margin = 3
                    }.Add(
                        new Panel(PanelType.Vertical).Add(
                            // node title
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Bold),
                                Stroke = "black",
                                Text = "Registered Number"
                            }
                        ).Add(
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Regular),
                                Background = "white"
                            }.Bind("Text", "Text")
                        ),
                        // bottom port panel
                        new Panel(PanelType.Horizontal)
                        {
                            Alignment = Spot.Bottom,
                            Margin = 3,
                            ItemTemplate = menuPortTemplate
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                Fill = "black",
                                PortId = "Out",
                                FromSpot = Spot.Bottom,
                                FromLinkable = true
                            }
                        )
                    )
                )
            )
            );

            diagram.NodeTemplateMap.Add("MENU", new Node(PanelType.Vertical)
            {
                SelectionElementName = "BODY"
            }.Bind(new Northwoods.Go.Models.Binding("Location", "Location").MakeTwoWay())
            .Add(
                // the main body consists of a Rectangle surrounding nested Panels
                new Panel(PanelType.Auto)
                {
                    Name = "BODY"
                }.Add(
                    new Shape("Rectangle")
                    {
                        Fill = "#f2564b",
                        Stroke = null
                    },
                    new Panel(PanelType.Vertical)
                    {
                        Margin = 3
                    }.Add(
                        // top port panel
                        new Panel(PanelType.Spot)
                        {
                            Alignment = Spot.Top
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                PortId = "In",
                                ToSpot = Spot.Top,
                                ToLinkable = true
                            }
                        ),
                        // node title
                        new Panel(PanelType.Horizontal).Add(
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Bold),
                                Stroke = "black",
                            }.Bind("Text", "Text")
                        ),
                        // list of actions
                        new Panel(PanelType.Vertical)
                        {
                            Stretch = Stretch.Horizontal,
                            Visible = true
                        }.Add(
                            // headered by a label and a PanelExpanderButton inside a table
                            new Panel(PanelType.Table)
                            {
                                Stretch = Stretch.Horizontal
                            }.Add(
                                new TextBlock("Options")
                                {
                                    Alignment = Spot.Left,
                                    Font = new Font("Segoe UI", 10),
                                    Stroke = "black",
                                },
                                Builder.Make<Panel>("PanelExpanderButton")
                                .Set(new { Column = 1, Alignment = Spot.Right })
                            ), // end Table panel
                               // with the list data bound in the Vertical Panel
                            new Panel(PanelType.Vertical)
                            {
                                Name = "COLLAPSIBLE",
                                Padding = 2,
                                Stretch = Stretch.Horizontal, // take up whole available width
                                Background = "white", // to distinguish from the node's body
                                DefaultAlignment = Spot.Left, // thus no need to specify alignment on each element
                                ItemTemplate = actionTemplate // the Panel created for each item in Panel.ItemList
                            }.Bind("ItemList", "Options") // bind Panel.ItemList to NodeData.Options
                        ),
                        // bottom port panel
                        new Panel(PanelType.Horizontal)
                        {
                            Alignment = Spot.Bottom,
                            Margin = 3,
                            ItemTemplate = menuPortTemplate
                        }.Bind("ItemList", "Options")
                    )
                )
            )
            );

            diagram.NodeTemplateMap.Add("OPEN_WEBSITE", new Node(PanelType.Vertical)
            {
                SelectionElementName = "Body"
            }.Bind(new Northwoods.Go.Models.Binding("Location", "Location").MakeTwoWay())
            .Add(
                new Panel(PanelType.Auto)
                {
                    Name = "BODY"
                }.Add(
                    new Shape("Rectangle")
                    {
                        Fill = "#7ca5eb",
                        Stroke = null
                    },
                    new Panel(PanelType.Vertical)
                    {
                        Margin = 3
                    }.Add(
                        // top port panel
                        new Panel(PanelType.Spot)
                        {
                            Alignment = Spot.Top
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                PortId = "In",
                                ToSpot = Spot.Top,
                                ToLinkable = true
                            }
                        ),
                        new Panel(PanelType.Vertical).Add(
                            // node title
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Bold),
                                Stroke = "black",
                                Text = "Open Website"
                            }
                        ).Add(
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Regular),
                                Background = "white"
                            }.Bind("Text", "Text")
                        ),
                        // bottom port panel
                        new Panel(PanelType.Horizontal)
                        {
                            Alignment = Spot.Bottom,
                            Margin = 3,
                            ItemTemplate = menuPortTemplate
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                Fill = "black",
                                PortId = "Out",
                                FromSpot = Spot.Bottom,
                                FromLinkable = true
                            }
                        )
                    )
                )
            )
            );

            diagram.NodeTemplateMap.Add("OPEN_APPLICATION", new Node(PanelType.Vertical)
            {
                SelectionElementName = "Body"
            }.Bind(new Northwoods.Go.Models.Binding("Location", "Location").MakeTwoWay())
            .Add(
                new Panel(PanelType.Auto)
                {
                    Name = "BODY"
                }.Add(
                    new Shape("Rectangle")
                    {
                        Fill = "#7ca5eb",
                        Stroke = null
                    },
                    new Panel(PanelType.Vertical)
                    {
                        Margin = 3
                    }.Add(
                        // top port panel
                        new Panel(PanelType.Spot)
                        {
                            Alignment = Spot.Top
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                PortId = "In",
                                ToSpot = Spot.Top,
                                ToLinkable = true
                            }
                        ),
                        new Panel(PanelType.Vertical).Add(
                            // node title
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Bold),
                                Stroke = "black",
                                Text = "Open Application"
                            }
                        ).Add(
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Regular),
                                Background = "white"
                            }.Bind("Text", "Text")
                        ),
                        // bottom port panel
                        new Panel(PanelType.Horizontal)
                        {
                            Alignment = Spot.Bottom,
                            Margin = 3,
                            ItemTemplate = menuPortTemplate
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                Fill = "black",
                                PortId = "Out",
                                FromSpot = Spot.Bottom,
                                FromLinkable = true
                            }
                        )
                    )
                )
            )
            );

            diagram.NodeTemplateMap.Add("PLAY_TONE_SEQUENCE", new Node(PanelType.Vertical)
            {
                SelectionElementName = "BODY"
            }.Bind(new Northwoods.Go.Models.Binding("Location", "Location").MakeTwoWay())
            .Add(
                // the main body consists of a Rectangle surrounding nested Panels
                new Panel(PanelType.Auto)
                {
                    Name = "BODY"
                }.Add(
                    new Shape("Rectangle")
                    {
                        Fill = "#c670e6",
                        Stroke = null
                    },
                    new Panel(PanelType.Vertical)
                    {
                        Margin = 3
                    }.Add(
                        // top port panel
                        new Panel(PanelType.Spot)
                        {
                            Alignment = Spot.Top
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                PortId = "In",
                                ToSpot = Spot.Top,
                                ToLinkable = true
                            }
                        ),
                        // node title
                        new Panel(PanelType.Horizontal).Add(
                            new TextBlock
                            {
                                Stretch = Stretch.Horizontal,
                                Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Bold),
                                Stroke = "black",
                                Text = "Play Tone Sequence"
                            }
                        ),
                        // list of tones
                        new Panel(PanelType.Vertical)
                        {
                            Stretch = Stretch.Horizontal,
                            Visible = true
                        }.Add(
                            // headered by a label and a PanelExpanderButton inside a table
                            new Panel(PanelType.Table)
                            {
                                Stretch = Stretch.Horizontal
                            }.Add(
                                new TextBlock("Tones")
                                {
                                    Alignment = Spot.Left,
                                    Font = new Font("Segoe UI", 10),
                                    Stroke = "black",
                                },
                                Builder.Make<Panel>("PanelExpanderButton")
                                .Set(new { Column = 1, Alignment = Spot.Right })
                            ), // end Table panel
                               // with the list data bound in the Vertical Panel
                            new Panel(PanelType.Vertical)
                            {
                                Name = "COLLAPSIBLE",
                                Padding = 2,
                                Stretch = Stretch.Horizontal, // take up whole available width
                                Background = "white", // to distinguish from the node's body
                                DefaultAlignment = Spot.Left, // thus no need to specify alignment on each element
                                ItemTemplate = toneTemplate // the Panel created for each item in Panel.ItemList
                            }.Bind("ItemList", "Options") // bind Panel.ItemList to NodeData.Options
                        ),
                        // bottom port panel
                        new Panel(PanelType.Horizontal)
                        {
                            Alignment = Spot.Bottom,
                            Margin = 3,
                            ItemTemplate = menuPortTemplate
                        }.Add(
                            new Shape
                            {
                                Width = 8,
                                Height = 8,
                                Fill = "black",
                                PortId = "Out",
                                FromSpot = Spot.Bottom,
                                FromLinkable = true
                            }
                        )
                    )
                )
            )
            );

            diagram.LinkTemplate = new Link
            {
                Routing = LinkRouting.Normal,
                RelinkableFrom = true,
                RelinkableTo = true,
                Deletable = true,
                Corner = 10
            }.Add(new Shape(), new Shape() { ToArrow = "Standard" });
        }

        public void CreateNodes()
        {
            var nodeDataSource = new List<NodeData>
            {
            };

            var linkDataSource = new List<LinkData>
            {
            };

            // create the Model with the above data, and assign to the Diagram
            diagram.Model = new Model
            {
                LinkFromPortIdProperty = "FromPort",  // required information:
                LinkToPortIdProperty = "ToPort",      // identifies data property names
                NodeDataSource = nodeDataSource,
                LinkDataSource = linkDataSource
            };
        }

        public NodeData RegisteredNumberNode(string number)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "REGISTERED_NUMBER", Text = number };
        }

        public NodeData MenuNode(string title, int numOptions)
        {
            if (numOptions > 10) return null;
            nodeCount++;
            var nodeData = new NodeData
            {
                Key = nodeCount,
                Category = "MENU",
                Text = "Menu - " + title,
                Options = new List<FieldData>()
            };
            for (int i = 0; i < numOptions; i++)
            {
                nodeData.Options.Add(new FieldData { Index = i.ToString(), Text = i.ToString(), Color = portColors[i] });
            }
            return nodeData;
        }

        public NodeData OpenApplicationNode(string path)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "OPEN_APPLICATION", Text = path };
        }

        public NodeData OpenWebsiteNode(string url)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "OPEN_WEBSITE", Text = url };
        }

        public NodeData PlayToneSequenceNode(List<string> sequence)
        {
            nodeCount++;
            var nodeData = new NodeData { Key = nodeCount, Category = "PLAY_TONE_SEQUENCE", Options = new List<FieldData>() };
            for (int i = 0; i < sequence.Count; i += 2)
            {
                nodeData.Options.Add(new FieldData { Text = sequence[i].ToString() + "Hz - " + sequence[i + 1].ToString() + "ms" });
            }
            return nodeData;
        }

        public NodeData TerminalNode(string personName)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "TERMINAL", Text = personName };
        }

        private void ExportCallGraph()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Json file|*.json";
            saveFileDialog1.Title = "Export call graph";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName == "") return;
            string savedFlowchart = diagram.Model.ToJson();
            File.WriteAllText(saveFileDialog1.FileName, savedFlowchart);
            MessageBox.Show("Exported call graph to " + saveFileDialog1.FileName, "Exported call graph", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ImportCallGraph()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Json files (*.json)|*.json",
                Title = "Import call graph"
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                using StreamReader reader = new(openFileDialog.FileName);
                string flowchart = reader.ReadToEnd();
                diagram.Model = Model.FromJson<Model>(flowchart);
                nodeForms.Clear();
                nodeCount = diagram.Model.NodeDataSource.Count();
                foreach (var item in diagram.Model.NodeDataSource)
                {
                    NodeData data = (NodeData)item;
                    switch (data.Category)
                    {
                        case "REGISTERED_NUMBER":
                            RegisteredNumberNodeForm form0 = new RegisteredNumberNodeForm();
                            form0.NUMBER = data.Text;
                            form0.LoadData(data.Text);
                            nodeForms[data.Key] = form0;
                            break;
                        case "MENU":
                            MenuNodeForm form1 = new MenuNodeForm();
                            form1.MENU_NAME = data.Text;
                            form1.NUM_OPTIONS = data.Options.Count;
                            form1.LoadData(data.Text, data.Options.Count);
                            nodeForms[data.Key] = form1;
                            break;
                        case "OPEN_WEBSITE":
                            OpenWebsiteNodeForm form2 = new OpenWebsiteNodeForm();
                            form2.URL = data.Text;
                            form2.LoadData(data.Text);
                            nodeForms[data.Key] = form2;
                            break;
                        case "OPEN_APPLICATION":
                            OpenApplicationNodeForm form3 = new OpenApplicationNodeForm();
                            form3.PATH = data.Text;
                            form3.LoadData(data.Text);
                            nodeForms[data.Key] = form3;
                            break;
                        case "PLAY_TONE_SEQUENCE":
                            PlayToneSequenceNodeForm form4 = new PlayToneSequenceNodeForm();
                            List<string> processedTones = data.Options.Select(item => item.Text).ToList();
                            List<string> rawTones = new List<string>();
                            foreach (var toneString in processedTones)
                            {
                                string[] parts = toneString.Split(" - ");
                                rawTones.Add(parts[0].Replace("Hz", ""));
                                rawTones.Add(parts[1].Replace("ms", ""));
                            }
                            form4.LoadData(rawTones);
                            form4.TONE_SEQUENCE = rawTones;
                            nodeForms[data.Key] = form4;
                            break;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
        }

        private void exportCallGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCallGraph();
        }

        private void importCallGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportCallGraph();
        }

        private void registeredNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegisteredNumberNodeForm form = new RegisteredNumberNodeForm();
            form.ShowDialog();
            if (form.NUMBER.Trim() != "")
            {
                diagram.Model.AddNodeData(RegisteredNumberNode(form.NUMBER));
                nodeForms.Add(nodeCount, form);
            }
        }

        private void menuNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuNodeForm form = new MenuNodeForm();
            form.ShowDialog();
            if (form.MENU_NAME.Trim() != "")
            {
                diagram.Model.AddNodeData(MenuNode(form.MENU_NAME, form.NUM_OPTIONS));
                nodeForms.Add(nodeCount, form);
            }
        }

        private void openWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWebsiteNodeForm form = new OpenWebsiteNodeForm();
            form.ShowDialog();
            if (form.URL.Trim() != "")
            {
                diagram.Model.AddNodeData(OpenWebsiteNode(form.URL));
                nodeForms.Add(nodeCount, form);
            }
        }

        private void openApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenApplicationNodeForm form = new OpenApplicationNodeForm();
            form.ShowDialog();
            if (form.PATH.Trim() != "")
            {
                diagram.Model.AddNodeData(OpenApplicationNode(form.PATH));
                nodeForms.Add(nodeCount, form);
            }
        }

        private void playToneSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayToneSequenceNodeForm form = new PlayToneSequenceNodeForm();
            form.ShowDialog();
            if (form.TONE_SEQUENCE.Count > 0)
            {
                diagram.Model.AddNodeData(PlayToneSequenceNode(form.TONE_SEQUENCE));
                nodeForms.Add(nodeCount, form);
            }
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl-S Save
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;  // Stops other controls on the form receiving event.
                ExportCallGraph();
            }
            // Ctrl-O open
            if (e.Control && e.KeyCode == Keys.O)
            {
                e.SuppressKeyPress = true;  // Stops other controls on the form receiving event.
                ImportCallGraph();
            }
        }

        private void nodeDoubleClicked(object sender, DiagramEvent e)
        {
            var part = (e.Subject as GraphObject).Part;
            if (part is Link) return;
            NodeData data = (NodeData)diagram.Model.FindNodeDataForKey(part.Key);
            if (data.Category.Equals("REGISTERED_NUMBER"))
            {
                RegisteredNumberNodeForm form = (RegisteredNumberNodeForm)nodeForms[data.Key];
                form.BeginEdit();
                form.ShowDialog();
                if (form.NUMBER.Trim() != "")
                {
                    Node node = diagram.FindNodeForKey(data.Key);
                    diagram.Model.Commit((m) =>
                    {
                        m.Set(node.Data, "Text", form.NUMBER);
                    });
                }
                form.EndEdit();
            }
            if (data.Category.Equals("MENU"))
            {
                MenuNodeForm form = (MenuNodeForm)nodeForms[data.Key];
                form.BeginEdit();
                form.ShowDialog();
                if (form.MENU_NAME.Trim() != "")
                {
                    Node node = diagram.FindNodeForKey(data.Key);
                    List<FieldData> options = new List<FieldData>();
                    if (form.NUM_OPTIONS > 10) return;
                    for (int i = 0; i < form.NUM_OPTIONS; i++)
                    {
                        options.Add(new FieldData { Index = i.ToString(), Text = i.ToString(), Color = portColors[i] });
                    }

                    diagram.Model.Commit((m) =>
                    {
                        m.Set(node.Data, "Text", form.MENU_NAME);
                        m.Set(node.Data, "Options", options);
                    });
                }
                form.EndEdit();
            }
            if (data.Category.Equals("OPEN_WEBSITE"))
            {
                OpenWebsiteNodeForm form = (OpenWebsiteNodeForm)nodeForms[data.Key];
                form.BeginEdit();
                form.ShowDialog();
                if (form.URL.Trim() != "")
                {
                    Node node = diagram.FindNodeForKey(data.Key);
                    diagram.Model.Commit((m) =>
                    {
                        m.Set(node.Data, "Text", form.URL);
                    });
                }
                form.EndEdit();
            }
            if (data.Category.Equals("OPEN_APPLICATION"))
            {
                OpenApplicationNodeForm form = (OpenApplicationNodeForm)nodeForms[data.Key];
                form.BeginEdit();
                form.ShowDialog();
                if (form.PATH.Trim() != "")
                {
                    Node node = diagram.FindNodeForKey(data.Key);
                    diagram.Model.Commit((m) =>
                    {
                        m.Set(node.Data, "Text", form.PATH);
                    });
                }
                form.EndEdit();
            }
            if (data.Category.Equals("PLAY_TONE_SEQUENCE"))
            {
                PlayToneSequenceNodeForm form = (PlayToneSequenceNodeForm)nodeForms[data.Key];
                form.BeginEdit();
                form.ShowDialog();
                if (form.TONE_SEQUENCE.Count > 0)
                {
                    Node node = diagram.FindNodeForKey(data.Key);
                    List<FieldData> tones = new List<FieldData>();
                    for (int i = 0; i < form.TONE_SEQUENCE.Count; i += 2)
                    {
                        tones.Add(new FieldData { Text = form.TONE_SEQUENCE[i].ToString() + "Hz - " + form.TONE_SEQUENCE[i + 1].ToString() + "ms" });
                    }
                    diagram.Model.Commit((m) =>
                    {
                        m.Set(node.Data, "Options", tones);
                    });
                }
                form.EndEdit();
            }
        }

        private void startIVRMenuItem_Click(object sender, EventArgs e)
        {
            if (SerialListener.running)
            {
                startIVRMenuItem.Enabled = false;
                SerialListener.Stop();
                startIVRMenuItem.Enabled = true;
                startIVRMenuItem.Text = "Start IVR";
                startIVRMenuItem.BackColor = Color.LimeGreen;
                MessageBox.Show("IVR Stopped");
            }
            else
            {
                startIVRMenuItem.Enabled = false;
                consoleTextBox.Clear();
                SerialListener.Start(diagram);
                startIVRMenuItem.Enabled = true;
                startIVRMenuItem.Text = "Stop IVR";
                startIVRMenuItem.BackColor = Color.Red;
                MessageBox.Show("IVR Started");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SerialListener.Stop();
        }

        private void consoleTextBox_TextChanged(object sender, EventArgs e)
        {
            consoleTextBox.SelectionStart = consoleTextBox.Text.Length;
            consoleTextBox.ScrollToCaret();
        }

        private void clearConsoleButton_Click(object sender, EventArgs e)
        {
            consoleTextBox.Clear();
        }
    }

    public class Model : GraphLinksModel<NodeData, int, object, LinkData, int, string> { }

    public class NodeData : Model.NodeData
    {
        public string Text { get; set; }
        public List<FieldData> Options { get; set; }

        public Northwoods.Go.Point Location { get; set; }
    }

    public class LinkData : Model.LinkData
    {
    }

    public class FieldData
    {
        public string Index { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
    }
}
