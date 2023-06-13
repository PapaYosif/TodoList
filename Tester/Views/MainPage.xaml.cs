using CommunityToolkit.Maui.Views;
using Tester.ViewModel;

namespace Tester.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        this.BindingContext = new MainViewModel();



    }

    private void addButton_Clicked(object sender, EventArgs e)
    {
        this.ShowPopup(new addTaskPage());
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {

        var changedTask = (sender as CheckBox).BindingContext;

        //var ctx = (MainViewModel)this.BindingContext;

        //ctx.SaveTaskCommand.Execute(changedTask);


    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        this.ShowPopup(new SettingsPage());
    }
}
