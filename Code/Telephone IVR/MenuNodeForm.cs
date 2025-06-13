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
    public partial class MenuNodeForm : Form
    {
        public string MENU_NAME = "";
        public int NUM_OPTIONS = 0;
        public MenuNodeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MENU_NAME = textBox1.Text;
            NUM_OPTIONS = (int) numericUpDown1.Value;
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
