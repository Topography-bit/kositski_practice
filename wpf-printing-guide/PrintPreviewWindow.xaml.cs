using System.Windows;
using System.Windows.Documents;
using PrintApp.Printing;

namespace PrintApp;

public partial class PrintPreviewWindow : Window
{
    private readonly FlowDocument _document;

    public PrintPreviewWindow(FlowDocument document)
    {
        InitializeComponent();
        _document = document;
        Viewer.Document = _document;
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        PrintHelper.PrintDocument(PrintHelper.CloneDocument(_document), "RTF документ");
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
