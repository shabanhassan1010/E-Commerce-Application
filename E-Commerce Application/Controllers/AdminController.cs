#region 
using E_Commerce.ApplicationLayer.Dtos.Users;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
#endregion

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    
    public class AdminController : ControllerBase
    {
        #region Context
        private readonly IUserService userService;
        public AdminController(IUserService userService)
        {
            this.userService = userService;
        }
        #endregion

        protected string? GetUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        #region GeTAllUsers
        [HttpGet]
        [EndpointSummary("Get All Users")]
        public async Task<ActionResult<PaginationResponse<UserDetailsDto>>> GeTAllUsers([FromQuery]int page = 1 , [FromQuery]  int size = 10 , [FromQuery] string? search = null)
        {
            var UserId = GetUserId();
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var users = await userService.GetUsersAsync(page, size , search);
            return Ok(users);
        }
        #endregion

        #region  Change Role
        [HttpPost("change-role")]
        [EndpointSummary("change user role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDto dto)
        {
            var UserId = GetUserId();
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var result = await userService.ChangeUserRoleAsync(dto.UserId, dto.NewRole);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Role Changes Succfully" });
        }
        #endregion
    }
}
