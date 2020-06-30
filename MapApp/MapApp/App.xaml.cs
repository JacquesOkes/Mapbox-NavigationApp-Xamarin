using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapApp
{
    public partial class App : Application
    {
        //GLOBAL point for destination setting
        public static string Destination = "0,0";
        public static string Origin = "0,0";
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
