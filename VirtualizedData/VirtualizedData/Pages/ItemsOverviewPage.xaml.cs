using System;
using System.Threading.Tasks;
using VirtualizedData.Models;
using VirtualizedData.Service;
using VirtualizedData.ViewModels;
using Xamarin.Forms;

namespace VirtualizedData.Pages
{
    public partial class ItemsOverviewPage : ContentPage
    {
        ItemsViewModel ViewModel => _vm ?? (_vm = BindingContext as ItemsViewModel);
        private ItemsViewModel _vm;

        public ItemsOverviewPage()
        {
            InitializeComponent();
            BindingContext = new ItemsViewModel(new DataModel(new FakeDataService()));
            
        }

        protected override void OnAppearing()
        {
            Task.Run(async () =>
            {
                try
                {
                    await ViewModel.LoadPage(1);
                }
                catch (Exception e)
                {

                }
            });
            base.OnAppearing();
        }
    }
}
