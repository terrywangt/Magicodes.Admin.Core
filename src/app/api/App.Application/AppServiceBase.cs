// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppServiceBase.cs
//           description :
//   
//           created by 雪雁 at  2018-07-12 18:13
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Timing;
using Magicodes.Admin;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace Magicodes.App.Application
{
    /// <summary>
    ///     APP服务接口基类
    ///     默认移除审计
    /// </summary>
    [DisableAuditing]
    [AbpAuthorize]
    public abstract class AppServiceBase : ApplicationService
    {
        protected AppServiceBase()
        {
            LocalizationSourceName = AdminConsts.AppLocalizationSourceName;
            EventBus = NullEventBus.Instance;
        }

        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        public IEventBus EventBus { get; set; }

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
        ///     设置创建模型默认属性
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
        ///     软删除
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
        ///     禁用租户筛选器
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
        ///     禁用租户筛选器
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