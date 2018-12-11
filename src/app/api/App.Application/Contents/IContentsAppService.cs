using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.App.Application.Contents.Contents.Dto;
using Magicodes.App.Application.Contents.Dto;

namespace Magicodes.App.Application.Contents
{
    /// <summary>
    /// 内容相关
    /// </summary>
    public interface IContentsAppService : IApplicationService    
    {
        /// <summary>
        /// 获取轮询图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<GetCarouselPictureListDto>> GetCarouselPictureList(GetCarouselPictureListInput input);

        /// <summary>
        /// 获取栏目列表接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<GetColumnListDto>> GetColumnList(GetColumnListInput input);

        /// <summary>
        /// 栏目详情接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetColumnDetailInfoOutput> GetColumnDetailInfo(GetColumnDetailInfoInput input);

        /// <summary>
        /// 文章列表接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<GetArticleListDto>> GetArticleList(GetArticleListInput input);

        /// <summary>
        /// 文章详情接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetArticleDetailInfoOutput> GetArticleDetailInfo(GetArticleDetailInfoInput input);

    }
}