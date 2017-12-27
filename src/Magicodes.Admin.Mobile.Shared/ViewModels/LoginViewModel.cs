using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Abp.MultiTenancy;
using Acr.UserDialogs;
using Magicodes.Admin.ApiClient;
using Magicodes.Admin.ApiClient.Models;
using Magicodes.Admin.Authorization.Accounts;
using Magicodes.Admin.Authorization.Accounts.Dto;
using Magicodes.Admin.Commands;
using Magicodes.Admin.Core.DataStorage;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Localization.Resources;
using Magicodes.Admin.Sessions;
using Magicodes.Admin.Sessions.Dto;
using Magicodes.Admin.ViewModels.Base;
using Magicodes.Admin.Views;

namespace Magicodes.Admin.ViewModels
{
    public class LoginViewModel : XamarinViewModel
    {
        public ICommand LoginUserCommand => HttpRequestCommand.Create(LoginUserAsync);
        public ICommand ChangeTenantCommand => HttpRequestCommand.Create(ChangeTenantAsync);
        public ICommand PageAppearingCommand => HttpRequestCommand.Create(PageAppearingAsync);

        public string CurrentTenancyNameOrDefault => _applicationContext.CurrentTenant != null
            ? _applicationContext.CurrentTenant.TenancyName
            : L.Localize("NotSelected");

        private readonly AbpAuthenticateModel _authenticateModel;
        private readonly AccessTokenManager _accessTokenManager;
        private readonly IAccountAppService _accountAppService;
        private readonly IApplicationContext _applicationContext;
        private readonly ISessionAppService _sessionAppService;
        private readonly IDataStorageManager _dataStorageManager;
        private string _userName;
        private string _password;
        private bool _isLoginEnabled;
        private string _tenancyName;
        private string _navigationData;
        private bool _isAutoLoggingIn;

        public LoginViewModel(AbpAuthenticateModel authenticateModel,
            AccessTokenManager accessTokenManager,
            IAccountAppService accountAppService,
            IApplicationContext applicationContext,
            ISessionAppService sessionAppService,
            IDataStorageManager dataStorageManager)
        {
            _authenticateModel = authenticateModel;
            _accessTokenManager = accessTokenManager;
            _accountAppService = accountAppService;
            _applicationContext = applicationContext;
            _sessionAppService = sessionAppService;
            _dataStorageManager = dataStorageManager;
        }

        private bool IsFromLogout()
        {
            return _navigationData == "From-Logout";
        }

        private void PopulateCredentialsFromStorage()
        {
            UserName = _dataStorageManager.Retrieve(DataStorageKey.Username, "");
            TenancyName = _dataStorageManager.Retrieve(DataStorageKey.TenancyName, "");
            var tenantId = _dataStorageManager.Retrieve<int?>(DataStorageKey.TenantId, null);

            if (tenantId == null)
            {
                _applicationContext.SetAsHost();
            }
            else
            {
                _applicationContext.SetAsTenant(TenancyName, tenantId.Value);
            }

            SetPassword();
            RaisePropertyChanged(() => CurrentTenancyNameOrDefault);
        }

        private async Task PageAppearingAsync()
        {
            PopulateCredentialsFromStorage();
            await AutoLoginIfRequired();
        }

        public override Task InitializeAsync(object navigationData)
        {
            _navigationData = (string)navigationData;
            return Task.CompletedTask;
        }

        public string TenancyName
        {
            get => _tenancyName;
            set
            {
                _tenancyName = value;
                RaisePropertyChanged(() => TenancyName);
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                SetLoginButtonEnabled();
                RaisePropertyChanged(() => UserName);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                SetLoginButtonEnabled();
                RaisePropertyChanged(() => Password);
            }
        }

        public void SetLoginButtonEnabled()
        {
            IsLoginEnabled = !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }

        public bool IsLoginEnabled
        {
            get => _isLoginEnabled;
            set
            {
                _isLoginEnabled = value;
                RaisePropertyChanged(() => IsLoginEnabled);
            }
        }

        public bool IsAutoLoggingIn
        {
            get => _isAutoLoggingIn;
            set
            {
                _isAutoLoggingIn = value;
                RaisePropertyChanged(() => IsAutoLoggingIn);
            }
        }

        private async Task LoginUserAsync()
        {
            await LoginUserAsync(AuthenticateSucceed);
        }

        private void SetPassword()
        {
            if (IsFromLogout())
            {
                Password = null;
                _dataStorageManager.RemoveIfExists(DataStorageKey.Password);
            }
            else
            {
                Password = _dataStorageManager.Retrieve(DataStorageKey.Password, "", true);
            }
        }

