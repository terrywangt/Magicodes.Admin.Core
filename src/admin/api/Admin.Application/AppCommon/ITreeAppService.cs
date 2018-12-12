using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.AppCommon
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