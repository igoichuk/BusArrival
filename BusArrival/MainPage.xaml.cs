using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;
using System.Runtime.Serialization.Json;
using BusArrival.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BusArrival
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public Stream ToStream(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //http://google.com
            //http://countdown.api.tfl.gov.uk/interfaces/ura/instant_V1
            //http://countdown.api.tfl.gov.uk/interfaces/ura/instant_V1?StopCode1=52053&DirectionID=1&VisitNumber=1&ReturnList=StopCode1,StopPointName,LineName,DestinationText,EstimatedTime,MessageUUID,MessageText,MessagePriority,MessageType,ExpireTime

            //53347 - Queens Avenue towards Archway

            var serviceUrl = "http://countdown.api.tfl.gov.uk/interfaces/ura/instant_V1?StopCode1=52053&DirectionID=1&VisitNumber=1&ReturnList=StopCode1,StopPointName,LineName,DestinationText,EstimatedTime,MessageUUID,MessageText,MessagePriority,MessageType,ExpireTime";
            var client = new System.Net.Http.HttpClient();
  
            //var responseStream = await client.GetStreamAsync(serviceUrl);
            //var reader = new StringReader(responseText);
            //while(reader.I.ReadLine())

            var responseText = await client.GetStringAsync(serviceUrl);
            responseText = responseText.Replace("]", "],");
            responseText = string.Format("[{0}]", responseText);

            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseText));

            var serializer = new DataContractJsonSerializer(typeof(object[][]));
            var response = (object[][])serializer.ReadObject(ms);

            textResult.Text = response.Length.ToString();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dataProvider = new TflBusArrivalsProvider();

            //listBusStops.ItemsSource = dataProvider.GetBusStopsInCircleAsync(51.590705, -0.14287, 150);

            var stops = await dataProvider.GetBusStopsInCircleAsync(new GpsLocation(51.590705, -0.14287), 150);
            listBusStops.ItemsSource = stops;

        }

        private async void listBusStops_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var dataProvider = new TflBusArrivalsProvider();
            busArrival.ItemsSource = await dataProvider.GetPredictionsAsync((BusStop)listBusStops.SelectedItem);
        }
    }
}
