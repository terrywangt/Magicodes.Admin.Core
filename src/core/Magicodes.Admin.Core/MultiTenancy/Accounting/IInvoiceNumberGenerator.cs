using System.Threading.Tasks;
using Abp.Dependency;

namespace Magicodes.Admin.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}