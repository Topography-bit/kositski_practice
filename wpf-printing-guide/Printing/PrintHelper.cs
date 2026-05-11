using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintApp.Printing;

public static class PrintHelper
{
    public static FlowDocument CreateSampleDocument()
    {
        var document = new FlowDocument
        {
            PagePadding = new Thickness(56),
            FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
            FontSize = 14,
            ColumnWidth = double.MaxValue
        };

        document.Blocks.Add(new Paragraph(new Run("RTF редактор + печать"))
        {
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 18)
        });

        document.Blocks.Add(new Paragraph(new Run(
            "Этот проект демонстрирует открытие и сохранение RTF, форматирование текста, предпросмотр и печать через стандартные средства WPF.")));

        var list = new List { MarkerStyle = TextMarkerStyle.Decimal };
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Введите или отформатируйте текст."))));
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Откройте предпросмотр перед печатью."))));
        list.ListItems.Add(new ListItem(new Paragraph(new Run("Отправьте документ на принтер через PrintDialog."))));
        document.Blocks.Add(list);

        return document;
    }

    public static FlowDocument CloneDocument(FlowDocument source)
    {
        using var stream = new MemoryStream();
        var sourceRange = new TextRange(source.ContentStart, source.ContentEnd);
        sourceRange.Save(stream, DataFormats.XamlPackage);
        stream.Position = 0;

        var clone = new FlowDocument
        {
            PagePadding = source.PagePadding,
            ColumnWidth = double.MaxValue
        };
        var targetRange = new TextRange(clone.ContentStart, clone.ContentEnd);
        targetRange.Load(stream, DataFormats.XamlPackage);
        return clone;
    }

    public static bool PrintDocument(FlowDocument document, string description)
    {
        var dialog = new PrintDialog();
        if (dialog.ShowDialog() != true)
        {
            return false;
        }

        document.PageWidth = dialog.PrintableAreaWidth;
        document.PageHeight = dialog.PrintableAreaHeight;
        document.ColumnWidth = double.MaxValue;
        var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
        dialog.PrintDocument(paginator, description);
        return true;
    }
}
