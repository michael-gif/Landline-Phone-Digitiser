using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Telephone_IVR
{
    public partial class NewActionForm : Form
    {
        Form1 form1;
        public NewActionForm(Form1 form)
        {
            form1 = form;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Action name must be specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!comboBox1.Items.Contains(comboBox1.Text))
            {
                MessageBox.Show("Action type must be valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            form1.AddAction(textBox1.Text, comboBox1.Text);
            Close();
        }
    }
}
