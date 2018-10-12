// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AlipayAppPayInput.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:43
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.Pay.Services.Dto
{
    public class AppPayInput
    {
        /// <summary>
        /// 对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
        /// </summary>
        [MaxLength(128)]
        public string Body { get; set; }

        /// <summary>商品的标题/交易标题/订单标题/订单关键字等。</summary>
        [Required]
        [MaxLength(256)]
        public string Subject { get; set; }

        /// <summary>订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]</summary>
        [Required]
        [Range(0.01, 100000000.0)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        ///     自定义数据
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "自定义数据")]
        public string CustomData { get; set; }

        /// <summary>
        ///     交易单号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "交易单号")]
        public string OutTradeNo { get; set; }
    }
}