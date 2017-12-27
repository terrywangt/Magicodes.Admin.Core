using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Authorization.Users.Profile;
using Magicodes.Admin.Commands;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Extensions;
using Magicodes.Admin.Models.Users;
using Magicodes.Admin.ViewModels.Base;
using Xamarin.Forms;
using MvvmHelpers;
using Magicodes.Admin.Localization;
using Magicodes.Admin.UI.Assets;
using Magicodes.Admin.Views;

namespace Magicodes.Admin.ViewModels
{
    public class UsersViewModel : XamarinViewModel
    {
        private readonly IUserAppService _userAppService;
        private readonly IProfileAppService _profileService;
        private readonly GetUsersInput _input;

        private UserListModel _selectedUser;
        private int _totalUsersCount;
        private int _currentPage;
        private bool _isInitialized;

        public ICommand RefreshUsersCommand => HttpRequestCommand.Create(RefreshUsersAsync);
        public ICommand PageAppearingCommand => HttpRequestCommand.Create(PageAppearingAsync);
        public ICommand CreateNewUserCommand => HttpRequestCommand.Create(CreateNewUserAsync);

        public ObservableRangeCollection<UserListModel> Users { get; set; }

        public UserListModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                RaisePropertyChanged(() => SelectedUser);
                if (_selectedUser != null)
                {
                    AsyncRunner.Run(GotoUserDetailsAsync(_selectedUser));
                }
            }
        }

        public string FilterText
        {
            get => _input.Filter;
            set
            {
                _input.Filter = value;
                AsyncRunner.Run(SearchWithDelayAsync(_input.Filter));
            }
        }

        public string Title => L.LocalizeWithParantheses("Users", _totalUsersCount);

        public UsersViewModel(IUserAppService userAppService, IProfileAppService profileService)
        {
            _userAppService = userAppService;
            _profileService = profileService;

            Users = new ObservableRangeCollection<UserListModel>();

            WatchUserListChange();

            _input = new GetUsersInput
            {
                Filter = "",
                MaxResultCount = PageDefaults.PageSize,
                SkipCount = 0
            };
        }

        private async Task PageAppearingAsync()
        {
            SelectedUser = null;

            if (_isInitialized)
            {
                return;
            }

            await RefreshUsersAsync();

            _isInitialized = true;
        }

        private async Task RefreshUsersAsync()
        {
            Users.Clear();
            _input.SkipCount = 0;
            await FetchUsersAsync();
        }

        public async Task CreateNewUserAsync()
        {
            await GotoUserDetailsAsync(null);
        }

        private async Task FetchUsersAsync()
        {
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(async () => await _userAppService.GetUsers(_input), result =>
                {
                    var users = ObjectMapper.Map<List<UserListModel>>(result.Items);

                    foreach (var user in users)
                    {
                        Users.Add(user);

                        AsyncRunner.Run(SetUserImageSourceAsync(user));
                    }

                    _totalUsersCount = result.TotalCount;
                    RaisePropertyChanged(() => Title);

                    return Task.CompletedTask;
                });
            });
        }

        private void WatchUserListChange()
        {
            MessagingCenter.Instance.SubscribeSafe<UserDetailsViewModel>(this, MessagingCenterKeys.UserListChanged, async sender =>
            {
                //refresh list whenever the "UserListChanged" message is sent
                await RefreshUsersAsync();
            });
        }

        private async Task SetUserImageSourceAsync(UserListModel userListModel)
        {
            if (userListModel.Photo != null)
            {
                return;
            }

            if (!userListModel.ProfilePictureId.HasValue)
            {
                userListModel.Photo = ImageSource.FromResource(AssetsHelper.ProfileImagePlaceholderNamespace);
                return;
            }

            var photo = await FetchProfilePictureOrNullAsync(userListModel.ProfilePictureId.Value);
            if (photo == null)
            {
                userListModel.Photo = ImageSource.FromResource(AssetsHelper.ProfileImagePlaceholderNamespace);
                return;
            }

            userListModel.Photo = ImageSource.FromStream(() => new MemoryStream(photo));
        }

        private async Task<byte[]> FetchProfilePictureOrNullAsync(Guid pictureId)
        {
            var output = await _profileService.GetProfilePictureById(pictureId);

            return string.IsNullOrEmpty(output.ProfilePicture)
                ? null
                : Convert.FromBase64String(output.ProfilePicture);
        }

        private async Task GotoUserDetailsAsync(UserListModel user)
        {
            await NavigationService.SetDetailPageAsync(typeof(UserDetailsView), user, pushToStack: true);
        }

        public async Task LoadMoreUserIfNeedsAsync(UserListModel shownItem)
        {
            if (IsBusy)
            {
                return;
            }

            if (shownItem != Users.Last())
            {
                return;
            }

            if (Users.Count >= _totalUsersCount)
            {
                return;
            }

            _input.SkipCount = PageDefaults.PageSize * ++_currentPage;
            await FetchUsersAsync();
        }

        private async Task SearchWithDelayAsync(string filterText)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (!string.IsNullOrEmpty(filterText))
            {
                await Task.Delay(PageDefaults.SearchDelayMilliseconds);

                if (filterText != _input.Filter)
                {
                    return;
                }
            }

            await RefreshUsersAsync();
        }
    }
}
