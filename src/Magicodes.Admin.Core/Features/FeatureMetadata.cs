using System;
using Abp.Localization;

namespace Magicodes.Admin.Features
{
    public class FeatureMetadata
    {
        public const string CustomFeatureKey = "FeatureMetadata";

        public FeatureMetadata()
        {
            TextHtmlColor = value => "inherit";
            IsVisibleOnPricingTable = false;
        }

        public Func<string, ILocalizableString> ValueTextNormalizer { get; set; }

        public bool IsVisibleOnPricingTable { get; set; }

        public Func<string, string> TextHtmlColor { get; set; }
    }
}