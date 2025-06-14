namespace Telephone_IVR
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            diagramControl1 = new Northwoods.Go.WinForms.DiagramControl();
            menuStrip1 = new MenuStrip();
            exportModelToolStripMenuItem = new ToolStripMenuItem();
            exportCallGraphToolStripMenuItem = new ToolStripMenuItem();
            importCallGraphToolStripMenuItem = new ToolStripMenuItem();
            startIVRMenuItem = new ToolStripMenuItem();
            contextMenuStrip1 = new ContextMenuStrip(components);
            registeredNumberToolStripMenuItem = new ToolStripMenuItem();
            menuNodeToolStripMenuItem = new ToolStripMenuItem();
            openWebsiteToolStripMenuItem = new ToolStripMenuItem();
            openApplicationToolStripMenuItem = new ToolStripMenuItem();
            playToneSequenceToolStripMenuItem = new ToolStripMenuItem();
            consoleTextBox = new TextBox();
            clearConsoleButton = new Button();
            menuStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // diagramControl1
            // 
            diagramControl1.AllowDrop = true;
            diagramControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            diagramControl1.BackColor = Color.White;
            diagramControl1.Location = new Point(0, 24);
            diagramControl1.Name = "diagramControl1";
            diagramControl1.Size = new Size(478, 426);
            diagramControl1.TabIndex = 0;
            diagramControl1.Text = "diagramControl1";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { exportModelToolStripMenuItem, startIVRMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // exportModelToolStripMenuItem
            // 
            exportModelToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportCallGraphToolStripMenuItem, importCallGraphToolStripMenuItem });
            exportModelToolStripMenuItem.Name = "exportModelToolStripMenuItem";
            exportModelToolStripMenuItem.Size = new Size(37, 20);
            exportModelToolStripMenuItem.Text = "File";
            // 
            // exportCallGraphToolStripMenuItem
            // 
            exportCallGraphToolStripMenuItem.Name = "exportCallGraphToolStripMenuItem";
            exportCallGraphToolStripMenuItem.Size = new Size(168, 22);
            exportCallGraphToolStripMenuItem.Text = "Export Call Graph";
            exportCallGraphToolStripMenuItem.Click += exportCallGraphToolStripMenuItem_Click;
            // 
            // importCallGraphToolStripMenuItem
            // 
            importCallGraphToolStripMenuItem.Name = "importCallGraphToolStripMenuItem";
            importCallGraphToolStripMenuItem.Size = new Size(168, 22);
            importCallGraphToolStripMenuItem.Text = "Import Call Graph";
            importCallGraphToolStripMenuItem.Click += importCallGraphToolStripMenuItem_Click;
            // 
            // startIVRMenuItem
            // 
            startIVRMenuItem.BackColor = Color.LimeGreen;
            startIVRMenuItem.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            startIVRMenuItem.ForeColor = Color.White;
            startIVRMenuItem.Name = "startIVRMenuItem";
            startIVRMenuItem.Size = new Size(71, 20);
            startIVRMenuItem.Text = "Start IVR";
            startIVRMenuItem.Click += startIVRMenuItem_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { registeredNumberToolStripMenuItem, menuNodeToolStripMenuItem, openWebsiteToolStripMenuItem, openApplicationToolStripMenuItem, playToneSequenceToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(179, 114);
            // 
            // registeredNumberToolStripMenuItem
            // 
            registeredNumberToolStripMenuItem.Name = "registeredNumberToolStripMenuItem";
            registeredNumberToolStripMenuItem.Size = new Size(178, 22);
            registeredNumberToolStripMenuItem.Text = "Registered Number";
            registeredNumberToolStripMenuItem.Click += registeredNumberToolStripMenuItem_Click;
            // 
            // menuNodeToolStripMenuItem
            // 
            menuNodeToolStripMenuItem.Name = "menuNodeToolStripMenuItem";
            menuNodeToolStripMenuItem.Size = new Size(178, 22);
            menuNodeToolStripMenuItem.Text = "Menu Node";
            menuNodeToolStripMenuItem.Click += menuNodeToolStripMenuItem_Click;
            // 
            // openWebsiteToolStripMenuItem
            // 
            openWebsiteToolStripMenuItem.Name = "openWebsiteToolStripMenuItem";
            openWebsiteToolStripMenuItem.Size = new Size(178, 22);
            openWebsiteToolStripMenuItem.Text = "Open Website";
            openWebsiteToolStripMenuItem.Click += openWebsiteToolStripMenuItem_Click;
            // 
            // openApplicationToolStripMenuItem
            // 
            openApplicationToolStripMenuItem.Name = "openApplicationToolStripMenuItem";
            openApplicationToolStripMenuItem.Size = new Size(178, 22);
            openApplicationToolStripMenuItem.Text = "Open Application";
            openApplicationToolStripMenuItem.Click += openApplicationToolStripMenuItem_Click;
            // 
            // playToneSequenceToolStripMenuItem
            // 
            playToneSequenceToolStripMenuItem.Name = "playToneSequenceToolStripMenuItem";
            playToneSequenceToolStripMenuItem.Size = new Size(178, 22);
            playToneSequenceToolStripMenuItem.Text = "Play Tone Sequence";
            playToneSequenceToolStripMenuItem.Click += playToneSequenceToolStripMenuItem_Click;
            // 
            // consoleTextBox
            // 
            consoleTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            consoleTextBox.BackColor = Color.Black;
            consoleTextBox.ForeColor = Color.White;
            consoleTextBox.Location = new Point(484, 24);
            consoleTextBox.Multiline = true;
            consoleTextBox.Name = "consoleTextBox";
            consoleTextBox.ReadOnly = true;
            consoleTextBox.ScrollBars = ScrollBars.Vertical;
            consoleTextBox.Size = new Size(316, 393);
            consoleTextBox.TabIndex = 2;
            consoleTextBox.TextChanged += consoleTextBox_TextChanged;
            // 
            // clearConsoleButton
            // 
            clearConsoleButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            clearConsoleButton.Location = new Point(483, 417);
            clearConsoleButton.Name = "clearConsoleButton";
            clearConsoleButton.Size = new Size(317, 33);
            clearConsoleButton.TabIndex = 3;
            clearConsoleButton.Text = "Clear Console";
            clearConsoleButton.UseVisualStyleBackColor = true;
            clearConsoleButton.Click += clearConsoleButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(clearConsoleButton);
            Controls.Add(consoleTextBox);
            Controls.Add(diagramControl1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            FormClosed += MainForm_FormClosed;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Northwoods.Go.WinForms.DiagramControl diagramControl1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem exportModelToolStripMenuItem;
        private ToolStripMenuItem startIVRMenuItem;
        private ToolStripMenuItem exportCallGraphToolStripMenuItem;
        private ToolStripMenuItem importCallGraphToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem menuNodeToolStripMenuItem;
        private ToolStripMenuItem openWebsiteToolStripMenuItem;
        private ToolStripMenuItem openApplicationToolStripMenuItem;
        private ToolStripMenuItem playToneSequenceToolStripMenuItem;
        private ToolStripMenuItem registeredNumberToolStripMenuItem;
        private TextBox consoleTextBox;
        private Button clearConsoleButton;
    }
}