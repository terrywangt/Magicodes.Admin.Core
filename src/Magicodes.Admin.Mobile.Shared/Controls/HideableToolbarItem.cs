using System.Threading.Tasks;
using Xamarin.Forms;

namespace Magicodes.Admin.Controls
{
    public class HideableToolbarItem : ToolbarItem
    {
        public ContentPage ParentPage { set; get; }

        public static BindableProperty IsVisibleProperty = BindableProperty.Create<HideableToolbarItem, bool>(o => o.IsVisible, false, propertyChanged: OnIsVisibleChanged);

        public HideableToolbarItem()
        {
            InitVisibility();
        }

        private async void InitVisibility()
        {
            await Task.Delay(100);
            OnIsVisibleChanged(this, false, IsVisible);
        }

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        private static void OnIsVisibleChanged(BindableObject bindable, bool oldValue, bool newValue)
        {
            if (!(bindable is HideableToolbarItem item))
            {
                return;
            }

            if (item.ParentPage == null)
            {
                return;
            }

            var toolbarItems = item.ParentPage.ToolbarItems;

            if (newValue && !toolbarItems.Contains(item))
            {
                toolbarItems.Add(item);
            }
            else if (!newValue && toolbarItems.Contains(item))
            {
                toolbarItems.Remove(item);
            }
        }
    }
}