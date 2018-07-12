using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Threading;
using Microsoft.AspNetCore.Identity;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin;
using Abp.Timing;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Magicodes.App.Application
{
    [AbpAuthorize]
    public abstract class AppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        public IEventBus EventBus { get; set; }

        protected AppServiceBase()
        {
            LocalizationSourceName = AdminConsts.LocalizationSourceName;
            EventBus = NullEventBus.Instance;
        }

        //protected virtual async Task<User> GetCurrentUserAsync()
        //{
        //    var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
        //    if (user == null)
        //    {
        //        throw new Exception("There is no current user!");
        //    }
        //    return user;
        //}

        //protected virtual User GetCurrentUser()
        //{
        //    return AsyncHelper.RunSync(GetCurrentUserAsync);
        //}

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
            }
        }

        protected virtual Tenant GetCurrentTenant()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetById(AbpSession.GetTenantId());
            }
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }


        /// <summary>
        /// 设置创建模型默认属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        protected void SetCreateModel<T>(T model) where T : IFullAudited, IMayHaveTenant
        {
            model.CreationTime = Clock.Now;
            model.CreatorUserId = AbpSession.UserId;
            model.TenantId = AbpSession.TenantId;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        protected void SetDeleteModel<T>(T model) where T : IFullAudited, IMayHaveTenant
        {
            model.IsDeleted = true;
            model.DeleterUserId = AbpSession.UserId;
            model.DeletionTime = Clock.Now;
        }

        /// <summary>
        /// 禁用租户筛选器
        /// </summary>
        /// <param name="func"></param>
        protected void DisableTenantFilterWitchAction<TResult>(Func<Task<TResult>> func)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                AsyncHelper.RunSync(func);
            }
        }

        /// <summary>
        /// 禁用租户筛选器
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected TResult DisableTenantFilterWitchFunc<TResult>(Func<Task<TResult>> func)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return AsyncHelper.RunSync(func);
            }
        }
    }
}