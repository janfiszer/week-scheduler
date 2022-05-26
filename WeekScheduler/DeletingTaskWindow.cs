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
    public partial class DeletingTaskWindow : Form
    {
        private Task taskToDelete;
        private weekPlaner main;
        public DeletingTaskWindow(weekPlaner main, Task taskToDelete)
        {
            InitializeComponent();

            this.taskToDelete = taskToDelete;
            this.main = main;

            showTaskProperties(taskToDelete);
        }
        private void showTaskProperties(Task task)
        {
            label1.Text = task.Name;
            label2.Text = task.Description;

            DateTime endingTime = task.PlanedTime.AddHours(task.Slots / Task.SlotsInAnHour);
            if (task.Slots % Task.SlotsInAnHour != 0) endingTime.AddMinutes(Task.SlotDurationMinutes);

            string minutes;
            if (endingTime.Minute == 0) minutes = "00";
            else minutes = "30";

            string date = task.PlanedTime.ToString();
            date = date.Remove(date.Length - 3, 3);

            label3.Text = $"{date} - {endingTime.Hour}:{minutes}";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            main.deleteTask(taskToDelete);

            this.Close();
        }
    }
}
