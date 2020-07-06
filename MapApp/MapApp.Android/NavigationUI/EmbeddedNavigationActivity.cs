using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Mapbox.Services.Android.Navigation.UI.V5;
using Com.Mapbox.Services.Android.Navigation.UI.V5.Listeners;
using Com.Mapbox.Services.Android.Navigation.V5.Routeprogress;
using Com.Mapbox.Geojson;
using Android.Support.Design.Widget;
using Android.Locations;
using Com.Mapbox.Services.Android.Navigation.V5.Navigation;
using Com.Mapbox.Mapboxsdk;
using System.Threading.Tasks;
using Com.Mapbox.Api.Directions.V5.Models;
using Android.Content.Res;
using Android.Preferences;
using Android.Text;
using Android.Text.Style;

namespace MapApp.Droid.NavigationUI
{
    [Activity(Label = "EmbeddedNavigationActivity")]
    public class EmbeddedNavigationActivity : AppCompatActivity,
      IOnNavigationReadyCallback, INavigationListener, IProgressChangeListener, IInstructionListListener
    {
        Point ORIGIN = Point.FromLngLat(-77.03194990754128, 38.909664963450105);
        Point DESTINATION = Point.FromLngLat(-77.0270025730133, 38.91057077063121);

        Com.Mapbox.Services.Android.Navigation.UI.V5.NavigationView navigationView;
        View spacer;
        TextView speedWidget;
        public FloatingActionButton fabNightModeToggle;

        public bool bottomSheetVisible = true;
        bool instructionListShown = false;

        #region IOnNavigationReadyCallback
      
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Initialize mapbox access token
            Mapbox.GetInstance(ApplicationContext, GetString(Resource.String.mapbox_access_token));

            SetTheme(Resource.Style.Theme_AppCompat_Light_NoActionBar);
            InitNightMode();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_embedded_navigation);
            navigationView = FindViewById<Com.Mapbox.Services.Android.Navigation.UI.V5.NavigationView>(Resource.Id.navigationView);
            fabNightModeToggle = FindViewById<FloatingActionButton>(Resource.Id.fabToggleNightMode);
            speedWidget = FindViewById<TextView>(Resource.Id.speed_limit);
            spacer = FindViewById<View>(Resource.Id.spacer);
            SetSpeedWidgetAnchor(Resource.Id.summaryBottomSheet);

