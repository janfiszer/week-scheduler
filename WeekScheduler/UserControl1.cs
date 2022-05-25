using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeekScheduler
{
    public partial class weekPlaner: UserControl
    {
        private List<Week> weeks = new List<Week>();
        private Week currentWeek;
        private Point tableLocation = new Point(25, 20);
        //public DateTime aDayFormCurrentWeek { get; set; }
        
        TableLayoutPanel weekSchedule;

        bool reminderShowed = false;

        public weekPlaner()
        {
            InitializeComponent();

            this.initTableLayout();

            // interval = one second
            this.timer.Interval = 1000;
        }
        private void initTableLayout()
        {
            DateTime staringDayOfTheWeek = Week.getStartDay(DateTime.Now);
            Week week = new Week(staringDayOfTheWeek);
            currentWeek = week;

            this.addColumnNames();
            this.addRowNames();

            initWeekSchedule();
        }

        private void initWeekSchedule()
        {
            weekSchedule = currentWeek.toTableLayout();

            weekSchedule.CausesValidation = false;
            weekSchedule.Margin = new Padding(4, 4, 4, 4);

            weekSchedule.Location = tableLocation;
            weekSchedule.Size = new Size(Week.columnCount * Week.columnWight + ((Week.columnCount - 1) * 3), Week.rowHeight * Week.rowCount + 52); // TODO: 55 -> maths
            weekSchedule.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            this.Controls.Add(weekSchedule);

            foreach (Label label in currentWeek.Labels)
            {
                if (label != null)
                    label.Click += new EventHandler(this.timeSlotClick);
                //label.MouseHover += new System.EventHandler(this.SlotMouseHover);
            }

        }

        private void addColumnNames()
        {
            List<Label> columnNames = new List<Label>();
            Point labelLocation = new Point(tableLocation.X + 5, 0);

            int index = 0;
            for (DateTime dt = currentWeek.StartDay; dt < currentWeek.EndDay; dt = dt.AddDays(1))
            {
                columnNames.Add(new Label());

                // setting basic label properies
                columnNames[index].Text = dt.DayOfWeek + " " + dt.Date.ToString("d");
                columnNames[index].Location = labelLocation;
                columnNames[index].Font = new Font("Century Gothic", 6F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
                columnNames[index].AutoSize = true;
                // adding labels to Controls
                this.Controls.Add(columnNames[index]);

                // iteration
                labelLocation = new Point(labelLocation.X + Week.columnWight + 5, labelLocation.Y);
                index++;
            }
        }
        private void addRowNames()
        {
            List<Label> rowNames = new List<Label>();
            Point labelLocation = new Point(0, tableLocation.Y - 4);

            int hour;
            string minutes;
            for (int index = 0; index <= Week.rowCount; ++index)
            {
                rowNames.Add(new Label());

                // setting basic label properies
                if (index % 2 == 0) minutes = "00";
                else minutes = "30";

                hour = index / 2 + Week.StartingHour;

                rowNames[index].Text = $"{hour}:{minutes}";
                rowNames[index].Location = labelLocation;
                rowNames[index].Font = new Font("Century Gothic", 6F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
                rowNames[index].AutoSize = true;

                // adding labels to Controls
                this.Controls.Add(rowNames[index]);

                labelLocation = new Point(0, labelLocation.Y + Week.rowHeight + 3);
            }
        }
        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
        private int getMaxDuration(Label label)
        {
            int maxDuration = 0;
            (int, int) indecies = currentWeek.indeciesOfLabel(label);
            while (indecies.Item1 < Week.rowCount - 1 && currentWeek.Labels[indecies.Item1, indecies.Item2].Text == String.Empty)
            {
                indecies.Item1++;

                maxDuration++;
            }
            return maxDuration;
        }
        private Task setTask(DateTime dateTime, int maxDuration)
        {
            AddingTaskWindow addingTaskWindow = new AddingTaskWindow(dateTime, maxDuration);
            addingTaskWindow.ShowDialog();

            if (addingTaskWindow.Task != null)
                return addingTaskWindow.Task;
            else
                return null;
        }
        private void timeSlotClick(object sender, EventArgs e)
        {
            Label clickedTimeSlot = sender as Label;
            if (clickedTimeSlot != null)
            {
                if (clickedTimeSlot.Text == "")
                {
                    int maxDuartion = getMaxDuration(clickedTimeSlot);
                    Task task = setTask(currentWeek.dateFromLabel(clickedTimeSlot), maxDuartion);

                    // when someone closes dialog window
                    if (task == null)
                        return;

                    // Adding task to apropriate collections
                    currentWeek.addTask(task);

                    this.Controls.Remove(weekSchedule);

                    initWeekSchedule();
                }
                else
                {
                    //MessageBox.Show(this.getTaskByLabel(clickedTimeSlot).ToString());
                    DeletingTaskWindow deletingTaskWindow = new DeletingTaskWindow(this, currentWeek.LabelTaskPairs[clickedTimeSlot]);
                    deletingTaskWindow.Show();
                }
            }
        }

        public void deleteTask(Task task)
        {
            currentWeek.deleteTask(task);

            this.Controls.Remove(weekSchedule);

            initWeekSchedule();
        }

        public void clearLabel(Label label, int deletedTaskSlots)
        {
            for (int i = 1; i < deletedTaskSlots; ++i)
            {
                Label l = new Label();

                Week.setLabelPropeties(l);

                l.Click += new EventHandler(this.timeSlotClick);

                weekSchedule.Controls.Add(l);
            }
            //currentWeek.Labels[0, 0] = label;


            label.Text = String.Empty;
            label.BackColor = SystemColors.Control;

            //weekSchedule.Controls.Add(new Label());
            weekSchedule.SetRowSpan(label, 1);
        }
        private DateTime setSecondToZero(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, 0);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (var item in this.currentWeek.LabelTaskPairs)
            {
                Task task = item.Value;
                int reminderTime = task.RemindBeforeInMinutes;
                if (reminderTime != -1)
                {
                    DateTime nowWithSeconds = DateTime.Now;

                    DateTime now = setSecondToZero(nowWithSeconds);

                    if (now.AddMinutes(reminderTime).Equals(task.PlanedTime) && !reminderShowed)
                    {
                        reminderShowed = true;
                        MessageBox.Show($"Your task: {task.Name}\nDescribrion: {task.Description}\nStarts in {task.RemindBeforeInMinutes} minutes", "REMINDER");
                    }
                }
            }
        }
        // Gówno 
        /*
        private void SlotMouseHover(object sender, EventArgs e)
        {
            Label timeSlot = sender as Label;
            if (timeSlot == null)
                return;

            if (timeSlot.Text == String.Empty)
            {
                timeSlot.BackColor = Color.White;
            }
            else
            {
                if (currentWeek.LabelTaskPairs[timeSlot].Type == TaskType.SCHOOL)
                {
                    timeSlot.BackColor = Color.Pink;
                }
            }

        }*/
    }
}
