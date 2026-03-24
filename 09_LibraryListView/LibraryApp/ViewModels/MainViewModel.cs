using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibraryApp.Models;

namespace LibraryApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Book> _books;
        private Book _selectedBook;
        private string _newTitle; private string _newAuthor;
        private int _newYear; private string _newGenre; private string _searchText;

        public MainViewModel(){
            Books = new ObservableCollection<Book>();
            LoadSampleData();
            AddBookCommand = new RelayCommand(AddBook, CanAddBook);
            RemoveBookCommand = new RelayCommand(RemoveBook, p => SelectedBook != null);
            EditBookCommand = new RelayCommand(EditBook, p => SelectedBook != null && !string.IsNullOrWhiteSpace(NewTitle));
            ClearFieldsCommand = new RelayCommand(ClearFields);
        }

        public ObservableCollection<Book> Books { get => _books; set { _books = value; OnPropertyChanged(nameof(Books)); } }
        public Book SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(nameof(SelectedBook));
                if (_selectedBook != null){ NewTitle = _selectedBook.Title; NewAuthor = _selectedBook.Author; NewYear = _selectedBook.Year; NewGenre = _selectedBook.Genre; }
            }
        }
        public string NewTitle { get => _newTitle; set { _newTitle = value; OnPropertyChanged(nameof(NewTitle)); } }
        public string NewAuthor { get => _newAuthor; set { _newAuthor = value; OnPropertyChanged(nameof(NewAuthor)); } }
        public int NewYear { get => _newYear; set { _newYear = value; OnPropertyChanged(nameof(NewYear)); } }
        public string NewGenre { get => _newGenre; set { _newGenre = value; OnPropertyChanged(nameof(NewGenre)); } }
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(nameof(SearchText)); OnPropertyChanged(nameof(FilteredBooks)); } }

        public ObservableCollection<Book> FilteredBooks{
            get{
                if (string.IsNullOrWhiteSpace(SearchText)) return Books;
                return new ObservableCollection<Book>(Books.Where(b =>
                    b.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    b.Author.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    b.Genre.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
            }
        }
        public ICommand AddBookCommand { get; }
        public ICommand RemoveBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        private void AddBook(object p){ Books.Add(new Book{ Title=NewTitle, Author=NewAuthor, Year=NewYear, Genre=NewGenre }); ClearFields(null); }
        private bool CanAddBook(object p) => !string.IsNullOrWhiteSpace(NewTitle) && !string.IsNullOrWhiteSpace(NewAuthor) && NewYear > 0;
        private void RemoveBook(object p){ if(SelectedBook!=null && MessageBox.Show($"Удалить \'{SelectedBook.Title}\'?","",MessageBoxButton.YesNo)==MessageBoxResult.Yes){ Books.Remove(SelectedBook); ClearFields(null); } }
        private void EditBook(object p){ if(SelectedBook!=null){ SelectedBook.Title=NewTitle; SelectedBook.Author=NewAuthor; SelectedBook.Year=NewYear; SelectedBook.Genre=NewGenre; } }
        private void ClearFields(object p){ NewTitle=""; NewAuthor=""; NewYear=0; NewGenre=""; SelectedBook=null; }
        private void LoadSampleData(){
            Books.Add(new Book{ Title="Война и мир", Author="Лев Толстой", Year=1869, Genre="Роман" });
            Books.Add(new Book{ Title="Преступление и наказание", Author="Федор Достоевский", Year=1866, Genre="Роман" });
            Books.Add(new Book{ Title="Мастер и Маргарита", Author="Михаил Булгаков", Year=1967, Genre="Фантастика" });
            Books.Add(new Book{ Title="1984", Author="Джордж Оруэлл", Year=1949, Genre="Антиутопия" });
            Books.Add(new Book{ Title="Гарри Поттер", Author="Дж. К. Роулинг", Year=1997, Genre="Фэнтези" });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
