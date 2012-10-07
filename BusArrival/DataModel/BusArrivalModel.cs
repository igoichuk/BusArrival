using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusArrival.DataModel
{
    public class BusStop
    {
        public string Id;
        public string Code { get; set; }
        public string Code2 { get; set; }
        public string Name { get; set; }
        public string Towards { get; set; }
        public string Indicator { get; set; }
        public int State { get; set; }
        public string LineInfo { get; set; }
        public GpsLocation Location { get; set; }

        public List<Prediction> Predictions { get; set; }
        public DateTime PredictionsUpdateTime = DateTime.MinValue;
    }

    public class Prediction
    {
        public string BusStopCode;
        public string LineName { get; set; }
        public string DestinationText { get; set; }
        public long EstimatedTime { get; set; } // UnixEpoch time
    }
}
