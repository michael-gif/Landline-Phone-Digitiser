namespace Telephone_IVR
{
    partial class PlayToneSequenceNodeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayToneSequenceNodeForm));
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            label1 = new Label();
            addToneButton = new Button();
            deleteToneButton = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            listView1.LabelEdit = true;
            listView1.Location = new Point(12, 27);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(134, 97);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Frequency";
            columnHeader1.Width = 65;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Duration (ms)";
            columnHeader2.Width = 65;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(86, 15);
            label1.TabIndex = 1;
            label1.Text = "Tone Sequence";
            // 
            // addToneButton
            // 
            addToneButton.BackgroundImage = (Image)resources.GetObject("addToneButton.BackgroundImage");
            addToneButton.BackgroundImageLayout = ImageLayout.Stretch;
            addToneButton.Location = new Point(152, 27);
            addToneButton.Name = "addToneButton";
            addToneButton.Size = new Size(25, 25);
            addToneButton.TabIndex = 2;
            addToneButton.UseVisualStyleBackColor = true;
            addToneButton.Click += button1_Click;
            // 
            // deleteToneButton
            // 
            deleteToneButton.BackgroundImage = (Image)resources.GetObject("deleteToneButton.BackgroundImage");
            deleteToneButton.BackgroundImageLayout = ImageLayout.Stretch;
            deleteToneButton.Location = new Point(152, 58);
            deleteToneButton.Name = "deleteToneButton";
            deleteToneButton.Size = new Size(25, 25);
            deleteToneButton.TabIndex = 3;
            deleteToneButton.UseVisualStyleBackColor = true;
            deleteToneButton.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 130);
            button3.Name = "button3";
            button3.Size = new Size(165, 23);
            button3.TabIndex = 4;
            button3.Text = "Add Node";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // NewPlayToneSequenceNodeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(189, 159);
            Controls.Add(button3);
            Controls.Add(deleteToneButton);
            Controls.Add(addToneButton);
            Controls.Add(label1);
            Controls.Add(listView1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewPlayToneSequenceNodeForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Tone Sequence Node";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private Label label1;
        private Button addToneButton;
        private Button deleteToneButton;
        private Button button3;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
    }
}