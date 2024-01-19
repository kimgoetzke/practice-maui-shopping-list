using ShoppingList.ViewModel;

namespace ShoppingList;

public partial class DetailPage
{
    public DetailPage(DetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((DetailViewModel)BindingContext).Item.From = ((DetailViewModel)BindingContext).Item.From;
    }
}