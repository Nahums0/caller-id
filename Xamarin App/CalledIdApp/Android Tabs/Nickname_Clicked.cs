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
using Android_Tabs;
using RestSharp;
using Newtonsoft.Json;
using System.Threading;

namespace Called_Id 
{
    [Activity(Label = "Nickname_Clicked")]
    public class Nickname_Clicked : Activity
    {
        private List<NicknameInfo> ContactsList;

        public static Android.Views.Animations.AlphaAnimation buttonClick = new Android.Views.Animations.AlphaAnimation(1F, 0.4F);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            buttonClick.Duration = 100;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.nickname_clicked_layout);

            FindViewById<TextView>(Resource.Id.tvCalledBy).Text = "Called " + Intent.GetStringExtra("name") + " By:";
            var btnBack=FindViewById<LinearLayout>(Resource.Id.btnBackButton);
            btnBack.Click += delegate
            {
                btnBack.StartAnimation(buttonClick);
                Finish();
            };

            Thread ThreadGetList = new Thread(GetContactsList);
            ThreadGetList.Start();
        }

        private void GetContactsList()
        {
            try
            {
                var CalledByArray = Intent.GetStringArrayListExtra("calledby");
                string query = "";
                foreach (var item in CalledByArray)
                {
                    query += item + ",";
                }
                if (query[query.Length - 1] == ',')
                {
                    query = query.Remove(query.Length - 1);
                }

                var client = new RestClient(AppConsts.RestBaseUrl);
                var request = new RestRequest("/api/List?data=" + query, Method.GET);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                content = content.Substring(1, content.Length - 2);
                content = content.Replace("\\\"", "\"");

                JsonClass.JsonList.RootObject Result = JsonConvert.DeserializeObject<JsonClass.JsonList.RootObject>(content);
                var List = Result.List;
                ContactsList = new List<NicknameInfo>();


                foreach (var item in List)
                {
                    var tempdict = new Dictionary<string, int>();
                    if (item.Nicknames.Count == 0)
                    {
                        tempdict.Add(item.Number, 1);
                    }
                    else
                    {
                        foreach (var nickname in item.Nicknames)
                        {
                            if (tempdict.ContainsKey(nickname.m_Item1))
                            {
                                tempdict[nickname.m_Item1]++;
                            }
                            else
                                tempdict.Add(nickname.m_Item1, 1);
                        }
                    }
                    int m = tempdict.Values.Max();
                    string MaxKey = tempdict.FirstOrDefault(x => x.Value == m).Key;
                    ContactsList.Add(new NicknameInfo()
                    {
                        Name = MaxKey,
                        Number = item.Number
                    });
                }
                RunOnUiThread(() =>
                {
                    FindViewById(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;

                    NicknamesInfoAdpater adapter = new NicknamesInfoAdpater(this, ContactsList);

                    var lview = FindViewById<ListView>(Resource.Id.lvNicknameInfoListView);
                    lview.Adapter = adapter;
                    lview.ItemClick += Lview_ItemClick;
                });
            }
            catch (Exception e)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "No connection", ToastLength.Short).Show();
                    Finish();
                });
            }
        }

        private void Lview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((ListView)e.Parent).GetChildAt(e.Position);
            item.StartAnimation(buttonClick);
        }
    }
}