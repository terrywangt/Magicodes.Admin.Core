using Abp.Domain.Entities;
using Magicodes.Admin.Core.Custom.Attachments;
using Magicodes.Admin.Core.Custom.Authorization;
using Magicodes.Admin.Core.Custom.Contents;
using Magicodes.Admin.Core.Custom.LogInfos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public partial class AdminDbContext
    {
        #region 注册激活筛选器逻辑
        /// <summary>
        /// 是否启用激活筛选器
        /// </summary>
        protected virtual bool IsIPassivableFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(AdminConsts.PassivableFilterName) == true;

        /// <summary>
        /// 重写是否筛选当前实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) => typeof(IPassivable).IsAssignableFrom(typeof(TEntity)) || base.ShouldFilterEntity<TEntity>(entityType);

        /// <summary>
        /// 重写筛选表达式的创建
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
        {
            var expression = base.CreateFilterExpression<TEntity>();

            if (!typeof(IPassivable).IsAssignableFrom(typeof(TEntity)))
            {
                return expression;
            }

            Expression<Func<TEntity, bool>> passivableFilter = e => !((IPassivable)e).IsActive || ((IPassivable)e).IsActive != IsIPassivableFilterEnabled;
            expression = expression == null ? passivableFilter : CombineExpressions(expression, passivableFilter);

            return expression;
        } 
        #endregion

        public virtual DbSet<AttachmentInfo> AttachmentInfos { get; set; }

        public virtual DbSet<ObjectAttachmentInfo> ObjectAttachmentInfos { get; set; }

        public virtual DbSet<ArticleInfo> ArticleInfos { get; set; }

        public virtual DbSet<ArticleSourceInfo> ArticleSourceInfos { get; set; }

        public virtual DbSet<ArticleTagInfo> ArticleTagInfos { get; set; }

        public virtual DbSet<ColumnInfo> ColumnInfos { get; set; }

        public virtual DbSet<SmsCodeLog> SmsCodeLogs { get; set; }

        public virtual DbSet<AppUserOpenId> AppUserOpenIds { get; set; }

        public virtual DbSet<TransactionLog> TransactionLogs { get; set; }
    }
}
