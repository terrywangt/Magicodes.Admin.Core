namespace Magicodes.Admin.Models.Common
{
    public class SelectableItem<T>
    {
        public bool IsSelected { get; set; }

        public T Item { get; set; }

        public SelectableItem()
        {
            
        }

        public SelectableItem(bool isSelected, T item)
        {
            IsSelected = isSelected;
            Item = item;
        }
    }
}