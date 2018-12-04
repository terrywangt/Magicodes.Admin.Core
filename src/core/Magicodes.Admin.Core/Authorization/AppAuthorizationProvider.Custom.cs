using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Magicodes.Admin.Authorization
{
    /// <summary>
    /// 应用程序权限提供程序
    /// </summary>
    public partial class AppAuthorizationProvider
    {

        /// <summary>
        /// 设置权限
        /// </summary>
        public void SetCustomPermissions(Permission root)
        {

            #region TransactionLog【交易日志】
            var transactionLog = root.CreateChildPermission(AppPermissions.Pages_TransactionLog, L("TransactionLog"));
            //transactionLog.CreateChildPermission(AppPermissions.Pages_TransactionLog_Create, L("CreateNew"));
            transactionLog.CreateChildPermission(AppPermissions.Pages_TransactionLog_Edit, L("Edit"));
            transactionLog.CreateChildPermission(AppPermissions.Pages_TransactionLog_Delete, L("Delete"));
            transactionLog.CreateChildPermission(AppPermissions.Pages_TransactionLog_Restore, L("Restore"));
            #endregion

            #region ArticleInfo_ArticleTagInfos【文章标签】
            var articleInfo_articleTagInfos = root.CreateChildPermission(AppPermissions.Pages_ArticleInfo_ArticleTagInfo, L("ArticleTagInfos"));
            articleInfo_articleTagInfos.CreateChildPermission(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Create, L("CreateNew"));
            articleInfo_articleTagInfos.CreateChildPermission(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Edit, L("Edit"));
            articleInfo_articleTagInfos.CreateChildPermission(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Delete, L("Delete"));
            articleInfo_articleTagInfos.CreateChildPermission(AppPermissions.Pages_ArticleInfo_ArticleTagInfo_Restore, L("Restore"));
            #endregion

            #region ArticleInfo【文章】
            var articleInfo = root.CreateChildPermission(AppPermissions.Pages_ArticleInfo, L("ArticleInfo"));
            articleInfo.CreateChildPermission(AppPermissions.Pages_ArticleInfo_Create, L("CreateNew"));
            articleInfo.CreateChildPermission(AppPermissions.Pages_ArticleInfo_Edit, L("Edit"));
            articleInfo.CreateChildPermission(AppPermissions.Pages_ArticleInfo_Delete, L("Delete"));
            articleInfo.CreateChildPermission(AppPermissions.Pages_ArticleInfo_Restore, L("Restore"));
            #endregion
		
            #region ArticleSourceInfo【文章来源】
            var articleSourceInfo = root.CreateChildPermission(AppPermissions.Pages_ArticleSourceInfo, L("ArticleSourceInfo"));
            articleSourceInfo.CreateChildPermission(AppPermissions.Pages_ArticleSourceInfo_Create, L("CreateNew"));
            articleSourceInfo.CreateChildPermission(AppPermissions.Pages_ArticleSourceInfo_Edit, L("Edit"));
            articleSourceInfo.CreateChildPermission(AppPermissions.Pages_ArticleSourceInfo_Delete, L("Delete"));
            articleSourceInfo.CreateChildPermission(AppPermissions.Pages_ArticleSourceInfo_Restore, L("Restore"));
            #endregion
		
            #region ColumnInfo【栏目】
            var columnInfo = root.CreateChildPermission(AppPermissions.Pages_ColumnInfo, L("ColumnInfo"));
            columnInfo.CreateChildPermission(AppPermissions.Pages_ColumnInfo_Create, L("CreateNew"));
            columnInfo.CreateChildPermission(AppPermissions.Pages_ColumnInfo_Edit, L("Edit"));
            columnInfo.CreateChildPermission(AppPermissions.Pages_ColumnInfo_Delete, L("Delete"));
            columnInfo.CreateChildPermission(AppPermissions.Pages_ColumnInfo_Restore, L("Restore"));
            #endregion
		
        }
    }
}