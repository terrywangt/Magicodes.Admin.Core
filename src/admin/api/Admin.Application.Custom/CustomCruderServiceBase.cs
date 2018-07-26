using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Timing;

namespace Admin.Application.Custom
{
    /// <summary>
    /// 自定义增删查改恢复导出服务基类
    /// 增
    /// 删
    /// 改
    /// 查
    /// 导出
    /// 恢复
    /// </summary>
    /// <typeparam name="TEntity">实体模型</typeparam>
    /// <typeparam name="TEntityDto">实体列表Dto</typeparam>
    /// <typeparam name="TPrimaryKey">主键</typeparam>
    /// <typeparam name="TGetAllInput">查询所有Input</typeparam>
    /// <typeparam name="TCreateInput">创建Input</typeparam>
    /// <typeparam name="TUpdateInput">修改Input</typeparam>
    /// <typeparam name="TExportDto">导出Dto</typeparam>
    [AbpAuthorize]
    public abstract class CustomCruderServiceBase<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TExportDto> : CustomCrudeServiceBase<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TExportDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedAndSortedResultRequest
        where TExportDto : class 

    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        protected CustomCruderServiceBase(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        /// <summary>
        /// 查询所有
        /// 默认关闭软删除筛选器以支持代码生成时的回收站服务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<TEntityDto>> GetAll(TGetAllInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                return await base.GetAll(input);
            }
        }

        /// <summary>
        /// 恢复当前数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task Restore(TPrimaryKey id)
        {
            CheckPermission(RestorePermissionName);
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var model = await Repository.GetAsync(id);
                if (model is ISoftDelete delete)
                {
                    delete.IsDeleted = false;
                }
                if (model is IModificationAudited moduiAudited)
                {
                    moduiAudited.LastModifierUserId = AbpSession.GetUserId();
                    moduiAudited.LastModificationTime = Clock.Now;
                }
            }
        }
    }
}