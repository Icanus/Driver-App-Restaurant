using DineDash.Enums;
using DineDash.MaterialDesign;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace DineDash.Converters
{
    public class OrderStatusIconConverter : IValueConverter
    {
        /// <param name="value">OrderStatus</param>
        /// <param name="targetType">Unused</param>
        /// <param name="parameter">Unused</param>
        /// <param name="culture">Unused</param>
        /// <returns>Unicode string of a Material icon.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OrderStatus s = (OrderStatus)value;

            switch (s)
            {
                case OrderStatus.Placed: return Icons.Receipt;
                case OrderStatus.Delivered: return Icons.CheckCircle;
                case OrderStatus.Processing: return Icons.IncompleteCircle;
                case OrderStatus.OnTheWay: return Icons.Moped;
                default: return Icons.Cancel;
            }
        }

        /// <summary>
        /// It is unnecessary to implement.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
