using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Btn_Clicked(object sender, EventArgs e)
        {

            //Location Generation
            double lat = 0;
            double lon = 0;

            //Attempt to gain current location
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(15), null, false);

                if (position == null)
                {
                    return;
                }

                lat = position.Latitude;
                lon = position.Longitude;

                App.Origin = lat.ToString() + "," + lon.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }


            if (NavTo.Text != "")
            {
                App.Destination = NavTo.Text;

                DependencyService.Register<INav>();
                DependencyService.Get<INav>().StartNativeIntentOrActivity();
            }
            else
            {
                //Alert for a GPS point to navigate to...
                await DisplayAlert("No GPS Provided", "Please provide a GPS co-ordinate to navigate to", "OK");
            }

        }
    }
}