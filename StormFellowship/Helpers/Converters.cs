using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace StormFellowship.Helpers;

public class BoolToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool b = value is bool flag && flag;
        bool invertParam = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);
        if (Invert ^ invertParam) b = !b;
        return b ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class NullToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value == null;
        bool invertParam = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);
        if (Invert ^ invertParam) isNull = !isNull;
        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrWhiteSpace(hex))
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class StringNotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool notEmpty = value is string str && !string.IsNullOrEmpty(str);
        bool invertParam = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);
        if (invertParam) notEmpty = !notEmpty;
        return notEmpty ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToMutedColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool muted = value is bool b && b;
        return muted ? new SolidColorBrush(Color.FromRgb(239, 68, 68)) : new SolidColorBrush(Color.FromRgb(148, 163, 184));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class ChannelTypeToGeometryConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Models.ChannelType type && Application.Current != null)
        {
            string key = type switch
            {
                Models.ChannelType.Text => "GeoHash",
                Models.ChannelType.Voice => "GeoSpeaker",
                Models.ChannelType.Announcements => "GeoScanner",
                Models.ChannelType.VoiceHub => "GeoDashboard",
                Models.ChannelType.TemporaryVoice => "GeoMic",
                _ => "GeoHash"
            };
            return Application.Current.TryFindResource(key) ?? DependencyProperty.UnsetValue;
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class ChannelTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Models.ChannelType type && Application.Current != null)
        {
            string key = type switch
            {
                Models.ChannelType.Text => "IconGradCyan",
                Models.ChannelType.Voice => "IconGradEmerald",
                Models.ChannelType.Announcements => "IconGradAmber",
                Models.ChannelType.VoiceHub => "IconGradPurple",
                Models.ChannelType.TemporaryVoice => "IconGradRose",
                _ => "IconGradCyan"
            };
            return Application.Current.TryFindResource(key) ?? new SolidColorBrush(Colors.Cyan);
        }
        return new SolidColorBrush(Colors.Cyan);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
