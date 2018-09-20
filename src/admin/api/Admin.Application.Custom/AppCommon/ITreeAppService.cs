using Abp.Application.Services;
using Admin.Application.Custom.AppCommon.Dto;
using Magicodes.Admin.Dto;
using System.Threading.Tasks;

namespace Admin.Application.Custom.AppCommon
{
    public interface ITreeAppService : IApplicationService
    {
				/// <summary>
        /// 获取栏目 树级列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TreeOutputDto> GetColumnInfoTreeNodes(Dto.GetTreeNodesInputDto input);
    }
}