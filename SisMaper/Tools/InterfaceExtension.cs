using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace SisMaper.Tools
{
    public static class InterfaceExtension
    {
        private static void SetSelection(this PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(passwordBox, new object[] { start, length });
        }
        public static void FocusEnd(this TextBox tb)
        {
            tb.Focus();
            tb.SelectionStart = tb.Text.Length;
        }
        public static void FocusTo(this PasswordBox pb, TextBox tb)
        {
            pb.Focus();
            pb.SetSelection(tb.SelectionStart, tb.SelectionLength);
        }
    }
}