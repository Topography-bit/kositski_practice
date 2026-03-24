using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using TaskPlanner.Models;

namespace TaskPlanner.ViewModels
{
    public class RelayCmd : ICommand{
        private readonly Action<object> _ex; private readonly Func<object,bool> _can;
        public RelayCmd(Action<object> ex, Func<object,bool> can=null){ _ex=ex; _can=can; }
        public bool CanExecute(object p) => _can?.Invoke(p) ?? true;
        public void Execute(object p) => _ex(p);
        public event EventHandler CanExecuteChanged;
        public void Raise() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    public class RelayCmd<T> : ICommand{
        private readonly Action<T> _ex;
        public RelayCmd(Action<T> ex) => _ex=ex;
        public bool CanExecute(object p) => true;
        public void Execute(object p){ if(p is T t) _ex(t); }
        public event EventHandler CanExecuteChanged;
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private string _currentTime=""; private string _newTitle=""; private DateTime _newDeadline=DateTime.Now.AddDays(1);

        public ObservableCollection<TaskItem> Tasks { get; } = new ObservableCollection<TaskItem>();
        public string CurrentTime { get=>_currentTime; set{_currentTime=value; OnPC(nameof(CurrentTime));} }
        public string NewTaskTitle { get=>_newTitle; set{_newTitle=value; OnPC(nameof(NewTaskTitle)); AddTaskCommand.Raise();} }
        public DateTime NewTaskDeadline { get=>_newDeadline; set{_newDeadline=value; OnPC(nameof(NewTaskDeadline));} }
        public RelayCmd AddTaskCommand { get; }
        public RelayCmd<TaskItem> RemoveTaskCommand { get; }
        public RelayCmd<TaskItem> CompleteTaskCommand { get; }

        public MainViewModel(){
            AddTaskCommand = new RelayCmd(_=>AddTask(), _=>!string.IsNullOrWhiteSpace(NewTaskTitle));
            RemoveTaskCommand = new RelayCmd<TaskItem>(t=>Tasks.Remove(t));
            CompleteTaskCommand = new RelayCmd<TaskItem>(t=>{ t.IsCompleted=true; t.Refresh(); });
            _timer = new DispatcherTimer{Interval=TimeSpan.FromSeconds(1)};
            _timer.Tick += (s,e)=>{ UpdTime(); foreach(var t in Tasks) t.Refresh(); };
            _timer.Start(); UpdTime();
            Tasks.Add(new TaskItem{Title="Сдать лабораторную",Deadline=DateTime.Now.AddHours(3)});
            Tasks.Add(new TaskItem{Title="Купить продукты",Deadline=DateTime.Now.AddDays(2)});
            Tasks.Add(new TaskItem{Title="Просроченная задача",Deadline=DateTime.Now.AddHours(-5)});
        }
        private void UpdTime(){ CurrentTime=DateTime.Now.ToString("dddd, dd MMMM yyyy | HH:mm:ss", new System.Globalization.CultureInfo("ru-RU")); }
        private void AddTask(){ Tasks.Add(new TaskItem{Title=NewTaskTitle.Trim(),Deadline=NewTaskDeadline}); NewTaskTitle=""; NewTaskDeadline=DateTime.Now.AddDays(1); }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPC(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
