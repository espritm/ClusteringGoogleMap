﻿using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering;
using System;

namespace ClusteringGoogleMap
{
    public class ClusterItem : Java.Lang.Object, IClusterItem
    {
        public LatLng Position { get; set; }
        public string Snippet { get; set; }
        public string Title { get; set; }
        public string m_sMoreCustomInformation { get; set; }
        public bool m_bIsFav { get; set; }


        public ClusterItem(double lat, double lon, string sMoreCustomInformation)
        {
            Position = new LatLng(lat, lon);
            Title = lat.ToString() + ", " + lon.ToString();

            m_sMoreCustomInformation = sMoreCustomInformation;
        }

        public ClusterItem(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {//To avoid Leaky abstraction.... http://stackoverflow.com/questions/10593022/monodroid-error-when-calling-constructor-of-custom-view-twodscrollview/10603714#10603714
        }
    }
}