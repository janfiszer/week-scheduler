using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WeekScheduler
{
    public class Week
    {
        // start od the week
        public DateTime StartDay { get; set; }
        // end of the week
        public DateTime EndDay { get; set; }

        // one slot is 30 minutes
        public static readonly int StartingHour = 6;
        // table properties
        public static readonly int columnWight = 150;
        public static readonly int rowHeight = 20;
        public static readonly int columnCount = 7;
        public static readonly int rowCount = 24;


        private List<Task> tasks = new List<Task>();
        public Label[,] Labels;
        public Dictionary<Label, Task> LabelTaskPairs = new Dictionary<Label, Task>();

        public Week()
        {

        }
        public Week(DateTime startDay)
        {
            this.StartDay = startDay;
            this.EndDay = startDay.AddDays(7);
        }
        public static DateTime getStartDay(DateTime day)
        {
            int dayOfWeek;
            if (day.DayOfWeek == 0) dayOfWeek = 7 - 1;
            else dayOfWeek = (int)day.DayOfWeek - 1;
            return new DateTime(day.Year, day.Month, day.Day - dayOfWeek);
        }
        public TableLayoutPanel toTableLayout()
        {
            TableLayoutPanel scheduledWeek = new TableLayoutPanel();
            // not sure what it does and why to use it
            //scheduledWeek.SuspendLayout();

            scheduledWeek.Name = "Scheduled week";


            scheduledWeek.ColumnCount = columnCount;
            scheduledWeek.RowCount = rowCount;

            for (int i = 0; i < columnCount; i++)
            {
                scheduledWeek.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWight));
            }
            for (int i = 0; i < rowCount; i++)
            {
                scheduledWeek.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            }

            Labels = new Label[rowCount, columnCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    Labels[i, j] = new Label();
                    //Labels[i, j].Text = "label nr " + i.ToString() + " is empty";
                    setLabelPropeties(Labels[i, j]);
                }
            }

            // adding task to the table and mapping it with assosiated labels
            foreach (Task task in tasks)
            {
                // it turned out that sunday is 0 not 7 ;dd
                int column;
                if (task.PlanedTime.DayOfWeek == DayOfWeek.Sunday) column = 6;
                else column = (int)task.PlanedTime.DayOfWeek - 1;

                int row = rowIndexFromAnHour(task.PlanedTime);
                
                LabelTaskPairs[Labels[row, column]] = task;

                Labels[row, column].Text = task.Name;
                Labels[row, column].BackColor = Task.SlotColors[task.Type];

                // case when Task.Slots > 1
                for (int i = 1; i < task.Slots; i++)
                {
                    Labels[row + i, column] = null;
                }
                Labels[row, column].Size = new Size(Week.columnWight, (Week.rowHeight + 2) * task.Slots);
                scheduledWeek.SetRowSpan(Labels[row, column], task.Slots);
            }

            // adding label to the table
            foreach (Label label in Labels)
            {
                if (label != null)
                    scheduledWeek.Controls.Add(label);
            }

            return scheduledWeek;
        }
        public void addTask(Task task/*, Label label.*/)
        {
            if (task == null)
            {
                return;
            }
            if (task.PlanedTime.Hour > Week.StartingHour + (Week.rowCount/Task.SlotsInAnHour) || task.PlanedTime.Hour < Week.StartingHour)
            {
                return;
            }
            if (task.PlanedTime.Minute != 30 && task.PlanedTime.Minute != 0)
            {
                return;
            }
            /*label.Text = task.Name;
            label.BackColor = Task.SlotColors[task.Type];

            LabelTaskPairs[label] = task;*/
            tasks.Add(task);
        }

        public void deleteTask(Task task)
        {
            tasks.Remove(task);
            // TODO: Removing by value better
            var item = LabelTaskPairs.First(i => i.Value.Equals(task));
            LabelTaskPairs.Remove(item.Key);
        }
        private int rowIndexFromAnHour(DateTime dateTime)
        {
            if (dateTime != null)
            {
                if (dateTime.Minute == Task.SlotDurationMinutes)
                    return ((dateTime.Hour - StartingHour) * Task.SlotsInAnHour) + 1; 
                else
                    return ((dateTime.Hour - StartingHour) * Task.SlotsInAnHour);
            }
            else 
                return -1;
        }
        public (int, int) indeciesOfLabel(Label label)
        {
            int i, j;
            for (i = 0; i < rowCount; i++)
            {
                for (j = 0; j < columnCount; j++)
                {
                    if (Labels[i, j] != null)
                    {
                        if (Labels[i, j].Equals(label))
                        {
                            return (i, j);
                        }
                    }
                }
            }
            return (-1, -1);
        }
        public DateTime dateFromLabel(Label label)
        {
            (int, int) indecies = indeciesOfLabel(label);

            DateTime date = this.StartDay.AddDays(indecies.Item2);

            int minutes;
            if (indecies.Item1 % Task.SlotsInAnHour == 0) minutes = 0;
            else minutes = Task.SlotDurationMinutes;
            
            int hour = indecies.Item1 / Task.SlotsInAnHour + StartingHour;

            return new DateTime(date.Year, date.Month, date.Day, hour, minutes, 0, 0);
        }
        public static void setLabelPropeties(Label label)
        {
            label.Text = String.Empty;
            //Labels[i, j].Dock = DockStyle.Fill;
            label.Size = new Size(Week.columnWight + 100, Week.rowHeight + 100);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            label.Margin = new Padding(0);
            label.Font = new Font("Showcard Gothic", 8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
        }
    }
}
