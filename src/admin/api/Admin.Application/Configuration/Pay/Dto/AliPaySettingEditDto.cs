using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Configuration.Pay.Dto
{
    public class AliPaySettingEditDto
    {
        /// <summary>
        /// 支付平台分配给开发者的应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 合作商户uid
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 支付宝网关
        /// 默认值:https://openapi.alipay.com/gateway.do 
        /// </summary>
        public string Gatewayurl { get; set; }

        /// <summary>
        /// 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        /// </summary>
        public string AlipayPublicKey { get; set; }

        /// <summary>
        /// 支付宝签名公钥
        /// </summary>
        public string AlipaySignPublicKey { get; set; }

        /// <summary>
        /// 商户私钥，您的原始格式RSA私钥
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 请求使用的编码格式，如utf-8,gbk,gb2312等 
        /// 默认值utf-8
        /// </summary>
        public string CharSet { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string Notify { get; set; }
        /// <summary>
        /// 商户生成签名字符串所使用的签名算法类型，目前支持RSA2和RSA，推荐使用RSA2
        /// 默认值 RSA2
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// 是否从文件读取公私钥 如果为true ，那么公私钥应该配置为密钥文件路径
        /// </summary>
        public bool IsKeyFromFile { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
