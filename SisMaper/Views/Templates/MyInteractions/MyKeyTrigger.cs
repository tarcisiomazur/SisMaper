using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Input;

namespace SisMaper.Views.Templates.MyInteractions;

public class MyMultiKeyTrigger : EventTriggerBase<UIElement>, IAddChild
{
    public static readonly DependencyProperty ActiveOnFocusProperty =
        DependencyProperty.Register("ActiveOnFocus", typeof(bool), typeof(MyMultiKeyTrigger));

    private UIElement targetElement;

    public MyMultiKeyTrigger()
    {
        Keys = new Collection<KeyTrigger>();
    }

    public Collection<KeyTrigger> Keys { get; }

    /// <summary>
    ///     The key that must be pressed for the trigger to fire.
    /// </summary>
    /// <summary>
    ///     The modifiers that must be active for the trigger to fire (the default is no modifiers pressed).
    /// </summary>
    /// <summary>
    ///     If true, the Trigger only listens to its trigger Source object, which means that element must have focus for the
    ///     trigger to fire.
    ///     If false, the Trigger listens at the root, so any unhandled KeyDown/Up messages will be caught.
    /// </summary>
    public bool ActiveOnFocus
    {
        get => (bool) GetValue(ActiveOnFocusProperty);
        set => SetValue(ActiveOnFocusProperty, value);
    }

    public void AddChild(object value)
    {
        KeyTrigger key = value as KeyTrigger;
        if (key != null)
        {
            Keys.Add(key);
        }
    }

    public void AddText(string text) { }

    /// <summary>
    ///     Determines whether or not to listen to the KeyDown or KeyUp event.
    /// </summary>
    protected override string GetEventName()
    {
        return "Loaded";
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (Keys.Any(key =>
                key.FiredOn == KeyTriggerFiredOn.KeyDown &&
                Keyboard.Modifiers == GetActualModifiers(key.Key, key.Modifiers) && key.Key == e.Key))
        {
            InvokeActions(e);
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (Keys.Any(key =>
                key.FiredOn == KeyTriggerFiredOn.KeyUp &&
                Keyboard.Modifiers == GetActualModifiers(key.Key, key.Modifiers) && key.Key == e.Key))
        {
            InvokeActions(e);
        }
    }

    private static ModifierKeys GetActualModifiers(Key key, ModifierKeys modifiers)
    {
        if (key == Key.LeftCtrl || key == Key.RightCtrl)
        {
            modifiers |= ModifierKeys.Control;
        }
        else if (key == Key.LeftAlt || key == Key.RightAlt || key == Key.System)
        {
            modifiers |= ModifierKeys.Alt;
        }
        else if (key == Key.LeftShift || key == Key.RightShift)
        {
            modifiers |= ModifierKeys.Shift;
        }

        return modifiers;
    }

    protected override void OnEvent(EventArgs eventArgs)
    {
        // Listen to keyboard events.
        if (ActiveOnFocus)
        {
            targetElement = Source;
        }
        else
        {
            targetElement = GetRoot(Source);
        }

        targetElement.KeyDown += OnKeyDown;
        targetElement.KeyUp += OnKeyUp;
    }

    protected override void OnDetaching()
    {
        if (targetElement != null)
        {
            targetElement.KeyDown -= OnKeyDown;
            targetElement.KeyUp -= OnKeyUp;
        }

        base.OnDetaching();
    }

    private static UIElement GetRoot(DependencyObject current)
    {
        UIElement last = null;

        while (current != null)
        {
            last = current as UIElement;
            current = VisualTreeHelper.GetParent(current);
        }

        return last;
    }
}