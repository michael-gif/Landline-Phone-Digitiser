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
            label1 = new Label();
            addToneButton = new Button();
            deleteToneButton = new Button();
            button3 = new Button();
            dataGridView1 = new DataGridView();
            Frequency = new DataGridViewTextBoxColumn();
            Duration = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
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
            addToneButton.Location = new Point(379, 27);
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
            deleteToneButton.Location = new Point(379, 58);
            deleteToneButton.Name = "deleteToneButton";
            deleteToneButton.Size = new Size(25, 25);
            deleteToneButton.TabIndex = 3;
            deleteToneButton.UseVisualStyleBackColor = true;
            deleteToneButton.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 268);
            button3.Name = "button3";
            button3.Size = new Size(392, 23);
            button3.TabIndex = 4;
            button3.Text = "Add Node";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Frequency, Duration });
            dataGridView1.Location = new Point(12, 27);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Size = new Size(361, 235);
            dataGridView1.TabIndex = 5;
            // 
            // Frequency
            // 
            Frequency.HeaderText = "Frequency (Hz)";
            Frequency.Name = "Frequency";
            Frequency.Width = 175;
            // 
            // Duration
            // 
            Duration.HeaderText = "Duration (ms)";
            Duration.Name = "Duration";
            Duration.Width = 175;
            // 
            // PlayToneSequenceNodeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 303);
            Controls.Add(dataGridView1);
            Controls.Add(button3);
            Controls.Add(deleteToneButton);
            Controls.Add(addToneButton);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlayToneSequenceNodeForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Tone Sequence Node";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Button addToneButton;
        private Button deleteToneButton;
        private Button button3;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Frequency;
        private DataGridViewTextBoxColumn Duration;
    }
}