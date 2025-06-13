using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Telephone_IVR
{
    public partial class PlayToneSequenceNodeForm : Form
    {
        public List<string> TONE_SEQUENCE = new List<string>();
        public PlayToneSequenceNodeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var row = new DataGridViewRow();
            row.Cells.Add(new DataGridViewTextBoxCell { Value = 500 });
            row.Cells.Add(new DataGridViewTextBoxCell { Value = 100 });
            dataGridView1.Rows.Add(row);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0) return;
            DataGridViewCell cell = dataGridView1.SelectedCells[0];
            int row = cell.RowIndex;
            dataGridView1.Rows.RemoveAt(row);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TONE_SEQUENCE.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell toneCell = row.Cells[0];
                DataGridViewCell durationCell = row.Cells[0];
                if (toneCell.Value == null) continue;
                string tone = toneCell.Value.ToString();
                string duration = durationCell.Value.ToString();
                TONE_SEQUENCE.Add(tone);
                TONE_SEQUENCE.Add(duration);
            }
            Close();
        }

        public void LoadData(List<string> rawTones)
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < rawTones.Count; i += 2)
            {
                var row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell { Value = rawTones[i] });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = rawTones[i + 1] });
                dataGridView1.Rows.Add(row);
            }
        }

        public void BeginEdit()
        {
            button3.Text = "Save Node";
        }

        public void EndEdit()
        {
            button3.Text = "Add Node";
        }
    }
}
