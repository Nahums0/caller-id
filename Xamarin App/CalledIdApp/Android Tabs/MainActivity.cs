using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System.Collections.Generic;
using Android_Tabs;

namespace Called_Id
{
    [Activity(Label = "Called_Id")]
    public class MainActivity : Activity
    {
        public static List<Nickname> NicknamesList;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SlidingTabsFragment fragment = new SlidingTabsFragment(Intent);
            transaction.Replace(Resource.Id.sample_content_fragment, fragment);
            transaction.Commit();
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.actionbar_main, menu);
        //    return base.OnCreateOptionsMenu(menu);
        //}
    }
}

