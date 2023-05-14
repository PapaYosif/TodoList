using Tester.ViewModel;

namespace Tester.Views;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();

		BindingContext = new MainViewModel();

	}
}

