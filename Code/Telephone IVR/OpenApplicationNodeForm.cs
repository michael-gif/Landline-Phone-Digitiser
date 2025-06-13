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
    public partial class OpenApplicationNodeForm : Form
    {
        public string PATH = "";
        public OpenApplicationNodeForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                FileName = "Select an executable",
                Filter = "EXE files (*.exe)|*.exe|Shortcut files (*.lnk)|*.lnk",
                Title = "Open executable file"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PATH = textBox1.Text;
            Close();
        }

        public void BeginEdit()
        {
            button1.Text = "Save Node";
        }

        public void EndEdit()
        {
            button1.Text = "Add Node";
        }
    }
}
