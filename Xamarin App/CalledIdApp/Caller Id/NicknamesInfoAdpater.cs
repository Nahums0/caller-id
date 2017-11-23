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
using Called_Id;
using Android_Tabs;

namespace Called_Id
{
    public class NicknameInfo
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public int Id { get; set; }
    }
    public class NicknamesInfoAdpater:BaseAdapter
    {
        private List<NicknameInfo> list;
        private Activity activity;

        public NicknamesInfoAdpater(Activity a, List<NicknameInfo> l)
        {
            activity = a;
            list = l;
        }

        public override int Count => list.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return list[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            var current = list[position];
            if (view == null)
            {
                view = activity.LayoutInflater.Inflate(Resource.Layout.ListviewRow_Nicknames_info, parent, false);
            }
            var tvName = view.FindViewById<TextView>(Resource.Id.tvNicknameInfoName);
            var tvNumber = view.FindViewById<TextView>(Resource.Id.tvNicknameInfoNumber);
            tvName.Text = current.Name;
            tvNumber.Text = current.Number;

            return view;
        }


    }
}