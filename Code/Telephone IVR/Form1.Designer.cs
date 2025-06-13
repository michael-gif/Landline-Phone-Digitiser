namespace Telephone_IVR
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            registeredNumbersListBox = new ListBox();
            panel1 = new Panel();
            actionListView = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            deleteActionButton = new Button();
            newActionButton = new Button();
            selectedNumberTextBox = new TextBox();
            label1 = new Label();
            panel2 = new Panel();
            browsePathButton = new Button();
            actionValueTextBox = new TextBox();
            label2 = new Label();
            label3 = new Label();
            registerNumberButton = new Button();
            deleteRegisteredNumberButton = new Button();
            menuStrip1 = new MenuStrip();
            loadIVRFileToolStripMenuItem = new ToolStripMenuItem();
            runIVRToolStripMenuItem = new ToolStripMenuItem();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // registeredNumbersListBox
            // 
            registeredNumbersListBox.FormattingEnabled = true;
            registeredNumbersListBox.ItemHeight = 15;
            registeredNumbersListBox.Location = new Point(2, 46);
            registeredNumbersListBox.Name = "registeredNumbersListBox";
            registeredNumbersListBox.Size = new Size(163, 424);
            registeredNumbersListBox.TabIndex = 1;
            registeredNumbersListBox.SelectedIndexChanged += registeredNumbersListBox_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(actionListView);
            panel1.Controls.Add(deleteActionButton);
            panel1.Controls.Add(newActionButton);
            panel1.Controls.Add(selectedNumberTextBox);
            panel1.Enabled = false;
            panel1.Location = new Point(171, 46);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 424);
            panel1.TabIndex = 2;
            // 
            // actionListView
            // 
            actionListView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            actionListView.Location = new Point(3, 90);
            actionListView.MultiSelect = false;
            actionListView.Name = "actionListView";
            actionListView.Size = new Size(194, 324);
            actionListView.TabIndex = 4;
            actionListView.UseCompatibleStateImageBehavior = false;
            actionListView.View = View.Details;
            actionListView.SelectedIndexChanged += actionListView_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Action Name";
            columnHeader1.Width = 90;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Action Type";
            columnHeader2.Width = 100;
            // 
            // deleteActionButton
            // 
            deleteActionButton.Location = new Point(3, 61);
            deleteActionButton.Name = "deleteActionButton";
            deleteActionButton.Size = new Size(194, 23);
            deleteActionButton.TabIndex = 3;
            deleteActionButton.Text = "Delete Action";
            deleteActionButton.UseVisualStyleBackColor = true;
            deleteActionButton.Click += deleteActionButton_Click;
            // 
            // newActionButton
            // 
            newActionButton.Location = new Point(3, 32);
            newActionButton.Name = "newActionButton";
            newActionButton.Size = new Size(194, 23);
            newActionButton.TabIndex = 2;
            newActionButton.Text = "New Action";
            newActionButton.UseVisualStyleBackColor = true;
            newActionButton.Click += newActionButton_Click;
            // 
            // selectedNumberTextBox
            // 
            selectedNumberTextBox.Location = new Point(3, 3);
            selectedNumberTextBox.Name = "selectedNumberTextBox";
            selectedNumberTextBox.Size = new Size(194, 23);
            selectedNumberTextBox.TabIndex = 0;
            selectedNumberTextBox.TextChanged += selectedNumberTextBox_TextChanged;
            // 
            // label1
            // 
            label1.Location = new Point(171, 28);
            label1.Name = "label1";
            label1.Size = new Size(200, 15);
            label1.TabIndex = 0;
            label1.Text = "Selected Number";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(browsePathButton);
            panel2.Controls.Add(actionValueTextBox);
            panel2.Enabled = false;
            panel2.Location = new Point(377, 46);
            panel2.Name = "panel2";
            panel2.Size = new Size(200, 424);
            panel2.TabIndex = 3;
            // 
            // browsePathButton
            // 
            browsePathButton.Location = new Point(3, 32);
            browsePathButton.Name = "browsePathButton";
            browsePathButton.Size = new Size(194, 23);
            browsePathButton.TabIndex = 5;
            browsePathButton.Text = "Browse";
            browsePathButton.UseVisualStyleBackColor = true;
            browsePathButton.Visible = false;
            browsePathButton.Click += browsePathButton_Click;
            // 
            // actionValueTextBox
            // 
            actionValueTextBox.Location = new Point(3, 3);
            actionValueTextBox.Name = "actionValueTextBox";
            actionValueTextBox.Size = new Size(194, 23);
            actionValueTextBox.TabIndex = 4;
            actionValueTextBox.Visible = false;
            actionValueTextBox.TextChanged += actionValueTextBox_TextChanged;
            // 
            // label2
            // 
            label2.Location = new Point(377, 28);
            label2.Name = "label2";
            label2.Size = new Size(200, 15);
            label2.TabIndex = 1;
            label2.Text = "Action Properties";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(2, 28);
            label3.Name = "label3";
            label3.Size = new Size(114, 15);
            label3.TabIndex = 4;
            label3.Text = "Registered Numbers";
            // 
            // registerNumberButton
            // 
            registerNumberButton.BackgroundImage = (Image)resources.GetObject("registerNumberButton.BackgroundImage");
            registerNumberButton.BackgroundImageLayout = ImageLayout.Stretch;
            registerNumberButton.Location = new Point(119, 24);
            registerNumberButton.Name = "registerNumberButton";
            registerNumberButton.Size = new Size(23, 23);
            registerNumberButton.TabIndex = 5;
            registerNumberButton.UseVisualStyleBackColor = true;
            registerNumberButton.Click += registerNumberButton_Click;
            // 
            // deleteRegisteredNumberButton
            // 
            deleteRegisteredNumberButton.BackgroundImage = (Image)resources.GetObject("deleteRegisteredNumberButton.BackgroundImage");
            deleteRegisteredNumberButton.BackgroundImageLayout = ImageLayout.Stretch;
            deleteRegisteredNumberButton.Location = new Point(142, 24);
            deleteRegisteredNumberButton.Name = "deleteRegisteredNumberButton";
            deleteRegisteredNumberButton.Size = new Size(23, 23);
            deleteRegisteredNumberButton.TabIndex = 6;
            deleteRegisteredNumberButton.UseVisualStyleBackColor = true;
            deleteRegisteredNumberButton.Click += deleteRegisteredNumberButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { loadIVRFileToolStripMenuItem, runIVRToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(580, 24);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // loadIVRFileToolStripMenuItem
            // 
            loadIVRFileToolStripMenuItem.Name = "loadIVRFileToolStripMenuItem";
            loadIVRFileToolStripMenuItem.Size = new Size(84, 20);
            loadIVRFileToolStripMenuItem.Text = "Load IVR file";
            // 
            // runIVRToolStripMenuItem
            // 
            runIVRToolStripMenuItem.Name = "runIVRToolStripMenuItem";
            runIVRToolStripMenuItem.Size = new Size(60, 20);
            runIVRToolStripMenuItem.Text = "Run IVR";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(580, 474);
            Controls.Add(deleteRegisteredNumberButton);
            Controls.Add(registerNumberButton);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(panel2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(registeredNumbersListBox);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ListBox registeredNumbersListBox;
        private Panel panel1;
        private Label label1;
        private Button deleteActionButton;
        private Button newActionButton;
        private Panel panel2;
        private Label label2;
        private TextBox selectedNumberTextBox;
        private TextBox actionValueTextBox;
        private Button browsePathButton;
        private Label label3;
        private Button registerNumberButton;
        private Button deleteRegisteredNumberButton;
        private ListView actionListView;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem loadIVRFileToolStripMenuItem;
        private ToolStripMenuItem runIVRToolStripMenuItem;
    }
}
