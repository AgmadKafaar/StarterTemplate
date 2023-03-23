using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarterTemplate.Core.Entities;
using StarterTemplate.Core.Services.Authentication;
using StarterTemplate.Core.Services.Authentication.Dto;

namespace StarterTemplate.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILdapService _ldapService;

        public AuthController(ILdapService ldapService)
        {
            _ldapService = ldapService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [Route("Login")]
        public ActionResult<User> Login(LoginDto loginModel)
        {
            if (loginModel == null) return BadRequest("Username and password must be supplied");

            if (string.IsNullOrEmpty(loginModel.Username))
            {
                return Problem("Username and password must be supplied", statusCode: StatusCodes.Status400BadRequest);
            }
            if (string.IsNullOrEmpty(loginModel.Password))
            {
                return Problem("Username and password must be supplied", statusCode: StatusCodes.Status400BadRequest);
            }

            var result = _ldapService.Login(loginModel.Username, loginModel.Password);

            if (result == null)
                return Problem("Invalid username or password", statusCode: StatusCodes.Status401Unauthorized, title: "User Not Found");

            return Ok(result);
        }
    }
}