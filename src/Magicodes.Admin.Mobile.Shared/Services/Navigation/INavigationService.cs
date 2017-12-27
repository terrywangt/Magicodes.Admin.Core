using System;
using System.Threading.Tasks;
using Magicodes.Admin.Views;
using Xamarin.Forms;

namespace Magicodes.Admin.Services.Navigation
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task SetMainPage<TView>(object navigationParameter = null, bool clearNavigationHistory = false) 
            where TView : IXamarinView;

        Task SetDetailPageAsync(Type viewType, object navigationParameter = null, bool pushToStack = false);

        Task<Page> GoBackAsync();

        Task ShowModalAsync(Page page);

        Task ShowModalAsync<TView>(object navigationParameter) where TView : IXamarinView;

        Task<Page> CloseModalAsync();
    }
}