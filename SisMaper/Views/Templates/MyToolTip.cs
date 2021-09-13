﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SisMaper.Views.Templates
{
    public class MyToolTip : ToolTip
    {
        public static readonly DependencyProperty AutoMoveProperty =
          DependencyProperty.RegisterAttached("AutoMove",
                                              typeof(bool),
                                              typeof(MyToolTip),
                                              new FrameworkPropertyMetadata(false, AutoMovePropertyChangedCallback));


        public static readonly DependencyProperty AutoMoveHorizontalOffsetProperty =
          DependencyProperty.RegisterAttached("AutoMoveHorizontalOffset",
                                              typeof(double),
                                              typeof(MyToolTip),
                                              new FrameworkPropertyMetadata(16d));
        

        public static readonly DependencyProperty AutoMoveVerticalOffsetProperty =
          DependencyProperty.RegisterAttached("AutoMoveVerticalOffset",
                                              typeof(double),
                                              typeof(MyToolTip),
                                              new FrameworkPropertyMetadata(16d));

        private static void AutoMovePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var toolTip = (MyToolTip)dependencyObject;
            if (eventArgs.OldValue != eventArgs.NewValue && eventArgs.NewValue != null)
            {
                var autoMove = (bool)eventArgs.NewValue;
                if (autoMove)
                {
                    toolTip.Opened += ToolTip_Opened;
                    toolTip.Closed += ToolTip_Closed;
                }
                else
                {
                    toolTip.Opened -= ToolTip_Opened;
                    toolTip.Closed -= ToolTip_Closed;
                }
            }
        }

        private static void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            var toolTip = (MyToolTip)sender;
            var target = toolTip.PlacementTarget as FrameworkElement;
            if (target != null)
            {
                // move the tooltip on openeing to the correct position
                toolTip.MoveToolTip(target);
                target.MouseMove += ToolTipTargetPreviewMouseMove;
                Console.WriteLine(">>tool tip opened");
            }
        }

        private static void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            var toolTip = (ToolTip)sender;
            var target = toolTip.PlacementTarget as FrameworkElement;
            if (target != null)
            {
                target.MouseMove -= ToolTipTargetPreviewMouseMove;
                Console.WriteLine
                    (">>tool tip closed");
            }
        }

        private static void ToolTipTargetPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var target = sender as FrameworkElement;
            var toolTip = (target != null ? target.ToolTip : null) as MyToolTip;
            toolTip.MoveToolTip(sender as IInputElement);
        }

        private void MoveToolTip(IInputElement target)
        {
            if (target == null)
            {
                return;
            }
            Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
            var hOffset = (double) GetValue(AutoMoveHorizontalOffsetProperty);
            var vOffset = (double) GetValue(AutoMoveHorizontalOffsetProperty);
            HorizontalOffset = Mouse.GetPosition(target).X + hOffset;
            VerticalOffset = Mouse.GetPosition(target).Y + vOffset;
            Console.WriteLine(">>ho {0:.2f} >> vo {1:.2f}", HorizontalOffset, VerticalOffset);
        }
        #region Image
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached("Image",
                typeof(ImageSource),
                typeof(MyToolTip),
                new FrameworkPropertyMetadata(default));

        #endregion
        
        #region CapsLock
        
        public static readonly DependencyProperty CapsLockProperty =
            DependencyProperty.RegisterAttached("CapsLock",
                typeof(bool),
                typeof(MyToolTip),
                new FrameworkPropertyMetadata(false, CapsLockPropertyChangedCallback));

        private static void CapsLockPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var toolTip = (MyToolTip)dependencyObject;
            if (eventArgs.OldValue != eventArgs.NewValue && eventArgs.NewValue != null)
            {
                var autoMove = (bool)eventArgs.NewValue;
                if (autoMove)
                {
                    toolTip.Opened += CapsLockVerify;
                }
                else
                {
                    toolTip.Opened -= CapsLockVerify;
                }
            }
        }
        private static void CapsLockVerify(object sender, RoutedEventArgs e)
        {
            var toolTip = (MyToolTip)sender;
            toolTip.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
        }
        
        #endregion
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text",
                typeof(string),
                typeof(MyToolTip),
                new FrameworkPropertyMetadata(""));

        public bool AutoMove
        {
            get { return (bool) GetValue(AutoMoveProperty); }
            set { SetValue(AutoMoveProperty, value); }
        }
        public ImageSource Image 
        {
            get { return (ImageSource) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public bool CapsLock 
        {
            get { return (bool) GetValue(CapsLockProperty); }
            set { SetValue(CapsLockProperty, value); }
        }
        public string Text 
        {
            get { return (string) GetValue(TextProperty); }
            set
            {
                Console.WriteLine(value);
                SetValue(TextProperty, value);
            }
        }
        
    }


}
