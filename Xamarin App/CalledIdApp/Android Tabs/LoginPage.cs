using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android_Tabs;
using Android.Content;
using System.IO;
using Android.Preferences;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Android.Provider;
using Xamarin.Contacts;
using System.Linq;

namespace Called_Id
{
    //TODO: update contacts on login
    public class JsonClass
    {
        public class Nickname
        {
            public string m_Item1 { get; set; }
            public string m_Item2 { get; set; }
        }

        public class RootObject
        {
            public int Id { get; set; }
            public string Number { get; set; }
            public List<Nickname> Nicknames { get; set; }
            public string MetaDataJsonForDb { get; set; }
        }
    }
    public static class AppConsts
    {
        public static string RestBaseUrl = "http://localhost:55011";
    }
    [Activity(Label = "LoginPage", MainLauncher = true)]
    public class LoginPage : Activity
    {
        Button btnLogin;
        Button btnRegister;
        EditText etPhoneNumber;
        Handler handler;

        //UI Events
        public void BtnLogin_Click(object sender, EventArgs e)
        {
            var Logged = Authenticate(etPhoneNumber.Text);
            if (Logged.Logged)
            {
                SaveCurrentUser(etPhoneNumber.Text,Logged.Data);
                ProceedToMainActivity(etPhoneNumber.Text,Logged.Data);
            }
            else
            {
                Toast.MakeText(this, "Unrecgnized User", ToastLength.Short);
                etPhoneNumber.Text = "";
            }
        }
        public async void BtnRegister_Click(object sender, EventArgs e)
        {
            //TODO: Add sms auth
            string Phonenumber = etPhoneNumber.Text;
            if (Phonenumber=="")
            {
                Toast.MakeText(this, "Enter A Phone Number", ToastLength.Short);
                return;
            }
            var client = new RestClient(AppConsts.RestBaseUrl+"/api/Ids/");
            var request = new RestRequest(Method.POST);

            var book = new AddressBook(this);
            //         new AddressBook (this); on Android
            if (!await book.RequestPermission())
            {
                Toast.MakeText(this, "Please allow access to your contacts", ToastLength.Long);
                return;
            }
            string query = Phonenumber + "_";
            string numbers_query="_";
            string names_query = "";

            foreach (var item in book)
            {
                if (item.DisplayName != null && item.Phones.Count() != 0)
                {
                    foreach (var phone in item.Phones)
                    {
                        if (phone.Number != null && phone.Number != "")
                        {
                            names_query += item.DisplayName+",";
                            numbers_query += phone.Number+",";
                            break;
                        }
                    }
                }
            }
            names_query.Remove(names_query.Length - 2);
            numbers_query.Remove(numbers_query.Length - 2);
            query += names_query + numbers_query;

            Toast.MakeText(this, query, ToastLength.Long).Show();

            IRestResponse response = client.Execute(request);
            var content = response.Content;
            var responsedata = Authenticate(Phonenumber);
            if (responsedata.Logged)
            {
                ProceedToMainActivity(Phonenumber, responsedata.Data);
            }
            else
                Toast.MakeText(this, "An Error Has Accured Please Try Again Later", ToastLength.Long);
        }

        private void ProceedToMainActivity(string PhoneNumber,string userdata)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            i.PutExtra("PhoneNumber", PhoneNumber);
            i.PutExtra("userdata", userdata);
            i.SetFlags(i.Flags | ActivityFlags.NoHistory);
            StartActivity(i);
            Finish();
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
           
        }

        private void CheckForLoggedUser()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();

            string Number = prefs.GetString("PhoneNumber", null);
            string Data = prefs.GetString("UserData", null);
            if (Number != null && Data != null)
            {
                ProceedToMainActivity(Number, Data);
            }
        }

        private (bool Logged,string Data) Authenticate(string phoneNumber)
        {
            try
            {
                var client = new RestClient(AppConsts.RestBaseUrl+"/api/Ids/" + phoneNumber);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                if (content == "null")
                    return (false,null);
                JsonClass.RootObject Result = JsonConvert.DeserializeObject<JsonClass.RootObject>(content);
                if (Result.Number == phoneNumber)
                    return (true,content);
            }
            catch
            {
                return (false,null);
            }
            return (false, null);
        }

        private void SaveCurrentUser(string Number, string UserData)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("PhoneNumber", Number);
            editor.PutString("UserData", UserData);
            editor.Apply();
        }

    }
}