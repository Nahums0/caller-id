using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Called_Id;
using Android_Tabs;

namespace Called_Id
{
    class SearchResultsAdapter : BaseAdapter
    {
        public List<string> Items;
        private Activity activity;

        public SearchResultsAdapter(Activity activity)
        {
            this.activity = activity;
            Items = new List<string>();
        }
        public SearchResultsAdapter(Activity activity,List<string>list)
        {
            this.activity = activity;
            Items = list;
        }

        public override int Count => Items.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return Items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            var current = Items[position];
            if (view == null)
            {
                view = activity.LayoutInflater.Inflate(Resource.Layout.ListviewRow_Nicknames_info, parent, false);
            }
            var tvName = view.FindViewById<TextView>(Resource.Id.tvNicknameInfoName);
            var tvNumber = view.FindViewById<TextView>(Resource.Id.tvNicknameInfoNumber);
            tvName.Text = current;
            tvNumber.Text = "";

            return view;
        }
    }
}