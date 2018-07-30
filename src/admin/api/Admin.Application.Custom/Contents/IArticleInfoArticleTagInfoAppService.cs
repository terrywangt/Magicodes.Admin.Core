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
	/// 
	/// </summary>
    public interface IArticleInfoArticleTagInfoAppService : IApplicationService
    {
		/// <summary>
		/// 获取列表
		/// </summary>
        Task<PagedResultDto<ArticleTagInfoListDto>> GetArticleTagInfos(GetArticleInfoArticleTagInfosInput input);

		/// <summary>
		/// 获取
		/// </summary>
        Task<GetArticleTagInfoForEditOutput> GetArticleTagInfoForEdit(NullableIdDto<long> input);

		/// <summary>
		/// 创建或编辑
		/// </summary>
        Task CreateOrUpdateArticleTagInfo(CreateOrUpdateArticleInfoArticleTagInfoDto input);

		/// <summary>
		/// 删除
		/// </summary>
        Task DeleteArticleTagInfo(EntityDto<long> input);

		/// <summary>
		/// 导出
		/// </summary>
        Task<FileDto> GetArticleTagInfosToExcel(GetArticleInfoArticleTagInfosInput input);
    }
}