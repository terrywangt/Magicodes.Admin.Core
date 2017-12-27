using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Magicodes.Admin.ApiClient;
using Magicodes.Admin.Core.Dependency;
using Magicodes.Admin.ViewModels.Base;
using Magicodes.Admin.Views;
using Xamarin.Forms;

namespace Magicodes.Admin.Services.Navigation
{
    public class NavigationService : INavigationService, ISingletonDependency
    {
        private readonly IAccessTokenManager _accessTokenManager;

        private static Page MainPage
        {
            get => Application.Current.MainPage;
            set => Application.Current.MainPage = value;
        }

        public NavigationService(IAccessTokenManager accessTokenManager)
        {
            _accessTokenManager = accessTokenManager;
        }

        public async Task InitializeAsync()
        {
            if (_accessTokenManager.IsUserLoggedIn)
            {
                await SetMainPage<MainView>();
            }
            else
            {
                await SetMainPage<LoginView>(clearNavigationHistory: true);
            }
        }

        public async Task SetMainPage<TView>(object navigationParameter = null, bool clearNavigationHistory = false) where TView : IXamarinView
        {
            var page = await CreatePage(typeof(TView), navigationParameter);

            if (MainPage is NavigationPage navigationPage)
            {
                if (clearNavigationHistory)
                {
                    MainPage = new NavigationPage(page); //TODO: Can clear in a different way? And release views..?
                }
                else
                {
                    await navigationPage.Navigation.PushAsync(page);
                }
            }
            else
            {
                MainPage = new NavigationPage(page);
            }
        }

        public async Task SetDetailPageAsync(Type viewType, object navigationParameter = null, bool pushToStack = false)
        {
            var currentPage = MainPage;

            if (currentPage is NavigationPage)
            {
                currentPage = currentPage.Navigation.NavigationStack.Last();
            }

            if (!(currentPage is MasterDetailPage masterDetailPage))
            {
                throw new Exception($"Current MainPage is not a {typeof(MasterDetailPage)}!");
            }

            var newPage = await CreatePage(viewType, navigationParameter);

            if (pushToStack && masterDetailPage.Detail is NavigationPage navPage)
            {
                await navPage.PushAsync(newPage);
            }
            else
            {
                masterDetailPage.Detail = new NavigationPage(newPage);
            }
        }

        public async Task<Page> GoBackAsync()
        {
            if (MainPage is NavigationPage navigationPage)
            {
                var currentPage = navigationPage.Navigation.NavigationStack.Last();
                if (currentPage is MasterDetailPage masterDetail && masterDetail.Detail is NavigationPage detailNavigationPage)
                {
                    if (detailNavigationPage.Navigation.NavigationStack.Count > 1)
                    {
                        return await detailNavigationPage.Navigation.PopAsync();
                    }
                }

                return await navigationPage.Navigation.PopAsync();
            }
            else if (MainPage is MasterDetailPage masterDetail && masterDetail.Detail is NavigationPage detailNavigationPage)
            {
                return await detailNavigationPage.Navigation.PopAsync();
            }

            return null;
        }

        public async Task ShowModalAsync(Page page)
        {
            if (MainPage is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PushModalAsync(page);
            }
        }

        public async Task ShowModalAsync<TView>(object navigationParameter) where TView : IXamarinView
        {
            var page = await CreatePage(typeof(TView), navigationParameter);
            await ShowModalAsync(page);
        }

        public async Task<Page> CloseModalAsync()
        {
            if (MainPage is NavigationPage navigationPage)
            {
                return await navigationPage.Navigation.PopModalAsync();
            }

            return null;
        }

        private static async Task<Page> CreatePage(Type viewType, object navigationParameter)
        {
            var view = (Page)DependencyResolver.Resolve(viewType);
            if (!(view.BindingContext is XamarinViewModel viewModel))
            {
                throw new Exception($"BindingContext of views must inherit {nameof(XamarinViewModel)}. Given view's BindingContext is not like that: {viewType}");
            }

            await viewModel.InitializeAsync(navigationParameter);
            return view;
        }
    }
}