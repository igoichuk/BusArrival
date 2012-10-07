using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using BusArrival.DataModel;

namespace BusArrival.ValueConverters
{
    class DistanceToMyLocationValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var distanceKm = GpsHelper.Distance(GpsHelper.GetMyLocation(), (GpsLocation)value);

            if (string.Equals(parameter, "Meters"))
                return (int) (distanceKm * 1000);

            return distanceKm; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
