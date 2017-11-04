
using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android_Tabs;
using Android.Content;
using System.IO;
using Android.Preferences;

namespace Called_Id
{
    [Activity(Label = "LoginPage", MainLauncher = true)]
    public class LoginPage : Activity
    {
        Button btnLogin;
        Button btnRegister;
        EditText etPhoneNumber;
        
        //UI Events
        public void BtnLogin_Click(object sender, System.EventArgs e)
        {
            bool Logged = Authenticate(etPhoneNumber.Text);
            if (Logged)
            {
                SaveCurrentUser(etPhoneNumber.Text);
                ProceedToMainActivity(etPhoneNumber.Text);
            }
            else
            {
                Toast.MakeText(this, "Either Phone Number Is Not Recognized Or An Error Has Accoured", ToastLength.Long).Show();
                etPhoneNumber.Text = "";
            }
        }

        private void ProceedToMainActivity(string Number)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            i.PutExtra("PhoneNumber", Number);
            i.SetFlags(i.Flags | ActivityFlags.NoHistory);
            StartActivity(i);
            Finish();
        }


        public void BtnRegister_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    

        private void SetupViews() // Asigns layout views to variables  
        {
            btnLogin = FindViewById<Button>(Resource.Id.btnLoginPageLogin);
            btnRegister = FindViewById<Button>(Resource.Id.btnLoginPageRegister);
            etPhoneNumber = FindViewById<EditText>(Resource.Id.etLoginPagePhoneNumber);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CheckForLoggedUser();

            SetContentView(Resource.Layout.login_page);

            SetupViews();
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;


           
            // Create your application here
        }

        private void CheckForLoggedUser()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();

            string content = prefs.GetString("PhoneNumber",null);
            if (content != null)
            {
                if (Authenticate(content))
                {
                    ProceedToMainActivity(content);
                }
            }


        }

        private bool Authenticate(string phoneNumber)
        {
            return true;
        }
        private void SaveCurrentUser(string Number)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("PhoneNumber", Number);
            editor.Apply();
        }

    }
}