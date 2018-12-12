using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Timing;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Dto;
using Magicodes.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace Magicodes.Admin
{
    [AbpAuthorize]
    public abstract class AppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        public IEventBus EventBus { get; set; }

        public IAppFolders AppFolders { get; set; }

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
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="model"></param>
        protected void SetCreateModel<Tkey>(EntityBase<Tkey> model)
        {
            model.CreationTime = Clock.Now;
            model.CreatorUserId = AbpSession.UserId;
            model.TenantId = AbpSession.TenantId;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="model"></param>
        protected void SetDeleteModel<Tkey>(EntityBase<Tkey> model)
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

        /// <summary>
        /// 拖拽排序支持
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TPrimaryKey">主键类型</typeparam>
        /// <param name="repository">仓储</param>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        protected async Task MoveTo<TEntity, TPrimaryKey>(IRepository<TEntity, TPrimaryKey> repository, MoveToInputDto<TPrimaryKey> input) where TEntity : class, IEntity<TPrimaryKey>, ISortNo
        {
            var sourceItem = await repository.GetAsync(input.SourceId);
            var targetItem = await repository.GetAsync(input.TargetId);
            switch (input.MoveToPosition)
            {
                case MoveToPositions.Up:
                    {
                        var list = repository.GetAll().Where(p => p.SortNo <= targetItem.SortNo);
                        foreach (var item in list)
                        {
                            item.SortNo--;
                        }
                    }
                    sourceItem.SortNo = targetItem.SortNo - 1;
                    break;
                case MoveToPositions.Down:
                    {
                        var list = repository.GetAll().Where(p => p.SortNo > targetItem.SortNo);
                        foreach (var item in list)
                        {
                            item.SortNo++;
                        }
                    }
                    sourceItem.SortNo = targetItem.SortNo + 1;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="codeHeader">编号头部</param>
        protected string GetCode(string codeHeader=null)
        {
            var guid = Guid.NewGuid().ToString("N").ToUpper();
            return codeHeader.IsNullOrWhiteSpace() ? guid : $"{codeHeader}{guid}";
        }
    }
}