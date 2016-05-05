using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using VirtualizedData.Annotations;
using VirtualizedData.Models;
using VirtualizedData.Service;
using Xamarin.Forms;

namespace VirtualizedData.ViewModels
{
    public class ItemsViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly CompositeDisposable _cds = new CompositeDisposable();
        private readonly IDataModel _dataModel;
        private readonly ReadOnlyObservableCollection<ItemEntryViewModel> _items;
        private bool _isBusy;
        private int _lastLoadedPage;
        private bool _morePagesAvailable;
        private ItemEntryViewModel _selectedItem;

        public ItemsViewModel(IDataModel dataModel)
        {
            _dataModel = dataModel;
            _cds.Add(_dataModel.RemainingPages.Subscribe(OnRemainingPagesChanged));
            var itemsObservable = _dataModel.Items.Connect();
            var operations = itemsObservable
                .Transform(CreateEntryViewModel)
                .Bind(out _items)
                .Subscribe();
            _cds.Add(operations);
        }

        public ReadOnlyObservableCollection<ItemEntryViewModel> Items => _items;

        public ItemEntryViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ICommand LoadNextPageCommand
        {
            get { return new Command(async () => await LoadPage(_lastLoadedPage + 1)); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public bool MorePagesAvailable
        {
            get { return _morePagesAvailable; }
            private set
            {
                _morePagesAvailable = value;
                OnPropertyChanged(nameof(MorePagesAvailable));
            }
        }

        public void Dispose()
        {
            _cds.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnRemainingPagesChanged(int remainingPages)
        {
            MorePagesAvailable = remainingPages != 0;
        }

        public async Task LoadPage(int pageNumber)
        {
            IsBusy = true;
            await _dataModel.GetPage(pageNumber);
            _lastLoadedPage = pageNumber;
            Debug.WriteLine("Current page: " + pageNumber);
            IsBusy = false;
        }

        private ItemEntryViewModel CreateEntryViewModel(Item item)
        {
            return new ItemEntryViewModel {ImageUrl = item.ImageUrl, Title = item.Title};
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}