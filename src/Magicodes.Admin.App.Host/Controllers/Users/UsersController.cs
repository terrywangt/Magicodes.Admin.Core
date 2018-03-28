using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Admin.App.User.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.App.Controllers.Account
{
    /// <summary>
    /// 用户
    /// </summary>
    [Produces("application/json")]
    [Route(ApiPrefix + "[controller]")]
    public class UsersController : AppControllerBase
    {
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Me")]
        public async Task<IActionResult> Get()
        {
            //TODO:[API]登陆
            //请结合描述或要点实现方法，并且在完成后删除掉TODO注释
            throw new NotSupportedException("Get");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpGet("ResetPasswordBySmsCode")]
        public async Task<IActionResult> ResetPasswordBySmsCode()
        {
            //TODO:[API]重置密码
            //请结合描述或要点实现方法，并且在完成后删除掉TODO注释
            throw new NotSupportedException("ResetPasswordBySmsCode");
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <response code="200">返回登陆信息</response>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(AppLoginOutput), 200)]
        public async Task<IActionResult> Login(AppLoginInput input)
        {
            //TODO:[API]登陆
            //请结合描述或要点实现方法，并且在完成后删除掉TODO注释
            throw new NotSupportedException("Login");
        }

        /// <summary>
        /// 授权访问
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <response code="400">参数错误</response>
        /// <response code="200">返回授权信息以及Token</response>
        [HttpPost("TokenAuth")]
        [ProducesResponseType(typeof(AppTokenAuthOutput), 200)]
        public async Task<IActionResult> TokenAuth(AppTokenAuthInput input)
        {
            return BadRequest("OpenIdOrUnionId错误");
            //TODO:[API]授权访问
            //请结合描述或要点实现方法，并且在完成后删除掉TODO注释
            throw new NotSupportedException("TokenAuth");
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(AppRegisterOutput), 200)]
        public async Task<IActionResult> Register(AppRegisterInput input)
        {
            //TODO:[API]登陆
            //请结合描述或要点实现方法，并且在完成后删除掉TODO注释
            throw new NotSupportedException("Login");
        }
    }
}
