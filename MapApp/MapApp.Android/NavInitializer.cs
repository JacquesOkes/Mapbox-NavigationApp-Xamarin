using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Mapbox.Mapboxsdk;
using MapApp.Droid;
using MapApp.Droid.NavigationUI;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(NavInitializer))]
namespace MapApp.Droid
{
    [Activity(Label = "NavInitializer")]
    class NavInitializer : INav
    {
        public void StartNativeIntentOrActivity()
        {
            //Begin Navigation Form
            var intent = new Intent(Android.App.Application.Context, typeof(EmbeddedNavigationActivity));
            Android.App.Application.Context.StartActivity(intent);
        }
    }
}