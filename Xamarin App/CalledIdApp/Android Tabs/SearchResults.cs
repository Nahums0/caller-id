using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using RestSharp;
using Android_Tabs;
using Newtonsoft.Json;

namespace Called_Id
{
    [Activity(Label = "SearchResults")]
    public class SearchResults : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SearchResults);
            //FindViewById<RelativeLayout>(Resource.Id.ResultsloadingPanel).Visibility = ViewStates.Gone;

            string query = this.Intent.GetStringExtra("query");
            QueryResults(query);

            // Create your application here
        }

        private void QueryResults(string query)
        {
            new Thread(() =>
            {
                var client = new RestClient(AppConsts.RestBaseUrl);
                var request = new RestRequest("/api/Ids/" + query, Method.GET);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                if (content == "null")
                {
                    this.RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Non Found", ToastLength.Short).Show();
                        Finish();
                    });
                    return;
                }
                JsonClass.RootObject Result = JsonConvert.DeserializeObject<JsonClass.RootObject>(content);
                List<string> list = new List<string>();
                foreach (var nick in Result.Nicknames)
                {
                    list.Add(nick.m_Item1);
                }
                string[] MostCommons = FindCommon(list);
                RunOnUiThread(() =>
                {
                    FindViewById<ListView>(Resource.Id.lvResulsList).Adapter = new SearchResultsAdapter(this, MostCommons.ToList());
                    FindViewById<RelativeLayout>(Resource.Id.ResultsloadingPanel).Visibility = ViewStates.Gone;
                });
            }).Start();
        }

        private string[] FindCommon(List<string> list)
        {
            if (list.Count == 1)
                return new string[1] { list[0] };
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var item in list)
            {
                if (dict.ContainsKey(item))
                    dict[item]++;
                else
                    dict.Add(item, 1);
            }

            Tuple<int, string> Max1 = new Tuple<int, string>(0, "");
            Tuple<int, string> Max2 = new Tuple<int, string>(0, "");

            foreach (var item in dict)
            {
                if (item.Value > Max2.Item1)
                {
                    if (item.Value > Max1.Item1)
                    {
                        Max1 = new Tuple<int, string>(item.Value, item.Key);
                    }
                    else
                        Max2 = new Tuple<int, string>(item.Value, item.Key);
                }
            }
            return new string[2] { Max1.Item2, Max2.Item2 };
        }
    }
}