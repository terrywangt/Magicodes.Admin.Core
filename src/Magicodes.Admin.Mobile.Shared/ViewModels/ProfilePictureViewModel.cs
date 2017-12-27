using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.ViewModels.Base;
using Xamarin.Forms;

namespace Magicodes.Admin.ViewModels
{
    public class ProfilePictureViewModel : XamarinViewModel
    {
        public ICommand CloseCommand => AsyncCommand.Create(NavigationService.CloseModalAsync);

        private ImageSource _photo;

        public ImageSource Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                RaisePropertyChanged(() => Photo);
            }
        }

        public override Task InitializeAsync(object navigationData)
        {
            var profilePictureBytes = (byte[])navigationData;
            Photo = ImageSource.FromStream(() => new MemoryStream(profilePictureBytes));
            return Task.CompletedTask;
        }
    }
}
