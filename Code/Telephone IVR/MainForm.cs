using Northwoods.Go;
using Northwoods.Go.Layouts;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Brush = Northwoods.Go.Brush;
using Font = Northwoods.Go.Font;
using Panel = Northwoods.Go.Panel;

namespace Telephone_IVR
{
    public partial class MainForm : Form
    {
        Diagram diagram;
        int nodeCount = 0;
        int portCount = 0;
        public MainForm()
        {
            InitializeComponent();
            diagram = diagramControl1.Diagram;

            contextMenuStrip1.Items.Insert(0, new ToolStripLabel("Add node") { Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold) });
            contextMenuStrip1.Items.Insert(1, new ToolStripSeparator());
            diagramControl1.ContextMenuStrip = contextMenuStrip1;

            SetupGraph();
            CreateNodes();
        }

        private void SetupGraph()
        {
            Shape.DefineFigureGenerator("IrritationHazard", (shape, w, h) =>
            {
                var geo = new Geometry();
                var fig = new PathFigure(.2 * w, 0, true);
                geo.Add(fig);

                fig.Add(new PathSegment(SegmentType.Line, .5 * w, .3 * h));
                fig.Add(new PathSegment(SegmentType.Line, .8 * w, 0));
                fig.Add(new PathSegment(SegmentType.Line, w, .2 * h));
                fig.Add(new PathSegment(SegmentType.Line, .7 * w, .5 * h));
                fig.Add(new PathSegment(SegmentType.Line, w, .8 * h));
                fig.Add(new PathSegment(SegmentType.Line, .8 * w, h));
                fig.Add(new PathSegment(SegmentType.Line, .5 * w, .7 * h));
                fig.Add(new PathSegment(SegmentType.Line, .2 * w, h));
                fig.Add(new PathSegment(SegmentType.Line, 0, .8 * h));
                fig.Add(new PathSegment(SegmentType.Line, .3 * w, .5 * h));
                fig.Add(new PathSegment(SegmentType.Line, 0, .2 * h).Close());
                geo.Spot1 = new Spot(.3, .3);
                geo.Spot2 = new Spot(.7, .7);
                return geo;
            });

            Shape.DefineFigureGenerator("ElectricalHazard", (shape, w, h) =>
            {
                var geo = new Geometry();
                var fig = new PathFigure(.37 * w, 0, true);
                geo.Add(fig);

                fig.Add(new PathSegment(SegmentType.Line, .5 * w, .11 * h));
                fig.Add(new PathSegment(SegmentType.Line, .77 * w, .04 * h));
                fig.Add(new PathSegment(SegmentType.Line, .33 * w, .49 * h));
                fig.Add(new PathSegment(SegmentType.Line, w, .37 * h));
                fig.Add(new PathSegment(SegmentType.Line, .63 * w, .86 * h));
                fig.Add(new PathSegment(SegmentType.Line, .77 * w, .91 * h));
                fig.Add(new PathSegment(SegmentType.Line, .34 * w, h));
                fig.Add(new PathSegment(SegmentType.Line, .34 * w, .78 * h));
                fig.Add(new PathSegment(SegmentType.Line, .44 * w, .8 * h));
                fig.Add(new PathSegment(SegmentType.Line, .65 * w, .56 * h));
                fig.Add(new PathSegment(SegmentType.Line, 0, .68 * h).Close());
                return geo;
            });

            Shape.DefineFigureGenerator("FireHazard", (shape, w, h) =>
            {
                var geo = new Geometry();
                var fig = new PathFigure(.1 * w, h, true);
                geo.Add(fig);

                fig.Add(new PathSegment(SegmentType.Bezier, .29 * w, 0, -.25 * w, .63 * h,
                  .45 * w, .44 * h));
                fig.Add(new PathSegment(SegmentType.Bezier, .51 * w, .42 * h, .48 * w, .17 * h,
                  .54 * w, .35 * h));
                fig.Add(new PathSegment(SegmentType.Bezier, .59 * w, .18 * h, .59 * w, .29 * h,
                  .58 * w, .28 * h));
                fig.Add(new PathSegment(SegmentType.Bezier, .75 * w, .6 * h, .8 * w, .34 * h,
                  .88 * w, .43 * h));
                fig.Add(new PathSegment(SegmentType.Bezier, .88 * w, .31 * h, .87 * w, .48 * h,
                  .88 * w, .43 * h));
                fig.Add(new PathSegment(SegmentType.Bezier, .9 * w, h, 1.17 * w, .76 * h,
                  .82 * w, .8 * h).Close());
                geo.Spot1 = new Spot(.07, .445);
                geo.Spot2 = new Spot(.884, .958);
                return geo;
            });

            diagram.ToolManager.MouseWheelBehavior = WheelMode.Zoom;
            diagram.AllowCopy = false;
            diagram.ToolManager.DraggingTool.DragsTree = true;
            diagram.CommandHandler.DeletesTree = true;
            diagram.Layout = new TreeLayout
            {
                Angle = 90, // make tree vertical instead of horizontal
                Arrangement = TreeArrangement.FixedRoots
            }
            ;
            diagram.UndoManager.IsEnabled = true;

            var greengrad = new Brush(new LinearGradientPaint(new Dictionary<float, string> {
                  { 0, "#B1E2A5" },
                  { 1, "#7AE060" }
            }));

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
                }.Bind("Text")
            );

            var toneTemplate = new Panel(PanelType.Horizontal).Add(
                new TextBlock
                {
                    Font = new Font("Segoe UI", 10),
                    Stroke = "black",
                }.Bind("Text")
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

            // each regular Node has body consisting of a title followed by a collapsible list of actions, controlled by a PanelExpanderButton
            diagram.NodeTemplateMap.Add("MENU", new Node(PanelType.Vertical)
            {
                SelectionElementName = "BODY"
            }.Add(
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
                            }.Bind("Text", "Label")
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
            }.Add(
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
                            }.Bind("Text", "Label")
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
            }.Add(
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
                            }.Bind("Text", "Label")
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
            }.Add(
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

            diagram.NodeTemplateMap.Add("TERMINAL", new Node(PanelType.Spot).Add(
              new Shape("Circle")
              {
                  Width = 55,
                  Height = 55,
                  Fill = greengrad,
                  Stroke = null
              },
              new TextBlock
              {
                  Font = new Font("Segoe UI", 10, Northwoods.Go.FontWeight.Bold),
                  Stroke = "black",
              }.Bind("Text")
            ));

            diagram.LinkTemplate = new Link
            {
                Routing = LinkRouting.Orthogonal,
                RelinkableFrom = true,
                RelinkableTo = true,
                Deletable = true,
                Corner = 10
            }.Add(new Shape(), new Shape() { ToArrow = "Standard" });
        }

        public void CreateNodes()
        {
            var nodeDataSource = new List<NodeData> {
                MenuNode("Menu1", 3),
                MenuNode("Menu2", 3)
            };

            var linkDataSource = new List<LinkData> {
                new LinkData { From = 1, FromPort = "0", To = 2, ToPort = "In" }
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

        public NodeData MenuNode(string title, int numOptions)
        {
            if (numOptions > 10) return null;
            string[] colors =
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
            nodeCount++;
            var nodeData = new NodeData
            {
                Key = nodeCount,
                Category = "MENU",
                Label = "Menu - " + title,
                Options = new List<FieldData>()
            };
            for (int i = 0; i < numOptions; i++)
            {
                nodeData.Options.Add(new FieldData { Index = portCount.ToString(), Text = i.ToString(), Color = colors[i] });
                portCount++;
            }
            return nodeData;
        }

        public NodeData OpenApplicationNode(string path)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "OPEN_APPLICATION", Label = path };
        }

        public NodeData OpenWebsiteNode(string url)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "OPEN_WEBSITE", Label = url };
        }

        public NodeData PlayToneSequenceNode(List<string> sequence)
        {
            nodeCount++;
            var nodeData = new NodeData { Key = nodeCount, Category = "PLAY_TONE_SEQUENCE", Options = new List<FieldData>() };
            for (int i = 0; i < sequence.Count; i+=2)
            {
                nodeData.Options.Add(new FieldData { Text = sequence[i].ToString() + "Hz - " + sequence[i+1].ToString() + "ms" });
            }
            return nodeData;
        }

        public NodeData TerminalNode(string personName)
        {
            nodeCount++;
            return new NodeData { Key = nodeCount, Category = "TERMINAL", Text = personName };
        }

        private void exportCallGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Json file|*.json";
            saveFileDialog1.Title = "Save flowchart";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName == "") return;
            string savedFlowchart = diagram.Model.ToJson();
            File.WriteAllText(saveFileDialog1.FileName, savedFlowchart);
            MessageBox.Show("Saved flowchart to " + saveFileDialog1.FileName, "Saved flowchart", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void importCallGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Json files (*.json)|*.json",
                Title = "Open flowchart"
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                using StreamReader reader = new(openFileDialog.FileName);
                string flowchart = reader.ReadToEnd();
                diagram.Model = Model.FromJson<Model>(flowchart);
            }
            catch (IOException ex)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
        }

        private void menuNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMenuNodeForm form = new NewMenuNodeForm();
            form.ShowDialog();
            if (form.MENU_NAME.Trim() != "")
                diagram.Model.AddNodeData(MenuNode(form.MENU_NAME, form.NUM_OPTIONS));
        }

        private void openWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewOpenWebsiteNodeForm form = new NewOpenWebsiteNodeForm();
            form.ShowDialog();
            if (form.URL.Trim() != "")
                diagram.Model.AddNodeData(OpenWebsiteNode(form.URL));
        }

        private void openApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewOpenApplicationNodeForm form = new NewOpenApplicationNodeForm();
            form.ShowDialog();
            if (form.PATH.Trim() != "")
                diagram.Model.AddNodeData(OpenApplicationNode(form.PATH));
        }

        private void playToneSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPlayToneSequenceNodeForm form = new NewPlayToneSequenceNodeForm();
            form.ShowDialog();
            if (form.TONE_SEQUENCE.Count > 0)
                diagram.Model.AddNodeData(PlayToneSequenceNode(form.TONE_SEQUENCE));
        }
    }

    public class Model : GraphLinksModel<NodeData, int, object, LinkData, int, string> { }

    public class NodeData : Model.NodeData
    {
        public string Label { get; set; }
        public List<FieldData> Options { get; set; }
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
