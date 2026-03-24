using System;
using System.ComponentModel;

namespace TaskPlanner.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        private string _title = ""; private DateTime _deadline; private bool _isCompleted;
        public string Title { get => _title; set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public DateTime Deadline { get => _deadline;
            set { _deadline = value; OnPropertyChanged(nameof(Deadline)); OnPropertyChanged(nameof(TimeLeft)); OnPropertyChanged(nameof(IsOverdue)); OnPropertyChanged(nameof(DeadlineFormatted)); OnPropertyChanged(nameof(TimeLeftFormatted)); } }
        public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged(nameof(IsCompleted)); } }
        public TimeSpan TimeLeft => Deadline - DateTime.Now;
        public bool IsOverdue => DateTime.Now > Deadline && !IsCompleted;
        public string DeadlineFormatted => Deadline.ToString("dd MMMM yyyy, HH:mm", new System.Globalization.CultureInfo("ru-RU"));
        public string TimeLeftFormatted{
            get{
                if (IsCompleted) return "Выполнено";
                var s = TimeLeft; if (s.TotalSeconds<=0) return "ПРОСРОЧЕНО";
                if (s.TotalDays>=1) return $"{(int)s.TotalDays} д. {s.Hours} ч. {s.Minutes} мин.";
                if (s.TotalHours>=1) return $"{s.Hours} ч. {s.Minutes} мин.";
                return $"{s.Minutes} мин. {s.Seconds} сек.";
            }
        }
        public void Refresh(){ OnPropertyChanged(nameof(TimeLeft)); OnPropertyChanged(nameof(IsOverdue)); OnPropertyChanged(nameof(TimeLeftFormatted)); }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
