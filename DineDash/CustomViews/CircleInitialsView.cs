using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DineDash.CustomViews
{
    public class CircleInitialsView : ContentView
    {
        public CircleInitialsView()
        {
            Content = new Grid
            {
                WidthRequest = 50,
                HeightRequest = 50,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    new Label {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = Color.White,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold
                    }
                }
            };

            var gradient = new RadialGradientBrush
            {
                Radius = 1,
                Center = new Point(0.5, 0.5),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromHex("#FF7F50"), 0),
                    new GradientStop(Color.FromHex("#FF6347"), 1)
                }
            };

            ((Grid)Content).Background = gradient;
            SetBinding(InitialsProperty, new Binding(nameof(Initials), source: this));
        }

        public static readonly BindableProperty InitialsProperty =
            BindableProperty.Create(nameof(Initials), typeof(string), typeof(CircleInitialsView), string.Empty);

        public string Initials
        {
            get => (string)GetValue(InitialsProperty);
            set => SetValue(InitialsProperty, value);
        }
    }
}
