using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Dto;
using Admin.Application.Custom.Contents.Dto;

namespace Admin.Application.Custom.Contents
{
	/// <summary>
	/// 文章
	/// </summary>
    public interface IArticleInfoAppService : IApplicationService
    {
		/// <summary>
		/// 获取文章列表
		/// </summary>
        Task<PagedResultDto<ArticleInfoListDto>> GetArticleInfos(GetArticleInfosInput input);

		/// <summary>
		/// 获取文章
		/// </summary>
        Task<GetArticleInfoForEditOutput> GetArticleInfoForEdit(NullableIdDto<long> input);

		/// <summary>
		/// 创建或编辑文章
		/// </summary>
        Task CreateOrUpdateArticleInfo(CreateOrUpdateArticleInfoDto input);

		/// <summary>
		/// 删除文章
		/// </summary>
        Task DeleteArticleInfo(EntityDto<long> input);

		/// <summary>
		/// 导出文章
		/// </summary>
        Task<FileDto> GetArticleInfosToExcel(GetArticleInfosInput input);
    }
}