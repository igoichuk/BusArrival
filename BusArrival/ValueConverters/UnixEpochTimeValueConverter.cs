using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
 
namespace BusArrival.ValueConverters
{
    public class UnixEpochTimeValueConverter : IValueConverter
    {
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var estimated = UnixEpoch.AddMilliseconds((long)value);
            return (estimated - DateTime.UtcNow).Minutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
