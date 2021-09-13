using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SisMaper.Tools;

namespace SisMaper.Views.Templates
{
    public class ToggleImage : ToggleButton
    {
        public ToggleImage()
        {
            Checked += Check;
            Unchecked += Uncheck;
        }
        
        private void Uncheck(object sender, RoutedEventArgs e) => SetValue(ImageProperty, UncheckImage);
        private void Check(object sender, RoutedEventArgs e) => SetValue(ImageProperty, CheckImage);
        
        
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached("Image",
                typeof(ImageSource),
                typeof(ToggleImage),
                new FrameworkPropertyMetadata(default));

        public ImageSource Image 
        {
            get { return (ImageSource) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        private ImageSource _checkImage;
        
        public ImageSource CheckImage
        {
            get => _checkImage;
            set
            {
                _checkImage = value;
                if (IsChecked.IsTrue())
                    Image = _checkImage;
            }
        }
        private ImageSource _uncheckImage;
        
        public ImageSource UncheckImage
        {
            get => _uncheckImage;
            set
            {
                _uncheckImage = value;
                if (!IsChecked.IsTrue())
                    Image = _uncheckImage;
            }
        }
    }
}