            navigationView.OnCreate(savedInstanceState);
            navigationView.Initialize(this);
        }

        protected override void OnStart()
        {
            base.OnStart();
            navigationView.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
            navigationView.OnResume();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            navigationView.OnLowMemory();
        }

        public override void OnBackPressed()
        {
            // If the navigation view didn't need to do anything, call super
            if (!navigationView.OnBackPressed())
            {
                base.OnBackPressed();
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            navigationView.OnSaveInstanceState(outState);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            navigationView.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnPause()
        {
            base.OnPause();
            navigationView.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
            navigationView.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            navigationView.OnDestroy();
            if (IsFinishing)
                SaveNightModeToPreferences(AppCompatDelegate.ModeNightAuto);
        }

        #region INavigationListener
        public void OnCancelNavigation()
        {
            // Navigation canceled, finish the activity
            Finish();
        }

        public void OnNavigationFinished()
        {
            // Intentionally empty
        }

        public void OnNavigationRunning()
        {
            // Intentionally empty
        }
        #endregion

        #region IProgressChangeListener
        public void OnProgressChange(Location location, RouteProgress routeProgress)
        {
            SetSpeed(location);
        }
        #endregion

        #region IInstructionListListener
        public void OnInstructionListVisibilityChanged(bool shown)
        {
            instructionListShown = shown;
            speedWidget.Visibility = shown ? ViewStates.Gone : ViewStates.Visible;
            if (instructionListShown)
            {
                fabNightModeToggle.Hide();
            }
            else
            {
                fabNightModeToggle.Show();
            }
        }
        #endregion

        async Task FetchRoute()
        {
            var builder = NavigationRoute.GetBuilder(this)
                                         .AccessToken("pk.eyJ1IjoiamFjcXVlc29rZXMiLCJhIjoiY2s5ZTZrbjF3MDIyODNlcGk2bjh1MnZ6MyJ9.qPz2yzlpOZQZvk7unMRvjQ")
                                         .Origin(ORIGIN)
                                         .Destination(DESTINATION)
                                         .Alternatives((Java.Lang.Boolean)true)
                                         .Build();
            var response = await builder.GetRouteAsync();
            if (response != null && response.Routes().Any())
            {
                StartNavigation(response.Routes().FirstOrDefault());
            }
        }

        void StartNavigation(DirectionsRoute directionsRoute)
        {
            var options = NavigationViewOptions.InvokeBuilder()
                                               .NavigationListener(this)
                                               .DirectionsRoute(directionsRoute)
                                               .ShouldSimulateRoute(true)                                               
                                               //.ProgressChangeListener(this)
                                               .InstructionListListener(this);
            SetBottomSheetCallback(options);
            SetupNightModeFab();

            navigationView.StartNavigation(options.Build());
        }

        void SetupNightModeFab()
        {
            fabNightModeToggle.Click += (sender, e) =>
            {
                ToggleNightMode();
            };
        }

        void ToggleNightMode()
        {
            int currentNightMode = GetCurrentNightMode();
            AlternativeNightMode(currentNightMode);
        }

        void InitNightMode()
        {
            int nightMode = RetrieveNightModeFromPreferences();
            AppCompatDelegate.DefaultNightMode = nightMode;
        }

        int GetCurrentNightMode()
        {
            return (int)Resources.Configuration.UiMode & (int)Android.Content.Res.UiMode.NightMask;
        }

        void AlternativeNightMode(int currentNightMode)
        {
            int newNightMode;
            if (currentNightMode == (int)Android.Content.Res.UiMode.NightMask)
            {
                newNightMode = AppCompatDelegate.ModeNightNo;
            }
            else
            {
                newNightMode = AppCompatDelegate.ModeNightYes;
            }
            SaveNightModeToPreferences(newNightMode);
            Recreate();
        }

        int RetrieveNightModeFromPreferences()
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            return preferences.GetInt("current_night_mode", AppCompatDelegate.ModeNightAuto);
        }

        void SaveNightModeToPreferences(int nightMode)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutInt("current_night_mode", nightMode);
            editor.Apply();
        }

        void SetSpeed(Location location)
        {
            string sb = new StringBuilder()
                .AppendLine(((int)(location.Speed * 2.2369)).ToString())
                .Append("MPH")
                .ToString();
            int mphTextSize = Resources.GetDimensionPixelSize(2000);   //Resource.Dimension.mph_text_size
            int speedTextSize = Resources.GetDimensionPixelSize(2000); //Resource.Dimension.speed_text_size

          SpannableString spannableString = new SpannableString(sb);
            spannableString.SetSpan(new AbsoluteSizeSpan(mphTextSize),
                                    sb.Length - 4, sb.Length, SpanTypes.InclusiveInclusive);
            spannableString.SetSpan(new AbsoluteSizeSpan(speedTextSize),
                                    0, sb.Length - 3, SpanTypes.InclusiveInclusive);

            speedWidget.Text = spannableString.ToString();
            if (!instructionListShown)
            {
                speedWidget.Visibility = ViewStates.Visible;
            }

        }

        /*
         * Sets the anchor of the spacer for the speed widget, thus setting the anchor for the speed widget
         * (The speed widget is anchored to the spacer, which is there because padding between items and
         * their anchors in CoordinatorLayouts is finicky.
         *
         * @param res resource for view of which to anchor the spacer
         */
        public void SetSpeedWidgetAnchor(int res)
        {
            var layoutParams = spacer.LayoutParameters as CoordinatorLayout.LayoutParams;
            layoutParams.AnchorId = res;
            spacer.LayoutParameters = layoutParams;
        }

        void SetBottomSheetCallback(NavigationViewOptions.Builder options)
        {
            options.BottomSheetCallback(new MyBottomSheetCallback(this));
        }

        public async void OnNavigationReady(bool p0)
        {           
              await  FetchRoute();          
        }
    }

    public class MyBottomSheetCallback : BottomSheetBehavior.BottomSheetCallback
    {
        EmbeddedNavigationActivity parent;

        public MyBottomSheetCallback(EmbeddedNavigationActivity parent)
        {
            this.parent = parent;
        }

        public override void OnSlide(View bottomSheet, float slideOffset)
        {
        }

        public override void OnStateChanged(View bottomSheet, int newState)
        {
            switch (newState)
            {
                case BottomSheetBehavior.StateHidden:
                    parent.bottomSheetVisible = false;
                    parent.fabNightModeToggle.Hide();
                    parent.SetSpeedWidgetAnchor(Resource.Id.recenterBtn);
                    break;
                case BottomSheetBehavior.StateExpanded:
                    parent.bottomSheetVisible = true;
                    break;
                case BottomSheetBehavior.StateSettling:
                    if (!parent.bottomSheetVisible)
                    {
                        // View needs to be anchored to the bottom sheet before it is finished expanding
                        // because of the animation
                        parent.fabNightModeToggle.Show();
                        parent.SetSpeedWidgetAnchor(Resource.Id.summaryBottomSheet);
                    }
                    break;
                default:
                    return;
            }
        }
    }
}