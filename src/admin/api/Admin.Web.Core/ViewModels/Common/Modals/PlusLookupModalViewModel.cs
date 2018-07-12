namespace Magicodes.Admin.ViewModels.Common.Modals
{
    public class PlusLookupModalViewModel : ILookupModalViewModel
    {
        public PlusLookupModalViewModel(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
    }
}