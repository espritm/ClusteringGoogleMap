using System;
using Android.App;
using Android.OS;
using Android.Gms.Maps;
using Com.Google.Maps.Android.Clustering;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Support.V7.App;

namespace ClusteringGoogleMap
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private GoogleMap m_map;
        private MapView m_mapView;
        private ClusterManager m_ClusterManager;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SupportActionBar.Hide();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainActivity);
            m_mapView = FindViewById<MapView>(Resource.Id.mainactivity_mapView);

            m_mapView.OnCreate(bundle);
            m_mapView.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap map)
        {
            m_map = map;

            //Initialize cluster manager. Setting the CameraIdleListener is mandatory 
            m_ClusterManager = new ClusterManager(this, m_map);
            m_ClusterManager.Renderer = new ClusterRenderer(this, m_map, m_ClusterManager);
            m_map.SetOnCameraIdleListener(m_ClusterManager);

            SetupMap();
        }

        private void SetupMap()
        {
            //Show Grenoble on the map
            LatLng LatLonGrenoble = new LatLng(45.188529, 5.724523);
            m_map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(LatLonGrenoble, Resources.GetInteger(Resource.Integer.default_zoom_level_googlemaps)));

            List<ClusterItem> lsMarkers = new List<ClusterItem>();

            //Add 50 markers using a spiral algorithm (cheers SushiHangover)
            for (int i = 0; i < 50; ++i)
            {
                double theta = i * Math.PI * 0.33f;
                double radius = 0.005 * Math.Exp(0.1 * theta);
                double x = radius * Math.Cos(theta);
                double y = radius * Math.Sin(theta);
                ClusterItem newMarker = new ClusterItem(LatLonGrenoble.Latitude + x, LatLonGrenoble.Longitude + y);
                lsMarkers.Add(newMarker);
            }

            //Add markers to the map through the cluster manager
            m_ClusterManager.AddItems(lsMarkers);
        }

        protected override void OnPause()
        {
            base.OnPause();
            m_mapView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            m_mapView.OnResume();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            m_mapView.OnSaveInstanceState(outState);
        }
    }
}

