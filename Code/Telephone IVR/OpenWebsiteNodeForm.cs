﻿using System;
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
    public partial class OpenWebsiteNodeForm : Form
    {
        public string URL = "";
        public OpenWebsiteNodeForm()
        {
            InitializeComponent();
            textBox1.KeyPress += new KeyPressEventHandler(CheckEnterKeyPress);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            URL = textBox1.Text;
            Close();
        }

        public void LoadData(string url)
        {
            textBox1.Text = url;
        }

        public void BeginEdit()
        {
            button1.Text = "Save Node";
        }

        public void EndEdit()
        {
            button1.Text = "Add Node";
        }

        private void OpenWebsiteNodeForm_Shown(object sender, EventArgs e)
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
