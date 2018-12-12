using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  栏目创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateColumnInfoDto
    {
        [Required]
        public ColumnInfoEditDto ColumnInfo { get; set; }
    }
}