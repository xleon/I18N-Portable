using System.Diagnostics;
using System.Reflection;
using I18NPortable;
using Xamarin.Forms;
using Sample.Forms.Core.Views;

namespace Sample.Forms.Core
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

		    var currentAssembly = GetType().GetTypeInfo().Assembly;

		    I18N.Current
		        .SetLogger(text => Debug.WriteLine(text))
		        .SetNotFoundSymbol("⛔")
		        .SetFallbackLocale("en")
		        .Init(currentAssembly);

            MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}