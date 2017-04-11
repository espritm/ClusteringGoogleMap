using Android.App;
using Android.OS;
using Android.Gms.Maps;
using Com.Google.Maps.Android.Clustering;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Support.V7.App;
using Android.Widget;
using System;
using Android.Views;

namespace ClusteringGoogleMap
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback, ClusterManager.IOnClusterItemInfoWindowClickListener, GoogleMap.IOnCameraIdleListener, GoogleMap.IInfoWindowAdapter, GoogleMap.IOnInfoWindowClickListener
    {
        private GoogleMap m_map;
        private MapView m_mapView;
        private ClusterManager m_ClusterManager;
        private ClusterRenderer m_ClusterRenderer;
        private ClusterManager m_ClusterManagerFav;
        private ClusterRenderer m_ClusterRendererFav;
        private CustomGoogleMapInfoWindow m_InfoWindowAdapter;
        public Dictionary<string, ClusterItem> m_dicAllMarkerOnMap = new Dictionary<string, ClusterItem>();

        
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
            //Initialize Map
            m_map = map;
            m_map.SetOnCameraIdleListener(this);
            m_map.SetInfoWindowAdapter(this);
            m_map.SetOnInfoWindowClickListener(this);

            //Initialize info window adapter
            m_InfoWindowAdapter = new CustomGoogleMapInfoWindow(this, m_dicAllMarkerOnMap);

            //Initialize Cluster manager
            ConfigCluster();

            //Initialize Cluster manager for favorites markers
            ConfigClusterFav();
            
            SetupMap();
        }

        private void ConfigCluster()
        {
            //Initialize cluster manager.
            m_ClusterManager = new ClusterManager(this, m_map);

            //Initialize cluster renderer, and keep a reference that will be usefull for the InfoWindowsAdapter
            m_ClusterRenderer = new ClusterRenderer(this, m_map, m_ClusterManager, m_dicAllMarkerOnMap);
            m_ClusterManager.Renderer = m_ClusterRenderer;

            //Custom info window : single markers only (a click on a cluster marker should not show info window)
            m_ClusterManager.MarkerCollection.SetOnInfoWindowAdapter(m_InfoWindowAdapter);

            //Handle Info Window's click event
            m_ClusterManager.SetOnClusterItemInfoWindowClickListener(this);
        }

        private void ConfigClusterFav()
        {
            //Initialize cluster manager for favorites markers.
            m_ClusterManagerFav = new ClusterManager(this, m_map);

            //Initialize cluster renderer, and keep a reference that will be usefull for the InfoWindowsAdapter
            m_ClusterRendererFav = new ClusterRenderer(this, m_map, m_ClusterManagerFav, m_dicAllMarkerOnMap);
            m_ClusterManagerFav.Renderer = m_ClusterRendererFav;

            //Custom info window : single markers only (a click on a cluster marker should not show info window)
            m_ClusterManagerFav.MarkerCollection.SetOnInfoWindowAdapter(m_InfoWindowAdapter);

            //Handle Info Window's click event
            m_ClusterManagerFav.SetOnClusterItemInfoWindowClickListener(this);
        }

        public void OnClusterItemInfoWindowClick(Java.Lang.Object p0)
        {
            //You can retrieve the ClusterItem clicked with a cast
            ClusterItem itemClicked = (ClusterItem)p0;

            Toast.MakeText(this, "Info Window clicked !", ToastLength.Short).Show();

            //Dismiss the info window clicked
            Marker markerClicked = m_ClusterRenderer.GetMarker(itemClicked);
            if (markerClicked == null)
                markerClicked = m_ClusterRendererFav.GetMarker(itemClicked);
            
            markerClicked.HideInfoWindow();
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
                double theta = i * System.Math.PI * 0.33f;
                double radius = 0.005 * System.Math.Exp(0.1 * theta);
                double x = radius * System.Math.Cos(theta);
                double y = radius * System.Math.Sin(theta);
                ClusterItem newMarker = new ClusterItem(LatLonGrenoble.Latitude + x, LatLonGrenoble.Longitude + y, "radius = " + radius);
                lsMarkers.Add(newMarker);
            }

            //Add markers to the map through the cluster manager
            m_ClusterManager.AddItems(lsMarkers);



            LatLng LatLonMeylan = new LatLng(45.2333, 5.7833);
            lsMarkers = new List<ClusterItem>();

            //Add 50 markers using a spiral algorithm (cheers SushiHangover)
            for (int i = 0; i < 50; ++i)
            {
                double theta = i * System.Math.PI * 0.29f;
                double radius = 0.004 * System.Math.Exp(0.2 * theta);
                double x = radius * System.Math.Cos(theta);
                double y = radius * System.Math.Sin(theta);
                ClusterItem newMarker = new ClusterItem(LatLonMeylan.Latitude + x, LatLonMeylan.Longitude + y, "radius = " + radius);
                newMarker.m_bIsFav = true;
                lsMarkers.Add(newMarker);
            }

            //Add markers to the map through the cluster manager
            m_ClusterManagerFav.AddItems(lsMarkers);
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

        public void OnCameraIdle()
        {
            m_ClusterManager.OnCameraIdle();
            m_ClusterManagerFav.OnCameraIdle();
        }

        public View GetInfoContents(Marker marker)
        {
            View v = m_ClusterManager.MarkerManager.GetInfoContents(marker);

            if (v == null)
                v = m_ClusterManagerFav.MarkerManager.GetInfoContents(marker);

            return v;
        }

        public View GetInfoWindow(Marker marker)
        {
            View v = m_ClusterManager.MarkerManager.GetInfoWindow(marker);

            if (v == null)
                v = m_ClusterManagerFav.MarkerManager.GetInfoWindow(marker);

            return v;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            m_ClusterManager.OnInfoWindowClick(marker);
            m_ClusterManagerFav.OnInfoWindowClick(marker);
        }
    }
}

