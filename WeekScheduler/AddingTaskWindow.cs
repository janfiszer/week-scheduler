using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeekScheduler
{
    public partial class AddingTaskWindow : Form
    {
        public Task Task { get; set; }
        private DateTime date;
        private int maxDuration;
        public AddingTaskWindow(DateTime date, int maxDuaration)
        {
            InitializeComponent();

            this.maxDuration = maxDuaration;
            this.date = date;
        }
        private void defaultValueChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                control.ForeColor = Color.Black;
            }
        }
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }
        private void initDomainsUpDown(int maxDuration)
        {
            for (int i = maxDuration * 30; i > 0; i-= 30)
            {
                domainUpDown1.Items.Add(i);
            }
            int maxReminderTime = 60 * 24 * 7;
            for (int i = maxReminderTime; i > 0; i--)
            {
                domainUpDown_remind.Items.Add(i);
            }
            domainUpDown_remind.SelectedIndex = maxReminderTime - 15;
        }
        private void initCombobox()
        {
            foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)))
            {
                comboBox1.Items.Add(taskType.ToString());
            }
            comboBox1.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == String.Empty)
            {
                MessageBox.Show("Task name can't be empty", "ERROR");
                return;
            }


            // TODO: DO IT MORE SMOOTHLY, FUCKING ENUM CASTING...
            TaskType taskType = (TaskType)Enum.Parse(typeof(TaskType), comboBox1.GetItemText(comboBox1.SelectedIndex) , true);
            if (checkBox1.Checked)
            {
                Task = new Task(textBox1.Text, date, taskType, textBox2.Text, Int32.Parse(domainUpDown1.Text) / 30, Int32.Parse(domainUpDown_remind.Text));
            }
            else
            {
                Task = new Task(textBox1.Text, date, taskType, textBox2.Text, Int32.Parse(domainUpDown1.Text) / 30);
            }
            //Task = new Task(textBox1.Text, date, (TaskType)comboBox1.SelectedItem, textBox2.Text, Int32.Parse(domainUpDown1.Text));
            this.Close();
        }

        private void AddingTaskWindow_Load(object sender, EventArgs e)
        {
            initCombobox();
            initDomainsUpDown(maxDuration);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                domainUpDown_remind.Enabled = false;
                checkBox1.ForeColor = Color.Gray;
            }
            else
            {
                domainUpDown_remind.Enabled = true;
                checkBox1.ForeColor = Color.Black;
            }
        }
    }
}
