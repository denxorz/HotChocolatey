using System;
using System.Windows;
using SharpVectors.Converters;

namespace HotChocolatey.View.Main
{
    public class SvgViewboxAttachedProperties : DependencyObject
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
                "Source",
                typeof(Uri),
                typeof(SvgViewboxAttachedProperties),
                new PropertyMetadata(null, OnSourceChanged));

        public static Uri GetSource(DependencyObject obj)
        {
            return (Uri)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, Uri value)
        {
            obj.SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SvgViewbox svgControl)
            {
                svgControl.Source = (Uri)e.NewValue;
            }
        }
    }
}