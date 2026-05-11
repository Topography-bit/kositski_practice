using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace WpfPaint;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ApplyDefaultAttributes();
        PaintCanvas.SizeChanged += (_, _) => ResizeBackgroundImage();
    }

    private void ApplyDefaultAttributes()
    {
        PaintCanvas.DefaultDrawingAttributes = new DrawingAttributes
        {
            Color = Colors.Black,
            Width = 5,
            Height = 5,
            StylusTip = StylusTip.Ellipse
        };
    }

    private void OnModeChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton || PaintCanvas is null)
        {
            return;
        }

        PaintCanvas.EditingMode = radioButton.Tag?.ToString() switch
        {
            "EraseByPoint" => InkCanvasEditingMode.EraseByPoint,
            "Select" => InkCanvasEditingMode.Select,
            _ => InkCanvasEditingMode.Ink
        };
    }

    private void OnColorChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ColorComboBox.SelectedItem is not ComboBoxItem item || PaintCanvas is null)
        {
            return;
        }

        var color = (Color)ColorConverter.ConvertFromString(item.Tag?.ToString() ?? "Black");
        var attrs = PaintCanvas.DefaultDrawingAttributes.Clone();
        attrs.Color = HighlighterCheckBox.IsChecked == true
            ? Color.FromArgb(120, color.R, color.G, color.B)
            : color;
        PaintCanvas.DefaultDrawingAttributes = attrs;
    }

    private void OnThicknessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (PaintCanvas is null || ThicknessLabel is null)
        {
            return;
        }

        var thickness = Math.Round(e.NewValue);
        ThicknessLabel.Text = thickness.ToString();

        if (HighlighterCheckBox?.IsChecked == true)
        {
            return;
        }

        var attrs = PaintCanvas.DefaultDrawingAttributes.Clone();
        attrs.Width = thickness;
        attrs.Height = thickness;
        PaintCanvas.DefaultDrawingAttributes = attrs;
    }

    private void OnHighlighterChanged(object sender, RoutedEventArgs e)
    {
        if (PaintCanvas is null || HighlighterCheckBox is null)
        {
            return;
        }

        var attrs = PaintCanvas.DefaultDrawingAttributes.Clone();
        attrs.IsHighlighter = HighlighterCheckBox.IsChecked == true;

        if (attrs.IsHighlighter)
        {
            attrs.Width = 20;
            attrs.Height = 8;
            attrs.StylusTip = StylusTip.Rectangle;
            attrs.Color = Color.FromArgb(120, attrs.Color.R, attrs.Color.G, attrs.Color.B);
        }
        else
        {
            attrs.Width = ThicknessSlider.Value;
            attrs.Height = ThicknessSlider.Value;
            attrs.StylusTip = StylusTip.Ellipse;
            attrs.Color = Color.FromArgb(255, attrs.Color.R, attrs.Color.G, attrs.Color.B);
        }

        PaintCanvas.DefaultDrawingAttributes = attrs;
    }

    private void OnOpenImage(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp|Все файлы|*.*"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze();

        BackgroundImage.Source = bitmap;
        ResizeBackgroundImage();
    }

    private void OnClear(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Очистить холст и фон?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        PaintCanvas.Strokes.Clear();
        BackgroundImage.Source = null;
    }

    private void OnUndo(object sender, RoutedEventArgs e)
    {
        if (PaintCanvas.Strokes.Count == 0)
        {
            return;
        }

        PaintCanvas.Strokes.RemoveAt(PaintCanvas.Strokes.Count - 1);
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (PaintCanvas.ActualWidth <= 0 || PaintCanvas.ActualHeight <= 0)
        {
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = "PNG Image (*.png)|*.png",
            FileName = "drawing.png"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var width = (int)Math.Ceiling(PaintCanvas.ActualWidth);
        var height = (int)Math.Ceiling(PaintCanvas.ActualHeight);
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

        PaintCanvas.Measure(new Size(width, height));
        PaintCanvas.Arrange(new Rect(0, 0, width, height));
        PaintCanvas.UpdateLayout();
        bitmap.Render(PaintCanvas);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        using var stream = File.Create(dialog.FileName);
        encoder.Save(stream);

        MessageBox.Show("Изображение сохранено.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ResizeBackgroundImage()
    {
        BackgroundImage.Width = PaintCanvas.ActualWidth;
        BackgroundImage.Height = PaintCanvas.ActualHeight;
    }
}
