using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Input;

namespace SisMaper.Views.Templates.MyInteractions;

public class KeyTrigger : DependencyObject
{
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger));

    public static readonly DependencyProperty ModifiersProperty =
        DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTrigger));

    public static readonly DependencyProperty FiredOnProperty =
        DependencyProperty.Register("FiredOn", typeof(KeyTriggerFiredOn), typeof(MyMultiKeyTrigger));

    public Key Key
    {
        get => (Key) GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public ModifierKeys Modifiers
    {
        get => (ModifierKeys) GetValue(ModifiersProperty);
        set => SetValue(ModifiersProperty, value);
    }

    public KeyTriggerFiredOn FiredOn
    {
        get => (KeyTriggerFiredOn) GetValue(FiredOnProperty);
        set => SetValue(FiredOnProperty, value);
    }
}