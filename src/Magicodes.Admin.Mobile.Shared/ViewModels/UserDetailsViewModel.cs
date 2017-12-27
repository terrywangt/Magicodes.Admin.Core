using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Abp.Application.Services.Dto;
using Acr.UserDialogs;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Controls;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Models.Users;
using Magicodes.Admin.Organizations.Dto;
using Magicodes.Admin.Services.Permission;
using Magicodes.Admin.UI;
using Magicodes.Admin.UI.Assets;
using Magicodes.Admin.Validations;
using Magicodes.Admin.ViewModels.Base;
using Xamarin.Forms;

namespace Magicodes.Admin.ViewModels
{
    public class UserDetailsViewModel : XamarinViewModel
    {
        private readonly IUserAppService _userAppService;
        private readonly IPermissionService _permissionService;

        private UserEditOrCreateModel _model;
        private UserCreateOrUpdateModel _userInput;
        private int _roleListViewHeight;
        private int _organizationUnitListViewHeight;
        private string _pageTitle;
        private bool _isNewUser;
        private bool _isDeleteButtonVisible;
        private bool _isUnlockButtonVisible;
        private bool _showRoles;
        private bool _showOrganizationUnits;
        private bool _setRandomPassword;

        public ICommand SaveUserCommand => AsyncCommand.Create(SaveUser);
        public ICommand UnlockUserCommand => AsyncCommand.Create(UnlockUser);
        public ICommand DeleteUserCommand => AsyncCommand.Create(DeleteUser);

        public bool ShowRoles
        {
            get => _showRoles;
            set
            {
                _showRoles = value;
                RaisePropertyChanged(() => ShowRoles);
            }
        }

        public bool ShowOrganizationUnits
        {
            get => _showOrganizationUnits;
            set
            {
                _showOrganizationUnits = value;
                RaisePropertyChanged(() => ShowOrganizationUnits);
            }
        }

        public int RoleListViewHeight
        {
            get => _roleListViewHeight;
            set
            {
                _roleListViewHeight = value;
                ShowRoles = _roleListViewHeight > 0;
                RaisePropertyChanged(() => RoleListViewHeight);
            }
        }

        public int OrganizationUnitListViewHeight
        {
            get => _organizationUnitListViewHeight;
            set
            {
                _organizationUnitListViewHeight = value;
                ShowOrganizationUnits = _organizationUnitListViewHeight > 0;
                RaisePropertyChanged(() => OrganizationUnitListViewHeight);
            }
        }

