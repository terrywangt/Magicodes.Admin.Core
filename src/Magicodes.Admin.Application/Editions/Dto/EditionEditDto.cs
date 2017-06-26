using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace Magicodes.Admin.Editions.Dto
{
    [AutoMap(typeof(SubscribableEdition))]
    public class EditionEditDto
    {
        public int? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public decimal? MonthlyPrice { get; set; }

        public decimal? AnnualPrice { get; set; }

        public int? TrialDayCount { get; set; }

        public int? WaitingDayAfterExpire { get; set; }

        public int? ExpiringEditionId { get; set; }
    }
}