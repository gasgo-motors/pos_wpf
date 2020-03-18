using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace PosClient.Helpers
{
    class FocusBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.GotMouseCapture += (sender, e) =>
            {
                try
                {
                    TextBox t = AssociatedObject as TextBox;
                    App.Current.TargetTextBox = t;
                    t.SelectAll();
                    //MouseButtonEventArgs oe = new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent };
                    //t.RaiseEvent(oe);
                }
                catch (Exception ex) { }
            };
        }
    }
}
