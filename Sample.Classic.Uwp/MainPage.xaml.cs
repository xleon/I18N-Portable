using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sample.Classic.Core;

namespace Sample.Classic.Uwp
{
    public sealed partial class MainPage : Page
    {
        public BaseViewModel ViewModel => (BaseViewModel) DataContext;

        public MainPage()
        {
            InitializeComponent();
            DataContext = new BaseViewModel();
        }

        private void ChangeLanguageButtonClick(object sender, RoutedEventArgs e)
        {
            var menu = new MenuFlyout();

            foreach (var language in ViewModel.LanguagesToSelect)
            {
                var item = new MenuFlyoutItem {Text = language.DisplayName, Tag = language.Locale};
                item.Click += ItemOnClick;
                menu.Items.Add(item);
            }

            menu.ShowAt((FrameworkElement)sender);
        }

        private void ItemOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var menuItem = (MenuFlyoutItem) sender;
            var locale = (string) menuItem.Tag;

            ViewModel.LoadLocale(locale);
        }
    }
}