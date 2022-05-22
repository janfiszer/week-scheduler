using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WeekScheduler
{
    public enum TaskType
    {
        OTHER,
        SCHOOL,
        WORK,
        SPORT
    }
    public class Task
    {
        public static readonly int SlotDurationMinutes = 30;
        public static readonly int SlotsInAnHour = 60 / SlotDurationMinutes;

        public static readonly Dictionary<TaskType, Color> SlotColors = new Dictionary<TaskType, Color>() {
            { TaskType.OTHER, Color.AliceBlue },
            { TaskType.SCHOOL, Color.Magenta },
            { TaskType.WORK, Color.Brown },
            { TaskType.SPORT, Color.Green }
        };


        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PlanedTime { get; set; }
        // one duration = half an hour
        public int Slots { get; set; }
        public TaskType Type { get; set; }
        public Task()
        {

        }
        public Task(string name,  DateTime planedTime, TaskType taskType, string description="", int duration=1)
        {
            this.Name = name;
            if (description == "")
            {
                this.Description = this.Name;
            }
            else
            {
                this.Description = description;
            }
            this.PlanedTime = planedTime;
            this.Slots = duration;
            this.Type = taskType;
        }
        public override string ToString()
        {
            return $"{Name} {Description} {PlanedTime} {PlanedTime.DayOfWeek}";
        }
    }
}
