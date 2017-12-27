using Abp;
using Abp.Modules;
using Castle.Core.Logging;
using Castle.Facilities.Logging;

namespace Magicodes.Admin.Core
{
    public static class ApplicationBootstrapper
    {
        public static AbpBootstrapper AbpBootstrapper { get; private set; }

        public static bool IsInitialized => AbpBootstrapper != null;

        public static void InitializeIfNeeds<T>()
            where T : AbpModule
        {
            if (IsInitialized)
            {
                return;
            }

            AbpBootstrapper = AbpBootstrapper.Create<T>(options =>
            {
                //Interceptors (dynamic proxying) are not supported in IOS platform. Also, it's not needed on the client side.
                options.DisableAllInterceptors = true;
            });

            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f =>
            {
                f.LogUsing<TraceLoggerFactory>();
            });

            AbpBootstrapper.Initialize();
        }
    }
}