using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Abp.Application.Services.Dto;
using Abp.Runtime.Security;
using Acr.UserDialogs;
using MvvmHelpers;
using Magicodes.Admin.Common;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.Extensions;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Models.Tenants;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.Services.Permission;
using Magicodes.Admin.UI;
using Magicodes.Admin.ViewModels.Base;
using Xamarin.Forms;
using Magicodes.Admin.Validations;

namespace Magicodes.Admin.ViewModels
{
    public class TenantDetailsViewModel : XamarinViewModel
    {
        private const string NotAssignedValue = "0";
        private readonly ITenantAppService _tenantAppService;
        private readonly ICommonLookupAppService _commonLookupAppService;
        private readonly IPermissionService _permissionService;

        private bool _isSubscriptionFieldVisible;
        private bool _isUnlimitedTimeSubscription;
        private bool _isInitialized;
        private TenantListModel _model;
        private ObservableRangeCollection<SubscribableEditionComboboxItemDto> _editions;
        private SubscribableEditionComboboxItemDto _selectedEdition;
        private bool _isNewTenant;
        private bool _isDeleteButtonVisible;
        private string _pageTitle;
        private bool _useHostDatabase;
        private string _adminEmailAddress;
        private string _adminPassword;
        private string _adminPasswordRepeat;
        private bool _isSetRandomPassword;

        public ICommand SaveTenantCommand => AsyncCommand.Create(SaveTenantAsync);
        public ICommand DeleteTenantCommand => AsyncCommand.Create(DeleteTenantAsync);

        public DateTime Today => DateTime.Now;
        public TenantListModel Model
        {
            get => _model;
            set
            {
                _model = value;
                IsUnlimitedTimeSubscription = _model?.SubscriptionEndDateUtc == null;
                RaisePropertyChanged(() => Model);
            }
        }

        public ObservableRangeCollection<SubscribableEditionComboboxItemDto> Editions
        {
            get => _editions;
            set
            {
                _editions = value;
                RaisePropertyChanged(() => Editions);
            }
        }

        public bool IsUnlimitedTimeSubscription
        {
            get => _isUnlimitedTimeSubscription;
            set
            {
                _isUnlimitedTimeSubscription = value;
                RaisePropertyChanged(() => IsUnlimitedTimeSubscription);
            }
        }

