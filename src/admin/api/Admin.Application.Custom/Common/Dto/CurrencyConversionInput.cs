using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Admin.Application.Custom.Common.Dto
{
    public class CurrencyConversionInput
    {
        /// <summary>
        /// 区域
        /// </summary>
        [MaxLength(10)]
        [Required]
        public string CultureName { get; set; } //区域(例如：en-us)
        /// <summary>
        /// 金额
        /// </summary>
        [Required]
        public decimal CurrencyValue { get; set; }
    }
}
