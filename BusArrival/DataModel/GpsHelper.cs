using System;

namespace BusArrival.DataModel
{
    public struct GpsLocation
    {
        public double Latitude;
        public double Longitude;

        public GpsLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    class GpsHelper
    {
        private const int EarthRadius = 6371; // 6371 km or 3960 mi

        // distance in km
        private static double Distance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double e = (Math.PI * latitude1 / 180);
            double f = (Math.PI * longitude1 / 180);
            double g = (Math.PI * latitude2 / 180);
            double h = (Math.PI * longitude2 / 180);
            double i = (Math.Cos(e) * Math.Cos(g) * Math.Cos(f) * Math.Cos(h) + Math.Cos(e) * Math.Sin(f) * Math.Cos(g) * Math.Sin(h) + Math.Sin(e) * Math.Sin(g));
            double j = (Math.Acos(i));
            double k = (EarthRadius * j);

            return k;
        }

        public static double Distance(GpsLocation location1, GpsLocation location2)
        {
            return Distance(location1.Latitude, location1.Longitude, location2.Latitude, location2.Longitude);
        }

        private static GpsLocation myLocation = new GpsLocation(51.58956282190771, -0.14318972826004028);
        public static GpsLocation GetMyLocation()
        {
            return myLocation;
        }

    }
}
