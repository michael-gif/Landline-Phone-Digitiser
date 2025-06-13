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
    public partial class RegisteredNumberNodeForm : Form
    {
        public string NUMBER = "";
        public RegisteredNumberNodeForm()
        {
            InitializeComponent();
            textBox1.KeyPress += new KeyPressEventHandler(CheckEnterKeyPress);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NUMBER = textBox1.Text;
            Close();
        }

        public void LoadData(string path)
        {
            textBox1.Text = path;
        }

        public void BeginEdit()
        {
            button1.Text = "Save Node";
        }

        public void EndEdit()
        {
            button1.Text = "Add Node";
        }

        private void RegisteredNumberNodeForm_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                button1.PerformClick();
            }
        }
    }
}
