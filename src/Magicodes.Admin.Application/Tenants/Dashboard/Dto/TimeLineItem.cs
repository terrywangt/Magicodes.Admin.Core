using System;
using System.Globalization;

namespace Magicodes.Admin.Tenants.Dashboard.Dto
{
    public class TimeLineItem
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string AutherName { get; set; }
        public string LongDate { get; set; }
        public string ShortDate { get; set; }
        public string TitleDate { get; set; }
        public string Text { get; set; }

        public TimeLineItem(DateTime date)
        {
            ShortDate = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            LongDate = date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
            TitleDate = date.ToString("dd MMM", CultureInfo.InvariantCulture); 
        }
    }
}