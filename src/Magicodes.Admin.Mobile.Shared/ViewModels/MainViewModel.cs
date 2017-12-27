using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Abp.IO.Extensions;
using MvvmHelpers;
using Magicodes.Admin.ApiClient.Models;
using Magicodes.Admin.Authorization.Users.Profile;
using Magicodes.Admin.Authorization.Users.Profile.Dto;
using Magicodes.Admin.Commands;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Models.NavigationMenu;
using Magicodes.Admin.Services.Navigation;
using Magicodes.Admin.ViewModels.Base;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Acr.UserDialogs;
using FFImageLoading;
using Magicodes.Admin.ApiClient;
using Magicodes.Admin.Controls;
using Magicodes.Admin.Core.Dependency;
using Magicodes.Admin.Localization;
using Magicodes.Admin.UI;
using Magicodes.Admin.UI.Assets;
using Magicodes.Admin.Views;

namespace Magicodes.Admin.ViewModels
{
    public class MainViewModel : XamarinViewModel
    {
        public ICommand PageAppearingCommand => HttpRequestCommand.Create(PageAppearing);
        public ICommand ChangeProfilePhotoCommand => new Command(ChangeProfilePhoto);
        public ICommand ShowProfilePhotoCommand => AsyncCommand.Create(ShowProfilePhoto);
        public ICommand LogoutCommand => AsyncCommand.Create(Logout);

        private readonly IProfileAppService _profileAppService;
        private readonly IApplicationContext _appContext;
        private readonly AbpAuthenticateModel _authenticateModel;
        private readonly ProxyProfileControllerService _profileControllerService;
        private readonly IAccessTokenManager _accessTokenManager;

        private const string ApplicationName = "Admin";
        private ImageSource _photo;
        private bool _isInitialized;
        private byte[] _profilePictureBytes;
        private string _userNameAndSurname;
        private bool _showMasterPage;
        private string _applicationInfo;

        private async Task PageAppearing()
        {
            if (_isInitialized)
            {
                return;
            }

            UserNameAndSurname = _appContext.LoginInfo.User.Name + " " + _appContext.LoginInfo.User.Surname;
            SetApplicationInfo();
            Photo = ImageSource.FromResource(AssetsHelper.ProfileImagePlaceholderNamespace);
            await GetUserPhoto(_appContext.LoginInfo.User.ProfilePictureId);
            BuildMenuItems();
            _isInitialized = true;
        }

        private void SetApplicationInfo()
        {
            ApplicationInfo = $"{ApplicationName}\n" +
                              $"v{_appContext.LoginInfo.Application.Version} " +
                              $"[{_appContext.LoginInfo.Application.ReleaseDate:yyyyMMdd}]";
        }

        public string UserNameAndSurname
        {
            get => _userNameAndSurname;
            set
            {
                _userNameAndSurname = value;
                RaisePropertyChanged(() => UserNameAndSurname);
            }
        }

        public string ApplicationInfo
        {
            get => _applicationInfo;
            set
            {
                _applicationInfo = value;
                RaisePropertyChanged(() => ApplicationInfo);
            }
        }

        public int MenuItemsCount
        {
            get
            {
                if (MenuItems == null)
                {
                    return -1;
                }

                return MenuItems.Count;
            }

        }

