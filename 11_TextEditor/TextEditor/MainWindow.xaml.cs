using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace TextEditor
{
    public partial class MainWindow : Window
    {
        private string currentFile = "";
        public MainWindow() { InitializeComponent(); }

        private void Save_Click(object sender, RoutedEventArgs e){
            var dlg = new SaveFileDialog{ Filter="Текстовые (*.txt)|*.txt|Все (*.*)|*.*", DefaultExt=".txt" };
            if (dlg.ShowDialog()==true){ File.WriteAllText(dlg.FileName, EditorBox.Text); currentFile=dlg.FileName; Title="TextEdit - "+Path.GetFileName(currentFile); }
        }
        private void Open_Click(object sender, RoutedEventArgs e){
            var dlg = new OpenFileDialog{ Filter="Текстовые (*.txt)|*.txt|Все (*.*)|*.*" };
            if (dlg.ShowDialog()==true){ EditorBox.Text=File.ReadAllText(dlg.FileName); currentFile=dlg.FileName; Title="TextEdit - "+Path.GetFileName(currentFile); }
        }
        private void Clear_Click(object sender, RoutedEventArgs e){
            if (EditorBox.Text.Length>0 && MessageBox.Show("Очистить?","",MessageBoxButton.YesNo)==MessageBoxResult.Yes) EditorBox.Text="";
        }
        private void FontCombo_Changed(object sender, SelectionChangedEventArgs e){
            if (EditorBox==null) return;
            EditorBox.FontFamily = new FontFamily(((ComboBoxItem)FontCombo.SelectedItem).Content.ToString());
        }
        private void SizeCombo_Changed(object sender, SelectionChangedEventArgs e){
            if (EditorBox==null) return;
            EditorBox.FontSize = Convert.ToDouble(((ComboBoxItem)SizeCombo.SelectedItem).Content.ToString());
        }
    }
}
