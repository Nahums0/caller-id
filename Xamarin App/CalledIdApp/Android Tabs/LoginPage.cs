
using Android.App;
using Android.OS;
using Android_Tabs;


namespace Called_Id
{
    [Activity(Label = "LoginPage", MainLauncher = true)]
    public class LoginPage : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_page);

            // Create your application here
        }
    }
}