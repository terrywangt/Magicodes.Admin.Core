using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.AppCommon
{
    /// <summary>
    /// 树形结构服务
    /// </summary>
    public class TreeAppService : AppServiceBase, ITreeAppService
    {
		private readonly IRepository<ColumnInfo, long> _columnInfoRepository;

        public TreeAppService(
		IRepository<ColumnInfo, long> columnInfoRepository
		)
        {
		_columnInfoRepository = columnInfoRepository;
        }

		/// <summary>
        /// 获取栏目 树级列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<TreeOutputDto> GetColumnInfoTreeNodes(Dto.GetTreeNodesInputDto input)
        {
            var data = _columnInfoRepository.GetAll().Where(p => p.ParentId == input.ParentId).ToList();
            var output = new TreeOutputDto()
            {
                Data = data.Select(p => new TreeItemDto()
                {
                    Data = new TreeItemDataDto()
                    {
                        Title = p.Title,
                        Id = p.Id
                    }
                }).ToList()
            };

            foreach (var treeItemDto in output.Data)
            {
                treeItemDto.Children = _columnInfoRepository.GetAll().Where(p => p.ParentId == treeItemDto.Data.Id)
                    .ToList().Select(p => new TreeItemDto()
                    {
                        Data = new TreeItemDataDto()
                        {
                            Title = p.Title,
                            Id = p.Id
                        }
                    }).ToList();
                treeItemDto.Leaf = treeItemDto.Children == null || treeItemDto.Children.Count == 0;
            }

            return Task.FromResult(output);
        }
	}
}