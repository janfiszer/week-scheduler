using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WeekScheduler
{
    public partial class weekPlaner : UserControl
    {
        public int MaxWeeksFoward { get; set; }
        private List<Week> weeks = new List<Week>();
        private List<TableLayoutPanel> weekSchedules = new List<TableLayoutPanel>();

        private int currentWeekIndex = 0;
        private TableLayoutPanel currentWeekSchedule;
        private Week currentWeek;
        private Point tableLocation = new Point(25, 20);


        private List<Label> columnNames = new List<Label>();
        // to reminde once 
        private bool reminderShowed = false;

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
            weeks.Add(week);

            this.addColumnNames();
            this.addRowNames();

            this.initWeekSchedule();
            weekSchedules.Add(currentWeekSchedule);

            MaxWeeksFoward = 20;
        }
        private void initWeekSchedule()
        {
            if (this.weekSchedules.Count != 0)
            {
                currentWeekSchedule.Visible = false;
            }
            refreshCurrentWeekSchedule();
        }
        private void refreshCurrentWeekSchedule()
        {
            currentWeekSchedule = currentWeek.toTableLayout();

            currentWeekSchedule.Name = currentWeek.StartDay.ToString();

            currentWeekSchedule.CausesValidation = false;
            currentWeekSchedule.Margin = new Padding(4, 4, 4, 4);

            currentWeekSchedule.Location = tableLocation;
            currentWeekSchedule.Size = new Size(Week.columnCount * Week.columnWight + ((Week.columnCount - 1) * 3), Week.rowHeight * Week.rowCount + 52); // TODO: 55 -> maths
            currentWeekSchedule.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            this.Controls.Add(currentWeekSchedule);

            foreach (Label label in currentWeek.Labels)
            {
                if (label != null)
                    label.Click += new EventHandler(this.timeSlotClick);
            }
        }
        private void addColumnNames()
        {
            // reseting previously added column names
            foreach (Control c in columnNames)
            {
                this.Controls.Remove(c);
            }
            columnNames.Clear();


            Point labelLocation = new Point(tableLocation.X + 5, 0);

            int index = 0;
            for (DateTime dt = currentWeek.StartDay; dt <= currentWeek.EndDay; dt = dt.AddDays(1))
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

                    addTaskAction(task);
                }
                else
                {
                    DeletingTaskWindow deletingTaskWindow = new DeletingTaskWindow(this, currentWeek.LabelTaskPairs[clickedTimeSlot]);
                    deletingTaskWindow.ShowDialog();
                }
            }
        }
        public void addTaskAction(Task task)
        {
            // Adding task to apropriate collections
            currentWeek.addTask(task);

            this.Controls.Remove(currentWeekSchedule);

            refreshCurrentWeekSchedule();

            weekSchedules[currentWeekIndex] = currentWeekSchedule;
        }
        public void deleteTask(Task task)
        {
            currentWeek.deleteTask(task);

            this.Controls.Remove(currentWeekSchedule);

            refreshCurrentWeekSchedule();

            weekSchedules[currentWeekIndex] = currentWeekSchedule;
        }
        public void clearLabel(Label label, int taskSlots)
        {
            for (int i = 1; i < taskSlots; ++i)
            {
                Label l = new Label();

                Week.setLabelPropeties(l);

                l.Click += new EventHandler(this.timeSlotClick);

                currentWeekSchedule.Controls.Add(l);
            }

            label.Text = String.Empty;
            label.BackColor = SystemColors.Control;

            currentWeekSchedule.SetRowSpan(label, 1);
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

                    DateTime now = Week.setSecondToZero(nowWithSeconds);

                    if (now.AddMinutes(reminderTime).Equals(task.PlanedTime) && !reminderShowed)
                    {
                        reminderShowed = true;
                        MessageBox.Show($"Your task: {task.Name}\nDescribrion: {task.Description}\nStarts in {task.RemindBeforeInMinutes} minutes", "REMINDER");
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Enabled == false)
            {
                button1.Enabled = true;
            }
            // memory safty
            if (weeks.Count > MaxWeeksFoward)
            {
                button2.Enabled = false;
            }
            currentWeekIndex++;

            if (currentWeekIndex == weekSchedules.Count)
            {
                Week week = new Week(currentWeek.StartDay.AddDays(7));
                weeks.Add(week);

                currentWeek = week;

                this.initWeekSchedule();

                weekSchedules.Add(currentWeekSchedule);
            }
            else
            {
                currentWeekSchedule.Visible = false;

                // moving one forward
                currentWeek = weeks[currentWeekIndex];
                currentWeekSchedule = weekSchedules[currentWeekIndex];

                currentWeekSchedule.Visible = true;
            }

            this.addColumnNames();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            currentWeekSchedule.Visible = false;

            // moving one backword
            currentWeekIndex--;
            currentWeekSchedule = weekSchedules[currentWeekIndex];
            currentWeek = weeks[currentWeekIndex];

            currentWeekSchedule.Visible = true;

            this.addColumnNames();

            if (currentWeekIndex == 0)
            {
                button1.Enabled = false;
            }
        }
    }
}
