using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskPlanner.Converters
{
    public class DeadlineColorConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            if (value is bool over && over) return new SolidColorBrush(Color.FromRgb(220, 50, 47));
            return new SolidColorBrush(Color.FromRgb(42, 161, 152));
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
