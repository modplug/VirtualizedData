using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace VirtualizedData.CustomControls
{
    /// <summary>
	/// A simple listview that exposes a bindable command to allow infinite loading behaviour.
	/// </summary>
	public class InfiniteListView : ListView
    {        
        /// <summary>
        /// Respresents the command that is fired to ask the view model to load additional data bound collection.
        /// </summary>
        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create("LoadMoreCommand",
            typeof (ICommand), typeof (InfiniteListView), default(ICommand));

        public static readonly BindableProperty ShowFooterProperty = BindableProperty.Create("ShowFooter", typeof (bool),
            typeof (InfiniteListView), false, BindingMode.OneWay, null, ShowOrHideFooter);

        public static readonly BindableProperty CanLoadMorePagesProperty = BindableProperty.Create("CanLoadMorePages", typeof(bool),
            typeof(InfiniteListView), false);

        private readonly StackLayout _footer;

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public bool ShowFooter
        {
            get { return (bool)GetValue(ShowFooterProperty); }
            set { SetValue(ShowFooterProperty, value); }
        }

        public bool CanLoadMorePages
        {
            get { return (bool)GetValue(CanLoadMorePagesProperty); }
            set { SetValue(CanLoadMorePagesProperty, value); }
        }

        private static void ShowOrHideFooter(BindableObject bindable, object oldValue, object newValue)
        {
            InfiniteListView obj = (InfiniteListView) bindable;
            if ((bool) oldValue == (bool) newValue)
                return;
            if ((bool)newValue)
            {
                if (obj.Footer != null)
                {
                    obj.Footer = obj._footer;
                }
            }
            else
            {
                // Setting footer to null didn't work so just give it a label to keep it's mouth shut :p
                obj.Footer = new Label();
            }
        }

        /// <summary>
        /// Creates a new instance of a <see cref="InfiniteListView" />
        /// </summary>
        public InfiniteListView()
        {
            ItemAppearing += InfiniteListView_ItemAppearing;
            ActivityIndicator indicator = new ActivityIndicator { IsRunning = true, HorizontalOptions = LayoutOptions.Center };
            Label label = new Label() { Text = "Loading items", HorizontalOptions = LayoutOptions.Center };
            _footer = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 100,
                Orientation = StackOrientation.Vertical,
                Children = {indicator, label}
            };
        }


        void InfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (!CanLoadMorePages)
                return;

            var items = ItemsSource as IList;
            if (items != null && e.Item == items[items.Count - 1])
            {
                if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                    LoadMoreCommand.Execute(null);
            }
        }
    }
}
