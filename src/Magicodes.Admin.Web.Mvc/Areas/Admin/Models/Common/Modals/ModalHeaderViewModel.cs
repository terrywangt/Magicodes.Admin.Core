using Magicodes.Plus.Core.ViewModels.Common.Modals;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Common.Modals
{
    public class ModalHeaderViewModel : IModalHeaderViewModel
    {
        public string Title { get; set; }

        public ModalHeaderViewModel(string title)
        {
            Title = title;
        }
    }
}