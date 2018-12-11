namespace Magicodes.App.Application.Contents.Dto
{
    /// <summary>
    /// 栏目详情接口 输入参数
    /// </summary>
    public class GetColumnDetailInfoInput    
    {
        /// <summary>
        /// 栏目Id
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 栏目编码
        /// </summary>
        public string Code { get; set; }


    }
}