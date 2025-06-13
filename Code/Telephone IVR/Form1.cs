using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Telephone_IVR
{
    public partial class Form1 : Form
    {
        PhoneNumber selectedNumber;
        BindingList<PhoneNumber> registeredNumbers = new BindingList<PhoneNumber>();

        public Form1()
        {
            InitializeComponent();
            registeredNumbersListBox.DataSource = registeredNumbers;
        }

        private void registerNumberButton_Click(object sender, EventArgs e)
        {
            registeredNumbers.Add(new PhoneNumber("00000000"));
            registeredNumbersListBox.ClearSelected();
            registeredNumbersListBox.SetSelected(registeredNumbers.Count - 1, true);
        }

        private void deleteRegisteredNumberButton_Click(object sender, EventArgs e)
        {
            if (registeredNumbersListBox.SelectedIndex < 0) return;
            registeredNumbers.RemoveAt(registeredNumbersListBox.SelectedIndex);
            selectedNumber = null;
            selectedNumberTextBox.Text = "";
            if (registeredNumbers.Count == 0)
            {
                panel1.Enabled = false;
                panel2.Enabled = false;
            }
        }

        private void registeredNumbersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (registeredNumbersListBox.SelectedItem == null) return;
            if (registeredNumbersListBox.SelectedItem == selectedNumber) return;

            selectedNumber = (PhoneNumber)registeredNumbersListBox.SelectedItem;
            selectedNumberTextBox.Text = selectedNumber.Number;
            panel1.Enabled = true;
            actionListView.Items.Clear();

            foreach (var action in selectedNumber.actions)
            {
                actionListView.Items.Add(action.Name).SubItems.Add(action.Type);
            }
        }

        private void selectedNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            if (registeredNumbersListBox.SelectedIndex < 0) return;
            int index = registeredNumbersListBox.SelectedIndex;
            registeredNumbers.RemoveAt(index);
            selectedNumber.Number = selectedNumberTextBox.Text;
            registeredNumbers.Insert(index, selectedNumber);
        }

        private void newActionButton_Click(object sender, EventArgs e)
        {
            new NewActionForm(this).ShowDialog();
        }

        public void AddAction(string name, string type)
        {
            Action action = new Action(name, type);
            selectedNumber.actions.Add(action);
            actionListView.Items.Add(action.Name).SubItems.Add(action.Type);
            actionListView.Select();
            actionListView.Items[actionListView.Items.Count - 1].Focused = true;
            actionListView.Items[actionListView.Items.Count - 1].Selected = true;
        }

        private void actionListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (actionListView.SelectedIndices.Count < 1) return;
            if (actionListView.SelectedIndices[0] < 0) return;
            panel2.Enabled = true;
            int actionIndex = actionListView.SelectedIndices[0];
            Action action = selectedNumber.actions[actionIndex];
            Console.WriteLine(action.Value);
            string type = action.Type;
            if (type == "Open website")
            {
                actionValueTextBox.Visible = true;
                actionValueTextBox.PlaceholderText = "Enter URL";
                browsePathButton.Visible = false;
                actionValueTextBox.Text = action.Value;
            }
            if (type == "Open application")
            {
                actionValueTextBox.Visible = true;
                actionValueTextBox.PlaceholderText = "Enter path to application";
                browsePathButton.Visible = true;
                actionValueTextBox.Text = action.Value;
            }
        }

        private void browsePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                FileName = "Select an executable",
                Filter = "EXE files (*.exe)|*.exe|Shortcut files (*.lnk)|*.lnk",
                Title = "Open executable file"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                actionValueTextBox.Text = openFileDialog.FileName;
                selectedNumber.actions[actionListView.SelectedIndices[0]].Value = openFileDialog.FileName;
            }
        }

        private void actionValueTextBox_TextChanged(object sender, EventArgs e)
        {
            selectedNumber.actions[actionListView.SelectedIndices[0]].Value = actionValueTextBox.Text;
        }

        private void deleteActionButton_Click(object sender, EventArgs e)
        {
            if (actionListView.SelectedIndices.Count == 0) return;
            int actionIndex = actionListView.SelectedIndices[0];
            actionListView.Items.RemoveAt(actionIndex);
            selectedNumber.actions.RemoveAt(actionIndex);
            actionValueTextBox.Visible = false;
            browsePathButton.Visible = false;
            panel2.Enabled = false;
        }
    }

    public class PhoneNumber
    {
        public string Number { get; set; }
        public BindingList<Action> actions = new BindingList<Action>();

        public PhoneNumber(string number)
        {
            Number = number;
        }

        public override string ToString()
        {
            return Number;
        }
    }

    public class Action
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public Action(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
