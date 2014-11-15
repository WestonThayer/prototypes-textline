using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TextLine
{
    /// <summary>
    /// XAML attached properties for TextBlocks to help position text in a natural way.
    /// </summary>
    public class TextLine : DependencyObject
    {
        #region Mode DP

        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(
            "Mode",
            typeof(TextLineModes),
            typeof(TextLine),
            new PropertyMetadata(TextLineModes.Baseline, ModePropertyChanged)
            );

        public static TextLineModes GetMode(DependencyObject d)
        {
            return (TextLineModes)d.GetValue(ModeProperty);
        }

        public static void SetMode(DependencyObject d, TextLineModes value)
        {
            d.SetValue(ModeProperty, value);
        }

        private static void ModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateMargin(d as TextBlock, (TextLineModes)e.NewValue, GetMargin(d));
        }

        #endregion

        #region Margin DP

        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
            "Margin",
            typeof(Thickness),
            typeof(TextLine),
            new PropertyMetadata(new Thickness(), MarginPropertyChanged)
            );

        public static Thickness GetMargin(DependencyObject d)
        {
            return (Thickness)d.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject d, Thickness value)
        {
            d.SetValue(MarginProperty, value);
        }

        private static void MarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateMargin(d as TextBlock, GetMode(d), (Thickness)e.NewValue);
        }

        #endregion

        public static void UpdateMargin(TextBlock textBlock)
        {
            UpdateMargin(textBlock, GetMode(textBlock), GetMargin(textBlock));
        }

        private static void UpdateMargin(TextBlock textBlock, TextLineModes mode, Thickness margin)
        {
            if (textBlock.ActualHeight == 0)
            {
                textBlock.TextLineBounds = TextLineBounds.Tight;

                // Unfortunately, we won't know the TextBlock's height until after load
                textBlock.Loaded += textBlock_Loaded;
                return;
            }

            Thickness currentMargin = textBlock.Margin;

            if (mode == TextLineModes.Baseline)
            {
                currentMargin.Bottom = margin.Bottom;
                currentMargin.Top = margin.Top - textBlock.ActualHeight;
            }
            else if (mode == TextLineModes.CapHeight)
            {
                currentMargin.Top = margin.Top;
                currentMargin.Bottom = margin.Bottom - textBlock.ActualHeight;
            }

            currentMargin.Left = margin.Left;
            currentMargin.Right = margin.Right;

            textBlock.Margin = currentMargin;
        }

        static void textBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Loaded -= textBlock_Loaded;

            UpdateMargin(textBlock, GetMode(textBlock), GetMargin(textBlock));
        }
    }
}
