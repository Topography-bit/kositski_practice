using System.IO;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Win32;
using PrintApp.Printing;

namespace PrintApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Editor.Document = PrintHelper.CreateSampleDocument();
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        Editor.Document = new FlowDocument(new Paragraph(new Run("Новый документ")));
        StatusLabel.Text = "Создан новый документ.";
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "RTF документ (*.rtf)|*.rtf|Все файлы|*.*"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        using var stream = File.OpenRead(dialog.FileName);
        var range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
        range.Load(stream, DataFormats.Rtf);
        StatusLabel.Text = $"Открыт файл: {Path.GetFileName(dialog.FileName)}";
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "RTF документ (*.rtf)|*.rtf",
            FileName = "document.rtf"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        using var stream = File.Create(dialog.FileName);
        var range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
        range.Save(stream, DataFormats.Rtf);
        StatusLabel.Text = $"Сохранено: {Path.GetFileName(dialog.FileName)}";
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        var previewDocument = PrintHelper.CloneDocument(Editor.Document);
        var previewWindow = new PrintPreviewWindow(previewDocument)
        {
            Owner = this
        };
        previewWindow.ShowDialog();
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        var document = PrintHelper.CloneDocument(Editor.Document);
        if (PrintHelper.PrintDocument(document, "RTF документ"))
        {
            StatusLabel.Text = "Документ отправлен на печать.";
        }
    }
}