        private async Task AutoLoginIfRequired()
        {
            if (Password == null)
            {
                return;
            }

            IsAutoLoggingIn = true;
            await LoginUserAsync();
        }

        private async Task ChangeTenantAsync()
        {
            var promptResult = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                Message = L.Localize("LeaveEmptyToSwitchToHost"),
                Text = _applicationContext.CurrentTenant?.TenancyName ?? "",
                OkText = L.Localize("Ok"),
                CancelText = L.Localize("Cancel"),
                Title = L.Localize("ChangeTenant"),
                Placeholder = L.LocalizeWithThreeDots("TenancyName"),
                MaxLength = AbpTenantBase.MaxTenancyNameLength
            });

            if (!promptResult.Ok)
            {
                return;
            }

            if (string.IsNullOrEmpty(promptResult.Text))
            {
                _applicationContext.SetAsHost();
                ApiUrlConfig.ResetBaseUrl();
                RaisePropertyChanged(() => CurrentTenancyNameOrDefault);
            }
            else
            {
                await SetTenantAsync(promptResult.Text);
            }
        }

        private async Task SetTenantAsync(string tenancyName)
        {
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                    async () => await _accountAppService.IsTenantAvailable(
                        new IsTenantAvailableInput { TenancyName = tenancyName }),
                    result => IsTenantAvailableExecuted(result, tenancyName)
                );
            });
        }

        private async Task IsTenantAvailableExecuted(IsTenantAvailableOutput result, string tenancyName)
        {
            var tenantAvailableResult = result;

            switch (tenantAvailableResult.State)
            {
                case TenantAvailabilityState.Available:
                    _applicationContext.SetAsTenant(tenancyName, tenantAvailableResult.TenantId.Value);
                    ApiUrlConfig.ChangeBaseUrl(tenantAvailableResult.ServerRootAddress);
                    RaisePropertyChanged(() => CurrentTenancyNameOrDefault);
                    break;
                case TenantAvailabilityState.InActive:
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync(L.Localize("TenantIsNotActive", tenancyName));
                    break;
                case TenantAvailabilityState.NotFound:
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync(L.Localize("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task NavigateToMainPage()
        {
            await NavigationService.SetMainPage<MainView>(clearNavigationHistory: true);
        }

        private async Task SetCurrentUserInfoAsync()
        {
            await WebRequestExecuter.Execute(async () =>
                await _sessionAppService.GetCurrentLoginInformations(), GetCurrentUserInfoExecuted);
        }

        private Task GetCurrentUserInfoExecuted(GetCurrentLoginInformationsOutput result)
        {
            _applicationContext.LoginInfo = result;
            return Task.CompletedTask;
        }

        private async Task LoginUserAsync(Func<AbpAuthenticateResultModel, Task> successCallback)
        {
            _authenticateModel.UserNameOrEmailAddress = _userName;
            _authenticateModel.Password = _password;

            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(_accessTokenManager.LoginAsync, successCallback, ex =>
                {
                    IsAutoLoggingIn = false;
                    return Task.CompletedTask;
                });
            }, LocalTranslation.Authenticating);
        }

        private async Task AuthenticateSucceed(AbpAuthenticateResultModel result)
        {
            if (result.ShouldResetPassword)
            {
                await UserDialogs.Instance.AlertAsync(L.Localize("ChangePasswordToLogin"), L.Localize("LoginFailed"), L.Localize("Ok"));
                return;
            }

            if (result.RequiresTwoFactorVerification)
            {
                await UserDialogs.Instance.AlertAsync(L.Localize("TwoFactorLoginIsNotSupported"), L.Localize("TwoFactorLogin"), L.Localize("Ok"));
                return;
            }

            await SaveCredentialsAsync();
            await SetCurrentUserInfoAsync();
            await UserConfigurationManager.GetAsync();
            await NavigateToMainPage();
        }

        private async Task SaveCredentialsAsync()
        {
            await _dataStorageManager.StoreAsync(DataStorageKey.Username, _authenticateModel.UserNameOrEmailAddress);
            await _dataStorageManager.StoreAsync(DataStorageKey.TenancyName, _applicationContext.CurrentTenant?.TenancyName);
            await _dataStorageManager.StoreAsync(DataStorageKey.TenantId, _applicationContext.CurrentTenant?.TenantId);
            await _dataStorageManager.StoreAsync(DataStorageKey.Password, _authenticateModel.Password, true);
        }
    }
}
