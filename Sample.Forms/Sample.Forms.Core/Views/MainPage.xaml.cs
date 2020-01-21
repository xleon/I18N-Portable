using Sample.Forms.Core.ViewModels;
using Xamarin.Forms;

namespace Sample.Forms.Core.Views
{
	public partial class MainPage : ContentPage
	{
        public MainPage()
		{
			InitializeComponent();

            BindingContext = new MainPageViewModel
            {
                // Do not do this
                CurrentPage = this
            };
		}
	}
}