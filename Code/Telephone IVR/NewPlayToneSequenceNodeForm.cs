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
    public partial class NewPlayToneSequenceNodeForm : Form
    {
        public List<string> TONE_SEQUENCE = new List<string>();
        public NewPlayToneSequenceNodeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Add("500").SubItems.Add("100");
            listView1.Items[listView1.Items.Count - 1].Focused = true;
            listView1.Items[listView1.Items.Count - 1].Selected = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;
            int actionIndex = listView1.SelectedIndices[0];
            listView1.Items.RemoveAt(actionIndex);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem tone in listView1.Items)
            {
                var duration = tone.SubItems[0];
                TONE_SEQUENCE.Add(tone.Text);
                TONE_SEQUENCE.Add(duration.Text);
            }
            Close();
        }
    }
}
