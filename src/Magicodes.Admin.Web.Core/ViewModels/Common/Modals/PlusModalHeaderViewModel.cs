namespace Magicodes.Admin.ViewModels.Common.Modals
{
    public class PlusModalHeaderViewModel : IModalHeaderViewModel
    {
        public PlusModalHeaderViewModel(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
    }
}