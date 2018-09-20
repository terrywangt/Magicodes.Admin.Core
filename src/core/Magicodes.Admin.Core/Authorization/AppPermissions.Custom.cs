using System;

namespace Magicodes.Admin.Authorization
{
    /// <summary>
    /// 定义应用程序权限名称常量
    /// <see cref="AppAuthorizationProvider"/> 权限定义.
    /// </summary>
    public partial class AppPermissions
    {
        #region TransactionLog【交易日志】

        public const string Pages_TransactionLog = "Pages.TransactionLog";

        public const string Pages_TransactionLog_Create = "Pages.TransactionLog.Create";

        public const string Pages_TransactionLog_Edit = "Pages.TransactionLog.Edit";

        public const string Pages_TransactionLog_Delete = "Pages.TransactionLog.Delete";

        public const string Pages_TransactionLog_Restore = "Pages.TransactionLog.Restore";

        #endregion

        #region ArticleTagInfos【文章标签】

        public const string Pages_ArticleInfo_ArticleTagInfo = "Pages.ArticleInfo.ArticleTagInfo";

		public const string Pages_ArticleInfo_ArticleTagInfo_Create = "Pages_ArticleInfo_ArticleTagInfo.Create";
							   
		public const string Pages_ArticleInfo_ArticleTagInfo_Edit = "Pages_ArticleInfo_ArticleTagInfo.Edit";	
							   
		public const string Pages_ArticleInfo_ArticleTagInfo_Delete = "Pages_ArticleInfo_ArticleTagInfo.Delete";
							   					
        public const string Pages_ArticleInfo_ArticleTagInfo_Restore = "Pages_ArticleInfo_ArticleTagInfo.Restore";

        #endregion

        #region ArticleInfo【文章】

        public const string Pages_ArticleInfo = "Pages.ArticleInfo";

		public const string Pages_ArticleInfo_Create = "Pages.ArticleInfo.Create";
	
		public const string Pages_ArticleInfo_Edit = "Pages.ArticleInfo.Edit";	

		public const string Pages_ArticleInfo_Delete = "Pages.ArticleInfo.Delete";

        public const string Pages_ArticleInfo_Restore = "Pages.ArticleInfo.Restore";

        #endregion
		
        #region ArticleSourceInfo【文章来源】

        public const string Pages_ArticleSourceInfo = "Pages.ArticleSourceInfo";

		public const string Pages_ArticleSourceInfo_Create = "Pages.ArticleSourceInfo.Create";
	
		public const string Pages_ArticleSourceInfo_Edit = "Pages.ArticleSourceInfo.Edit";	

		public const string Pages_ArticleSourceInfo_Delete = "Pages.ArticleSourceInfo.Delete";

        public const string Pages_ArticleSourceInfo_Restore = "Pages.ArticleSourceInfo.Restore";

        #endregion
		
        #region ColumnInfo【栏目】

        public const string Pages_ColumnInfo = "Pages.ColumnInfo";

		public const string Pages_ColumnInfo_Create = "Pages.ColumnInfo.Create";
	
		public const string Pages_ColumnInfo_Edit = "Pages.ColumnInfo.Edit";	

		public const string Pages_ColumnInfo_Delete = "Pages.ColumnInfo.Delete";

        public const string Pages_ColumnInfo_Restore = "Pages.ColumnInfo.Restore";

        #endregion
		
	}
}