using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using System.Runtime.Serialization.Json;

namespace BusArrival.DataModel
{
    public interface IBusArrivalsProvider
    {
        Task<IEnumerable<BusStop>> GetBusStopsInCircleAsync(GpsLocation location, double radius);
        Task<IEnumerable<Prediction>> GetPredictionsAsync(BusStop busStop);
    }

    public class TflBusArrivalsProvider : IBusArrivalsProvider
    {
        private const string serviceUrl = "http://countdown.api.tfl.gov.uk/interfaces/ura/instant_V1";
        private static BusRouteComparer busRouteComparer = new BusRouteComparer();

        private object[][] Deserialize(string responseText)
        {
            responseText = responseText.Replace("]", "],");
            responseText = string.Format("[{0}]", responseText);

            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseText));

            var serializer = new DataContractJsonSerializer(typeof(object[][]));
            var response = (object[][])serializer.ReadObject(ms);

            return response;
        }

        public async Task<IEnumerable<BusStop>> GetBusStopsInCircleAsync(GpsLocation location, double radius)
        {
            var requestUrl = new StringBuilder(serviceUrl);
            requestUrl.AppendFormat("?StopAlso=True&Circle={0},{1},{2}&ReturnList=StopPointName,StopID,StopCode1,StopCode2,StopPointIndicator,StopPointState,Towards,Latitude,Longitude,LineName", location.Latitude, location.Longitude, radius);

            var client = new System.Net.Http.HttpClient();
            var response = Deserialize(await client.GetStringAsync(requestUrl.ToString()));

            var busStopLineInfo = new Dictionary<string, HashSet<string>>();

            var busStops = new List<BusStop>();
            foreach (var item in response)
            {
                var itemType = Convert.ToInt32(item[0]);

                // read bus stop info
                if (itemType == 0)
                {
                    var busStop = new BusStop
                    {
                        Name = item[1] as string,
                        Id = item[2] as string,
                        Code = item[3] as string,
                        Code2 = item[4] as string,
                        Towards = item[5] as string,
                        Indicator = item[6] as string,
                        State = Convert.ToInt32(item[7]),
                        Location = new GpsLocation(Convert.ToDouble(item[8]), Convert.ToDouble(item[9]))
                    };

                    // filter out ghost stops (without any codes or buses)
                    if (busStop.Code != null || busStop.Code2 != null)
                        busStops.Add(busStop);
                }

                // read line info
                if (itemType == 1)
                {
                    var lineInfo = item[10] as string;
                    if (!string.IsNullOrEmpty(lineInfo))
                    {
                        var stopId = item[2] as string;
                        HashSet<string> linesInfo;
                        if (!busStopLineInfo.TryGetValue(stopId, out linesInfo))
                        {
                            linesInfo = new HashSet<string>();
                            busStopLineInfo.Add(stopId, linesInfo);
                        }
                        linesInfo.Add(lineInfo);
                    }
                }
            }

            // add line info to bus stops
            foreach (var busStop in busStops)
            {
                if (busStopLineInfo.ContainsKey(busStop.Id))
                {
                    var lineInfo = string.Concat(busStopLineInfo[busStop.Id].OrderBy(i => i, busRouteComparer).Select(i => i + ","));
                    busStop.LineInfo = lineInfo.Remove(lineInfo.Length - 1, 1);
                }
            }
            return busStops; 
        }

        public async Task<IEnumerable<Prediction>> GetPredictionsAsync(BusStop busStop)
        {
            var requestUrl = new StringBuilder(serviceUrl);
            requestUrl.AppendFormat("?StopCode1={0}&DirectionID=1&VisitNumber=1&ReturnList=StopCode1,StopPointName,LineName,DestinationText,EstimatedTime,ExpireTime", busStop.Code);

            var client = new System.Net.Http.HttpClient();
            var response = Deserialize(await client.GetStringAsync(requestUrl.ToString()));

            var predictions = new List<Prediction>();
            foreach (var item in response)
            {
                var itemType = Convert.ToInt32(item[0]);

                // read prediction
                if (itemType == 1)
                {
                    var prediction = new Prediction
                    {
                        BusStopCode = busStop.Code,
                        LineName = item[3] as string,
                        DestinationText = item[4] as string,
                        EstimatedTime = Convert.ToInt64(item[5])
                    };
                    predictions.Add(prediction);
                }
            }

            return predictions;
        }
    }

    class BusRouteComparer : IComparer<string>
    {
        // if numbers compare as number otherwise compare as strings
        public int Compare(string s1, string s2)
        {
            int x1, x2;
            if (int.TryParse(s1, out x1))
            {
                return int.TryParse(s2, out x2) ? x1.CompareTo(x2) : -1;
            }
            else
            {
                return int.TryParse(s2, out x2) ? 1 : s1.CompareTo(s2);
            }
        }
    }
}
