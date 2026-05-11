using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FinDash;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, AssetData> _assets = [];
    private readonly Brush[] _palette =
    [
        new SolidColorBrush(Color.FromRgb(9, 105, 218)),
        new SolidColorBrush(Color.FromRgb(26, 127, 55)),
        new SolidColorBrush(Color.FromRgb(191, 135, 0)),
        new SolidColorBrush(Color.FromRgb(207, 34, 46)),
        new SolidColorBrush(Color.FromRgb(130, 80, 223))
    ];
    private bool _darkTheme;

    public MainWindow()
    {
        InitializeComponent();
        GenerateAssets();
        AssetComboBox.ItemsSource = _assets.Keys;
        AssetComboBox.SelectedIndex = 0;
        ApplyTheme();
    }

    private void AssetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RefreshDashboard();
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        _darkTheme = !_darkTheme;
        ApplyTheme();
        RefreshDashboard();
    }

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        RefreshDashboard();
    }

    private void GenerateAssets()
    {
        var random = new Random(42);
        AddAsset("BTC", 69400, random);
        AddAsset("ETH", 3680, random);
        AddAsset("AAPL", 186, random);
        AddAsset("MSFT", 412, random);
    }

    private void AddAsset(string symbol, decimal startPrice, Random random)
    {
        var prices = new List<PricePoint>();
        var price = startPrice;
        for (var i = 29; i >= 0; i--)
        {
            price *= 1 + (decimal)(random.NextDouble() * 0.06 - 0.03);
            prices.Add(new PricePoint(DateTime.Today.AddDays(-i), decimal.Round(price, 2)));
        }

        var volumes = Enumerable.Range(0, 7)
            .Select(_ => random.Next(65_000, 420_000))
            .ToArray();

        var allocations = new Dictionary<string, double>
        {
            ["Акции"] = random.Next(20, 42),
            ["Крипто"] = random.Next(18, 35),
            ["Облигации"] = random.Next(10, 28),
            ["Кэш"] = random.Next(8, 18)
        };

        _assets[symbol] = new AssetData(symbol, prices, volumes, Normalize(allocations));
    }

    private static Dictionary<string, double> Normalize(Dictionary<string, double> values)
    {
        var sum = values.Values.Sum();
        return values.ToDictionary(pair => pair.Key, pair => Math.Round(pair.Value / sum * 100, 1));
    }

    private AssetData? CurrentAsset()
    {
        return AssetComboBox.SelectedItem is string key && _assets.TryGetValue(key, out var asset) ? asset : null;
    }

    private void RefreshDashboard()
    {
        var asset = CurrentAsset();
        if (asset is null)
        {
            return;
        }

        var last = asset.Prices[^1].Price;
        var previous = asset.Prices[^2].Price;
        var change = (last - previous) / previous * 100;

        PriceLabel.Text = $"{last:N2} $";
        ChangeLabel.Text = $"{change:+0.00;-0.00;0.00}%";
        ChangeLabel.Foreground = change >= 0 ? Brushes.ForestGreen : Brushes.Firebrick;
        VolumeLabel.Text = $"{asset.Volumes[^1]:N0}";

        DrawLineChart(asset);
        DrawPieChart(asset);
        DrawBarChart(asset);
    }

    private void DrawLineChart(AssetData asset)
    {
        PriceCanvas.Children.Clear();
        if (PriceCanvas.ActualWidth <= 20 || PriceCanvas.ActualHeight <= 20)
        {
            return;
        }

        var padding = 34d;
        var width = PriceCanvas.ActualWidth - padding * 2;
        var height = PriceCanvas.ActualHeight - padding * 2;
        var min = asset.Prices.Min(point => point.Price);
        var max = asset.Prices.Max(point => point.Price);
        var range = Math.Max(1, (double)(max - min));
        var points = new PointCollection();

        DrawGrid(PriceCanvas, padding);

        for (var i = 0; i < asset.Prices.Count; i++)
        {
            var point = asset.Prices[i];
            var x = padding + width * i / (asset.Prices.Count - 1);
            var y = padding + height - ((double)(point.Price - min) / range * height);
            points.Add(new Point(x, y));

            var marker = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = _palette[0],
                ToolTip = $"{point.Date:dd.MM}: {point.Price:N2} $"
            };
            Canvas.SetLeft(marker, x - 4);
            Canvas.SetTop(marker, y - 4);
            PriceCanvas.Children.Add(marker);

            if (i % 5 == 0)
            {
                AddCanvasText(PriceCanvas, point.Date.ToString("dd.MM"), x - 18, PriceCanvas.ActualHeight - 24, 11);
            }
        }

        PriceCanvas.Children.Add(new Polyline
        {
            Points = points,
            Stroke = _palette[0],
            StrokeThickness = 2.5,
            StrokeLineJoin = PenLineJoin.Round
        });
    }

    private void DrawPieChart(AssetData asset)
    {
        PortfolioCanvas.Children.Clear();
        if (PortfolioCanvas.ActualWidth <= 20 || PortfolioCanvas.ActualHeight <= 20)
        {
            return;
        }

        var radius = Math.Min(PortfolioCanvas.ActualWidth, PortfolioCanvas.ActualHeight) * 0.31;
        var center = new Point(PortfolioCanvas.ActualWidth * 0.42, PortfolioCanvas.ActualHeight * 0.48);
        var angle = -90d;
        var index = 0;

        foreach (var pair in asset.Allocation)
        {
            var sweep = pair.Value / 100d * 360d;
            PortfolioCanvas.Children.Add(CreateSlice(center, radius, angle, sweep, _palette[index % _palette.Length]));
            AddCanvasText(PortfolioCanvas, $"{pair.Key}: {pair.Value:0.#}%", PortfolioCanvas.ActualWidth * 0.72, 24 + index * 24, 12);
            angle += sweep;
            index++;
        }
    }

    private void DrawBarChart(AssetData asset)
    {
        VolumeCanvas.Children.Clear();
        if (VolumeCanvas.ActualWidth <= 20 || VolumeCanvas.ActualHeight <= 20)
        {
            return;
        }

        var days = new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        var padding = 30d;
        var width = VolumeCanvas.ActualWidth - padding * 2;
        var height = VolumeCanvas.ActualHeight - padding * 2;
        var max = asset.Volumes.Max();
        var barWidth = width / asset.Volumes.Length * 0.62;

        DrawGrid(VolumeCanvas, padding);

        for (var i = 0; i < asset.Volumes.Length; i++)
        {
            var barHeight = asset.Volumes[i] / (double)max * height;
            var x = padding + i * width / asset.Volumes.Length + barWidth * 0.25;
            var y = padding + height - barHeight;

            VolumeCanvas.Children.Add(new Rectangle
            {
                Width = barWidth,
                Height = barHeight,
                Fill = _palette[1],
                RadiusX = 4,
                RadiusY = 4,
                ToolTip = $"{days[i]}: {asset.Volumes[i]:N0}"
            }.At(x, y));
            AddCanvasText(VolumeCanvas, days[i], x + barWidth / 2 - 10, VolumeCanvas.ActualHeight - 24, 11);
        }
    }

    private void DrawGrid(Canvas canvas, double padding)
    {
        var lineBrush = _darkTheme ? Brushes.DimGray : Brushes.Gainsboro;
        for (var i = 0; i <= 4; i++)
        {
            var y = padding + (canvas.ActualHeight - padding * 2) * i / 4;
            canvas.Children.Add(new Line
            {
                X1 = padding,
                X2 = canvas.ActualWidth - padding,
                Y1 = y,
                Y2 = y,
                Stroke = lineBrush,
                StrokeThickness = 1
            });
        }
    }

    private static Path CreateSlice(Point center, double radius, double startAngle, double sweepAngle, Brush fill)
    {
        var start = PointOnCircle(center, radius, startAngle);
        var end = PointOnCircle(center, radius, startAngle + sweepAngle);
        var figure = new PathFigure { StartPoint = center, IsClosed = true };
        figure.Segments.Add(new LineSegment(start, true));
        figure.Segments.Add(new ArcSegment(end, new Size(radius, radius), 0, sweepAngle > 180, SweepDirection.Clockwise, true));
        figure.Segments.Add(new LineSegment(center, true));

        return new Path
        {
            Data = new PathGeometry([figure]),
            Fill = fill,
            Stroke = Brushes.White,
            StrokeThickness = 1
        };
    }

    private static Point PointOnCircle(Point center, double radius, double angleDegrees)
    {
        var angle = angleDegrees * Math.PI / 180d;
        return new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
    }

    private void AddCanvasText(Canvas canvas, string text, double x, double y, double fontSize)
    {
        var block = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            Foreground = _darkTheme ? Brushes.Gainsboro : Brushes.DimGray
        };
        Canvas.SetLeft(block, x);
        Canvas.SetTop(block, y);
        canvas.Children.Add(block);
    }

    private void ApplyTheme()
    {
        var background = _darkTheme ? Color.FromRgb(22, 27, 34) : Colors.White;
        var panel = _darkTheme ? Color.FromRgb(33, 38, 45) : Color.FromRgb(246, 248, 250);
        var border = _darkTheme ? Color.FromRgb(48, 54, 61) : Color.FromRgb(208, 215, 222);
        var foreground = _darkTheme ? Brushes.WhiteSmoke : Brushes.Black;

        RootGrid.Background = new SolidColorBrush(background);
        foreach (var element in new[] { PriceCard, ChangeCard, VolumeCard, LinePanel, PiePanel, BarPanel })
        {
            element.Background = new SolidColorBrush(panel);
            element.BorderBrush = new SolidColorBrush(border);
        }

        foreach (var canvas in new[] { PriceCanvas, PortfolioCanvas, VolumeCanvas })
        {
            canvas.Background = new SolidColorBrush(_darkTheme ? Color.FromRgb(13, 17, 23) : Colors.White);
        }

        Foreground = foreground;
        ThemeButton.Content = _darkTheme ? "Светлая тема" : "Темная тема";
    }

    private sealed record PricePoint(DateTime Date, decimal Price);

    private sealed record AssetData(
        string Symbol,
        List<PricePoint> Prices,
        int[] Volumes,
        Dictionary<string, double> Allocation);
}

internal static class CanvasElementExtensions
{
    public static T At<T>(this T element, double x, double y) where T : UIElement
    {
        Canvas.SetLeft(element, x);
        Canvas.SetTop(element, y);
        return element;
    }
}
