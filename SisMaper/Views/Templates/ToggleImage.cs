using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SisMaper.Tools;

namespace SisMaper.Views.Templates;

public class ToggleImage : ToggleButton
{
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.RegisterAttached("Image",
            typeof(ImageSource),
            typeof(ToggleImage),
            new FrameworkPropertyMetadata(default));

    private ImageSource _checkImage;
    private ImageSource _uncheckImage;

    public ToggleImage()
    {
        Checked += Check;
        Unchecked += Uncheck;
    }

    public ImageSource Image
    {
        get => (ImageSource) GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public ImageSource CheckImage
    {
        get => _checkImage;
        set
        {
            _checkImage = value;
            if (IsChecked.IsTrue())
            {
                Image = _checkImage;
            }
        }
    }

    public ImageSource UncheckImage
    {
        get => _uncheckImage;
        set
        {
            _uncheckImage = value;
            if (!IsChecked.IsTrue())
            {
                Image = _uncheckImage;
            }
        }
    }

    private void Uncheck(object sender, RoutedEventArgs e)
    {
        SetValue(ImageProperty, UncheckImage);
    }

    private void Check(object sender, RoutedEventArgs e)
    {
        SetValue(ImageProperty, CheckImage);
    }
}