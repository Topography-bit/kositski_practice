using System.ComponentModel;

namespace LibraryApp.Models
{
    public class Book : INotifyPropertyChanged
    {
        private string _title;
        private string _author;
        private int _year;
        private string _genre;

        public string Title { get => _title; set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public string Author { get => _author; set { _author = value; OnPropertyChanged(nameof(Author)); } }
        public int Year { get => _year; set { _year = value; OnPropertyChanged(nameof(Year)); } }
        public string Genre { get => _genre; set { _genre = value; OnPropertyChanged(nameof(Genre)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string p) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
