using Sample.XF.ViewModels;

namespace Sample.XF.Views
{
	public partial class MainPage
	{
        public MainPage()
		{
			InitializeComponent();

            BindingContext = new MainPageViewModel
            {
                // Do not do this in a real project ;)
                CurrentPage = this
            };
		}
	}
}