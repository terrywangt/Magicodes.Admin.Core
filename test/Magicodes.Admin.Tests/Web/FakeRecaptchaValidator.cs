﻿using System.Threading.Tasks;
using Magicodes.Admin.Security.Recaptcha;

namespace Magicodes.Admin.Tests.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public async Task ValidateAsync(string captchaResponse)
        {
            
        }
    }
}
