using Abp.Web.Models;
using Acr.UserDialogs;
using Flurl.Http;
using Magicodes.Admin.Extensions;
using Magicodes.Admin.Localization;
using Newtonsoft.Json;

namespace Magicodes.Admin.Core.Threading
{
    public class AbpExceptionHandler
    {
        public static bool HandleIfAbpResponse(FlurlHttpException httpException)
        {
            var errorResponse = httpException.Call.ErrorResponseBody;
            if (errorResponse == null)
            {
                return false;
            }

            if (!errorResponse.Contains("__abp"))
            {
                return false;
            }

            var ajaxResponse = JsonConvert.DeserializeObject<AjaxResponse>(errorResponse);

            if (ajaxResponse?.Error == null)
            {
                return false;
            }

            UserDialogs.Instance.HideLoading();

            if (string.IsNullOrEmpty(ajaxResponse.Error.Details))
            {
                UserDialogs.Instance.Alert(ajaxResponse.Error.GetConsolidatedMessage(), L.Localize("Error"));
            }
            else
            {
                UserDialogs.Instance.Alert(ajaxResponse.Error.Details, ajaxResponse.Error.GetConsolidatedMessage());
            }

            return true;
        }
    }
}