using System.Threading.Tasks;

namespace Magicodes.Admin.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}