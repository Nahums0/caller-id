using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Views.InputMethods;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;
using Called_Id;
using Newtonsoft.Json;
using RestSharp;
using Android_Tabs;

namespace Called_Id
{
    public class SlidingTabsFragment : Fragment
    {
        private SlidingTabScrollView mSlidingTabScrollView;
        private ViewPager mViewPager;
        private Intent intent;

        public SlidingTabsFragment()
        {
            
        }

        public SlidingTabsFragment(Intent i) { intent = i; }
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_samle, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            mSlidingTabScrollView = view.FindViewById<SlidingTabScrollView>(Resource.Id.sliding_tabs);
            mViewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
            mViewPager.Adapter = new MainPagerAdapter(intent,view);

            mSlidingTabScrollView.ViewPager = mViewPager;
        }

    }

    public class MainPagerAdapter : PagerAdapter
    {
        static int count = 0;
        List<string> Tabs = new List<string>();
        Context context;
        Intent intent;
        View view;
        JsonClass.RootObject UserDataObject;
        SearchResultsAdapter resultsAdapter;
        private ListView lview;
        View secondview;
        //private ListView lvResults;
        private bool FirstOpen = true;

        public MainPagerAdapter(Intent i,View view)
        {
            secondview = view;
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
            view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.pager_item, container, false);
            container.AddView(view);

            lview = view.FindViewById<ListView>(Resource.Id.listview);
            var sview = view.FindViewById<SearchView>(Resource.Id.searchview);
            var cnumber = view.FindViewById<TextView>(Resource.Id.tvCurrentNumber);
            var swiperefreshview = view.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout>(Resource.Id.swiperefresh);
            var rlAbout = view.FindViewById<RelativeLayout>(Resource.Id.rlFragAbout);

            view.FindViewById<ListView>(Resource.Id.lvSearchResults).Visibility=ViewStates.Gone;
            
            if (FirstOpen)
            {
                swiperefreshview.Refresh += Swiperefreshview_Refresh;
                swiperefreshview.Refreshing = true;
                Swiperefreshview_Refresh(swiperefreshview, null);
                FirstOpen = false;
            }

            sview.SetQueryHint("Search A Specific Phone Number");
            NicknamesAdapter adapter = GetNicknamesList(intent.GetStringExtra("userdata"));

            //resultsAdapter = new SearchResultsAdapter((Activity)context,new List<string>() { "a" });

            //lvResults.Adapter = resultsAdapter;

            lview.Adapter = adapter;
            lview.ItemClick += Lview_ItemClick;
            string PhoneNumber = intent.GetStringExtra("PhoneNumber");

            if (PhoneNumber.Length == 9)
                cnumber.Text = "Names For :  " + PhoneNumber.Substring(0, 2) + " - " + PhoneNumber.Substring(2);
            else if (PhoneNumber.Length == 10)
                cnumber.Text = "Names For :  " + PhoneNumber.Substring(0, 3) + " - " + PhoneNumber.Substring(3);
            else
                cnumber.Text = "Names For :  " + PhoneNumber;

            sview.QueryTextSubmit += Sview_QueryTextSubmit;
            
            switch (position)
            {
                case 0:
                    ShowKeyboard();
                    //lvResults.Adapter = resultsAdapter;
                    lview.Visibility = ViewStates.Visible;
                    swiperefreshview.Visibility = ViewStates.Visible;
                    cnumber.Visibility = ViewStates.Visible;
                    rlAbout.Visibility = ViewStates.Invisible;
                    sview.Visibility = ViewStates.Invisible;
                    break;
                case 1:
                    lview.Visibility = ViewStates.Gone;
                    rlAbout.Visibility = ViewStates.Invisible;
                    swiperefreshview.Visibility = ViewStates.Gone;
                    cnumber.Visibility = ViewStates.Gone;
                    sview.Visibility = ViewStates.Visible;

                    sview.Focusable = true;
                    sview.FocusableInTouchMode = true;
                    if (count++ == 0)
                    {
                        HideKeyboard();
                    }
                    else
                    {
                        sview.RequestFocus();
                    }
                    break;
                case 2:

                    ShowKeyboard();
                    //lvResults.Visibility = ViewStates.Gone;
                    rlAbout.Visibility = ViewStates.Visible;
                    cnumber.Visibility = ViewStates.Gone;
                    swiperefreshview.Visibility = ViewStates.Gone;
                    sview.Visibility = ViewStates.Gone;
                    break;
            }
            
            return view;
        }

        private void HideKeyboard()
        {
            var v = ((Activity)context).CurrentFocus;
            if (v != null)
            {
                Thread.Sleep(10);
                InputMethodManager imn = (InputMethodManager)((Activity)context).GetSystemService(Context.InputMethodService);
                imn.HideSoftInputFromWindow(v.WindowToken, 0);
            }
        }
        private void ShowKeyboard()
        {
            var v = ((Activity)context).CurrentFocus;
            if (v != null && v.Width < v.Height)
            {
                Thread.Sleep(10);
                InputMethodManager imn = (InputMethodManager)((Activity)context).GetSystemService(Context.InputMethodService);
                imn.ShowSoftInput(v, ShowFlags.Forced);
            }
        }


        private void Sview_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            new Thread(() =>
            {
                string query = ((SearchView)sender).Query;
                HideKeyboard();

                Intent i = new Intent(context, typeof(SearchResults));
                i.PutExtra("query", query);
                ((Activity)context).StartActivity(i);               
            }).Start();

            //SearchResultsAdapter searchResultsAdapter = new SearchResultsAdapter((Activity)context,list );

        }


        private void Swiperefreshview_Refresh(object sender, EventArgs e)
        {
            var swiperefresher = (Android.Support.V4.Widget.SwipeRefreshLayout)sender;
            new Thread(() =>
            {
                //Cancels the update after 7 seconds
                {
                    var timer = new System.Timers.Timer();
                    timer.Interval = 1000;
                    int count = 0;
                    timer.Elapsed += delegate
                    {
                        if (count++ == 7)
                        {
                            ((Activity)context).RunOnUiThread(() =>
                            {
                                if (swiperefresher.Refreshing)
                                {
                                    swiperefresher.Refreshing = false;
                                    Toast.MakeText(context, "Connection Timed Out", ToastLength.Short).Show();
                                }
                            });
                        }
                    };
                    timer.Start();
                }

                var tuple = RestQueries.Authenticate(intent.GetStringExtra("PhoneNumber"));

                if (tuple.Logged)
                {
                    if (tuple.Data != "" && tuple.Data != null)
                    {
                        var adapter = GetNicknamesList(tuple.Data);
                        ((Activity)context).RunOnUiThread(() =>
                        {
                            lview.Adapter = adapter;
                            swiperefresher.Refreshing = false;
                        });
                    }
                    else
                    {
                        ((Activity)context).RunOnUiThread(() =>
                        {
                            swiperefresher.Refreshing = false;
                            Toast.MakeText(context, "No Connection", ToastLength.Short).Show();
                        });
                    }
                }
                else
                {
                    ((Activity)context).RunOnUiThread(() =>
                    {
                        swiperefresher.Refreshing = false;
                        Toast.MakeText(context, "No Connection", ToastLength.Short).Show();
                    });
                }
            }).Start();
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
        private NicknamesAdapter GetNicknamesList(string userdata)
        {
            string UserData = userdata;
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
            var item = ((ListView)e.Parent).GetChildAt(e.Position); // Gets the clicked view
            item.StartAnimation(Nickname_Clicked.buttonClick);      // Animates the clicked view

            var ClickedItem = MainActivity.NicknamesList[e.Position];       // Get the nickname that was clicked
            var activity2 = new Intent(context, typeof(Nickname_Clicked));  

            activity2.PutExtra("name", ClickedItem.Name);    // Inserts the nickname into the intent under the "name" key

            List<string> CalledByList = new List<string>();
            foreach (var item2 in UserDataObject.Nicknames)  // Get all the users that call the current user by that name
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

            context.StartActivity(activity2);  // Starts the intent
        }

        public string GetHeaderTitle(int position)
        {
            return Tabs[position];
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            HideKeyboard();
            container.RemoveView((View)@object);
        }

    }
}