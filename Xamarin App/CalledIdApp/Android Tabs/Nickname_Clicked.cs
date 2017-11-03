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

namespace Called_Id
{
    [Activity(Label = "Nickname_Clicked")]
    public class Nickname_Clicked : Activity
    {
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
            NicknamesInfoAdpater adapter = new NicknamesInfoAdpater(this, new List<NicknameInfo>()
            {
                new NicknameInfo{Name="Yaron",Number="0545896245"},
                new NicknameInfo{Name="Moshe",Number="0528953660"},
                new NicknameInfo{Name="John",Number="0526853520"},
            });
            var lview = FindViewById<ListView>(Resource.Id.lvNicknameInfoListView);
            lview.Adapter = adapter;
            lview.ItemClick += Lview_ItemClick;
        }

        private void Lview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((ListView)e.Parent).GetChildAt(e.Position);
            item.StartAnimation(buttonClick);
        }
    }
}