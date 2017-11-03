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
using Java.Lang;
using Android_Tabs;

namespace Called_Id
{
    public class Nickname
    {
        public int Id   {get;set;}
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class NicknamesAdapter:BaseAdapter
    {
        private List<Nickname> list;
        private Activity activity;

        public NicknamesAdapter(Activity a,List<Nickname>l)
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
            if (view == null)
            {
                view = activity.LayoutInflater.Inflate(Resource.Layout.ListViewRow, parent, false);
            }
            var tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            var tvCount = view.FindViewById<TextView>(Resource.Id.tvCount);

            tvName.Text = list[position].Name;
            tvCount.Text = list[position].Count.ToString();

            return view;
        }
    }
}