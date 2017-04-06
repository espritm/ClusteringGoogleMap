
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;

namespace ClusteringGoogleMap
{
    public class CustomGoogleMapInfoWindow : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private Activity m_context;
        private View m_View;
        private Marker m_currentMarker;
        private ClusterRenderer m_ClusterRenderer;

        public CustomGoogleMapInfoWindow(Activity context, ClusterRenderer clusterRenderer)
        {
            m_context = context;
            m_View = m_context.LayoutInflater.Inflate(Resource.Layout.CustomGoogleMapInfoWindow, null);
            m_ClusterRenderer = clusterRenderer;
        }

        public View GetInfoWindow(Marker marker)
        {
            //Use the default info window
            return null;
        }

        public View GetInfoContents(Marker marker)
        {
            if (marker == null)
                return null;

            m_currentMarker = marker;

            //Retrieve the ClusterItem associated to the marker
            ClusterItem clusterItem = null;
            m_ClusterRenderer.m_dicMarkerToClusterItem.TryGetValue(m_currentMarker.Id, out clusterItem);

            ImageView imageview = m_View.FindViewById<ImageView>(Resource.Id.CustomGoogleMapInfoWindow_imageview);
            TextView textviewTitle = m_View.FindViewById<TextView>(Resource.Id.CustomGoogleMapInfoWindow_textview_title);
            TextView textviewDescription = m_View.FindViewById<TextView>(Resource.Id.CustomGoogleMapInfoWindow_textview_description);
            RatingBar ratingBar = m_View.FindViewById<RatingBar>(Resource.Id.CustomGoogleMapInfoWindow_ratingbar);

            if (marker.Title != null && marker.Title.Contains("Grenoble"))
                imageview.SetImageResource(Resource.Drawable.logo_grenoble);

            textviewTitle.Text = marker.Title;

            textviewDescription.Text = marker.Snippet;

            if (clusterItem != null)
                textviewDescription.Text += "\nMore info : " + clusterItem.m_sMoreCustomInformation;

            ratingBar.Rating = 5;

            return m_View;
        }
    }
}