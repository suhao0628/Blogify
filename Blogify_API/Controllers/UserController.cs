using Blogify_API.Dtos;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogify_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Register 
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] UserRegisterDto register)
        {
            if (!_userService.IsUniqueUser(register))
            {
                return BadRequest("Username " + register.Email + " is already taken.");
            }
            else
            {
                return Ok(await _userService.Register(register));
            }
        }
        #endregion

        #region Log in
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginDto login)
        {
            return Ok(await _userService.Login(login));
        }
        #endregion
    }
}
