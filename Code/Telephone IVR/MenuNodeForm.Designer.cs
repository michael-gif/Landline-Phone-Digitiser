namespace Telephone_IVR
{
    partial class MenuNodeForm
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
            textBox1 = new TextBox();
            label1 = new Label();
            numericUpDown1 = new NumericUpDown();
            label2 = new Label();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(91, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(155, 23);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 1;
            label1.Text = "Menu Name";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(126, 35);
            numericUpDown1.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(120, 23);
            numericUpDown1.TabIndex = 2;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 37);
            label2.Name = "label2";
            label2.Size = new Size(108, 15);
            label2.TabIndex = 3;
            label2.Text = "Number of options";
            // 
            // button1
            // 
            button1.Location = new Point(12, 64);
            button1.Name = "button1";
            button1.Size = new Size(234, 23);
            button1.TabIndex = 4;
            button1.Text = "Add Node";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // NewMenuNodeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(251, 90);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(numericUpDown1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewMenuNodeForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add a Menu Node";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private NumericUpDown numericUpDown1;
        private Label label2;
        private Button button1;
    }
}