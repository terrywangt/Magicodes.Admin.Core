// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : Currency.cs
//           description :金额
//   
//           created by 雪雁 at  2018-08-21 17:37
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Abp.Domain.Values;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AutoMapper.Internal;

namespace Magicodes.Admin.Core.Custom.LogInfos
{
    /// <summary>
    ///     金额（支持多种货币类型）
    /// </summary>
    [Owned]
    [Description("金额")]
    public class Currency
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        [MaxLength(10)]
        public string CultureName { get; internal set; } //区域(例如：en-us)

        private CultureInfo _culture;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="currencyValue"></param>
        public Currency(CultureInfo culture, decimal currencyValue)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            CultureName = culture.Name;
            _culture = culture;
            CurrencyValue = currencyValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="currencyValue"></param>
        public Currency(string cultureName, decimal currencyValue)
        {
            CultureName = cultureName;
            _culture = null;
            CurrencyValue = currencyValue;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public CultureInfo Culture
        {
            get
            {
                if (CultureName == null)
                {
                    return null;
                }

                if (_culture != null)
                {
                    return _culture;
                }

                _culture = CultureInfo.CreateSpecificCulture(CultureName);
                return _culture;
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        public decimal CurrencyValue { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public bool IsNull => CultureName == null;

        /// <summary>
        /// 
        /// </summary>
        public static Currency Null => new Currency((string)null, 0);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => IsNull ? string.Empty : string.Format(Culture, "{0:c}", CurrencyValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyStr"></param>
        /// <returns></returns>
        public static Currency Parse(string currencyStr) => ParseWithCulture(currencyStr, CultureInfo.CurrentUICulture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyStr"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Currency ParseWithCulture(string currencyStr, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(currencyStr))
            {
                return Null;
            }

            var digitPos = -1;
            var stringValue = currencyStr;

            while (digitPos < stringValue.Length
                   && !char.IsDigit(stringValue, ++digitPos))
            {
            }

            if (digitPos < stringValue.Length)
            {
                return new Currency(culture, decimal.Parse(
                    stringValue.Substring(digitPos), culture));
            }

            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => IsNull ? 0 : ToString().GetHashCode();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var propertiesForCompare = this.GetPropertiesForCompare();
            return !((IEnumerable<PropertyInfo>)propertiesForCompare).Any<PropertyInfo>() || ((IEnumerable<PropertyInfo>)propertiesForCompare).All<PropertyInfo>((Func<PropertyInfo, bool>)(property => object.Equals(property.GetValue((object)this, (object[])null), property.GetValue(obj, (object[])null))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Currency x, Currency y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals((object)y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Currency x, Currency y) => !(x == y);

        private PropertyInfo[] GetPropertiesForCompare()
        {
            return ((IEnumerable<PropertyInfo>)this.GetType().GetTypeInfo().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>)(t => GetSingleAttributeOrDefault<IgnoreOnCompareAttribute>((MemberInfo)t, (IgnoreOnCompareAttribute)null, true) == null)).ToArray<PropertyInfo>();
        }

        private static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute), bool inherit = true)
            where TAttribute : Attribute
        {
            return memberInfo.IsDefined(typeof(TAttribute), inherit) ? memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First() : defaultValue;
        }
    }
}