using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Contents.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Contents
{
	/// <summary>
	/// 文章来源
	/// </summary>
    public interface IArticleSourceInfoAppService : IApplicationService
    {
		/// <summary>
		/// 获取文章来源列表
		/// </summary>
        Task<PagedResultDto<ArticleSourceInfoListDto>> GetArticleSourceInfos(GetArticleSourceInfosInput input);

		/// <summary>
		/// 获取文章来源
		/// </summary>
        Task<GetArticleSourceInfoForEditOutput> GetArticleSourceInfoForEdit(NullableIdDto<long> input);

		/// <summary>
		/// 创建或编辑文章来源
		/// </summary>
        Task CreateOrUpdateArticleSourceInfo(CreateOrUpdateArticleSourceInfoDto input);

		/// <summary>
		/// 删除文章来源
		/// </summary>
        Task DeleteArticleSourceInfo(EntityDto<long> input);

		/// <summary>
		/// 导出文章来源
		/// </summary>
        Task<FileDto> GetArticleSourceInfosToExcel(GetArticleSourceInfosInput input);
    }
}