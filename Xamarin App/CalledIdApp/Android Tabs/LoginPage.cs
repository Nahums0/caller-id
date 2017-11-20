using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Called_Id;
using Android.Content;
using System.IO;
using Android.Preferences;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Android.Provider;
using Xamarin.Contacts;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Android_Tabs;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Called_Id
{
    //TODO: update contacts on login
    public class JsonClass
    {
        public class JsonList
        {
            public class Nickname
            {
                public string m_Item1 { get; set; }
                public string m_Item2 { get; set; }
            }

            public class List
            {
                public int Id { get; set; }
                public string Number { get; set; }
                public List<Nickname> Nicknames { get; set; }
            }

            public class RootObject
            {
                public List<List> List { get; set; }
            }
        }
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
        public static string RestBaseUrl = "http://77.138.13.115/";
        public static string AppCenterSecret = "{Secret}";
    }
    public static class RestQueries
    {
        ///<summary>
        ///<para>Posts a new user to the server</para>
        ///<para>Uses context the get user's contacts and a user entered phone number</para>
        ///</summary>
        public static async Task<string> PostUser(Context context, string Phonenumber)
        {
            var client = new RestClient(AppConsts.RestBaseUrl + "/api/Ids/");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;

            var book = new AddressBook(context);
            if (!await book.RequestPermission())
            {
                return "-1";
            }
            string query = Phonenumber + "_";
            string numbers_query = "_";
            string names_query = "";

            foreach (var item in book)
            {
                if (item.DisplayName != null && item.Phones.Count() != 0)
                {
                    foreach (var phone in item.Phones)
                    {
                        if (phone.Number != null && phone.Number != "")
                        {
                            names_query += item.DisplayName + ",";
                            numbers_query += phone.Number + ",";
                            break;
                        }
                    }
                }
            }
            names_query.Remove(names_query.Length - 2);
            numbers_query.Remove(numbers_query.Length - 2);
            query += names_query + numbers_query;
            query = query.Substring(0, query.Length - 2);
            request.AddBody(query);

            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return content;
        }
        /// <summary>
        /// <para>Recives a phone number and uthenticates the user</para>
        /// </summary>
        /// <param name="phoneNumber">User's phone number</param>
        /// <returns>Return a tuple -> Logged will determine if user logged in successfully and Data will contaion the query result</returns>
        public static (bool Logged, string Data) Authenticate(string phoneNumber)
        {
            try
            {
                var client = new RestClient(AppConsts.RestBaseUrl);
                var request = new RestRequest("/api/Ids/" + phoneNumber, Method.GET);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                if (content == "null")
                    return (false, null);
                JsonClass.RootObject Result = JsonConvert.DeserializeObject<JsonClass.RootObject>(content);
                if (Result.Number == phoneNumber)
                    return (true, content);
            }
            catch
            {
                return (false, null);
            }
            return (false, null);
        }
        /// <summary>
        /// <para>Delete the user which phone number is the same as the parameter</para>
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        public static bool DeleteUser(string phonenumber)
        {
            try
            {
                var client = new RestClient(AppConsts.RestBaseUrl + "api/ids" + phonenumber);
                var request = new RestRequest(Method.DELETE);
                client.Execute(request);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


    [Activity(Label = "Caller ID", MainLauncher = true)]
    public class LoginPage : Activity
    {
        Button btnLogin;
        Button btnRegister;
        EditText etPhoneNumber;

        //UI Events
        public void BtnLogin_Click(object sender, EventArgs e)
        {
            string PhoneNumber = etPhoneNumber.Text;

            //Progress bar code
            var LoadPanel=FindViewById(Resource.Id.LoginPageloadingPanel);
            LoadPanel.Visibility = Android.Views.ViewStates.Visible;
            FindViewById<ProgressBar>(Resource.Id.LoginPageSpinner).IndeterminateDrawable.SetColorFilter(new Android.Graphics.Color(255,255,255),Android.Graphics.PorterDuff.Mode.Multiply);


            new Thread(() => 
            {
                var Logged = RestQueries.Authenticate(PhoneNumber);
                if (Logged.Logged)
                {
                    SaveCurrentUser(PhoneNumber, Logged.Data);
                    RunOnUiThread(() =>
                    {
                        ProceedToMainActivity(PhoneNumber, Logged.Data);
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Unrecgnized User", ToastLength.Short).Show();
                        LoadPanel.Visibility = Android.Views.ViewStates.Gone;
                        etPhoneNumber.Text = "";
                    });
                }
            }).Start();
            
        }                 // Login button handler
        public void BtnRegister_Click(object sender, EventArgs e)
        {
            //TODO: Add sms auth
            var LoadPanel = FindViewById(Resource.Id.LoginPageloadingPanel);
            LoadPanel.Visibility = Android.Views.ViewStates.Visible;
            FindViewById<ProgressBar>(Resource.Id.LoginPageSpinner).IndeterminateDrawable.SetColorFilter(new Android.Graphics.Color(255, 255, 255), Android.Graphics.PorterDuff.Mode.Multiply);
            Context context = this;
            var t = Task.Run(async () =>
            {
                string Phonenumber = etPhoneNumber.Text;
                if (Phonenumber == "" || !ValidateNumber(Phonenumber))
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Enter A Valid International Number", ToastLength.Short).Show();
                        LoadPanel.Visibility = Android.Views.ViewStates.Gone;
                    });
                    return;
                }

                var content = await RestQueries.PostUser(context, Phonenumber);

                if (content == "-1")
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Access Denied", ToastLength.Long).Show();
                        LoadPanel.Visibility = Android.Views.ViewStates.Gone;
                    });
                    return;
                }

                var responsedata = RestQueries.Authenticate(Phonenumber);
                if (responsedata.Logged)
                {
                    SaveCurrentUser("0546141122",responsedata.Data);
                    ProceedToMainActivity(Phonenumber, responsedata.Data);
                }
                else
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "An Error Has Accured Please Try Again Later", ToastLength.Long).Show();
                        LoadPanel.Visibility = Android.Views.ViewStates.Gone;
                    });
            });
        }        // Register button handler

        private void ProceedToMainActivity(string PhoneNumber, string userdata)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            i.PutExtra("PhoneNumber", PhoneNumber);
            i.PutExtra("userdata", userdata);
            //i.SetFlags(i.Flags | ActivityFlags.NoHistory);
            StartActivity(i);
            Finish();
        }// Starts the main activity with user's phone number and user data (no history)

        private void SetupViews() // Asigns layout views to variables  
        {
            btnLogin = FindViewById<Button>(Resource.Id.btnLoginPageLogin);
            btnRegister = FindViewById<Button>(Resource.Id.btnLoginPageRegister);
            etPhoneNumber = FindViewById<EditText>(Resource.Id.etLoginPagePhoneNumber);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppCenter.Start(AppConsts.AppCenterSecret,
                   typeof(Analytics), typeof(Crashes));
            
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
        }                                      // Check if a user is already logged in

        private void SaveCurrentUser(string Number, string UserData)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("PhoneNumber", Number);
            editor.PutString("UserData", UserData);
            editor.Apply();
        }           // Saves current user's data into a preference
        public bool ValidateNumber(string Number)
        {
            try
            {
                Number = Number.Replace(" ", "");

                for (int i = 1; i < Number.Length; i++)
                {
                    if (!char.IsDigit(Number[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }                              // Validates a if a number is real

    }
}