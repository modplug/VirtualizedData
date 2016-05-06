using System;
using System.Threading.Tasks;
using VirtualizedData.Models;
using VirtualizedData.Services;
using VirtualizedData.ViewModels;
using Xamarin.Forms;

namespace VirtualizedData.Pages
{
    public partial class MoviesPage : ContentPage
    {
        MoviesViewModel ViewModel => _vm ?? (_vm = BindingContext as MoviesViewModel);
        private MoviesViewModel _vm;

        public MoviesPage()
        {
            InitializeComponent();
            BindingContext = new MoviesViewModel(new DataModel(new MovieService()));
            
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
