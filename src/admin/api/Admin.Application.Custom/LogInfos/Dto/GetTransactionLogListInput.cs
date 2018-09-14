using Abp.Extensions;
using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;

namespace Admin.Application.Custome.LogInfos.Dto
{
    /// <summary>
    ///  交易日志搜索参数
    /// </summary>
    public partial class GetTransactionLogsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 是否仅获取回收站数据
        /// </summary>
		public bool IsOnlyGetRecycleData { get; set; }

		
		/// <summary>
		/// 创建开始时间
		/// </summary>
        public DateTime? CreationDateStart { get; set; }
        
		/// <summary>
		/// 创建结束时间
		/// </summary>
        public DateTime? CreationDateEnd { get; set; }
				
		/// <summary>
		/// 关键字
		/// </summary>
		public string Filter { get; set; }

        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
		
				Sorting = "CreationTime DESC";

		
            }
        }
    }
		
}