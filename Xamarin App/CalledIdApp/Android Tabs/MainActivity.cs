using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System.Collections.Generic;
using Android_Tabs;
using System.Threading.Tasks;
using System.Threading;

namespace Called_Id
{
    [Activity(Label = "Called_Id")]
    public class MainActivity : Activity
    {
        public static List<Nickname> NicknamesList;

        private async void PostUser()
        {
            await RestQueries.PostUser(this, Intent.GetStringExtra("PhoneNumber"));
        }


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            new Thread(PostUser).Start();
        
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SlidingTabsFragment fragment = new SlidingTabsFragment(Intent);
            transaction.Replace(Resource.Id.sample_content_fragment, fragment);
            transaction.Commit();
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Android.Resource.Menu.actionbar_main, menu);
        //    return base.OnCreateOptionsMenu(menu);
        //}
    }
}

