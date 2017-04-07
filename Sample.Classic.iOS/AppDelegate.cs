using Foundation;
using Sample.Classic.Core;
using UIKit;

namespace Sample.Classic.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var app = new App();

            Window.RootViewController = new HomeViewController();
            Window.MakeKeyAndVisible();

            return true;
        }
    }
}