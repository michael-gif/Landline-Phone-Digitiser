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
            diagramControl1 = new Northwoods.Go.WinForms.DiagramControl();
            menuStrip1 = new MenuStrip();
            exportModelToolStripMenuItem = new ToolStripMenuItem();
            exportCallGraphToolStripMenuItem = new ToolStripMenuItem();
            importCallGraphToolStripMenuItem = new ToolStripMenuItem();
            importModelToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // diagramControl1
            // 
            diagramControl1.AllowDrop = true;
            diagramControl1.BackColor = Color.White;
            diagramControl1.Dock = DockStyle.Fill;
            diagramControl1.Location = new Point(0, 24);
            diagramControl1.Name = "diagramControl1";
            diagramControl1.Size = new Size(800, 426);
            diagramControl1.TabIndex = 0;
            diagramControl1.Text = "diagramControl1";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { exportModelToolStripMenuItem, importModelToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // exportModelToolStripMenuItem
            // 
            exportModelToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportCallGraphToolStripMenuItem, importCallGraphToolStripMenuItem });
            exportModelToolStripMenuItem.Size = new Size(37, 20);
            exportModelToolStripMenuItem.Text = "File";
            // 
            // exportCallGraphToolStripMenuItem
            // 
            exportCallGraphToolStripMenuItem.Name = "exportCallGraphToolStripMenuItem";
            exportCallGraphToolStripMenuItem.Size = new Size(180, 22);
            exportCallGraphToolStripMenuItem.Text = "Export Call Graph";
            exportCallGraphToolStripMenuItem.Click += exportCallGraphToolStripMenuItem_Click;
            // 
            // importCallGraphToolStripMenuItem
            // 
            importCallGraphToolStripMenuItem.Name = "importCallGraphToolStripMenuItem";
            importCallGraphToolStripMenuItem.Size = new Size(180, 22);
            importCallGraphToolStripMenuItem.Text = "Import Call Graph";
            importCallGraphToolStripMenuItem.Click += importCallGraphToolStripMenuItem_Click;
            // 
            // importModelToolStripMenuItem
            // 
            importModelToolStripMenuItem.Size = new Size(60, 20);
            importModelToolStripMenuItem.Text = "Run IVR";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(diagramControl1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Northwoods.Go.WinForms.DiagramControl diagramControl1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem exportModelToolStripMenuItem;
        private ToolStripMenuItem importModelToolStripMenuItem;
        private ToolStripMenuItem exportCallGraphToolStripMenuItem;
        private ToolStripMenuItem importCallGraphToolStripMenuItem;
    }
}