        public bool IsNewTenant
        {
            get => _isNewTenant;
            set
            {
                _isNewTenant = value;
                IsDeleteButtonVisible = !_isNewTenant && _permissionService.HasPermission(PermissionKey.TenantDelete);
                PageTitle = _isNewTenant ? L.Localize("CreatingNewTenant") : L.Localize("EditTenant");
                RaisePropertyChanged(() => IsNewTenant);
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

        public string AdminEmailAddress
        {
            get => _adminEmailAddress;
            set
            {
                _adminEmailAddress = value;
                RaisePropertyChanged(() => AdminEmailAddress);
            }
        }

        public string AdminPassword
        {
            get => _adminPassword;
            set
            {
                _adminPassword = value;
                RaisePropertyChanged(() => AdminPassword);
            }
        }

        public string AdminPasswordRepeat
        {
            get => _adminPasswordRepeat;
            set
            {
                _adminPasswordRepeat = value;
                RaisePropertyChanged(() => AdminPasswordRepeat);
            }
        }

        public bool UseHostDatabase
        {
            get => _useHostDatabase;
            set
            {
                _useHostDatabase = value;
                RaisePropertyChanged(() => UseHostDatabase);
            }
        }

        public bool IsSetRandomPassword
        {
            get => _isSetRandomPassword;
            set
            {
                _isSetRandomPassword = value;
                if (_isSetRandomPassword)
                {
                    AdminPassword = null;
                    AdminPasswordRepeat = null;
                }

                RaisePropertyChanged(() => IsSetRandomPassword);
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

        public bool IsSelectedEditionFree
        {
            get
            {
                if (Model == null)
                {
                    return true;
                }

                if (!Model.EditionId.HasValue)
                {
                    return true;
                }

                if (!SelectedEdition.IsFree.HasValue)
                {
                    return true;
                }

                return SelectedEdition.IsFree.Value;
            }
        }

        public SubscribableEditionComboboxItemDto SelectedEdition
        {
            get => _selectedEdition;
            set
            {
                _selectedEdition = value;
                if (_isInitialized)
                {
                    UpdateModel();
                }

                IsSubscriptionFieldVisible = SelectedEdition != null && SelectedEdition.Value != NotAssignedValue;
                RaisePropertyChanged(() => IsSelectedEditionFree);
                RaisePropertyChanged(() => SelectedEdition);
            }
        }

        public bool IsSubscriptionFieldVisible
        {
            get => _isSubscriptionFieldVisible;
            set
            {
                _isSubscriptionFieldVisible = value;
                RaisePropertyChanged(() => IsSubscriptionFieldVisible);
            }
        }

        public TenantDetailsViewModel(ITenantAppService tenantAppService,
            ICommonLookupAppService commonLookupAppService,
            IPermissionService permissionService)
        {
            _tenantAppService = tenantAppService;
            _commonLookupAppService = commonLookupAppService;
            _permissionService = permissionService;
            _editions = new ObservableRangeCollection<SubscribableEditionComboboxItemDto>();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await SetBusyAsync(async () =>
            {
                _isInitialized = false;
                Model = (TenantListModel)navigationData;

                if (Model == null)
                {
                    InitializeNewTenant();
                }
                else
                {
                    InitializeEditTenant();
                }

                await PopulateEditionsCombobox(() =>
                {
                    SetSelectedEdition(Model.EditionId);
                    _isInitialized = true;
                });
            });
        }

        public async Task SaveTenantAsync()
        {
            if (IsNewTenant)
            {
                await CreateTenantAsync();
            }
            else
            {
                await UpdateTenantAsync();
            }
        }

        private void InitializeNewTenant()
        {
            IsNewTenant = true;
            Model = new TenantListModel
            {
                IsActive = true
            };

            UseHostDatabase = true;
            IsSetRandomPassword = true;
        }

        private void InitializeEditTenant()
        {
            IsNewTenant = false;
            IsSetRandomPassword = false;
            UseHostDatabase = string.IsNullOrEmpty(Model.ConnectionString);
            if (!string.IsNullOrEmpty(Model.ConnectionString))
            {
                Model.ConnectionString = SimpleStringCipher.Instance.Decrypt(Model.ConnectionString);
            }
        }

        private async Task UpdateTenantAsync()
        {
            var isValid = true;

            await SetBusyAsync(async () =>
                    await WebRequestExecuter.Execute(async () =>
                    {
                        var input = ObjectMapper.Map<TenantEditDto>(Model);
                        NormalizeTenantUpdateInput(input);

                        if (!ValidateInput(input))
                        {
                            isValid = false;
                            return;

                        }

                        await _tenantAppService.UpdateTenant(input);

                    }, async () =>
                    {
                        if (!isValid)
                        {
                            return;
                        }

                        await SaveCompleted();
                    })

                , L.Localize("SavingWithThreeDot")
            );
        }

        private async Task CreateTenantAsync()
        {
            var isValid = true;

            await SetBusyAsync(async () =>
                    await WebRequestExecuter.Execute(async () =>
                    {
                        var input = ObjectMapper.Map<CreateTenantInput>(Model);
                        input.AdminEmailAddress = AdminEmailAddress;
                        input.AdminPassword = AdminPassword;
                        NormalizeTenantCreateInput(input);

                        if (!ValidateInput(input))
                        {
                            isValid = false;
                            return;
                        }

                        await _tenantAppService.CreateTenant(input);

                    }, async () =>
                    {
                        if (!isValid)
                        {
                            return;
                        }

                        await SaveCompleted();
                    })

                , L.Localize("SavingWithThreeDot")
            );
        }

        private async Task SaveCompleted()
        {
            MessagingCenter.Send(this, MessagingCenterKeys.TenantListChanged);
            await NavigationService.GoBackAsync();
        }

        private bool ValidateInput(object input)
        {
            if (AdminPassword != AdminPasswordRepeat)
            {
                UserDialogHelper.Warn("PasswordsDontMatch");
                return false;
            }

            var validationResult = DataAnnotationsValidator.Validate(input);
            if (validationResult.IsValid)
            {
                return true;
            }

            UserDialogHelper.Warn(validationResult.ConsolidatedMessage);
            return false;
        }

        private async Task DeleteTenantAsync()
        {
            var accepted = await UserDialogs.Instance.ConfirmAsync(L.Localize("TenantDeleteWarningMessage", Model.TenancyName),
                L.Localize("AreYouSure"), L.Localize("Ok"), L.Localize("Cancel"));

            if (!accepted)
            {
                return;
            }

            await SetBusyAsync(async () =>
            {
                await _tenantAppService.DeleteTenant(new EntityDto(Model.Id));
                MessagingCenter.Send(this, MessagingCenterKeys.TenantListChanged);
                await NavigationService.GoBackAsync();
            });
        }

        private void NormalizeTenantUpdateInput(TenantEditDto input)
        {
            input.EditionId = NormalizeEditionId(input.EditionId);
            input.SubscriptionEndDateUtc = NormalizeSubscriptionEndDateUtc(input.SubscriptionEndDateUtc);
        }

        private void NormalizeTenantCreateInput(CreateTenantInput input)
        {
            input.EditionId = NormalizeEditionId(input.EditionId);
            input.SubscriptionEndDateUtc = NormalizeSubscriptionEndDateUtc(input.SubscriptionEndDateUtc);
        }

        private int? NormalizeEditionId(int? editionId)
        {
            return editionId.HasValue && editionId.Value == 0 ? null : editionId;
        }

        private DateTime? NormalizeSubscriptionEndDateUtc(DateTime? subscriptionEndDateUtc)
        {
            if (IsUnlimitedTimeSubscription)
            {
                return null;
            }

            return subscriptionEndDateUtc.GetEndOfDate();
        }

        private async Task PopulateEditionsCombobox(Action editionsPopulated)
        {
            var editions = await _commonLookupAppService.GetEditionsForCombobox();
            Editions.ReplaceRange(editions.Items);
            AddNotAssignedItem();
            editionsPopulated();
        }

        private void AddNotAssignedItem()
        {
            Editions.Insert(0, new SubscribableEditionComboboxItemDto(NotAssignedValue,
                string.Format("- {0} -", L.Localize("NotAssigned")), null));
        }

        private void SetSelectedEdition(int? editionId)
        {
            SelectedEdition = editionId.HasValue ?
                Editions.Single(e => e.Value == editionId.Value.ToString()) :
                Editions.Single(e => e.Value == NotAssignedValue);
        }

        private void UpdateModel()
        {
            if (SelectedEdition != null &&
                int.TryParse(SelectedEdition.Value, out var selectedEditionId))
            {
                Model.EditionId = selectedEditionId;
            }
            else
            {
                Model.EditionId = null;
            }

            Model.IsInTrialPeriod = !IsSelectedEditionFree;
        }

    }
}