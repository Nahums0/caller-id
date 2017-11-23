using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Support.V4.View;
using Called_Id;
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
}