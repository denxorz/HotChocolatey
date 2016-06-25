using System;
using System.Windows;
using SharpVectors.Converters;

namespace HotChocolatey.View.Main
{
    public class SvgViewboxAttachedProperties : DependencyObject
    {
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
            var svgControl = obj as SvgViewbox;
            if (svgControl != null)
            {
                svgControl.Source = (Uri)e.NewValue;
            }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source",
                typeof(Uri), typeof(SvgViewboxAttachedProperties),
                // default value: null
                new PropertyMetadata(null, OnSourceChanged));
    }
}