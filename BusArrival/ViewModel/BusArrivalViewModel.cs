using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusArrival.DataModel;
using BusArrival.Common;

namespace BusArrival.ViewModel
{
    class BusArrivalViewModel: BindableBase
    {
        public BusArrivalViewModel()
        {
            Initialize();
        }

        public async void Initialize()
        {
            StopsNearby = (await DataProvider.GetBusStopsInCircleAsync(new GpsLocation(51.590705, -0.14287), 150))
                .OrderBy(b => GpsHelper.Distance(GpsHelper.GetMyLocation(), b.Location))
                .ToList();
            SelectedBusStop = StopsNearby.FirstOrDefault();
        }

        // subject of dependency injection
        public IBusArrivalsProvider DataProvider = new TflBusArrivalsProvider();

        private List<BusStop> _stopsNearby = null;
        public List<BusStop> StopsNearby 
        {
            get { return _stopsNearby; }
            set { SetProperty(ref _stopsNearby, value);  } 
        }

        public List<BusStop> Favorites { get; set; }

        private BusStop _selectedBusStop = null;
        public BusStop SelectedBusStop 
        {
            get { return _selectedBusStop; } 
            set 
            { 
                SetProperty(ref _selectedBusStop, value);
                UpdatePredictions(_selectedBusStop);
            } 
        }

        private async void UpdatePredictions(BusStop busStop)
        {
            if (busStop == null)
                return;

            if ((DateTime.Now - busStop.PredictionsUpdateTime).TotalSeconds > 30)
            {
                busStop.Predictions = (await DataProvider.GetPredictionsAsync(busStop)).ToList();
            }
        }
    }
}
