using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Rg.Plugins.Popup.Platforms.Android.Activities
{
    [Activity(Label = "", Theme = "@android:style/Theme.Translucent.NoTitleBar", NoHistory = true)]
    public class PopupActivity : Activity 
    {
        private View? _popupView;
        private int _enterAnimationResourceId;
        private int _exitAnimationResourceId;
        private int _noAnimationResourceId;

        //keep trying to find a way to override the animation/transition for this activity to replace the apparent 'wipe down' with something else. 
        
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _enterAnimationResourceId = Popup.Context.Resources.GetIdentifier("enter_activity", "anim", "au.gov.ato.ATOTax");
            _exitAnimationResourceId = Popup.Context.Resources.GetIdentifier("exit_activity", "anim", "au.gov.ato.ATOTax");
            _noAnimationResourceId = Popup.Context.Resources.GetIdentifier("None", "anim", "au.gov.ato.ATOTax");
            
            OverridePendingTransition(_enterAnimationResourceId, _noAnimationResourceId);

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

            // NOTE: neither of these options are ideal
            // var flags = WindowManagerFlags.Fullscreen;      // Result in status bar being toggled/pushed off screen
            var flags = WindowManagerFlags.ForceNotFullscreen; // works perfectly but Results in a quick but ugly downward moving mask on the way out

            Window?.SetFlags(flags, flags);

            var decorView = (FrameLayout?)Window?.DecorView;
            decorView?.AddView(_popupView);
        }
        
        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(_exitAnimationResourceId, _exitAnimationResourceId);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
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
        
    }
}
