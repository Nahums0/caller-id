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
using Newtonsoft.Json;

namespace Called_Id
{
    public class SlidingTabsFragment : Fragment
    {
        private SlidingTabScrollView mSlidingTabScrollView;
        private ViewPager mViewPager;
        private Intent intent;


        public SlidingTabsFragment(Intent i) { intent = i; }
       
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
            mViewPager.Adapter = new SamplePagerAdapter(intent);

            mSlidingTabScrollView.ViewPager = mViewPager;
        }

    }

    public class SamplePagerAdapter : PagerAdapter
    {
        List<string> Tabs = new List<string>();
        Context context;
        Intent intent;
        JsonClass.RootObject UserDataObject;


        public SamplePagerAdapter(Intent i)
        {
            intent = i;
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

            NicknamesAdapter adapter = GetNicknamesList();
            lview.Adapter = adapter;

            lview.ItemClick += Lview_ItemClick;

            string PhoneNumber = intent.GetStringExtra("PhoneNumber");
            cnumber.Text = "Names For :  " + PhoneNumber.Substring(0, 3) + " - " + PhoneNumber.Substring(3);

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


        private int NicknamesListContains(List<Nickname>list,JsonClass.Nickname obj)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name==obj.m_Item1)
                {
                    return i;
                }
            }
            return -1;
        }
        private NicknamesAdapter GetNicknamesList()
        {
            string UserData=intent.GetStringExtra("userdata");
            UserDataObject = JsonConvert.DeserializeObject<JsonClass.RootObject>(UserData);
            var List = new List<Nickname>();

            foreach (var item in UserDataObject.Nicknames)
            {
                var c = 5;
            }

            for (int i = 0; i < UserDataObject.Nicknames.Count; i++)
            {
                int result = NicknamesListContains(List, UserDataObject.Nicknames[i]);
                if (result != -1)
                {
                    List[result].Count++;
                }
                else
                    List.Add(new Nickname { Name = UserDataObject.Nicknames[i].m_Item1, Count = 1 });
            }
            MainActivity.NicknamesList = List;
            NicknamesAdapter adapter = new NicknamesAdapter((Activity)context, MainActivity.NicknamesList);
            return adapter;
        }


        private void Lview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((ListView)e.Parent).GetChildAt(e.Position);
            item.StartAnimation(Nickname_Clicked.buttonClick);

            var ClickedItem = MainActivity.NicknamesList[e.Position];
            var activity2 = new Intent(context, typeof(Nickname_Clicked));

            activity2.PutExtra("name", ClickedItem.Name);

            List<string> CalledByList = new List<string>();
            foreach (var item2 in UserDataObject.Nicknames)
            {
                var item2item1 = item2.m_Item1;
                var item2item2 = item2.m_Item2;
                var b = ClickedItem.Name;
                if (item2item1==ClickedItem.Name)
                {
                    CalledByList.Add(item2item2);
                }
            }
            activity2.PutStringArrayListExtra("calledby", CalledByList);
            //activity2.SetFlags(activity2.Flags | ActivityFlags.);

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