        public UserCreateOrUpdateModel UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                RaisePropertyChanged(() => UserInput);
            }
        }

        public UserEditOrCreateModel Model
        {
            get => _model;
            set
            {
                _model = value;
                SetRoleListViewHeight(_model.Roles);
                SetOrganizationUnitListViewHeight(_model.AllOrganizationUnits);

                RaisePropertyChanged(() => Model);
            }
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        public bool IsDeleteButtonVisible
        {
            get => _isDeleteButtonVisible;
            set
            {
                _isDeleteButtonVisible = value;
                RaisePropertyChanged(() => IsDeleteButtonVisible);
            }
        }

        public bool IsUnlockButtonVisible
        {
            get => _isUnlockButtonVisible;
            set
            {
                _isUnlockButtonVisible = value;
                RaisePropertyChanged(() => IsUnlockButtonVisible);
            }
        }

        public bool SetRandomPassword
        {
            get => _setRandomPassword;
            set
            {
                _setRandomPassword = value;
                UserInput.SetRandomPassword = value;
                RaisePropertyChanged(() => SetRandomPassword);
            }
        }

        public bool IsNewUser
        {
            get => _isNewUser;
            set
            {
                _isNewUser = value;
                IsDeleteButtonVisible = !_isNewUser && _permissionService.HasPermission(PermissionKey.UserDelete);
                IsUnlockButtonVisible = !_isNewUser && _permissionService.HasPermission(PermissionKey.UserEdit);
                PageTitle = _isNewUser ? L.Localize("CreatingNewUser") : L.Localize("EditUser");
                RaisePropertyChanged(() => IsNewUser);
            }
        }


        public UserDetailsViewModel(IUserAppService userAppService, IPermissionService permissionService)
        {
            _userAppService = userAppService;
            _permissionService = permissionService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await SetBusyAsync(async () =>
            {
                _userInput = new UserCreateOrUpdateModel();
                IsNewUser = navigationData == null;
                UserInput.SetRandomPassword = IsNewUser;
                SetRandomPassword = IsNewUser;
                UserInput.SendActivationEmail = IsNewUser;
                RaisePropertyChanged(() => UserInput);

                var userListModel = (UserListModel)navigationData;
                var user = await _userAppService.GetUserForEdit(new NullableIdDto<long>(userListModel?.Id));
                Model = ObjectMapper.Map<UserEditOrCreateModel>(user);
                Model.OrganizationUnits = ObjectMapper.Map<List<OrganizationUnitModel>>(user.AllOrganizationUnits);

                if (IsNewUser)
                {
                    Model.Photo = ImageSource.FromResource(AssetsHelper.ProfileImagePlaceholderNamespace);
                    Model.User = new UserEditDto
                    {
                        IsActive = true,
                        IsLockoutEnabled = true,
                        ShouldChangePasswordOnNextLogin = true,
                    };
                }
                else
                {
                    Model.Photo = userListModel.Photo;
                    Model.LastLoginTime = userListModel.LastLoginTime;
                    Model.CreationTime = userListModel.CreationTime;
                    Model.IsEmailConfirmed = userListModel.IsEmailConfirmed;
                }
            });
        }

        private void SetRoleListViewHeight(UserRoleDto[] roles)
        {
            if (roles == null || !roles.Any())
            {
                RoleListViewHeight = 0;
            }
            else
            {
                RoleListViewHeight = ControlSetting.ListViewLineHeight * roles.Length;
            }
        }

        private void SetOrganizationUnitListViewHeight(IReadOnlyCollection<OrganizationUnitDto> organizationUnits)
        {
            if (organizationUnits == null || !organizationUnits.Any())
            {
                OrganizationUnitListViewHeight = 0;
            }
            else
            {
                OrganizationUnitListViewHeight = ControlSetting.ListViewLineHeight * organizationUnits.Count;
            }
        }

        private bool ValidateInput()
        {
            //Since DataAnnotationsValidator doesn't work for nested object validation. We manually do validation for each nested object.
            var userInputValidationResult = DataAnnotationsValidator.Validate(UserInput);
            var userValidationResult = DataAnnotationsValidator.Validate(UserInput.User);

            if (userInputValidationResult.IsValid && userValidationResult.IsValid)
            {
                return true;
            }

            UserDialogHelper.Warn(userInputValidationResult
                .AddRange(userValidationResult.ValidationErrors)
                .ConsolidatedMessage);

            return false;
        }

        private async Task SaveUser()
        {
            UserInput.User = Model.User;
            UserInput.AssignedRoleNames = Model.Roles.Where(x => x.IsAssigned).Select(x => x.RoleName).ToArray();
            UserInput.OrganizationUnits = Model.OrganizationUnits.Select(x => x.Id).ToList();

            if (!ValidateInput())
            {
                return;
            }

            await SetBusyAsync(async () =>
            {
                await _userAppService.CreateOrUpdateUser(UserInput);
                MessagingCenter.Send(this, MessagingCenterKeys.UserListChanged);
                await NavigationService.GoBackAsync();
            }, L.Localize("SavingWithThreeDot"));
        }

        private async Task UnlockUser()
        {
            if (!Model.User.Id.HasValue)
            {
                return;
            }

            await SetBusyAsync(async () =>
            {
                await _userAppService.UnlockUser(new EntityDto<long>(Model.User.Id.Value));
                MessagingCenter.Send(this, MessagingCenterKeys.UserListChanged);
                await NavigationService.GoBackAsync();
            });
        }

        private async Task DeleteUser()
        {
            var accepted = await UserDialogs.Instance.ConfirmAsync(L.Localize("UserDeleteWarningMessage", Model.User.UserName),
                L.Localize("AreYouSure"), L.Localize("Ok"), L.Localize("Cancel"));

            if (!accepted)
            {
                return;
            }

            await SetBusyAsync(async () =>
            {
                if (!Model.User.Id.HasValue)
                {
                    return;
                }

                await _userAppService.DeleteUser(new EntityDto<long>(Model.User.Id.Value));
                MessagingCenter.Send(this, MessagingCenterKeys.UserListChanged);
                await NavigationService.GoBackAsync();
            });
        }
    }

    
}
