using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WorkTimeLog
{
    class WorkTimeLogEntryToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Contract.Requires(value is WorkState);
            Contract.Requires(parameter is WorkState);

            return (WorkState)value == (WorkState)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Contract.Requires(value is bool);
            Contract.Requires(parameter is WorkState);

            bool useValue = (bool)value;

            if (useValue)
            {
                return (WorkState)parameter;
            }
            else
            {
                return null;
            }
        }
    }
}
