using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Mapbox.Mapboxsdk;

namespace MapApp.Droid
{
    [Activity(Label = "MapApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //// Set access token
            //string mapboxAccessToken = GetString(Resource.String.mapbox_access_token);
            //if (string.IsNullOrWhiteSpace(mapboxAccessToken))
            //    System.Diagnostics.Debug.WriteLine("mapbox token is not set");

            //Mapbox.GetInstance(ApplicationContext, "pk.eyJ1IjoiamFjcXVlc29rZXMiLCJhIjoiY2s5ZTZrbjF3MDIyODNlcGk2bjh1MnZ6MyJ9.qPz2yzlpOZQZvk7unMRvjQ");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}