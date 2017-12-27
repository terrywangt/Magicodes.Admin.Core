using System;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Abp.UI;
using Acr.UserDialogs;
using Flurl.Http;
using Magicodes.Admin.Extensions;
using Magicodes.Admin.Localization.Resources;
using Plugin.Connectivity;
using Magicodes.Admin.Localization;

namespace Magicodes.Admin.Core.Threading
{
    public static class WebRequestExecuter
    {
        public static async Task Execute<TResult>(
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback = null,
            Action finallyCallback = null)
        {
            if (successCallback == null)
            {
                successCallback = _ => Task.CompletedTask;
            }

            if (failCallback == null)
            {
                failCallback = _ => Task.CompletedTask;
            }

            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    UserDialogs.Instance.HideLoading();

                    var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.NoInternet,
                        LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

                    if (accepted)
                    {
                        await Execute(func, successCallback, failCallback);
                    }
                    else
                    {
                        await failCallback(new System.Exception(LocalTranslation.NoInternet));
                    }
                }
                else
                {
                    await successCallback(await func());
                }
            }
            catch (System.Exception exception)
            {
                await HandleException(exception, func, successCallback, failCallback);
            }
            finally
            {
                finallyCallback?.Invoke();
            }
        }

        public static async Task Execute(
            Func<Task> func,
            Func<Task> successCallback = null,
            Func<System.Exception, Task> failCallback = null,
            Action finallyCallback = null)
        {
            if (successCallback == null)
            {
                successCallback = () => Task.CompletedTask;
            }

            if (failCallback == null)
            {
                failCallback = _ => Task.CompletedTask;
            }

            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    UserDialogs.Instance.HideLoading();

                    var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.NoInternet,
                        LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

                    if (accepted)
                    {
                        await Execute(func, successCallback, failCallback);
                    }
                    else
                    {
                        await failCallback(new System.Exception(LocalTranslation.NoInternet));
                    }
                }
                else
                {
                    await func();
                    await successCallback();
                }
            }
            catch (System.Exception ex)
            {
                await HandleException(ex, func, successCallback, failCallback);
            }
            finally
            {
                finallyCallback?.Invoke();
            }
        }

        private static async Task HandleException<TResult>(System.Exception exception,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            UserDialogs.Instance.HideLoading();

            switch (exception)
            {
                case UserFriendlyException userFriendlyException:
                    await HandleUserFriendlyException(userFriendlyException, failCallback);
                    break;
                case FlurlHttpTimeoutException httpTimeoutException:
                    await HandleFlurlHttpTimeoutException(httpTimeoutException, func, successCallback, failCallback);
                    break;
                case FlurlHttpException httpException:
                    await HandleFlurlHttpException(httpException, func, successCallback, failCallback);
                    break;
                case AbpValidationException abpValidationException:
                    await HandleAbpValidationException(abpValidationException, failCallback);
                    break;
                default:
                    await HandleDefaultException(exception, func, successCallback, failCallback);
                    break;
            }
        }

        private static async Task HandleException(System.Exception exception,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            UserDialogs.Instance.HideLoading();

            switch (exception)
            {
                case UserFriendlyException userFriendlyException:
                    await HandleUserFriendlyException(userFriendlyException, failCallback);
                    break;
                case FlurlHttpTimeoutException httpTimeoutException:
                    await HandleFlurlHttpTimeoutException(httpTimeoutException, func, successCallback, failCallback);
                    break;
                case FlurlHttpException httpException:
                    await HandleFlurlHttpException(httpException, func, successCallback, failCallback);
                    break;
                case AbpValidationException abpValidationException:
                    await HandleAbpValidationException(abpValidationException, failCallback);
                    break;
                default:
                    await HandleDefaultException(exception, func, successCallback, failCallback);
                    break;
            }
        }

        private static async Task HandleUserFriendlyException(UserFriendlyException userFriendlyException,
            Func<System.Exception, Task> failCallback)
        {
            if (string.IsNullOrEmpty(userFriendlyException.Details))
            {
                UserDialogs.Instance.Alert(userFriendlyException.Message, L.Localize("Error"));
            }
            else
            {
                UserDialogs.Instance.Alert(userFriendlyException.Details, userFriendlyException.Message);
            }

            await failCallback(userFriendlyException);
        }

        private static async Task HandleFlurlHttpTimeoutException<TResult>(
            FlurlHttpTimeoutException httpTimeoutException,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.RequestTimedOut,
                LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpTimeoutException);
            }
        }

        private static async Task HandleFlurlHttpTimeoutException(FlurlHttpTimeoutException httpTimeoutException,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.RequestTimedOut,
                LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpTimeoutException);
            }
        }

        private static async Task HandleFlurlHttpException(FlurlHttpException httpException,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            if (AbpExceptionHandler.HandleIfAbpResponse(httpException))
            {
                await failCallback(httpException);
                return;
            }

            var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.HttpException,
                LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpException);
            }
        }

        private static async Task HandleFlurlHttpException<TResult>(FlurlHttpException httpException,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            if (AbpExceptionHandler.HandleIfAbpResponse(httpException))
            {
                await failCallback(httpException);
                return;
            }

            var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.HttpException,
                LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpException);
            }
        }

        private static async Task HandleAbpValidationException(AbpValidationException abpValidationException,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogs.Instance.AlertAsync(abpValidationException.GetConsolidatedMessage(),
                LocalTranslation.MessageTitle, LocalTranslation.Ok);

            await failCallback(abpValidationException);
        }

        private static async Task HandleDefaultException(System.Exception exception,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.UnhandledWebRequestException,
                LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);
            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(exception);
            }
        }

        private static async Task HandleDefaultException<TResult>(System.Exception exception,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            var accepted = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.UnhandledWebRequestException,
                LocalTranslation.MessageTitle, LocalTranslation.Ok, LocalTranslation.Cancel);

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(exception);
            }
        }
    }
}