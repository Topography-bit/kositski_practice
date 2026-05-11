using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ServerMonitor;

public partial class MainWindow : Window
{
    private const int MaxPoints = 60;
    private readonly DispatcherTimer _sampleTimer;
    private readonly DispatcherTimer _uptimeTimer;
    private readonly List<double> _cpuData = [];
    private readonly List<double> _ramData = [];
    private readonly Random _random = new();
    private DateTime _startedAt = DateTime.Now;
    private bool _running = true;
    private double _cpu = 35;
    private double _ram = 48;

    public MainWindow()
    {
        InitializeComponent();

        _sampleTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _sampleTimer.Tick += (_, _) => AddSample();
        _sampleTimer.Start();

        _uptimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _uptimeTimer.Tick += (_, _) => UpdateUptime();
        _uptimeTimer.Start();

        for (var i = 0; i < 20; i++)
        {
            AddSample();
        }
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _running = !_running;
        ToggleButton.Content = _running ? "Стоп" : "Старт";

        if (_running)
        {
            _sampleTimer.Start();
            _uptimeTimer.Start();
        }
        else
        {
            _sampleTimer.Stop();
            _uptimeTimer.Stop();
        }
    }

    private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        RedrawCharts();
    }

    private void AddSample()
    {
        _cpu = NextValue(_cpu, 8);
        _ram = NextValue(_ram, 4);

        Push(_cpuData, _cpu);
        Push(_ramData, _ram);

        CpuLabel.Text = $"{_cpu:0}%";
        RamLabel.Text = $"{_ram:0}%";
        RedrawCharts();
    }

    private void UpdateUptime()
    {
        UptimeLabel.Text = (DateTime.Now - _startedAt).ToString(@"hh\:mm\:ss");
    }

    private double NextValue(double current, double spread)
    {
        var next = current + (_random.NextDouble() * spread * 2 - spread);
        return Math.Clamp(next, 5, 98);
    }

    private static void Push(List<double> data, double value)
    {
        data.Add(value);
        if (data.Count > MaxPoints)
        {
            data.RemoveAt(0);
        }
    }

    private void RedrawCharts()
    {
        DrawChart(CpuCanvas, _cpuData, Color.FromRgb(9, 105, 218));
        DrawChart(RamCanvas, _ramData, Color.FromRgb(26, 127, 55));
    }

    private static void DrawChart(System.Windows.Controls.Canvas canvas, IReadOnlyList<double> values, Color color)
    {
        canvas.Children.Clear();
        if (values.Count < 2 || canvas.ActualWidth <= 10 || canvas.ActualHeight <= 10)
        {
            return;
        }

        var width = canvas.ActualWidth;
        var height = canvas.ActualHeight;
        var padding = 28d;
        var chartWidth = width - padding * 2;
        var chartHeight = height - padding * 2;

        for (var i = 0; i <= 4; i++)
        {
            var y = padding + chartHeight * i / 4;
            canvas.Children.Add(new Line
            {
                X1 = padding,
                X2 = width - padding,
                Y1 = y,
                Y2 = y,
                Stroke = Brushes.Gainsboro,
                StrokeThickness = 1
            });
        }

        var points = new PointCollection();
        for (var i = 0; i < values.Count; i++)
        {
            var x = padding + chartWidth * i / (MaxPoints - 1);
            var y = padding + chartHeight - values[i] / 100d * chartHeight;
            points.Add(new Point(x, y));
        }

        canvas.Children.Add(new Polyline
        {
            Points = points,
            Stroke = new SolidColorBrush(color),
            StrokeThickness = 2.5,
            StrokeLineJoin = PenLineJoin.Round
        });
    }
}
