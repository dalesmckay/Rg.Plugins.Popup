using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Rg.Plugins.Popup.Platforms.Android.Activities
{
    [Activity(Label = "", Theme = "@android:style/Theme.Translucent.NoTitleBar", NoHistory = true)]
    public class PopupActivity : Activity
    {
        private View? _popupView;
        
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var popupIdVal = Intent?.GetStringExtra("popupId");

            if (!Guid.TryParse(popupIdVal, out var popupId))
            {
                throw new Exception("Invalid PopupId");
            }

            if (!Popup.PopupViewCache.TryGetValue(popupId, out _popupView))
            {
                throw new Exception("Popup instance not found");
            }

            Popup.Context = this;

            var decorView = (FrameLayout?)Window?.DecorView;
            decorView?.AddView(_popupView);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_popupView != null)
            {
                var decorView = (FrameLayout?)Window?.DecorView;
                decorView?.RemoveView(_popupView);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            Finish();
        }
    }
}
