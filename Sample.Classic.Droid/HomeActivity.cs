using Android.App;
using Android.OS;
using Sample.Classic.Core;

namespace Sample.Classic.Droid
{
    [Activity(Label = "Sample.Classic.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var app = new App();

            SetContentView(Resource.Layout.Home);
        }
    }
}

