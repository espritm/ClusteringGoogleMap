using Android.App;
using Android.Views;
using Android.Widget;
using Com.Google.Maps.Android.Clustering.View;
using Android.Gms.Maps;
using Com.Google.Maps.Android.Clustering;
using Com.Google.Maps.Android.UI;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Java.Lang;
using System.Collections.Generic;

namespace ClusteringGoogleMap
{
    public class ClusterRenderer : DefaultClusterRenderer
    {
        private IconGenerator m_iconGeneratorForMarkerGroup;
        private ImageView m_imageviewForMarkerGroup;
        public Dictionary<string, ClusterItem> m_dicAllMarkerOnMap;

        public ClusterRenderer(Activity context, GoogleMap map, ClusterManager clusterManager, Dictionary<string, ClusterItem> dicAllMarkerOnMap) 
            : base(context, map, clusterManager)
        {
            m_dicAllMarkerOnMap = dicAllMarkerOnMap;
            InitViewForMarkerGroup(context);
        }

        private void InitViewForMarkerGroup(Activity context)
        {
            //Retrieve views from AXML to display groups of markers (clustering)
            View viewMarkerClusterGrouped = context.LayoutInflater.Inflate(Resource.Layout.marker_cluster_grouped, null);
            m_imageviewForMarkerGroup = viewMarkerClusterGrouped.FindViewById<ImageView>(Resource.Id.marker_cluster_grouped_imageview);

            //Configure the groups of markers icon generator with the view. The icon generator will be used to display the marker's picture with a text
            m_iconGeneratorForMarkerGroup = new IconGenerator(context);
            m_iconGeneratorForMarkerGroup.SetContentView(viewMarkerClusterGrouped);
            m_iconGeneratorForMarkerGroup.SetBackground(null);
        }
        
        //Draw a single marker
        protected override void OnBeforeClusterItemRendered(Java.Lang.Object p0, MarkerOptions markerOptions)
        {
            ClusterItem clusterItem = (ClusterItem)p0;

            //Icon for single marker
            if (clusterItem.m_bIsFav)
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.marker_fav));
            else
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.marker));

            //Text for Info Window
            markerOptions.SetTitle("Grenoble");
            markerOptions.SetSnippet(markerOptions.Position.Latitude.ToString() + ", " + markerOptions.Position.Longitude.ToString());
        }

        //Draw a grouped marker
        protected override void OnBeforeClusterRendered(ICluster p0, MarkerOptions markerOptions)
        {
            bool bIsFav = false;
            foreach (ClusterItem itm in p0.Items)
            {
                bIsFav = itm.m_bIsFav;
                break;
            }

            //Retrieve the number we have to display inside the group marker
            string sNumberOfMarkersGrouped = p0.Size.ToString();

            //Icon of a group marker :
            //  First, set the imageview's source with the right picture
            //  Then, use the icon generator to set the icon of the marker with the text containing the number of markers grouped
            if (bIsFav)
                m_imageviewForMarkerGroup.SetImageResource(Resource.Drawable.marker_cluster_grouped_fav);
            else
                m_imageviewForMarkerGroup.SetImageResource(Resource.Drawable.marker_cluster_grouped);
            Bitmap icon = m_iconGeneratorForMarkerGroup.MakeIcon(sNumberOfMarkersGrouped);
            markerOptions.SetIcon(BitmapDescriptorFactory.FromBitmap(icon));
        }

        //After a cluster item have been rendered to a marker
        protected override void OnClusterItemRendered(Object item, Marker marker)
        {
            base.OnClusterItemRendered(item, marker);

            ClusterItem clusterItem = (ClusterItem)item;

            if (!m_dicAllMarkerOnMap.ContainsKey(marker.Id))
                m_dicAllMarkerOnMap.Add(marker.Id, clusterItem);
        }
    }
}