        public ImageSource Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                RaisePropertyChanged(() => Photo);
            }
        }

        public MainViewModel(
            IProfileAppService profileAppService,
            AbpAuthenticateModel authenticateModel,
            ProxyProfileControllerService profileControllerService,
            IApplicationContext appContext,
            IAccessTokenManager accessTokenManager)
        {
            _profileAppService = profileAppService;
            _authenticateModel = authenticateModel;
            _profileControllerService = profileControllerService;
            _appContext = appContext;
            _accessTokenManager = accessTokenManager;
        }

        private async Task GetUserPhoto(string profilePictureId)
        {
            if (!Guid.TryParse(profilePictureId, out var guid))
            {
                return;
            }

            var result = await _profileAppService.GetProfilePictureById(guid);
            _profilePictureBytes = Convert.FromBase64String(result.ProfilePicture);
            Photo = ImageSource.FromStream(() => new MemoryStream(_profilePictureBytes));
        }

        private void ChangeProfilePhoto()
        {
            UserDialogs.Instance.ActionSheet(new ActionSheetConfig
            {
                Title = L.Localize("ChangeProfilePicture"),
                Options = new List<ActionSheetOption>  {
                    new ActionSheetOption(L.Localize("PickFromGallery"),  async () => await PickProfilePictureAsync(CropImageIfNeedsAsync)),
                    new ActionSheetOption(L.Localize("TakePhoto"),  async () => await TakeProfilePhotoAsync(CropImageIfNeedsAsync))
                }
            });
        }

        /// <summary>
        /// Shows a crop view to crop the media file.
        /// Native image cropping feature is available only on UWP and IOS.
        /// For Android devices, custom cropping is implemented.
        /// </summary>
        private async Task CropImageIfNeedsAsync(MediaFile photo)
        {
            if (photo == null)
            {
                return;
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                var cropModalView = new CropView(photo.Path, OnCropCompleted, L.Localize("Ok"), L.Localize("Rotate"), L.Localize("Cancel"));
                await NavigationService.ShowModalAsync(cropModalView);
            }
            else
            {
                await OnCropCompleted(File.ReadAllBytes(photo.Path), Path.GetFileName(photo.Path));
            }
        }

        private async Task OnCropCompleted(byte[] croppedImageBytes, string fileName)
        {
            if (croppedImageBytes == null)
            {
                return;
            }

            var jpgStream = await ResizeImageAsync(croppedImageBytes);
            await SaveProfilePhoto(jpgStream.GetAllBytes(), fileName);
        }

        private async Task<Stream> ResizeImageAsync(byte[] imageBytes, int width = 256, int height = 256)
        {
            var result = ImageService.Instance.LoadStream(token =>
            {
                var tcs = new TaskCompletionSource<Stream>();
                tcs.TrySetResult(new MemoryStream(imageBytes));
                return tcs.Task;
            }).DownSample(width, height);

            return await result.AsJPGStreamAsync();
        }

        private static async Task PickProfilePictureAsync(Func<MediaFile, Task> picturePicked)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                return;
            }

            using (var photo = await CrossMedia.Current.PickPhotoAsync())
            {
                await picturePicked(photo);
            }
        }

        private async Task TakeProfilePhotoAsync(Func<MediaFile, Task> photoTaken)
        {
            if (!CrossMedia.Current.IsCameraAvailable ||
                !CrossMedia.Current.IsTakePhotoSupported)
            {
                UserDialogHelper.Warn("NoCamera");
                return;
            }

            await SetBusyAsync(async () =>
            {
                using (var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    DefaultCamera = CameraDevice.Front,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    AllowCropping = true,
                    CompressionQuality = 80,
                    MaxWidthHeight = 700
                }))
                {
                    await photoTaken(photo);
                }
            });
        }

        private async Task SaveProfilePhoto(byte[] photoAsBytes, string fileName)
        {
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(async () => await UpdateProfilePhoto(photoAsBytes, fileName), () =>
                {
                    Photo = ImageSource.FromStream(() => new MemoryStream(photoAsBytes));
                    CloneProfilePicture(photoAsBytes);

                    return Task.CompletedTask;
                });
            });
        }

        private void CloneProfilePicture(byte[] photoAsBytes)
        {
            _profilePictureBytes = new byte[photoAsBytes.Length];
            photoAsBytes.CopyTo(_profilePictureBytes, 0);
        }

        private async Task UpdateProfilePhoto(byte[] photoAsBytes, string fileName)
        {
            using (Stream photoStream = new MemoryStream(photoAsBytes))
            {
                var upoadResult = await _profileControllerService.UploadProfilePicture(photoStream, fileName);
                if (upoadResult?.FileName != null)
                {
                    await _profileAppService.UpdateProfilePicture(new UpdateProfilePictureInput
                    {
                        FileName = upoadResult.FileName
                    });
                }
            }
        }

        private async Task ShowProfilePhoto()
        {
            if (_profilePictureBytes == null)
            {
                return;
            }

            await NavigationService.ShowModalAsync<ProfilePictureView>(_profilePictureBytes);
        }

        #region Navigation Menu

        public void BuildMenuItems()
        {
            var grantedMenutems = _appContext.Configuration.Auth.GrantedPermissions;
            MenuItems = DependencyResolver.Resolve<IMenuProvider>().GetAuthorizedMenuItems(grantedMenutems);
            RaisePropertyChanged(() => MenuItemsCount);
        }

        private ObservableRangeCollection<NavigationMenuItem> _menuItems;
        public ObservableRangeCollection<NavigationMenuItem> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                RaisePropertyChanged(() => MenuItems);
            }
        }


        private NavigationMenuItem _selectedMenuItem;
        public NavigationMenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem = value;
                ClearSelectedMenu();
                if (_selectedMenuItem != null)
                {
                    AsyncRunner.Run(NavigateToMenuAsync(_selectedMenuItem));
                }

                RaisePropertyChanged(() => SelectedMenuItem);
            }
        }

        public bool ShowMasterPage
        {
            get => _showMasterPage;
            set
            {
                _showMasterPage = value;
                RaisePropertyChanged(() => ShowMasterPage);
            }
        }

        private void ClearSelectedMenu()
        {
            MenuItems.ForEach(m => m.IsSelected = false);
        }

        private async Task NavigateToMenuAsync(NavigationMenuItem newNavigationMenu)
        {
            ShowMasterPage = false;
            SelectedMenuItem.IsSelected = true;
            await NavigationService.SetDetailPageAsync(newNavigationMenu.ViewType, _selectedMenuItem.NavigationParameter);
        }

        private async Task Logout()
        {
            _accessTokenManager.Logout();
            _appContext.LoginInfo = null;
            _authenticateModel.Password = null;
            _selectedMenuItem = null;
            await NavigationService.SetMainPage<LoginView>("From-Logout", clearNavigationHistory: true);
        }

        #endregion
    }
}
