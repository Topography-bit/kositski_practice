using System;
using System.Collections.Generic;
using System.Windows;

namespace TodoApp
{
    public partial class MainWindow : Window
    {
        private List<string> tasks = new List<string>();
        public MainWindow() { InitializeComponent(); }

        private void AddTask_Click(object sender, RoutedEventArgs e){
            string task = NewTaskBox.Text.Trim();
            if (string.IsNullOrEmpty(task)){ MessageBox.Show("Введите задачу!"); return; }
            tasks.Add(task); TaskList.Items.Add(task);
            NewTaskBox.Text = ""; NewTaskBox.Focus();
        }
        private void RemoveTask_Click(object sender, RoutedEventArgs e){
            if (TaskList.SelectedIndex == -1) return;
            int i = TaskList.SelectedIndex; tasks.RemoveAt(i); TaskList.Items.RemoveAt(i);
        }
        private void CompleteTask_Click(object sender, RoutedEventArgs e){
            if (TaskList.SelectedIndex == -1) return;
            int i = TaskList.SelectedIndex;
            if (!tasks[i].StartsWith("\u2713 ")){ tasks[i] = "\u2713 " + tasks[i]; TaskList.Items[i] = tasks[i]; }
        }
        private void ClearAll_Click(object sender, RoutedEventArgs e){
            if (MessageBox.Show("Очистить?","",MessageBoxButton.YesNo)==MessageBoxResult.Yes){ tasks.Clear(); TaskList.Items.Clear(); }
        }
        private void SortTasks_Click(object sender, RoutedEventArgs e){
            tasks.Sort(); TaskList.Items.Clear(); foreach(var t in tasks) TaskList.Items.Add(t);
        }
    }
}
