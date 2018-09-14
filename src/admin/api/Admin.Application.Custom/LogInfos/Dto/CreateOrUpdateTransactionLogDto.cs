using System.ComponentModel.DataAnnotations;

namespace Admin.Application.Custom.LogInfos.Dto
{
    /// <summary>
    ///  交易日志创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateTransactionLogDto
    {
        [Required]
        public TransactionLogEditDto TransactionLog { get; set; }
    }
}