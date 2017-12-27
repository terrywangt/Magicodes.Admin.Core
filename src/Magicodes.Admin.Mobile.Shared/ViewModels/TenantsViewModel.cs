using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Magicodes.Admin.Commands;
using Magicodes.Admin.Core.Threading;
using Magicodes.Admin.Extensions;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Models.Tenants;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.ViewModels.Base;
using Magicodes.Admin.Views;
using Xamarin.Forms;

namespace Magicodes.Admin.ViewModels
{
    public class TenantsViewModel : XamarinViewModel
    {
        private readonly ITenantAppService _tenantAppService;
        private int _totalTenantsCount;
        private int _currentPage;
        private string _filterText;
        private GetTenantsInput _filter;
        private ObservableRangeCollection<TenantListModel> _tenants = new ObservableRangeCollection<TenantListModel>();
        private TenantListModel _selectedTenant;
        private string _title;
        private bool _isInitialized;

        public ICommand RefreshTenantsCommand => HttpRequestCommand.Create(RefreshTenantsAsync);
        public ICommand PageAppearingCommand => HttpRequestCommand.Create(PageAppearingAsync);
        public ICommand CreateNewTenantCommand => HttpRequestCommand.Create(CreateNewTenant);


        public ObservableRangeCollection<TenantListModel> Tenants
        {
            get => _tenants;
            set
            {
                _tenants = value;
                RaisePropertyChanged(() => Tenants);
            }
        }

        public TenantListModel SelectedTenant
        {
            get => _selectedTenant;
            set
            {
                _selectedTenant = value;
                RaisePropertyChanged(() => SelectedTenant);
                if (_selectedTenant != null)
                {
                    AsyncRunner.Run(GotoTenantDetailsAsync(_selectedTenant));
                }
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                AsyncRunner.Run(SearchWithDelayAsync(_filterText));
            }
        }

        public int TotalTenantsCount
        {
            get => _totalTenantsCount;
            set
            {
                _totalTenantsCount = value;
                Title = L.LocalizeWithParantheses("Tenants", _totalTenantsCount);
                RaisePropertyChanged(() => TotalTenantsCount);
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public TenantsViewModel(ITenantAppService tenantAppService)
        {
            _tenantAppService = tenantAppService;
            _title = L.Localize("Tenants");
            WatchTenantListChange();
        }
         
        public async Task PageAppearingAsync()
        {
            SelectedTenant = null;
            if (_isInitialized)
            {
                return;
            }

            await FetchDataAsync(overwrite: true);
            _isInitialized = true;
        }

        public async Task FetchDataAsync(string filterText = null, bool overwrite = false, int skipCount = 0)
        {
            await SetBusyAsync(async () =>
            {
                _filter = new GetTenantsInput
                {
                    Filter = filterText,
                    MaxResultCount = PageDefaults.PageSize,
                    SkipCount = skipCount
                };

                await FetchTenantsAsync(overwrite);
            });
        }

        public async Task RefreshTenantsAsync()
        {
            await SetBusyAsync(async () =>
            {
                var originalSkipCount = _filter.SkipCount;
                var originalMaxResultCount = _filter.MaxResultCount;

                _filter.SkipCount = 0;
                _filter.MaxResultCount = Tenants.Count;

                await FetchTenantsAsync(true);

                _filter.SkipCount = originalSkipCount;
                _filter.MaxResultCount = originalMaxResultCount;
            });
        }

        public async Task CreateNewTenant()
        {
            await GotoTenantDetailsAsync(null);
        }

        public async Task LoadMoreTenantsIfNeedsAsync(TenantListModel shownItem)
        {
            if (IsBusy)
            {
                return;
            }

            if (shownItem != Tenants.Last())
            {
                return;
            }

            if (Tenants.Count >= TotalTenantsCount)
            {
                return;
            }

            await FetchDataAsync(null, false, PageDefaults.PageSize * ++_currentPage);
        }

        private async Task SearchWithDelayAsync(string filterText)
        {
            if (!string.IsNullOrEmpty(filterText))
            {
                await Task.Delay(PageDefaults.SearchDelayMilliseconds);
                if (filterText != _filterText)
                {
                    return;
                }
            }

            await FetchDataAsync(filterText, true);
        }

        private async Task FetchTenantsAsync(bool overwrite = false)
        {
            var result = await _tenantAppService.GetTenants(_filter);
            if (overwrite)
            {
                Tenants.ReplaceRange(ObjectMapper.Map<List<TenantListModel>>(result.Items));
            }
            else
            {
                Tenants.AddRange(ObjectMapper.Map<List<TenantListModel>>(result.Items));
            }

            TotalTenantsCount = result.TotalCount;
        }

        private void WatchTenantListChange()
        {
            MessagingCenter.Instance.SubscribeSafe<TenantDetailsViewModel>(this,
                MessagingCenterKeys.TenantListChanged, async sender =>
            {
                await RefreshTenantsAsync();
            });
        }

        private async Task GotoTenantDetailsAsync(TenantListModel tenant)
        {
            await NavigationService.SetDetailPageAsync(typeof(TenantDetailsView), tenant, pushToStack: true);
        }

    }
}
