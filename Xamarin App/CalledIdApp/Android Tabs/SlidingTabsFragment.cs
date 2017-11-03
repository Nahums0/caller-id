using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;
using Android.Views.InputMethods;
using Android_Tabs;

namespace Called_Id
{
    public class SlidingTabsFragment : Fragment
    {
        private SlidingTabScrollView mSlidingTabScrollView;
        private ViewPager mViewPager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            return inflater.Inflate(Resource.Layout.fragment_samle, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            mSlidingTabScrollView = view.FindViewById<SlidingTabScrollView>(Resource.Id.sliding_tabs);
            mViewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
            mViewPager.Adapter = new SamplePagerAdapter();

            mSlidingTabScrollView.ViewPager = mViewPager;
        }

    }

    public class SamplePagerAdapter : PagerAdapter
    {
        List<string> Tabs = new List<string>();
        Context context;

        public SamplePagerAdapter()
        {
            Tabs.Add("Names");
            Tabs.Add("Search");
            Tabs.Add("About");
        }
        public override int Count
        {
            get
            {
                return Tabs.Count;
            }
        }
        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }


        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            context = container.Context;
            View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.pager_item, container, false);
            container.AddView(view);

            TextView tv = view.FindViewById<TextView>(Resource.Id.item_title);
            var lview = view.FindViewById<ListView>(Resource.Id.listview);
            var sview = view.FindViewById<SearchView>(Resource.Id.searchview);
            var cnumber = view.FindViewById<TextView>(Resource.Id.tvCurrentNumber);


            sview.SetQueryHint("Search A Specific Phone Number");

            MainActivity.NicknamesList = new List<Nickname>()
            {
                new Nickname{Name="Nahum",Count=12},
                new Nickname{Name="Omer",Count=7},
                new Nickname{Name="Moshe",Count=5}
            };
            NicknamesAdapter adapter = new NicknamesAdapter((Activity)context, MainActivity.NicknamesList);
            lview.ItemClick += Lview_ItemClick;
            lview.Adapter = adapter;
            switch (position)
            {
                case 0:
                    lview.Visibility = ViewStates.Visible;
                    cnumber.Visibility = ViewStates.Visible;
                    sview.Visibility = ViewStates.Invisible;

                    break;
                case 1:
                    lview.Visibility = ViewStates.Gone;
                    cnumber.Visibility = ViewStates.Gone;
                    sview.Visibility = ViewStates.Visible;
                    break;
                case 2:
                    lview.Visibility = ViewStates.Gone;
                    cnumber.Visibility = ViewStates.Gone;
                    sview.Visibility = ViewStates.Gone;
                    tv.Text = "This Is The About Fragment";
                    break;
            }


            return view;
        }

        private void Lview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((ListView)e.Parent).GetChildAt(e.Position);
            item.StartAnimation(Nickname_Clicked.buttonClick);

            var ClickedItem = MainActivity.NicknamesList[e.Position];
            var activity2 = new Intent(context, typeof(Nickname_Clicked));
            activity2.PutExtra("name", ClickedItem.Name);
            context.StartActivity(activity2);
        }

        public string GetHeaderTitle(int position)
        {
            return Tabs[position];
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

    }
}