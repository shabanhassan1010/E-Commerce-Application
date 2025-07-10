using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class RolesController : ControllerBase
    {
        #region Context
        private readonly RoleManager<IdentityRole> roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        #endregion

        #region GetRoles
        [HttpGet]
        [EndpointSummary("Get All Roles")]
        public ActionResult<IEnumerable<string>> GetRoles()
        {
            var roles = roleManager.Roles.Select(x => x.Name).ToList();
            return roles;
        }
        #endregion

        #region CreateRole
        [HttpPost]
        [EndpointSummary("Create Role")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role Must not Empty");

            // check if the role is exist before or not
            var existingRole = await roleManager.RoleExistsAsync(roleName);
            if (existingRole)
                return BadRequest("This Role is Already Exist");

            //creat role
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return Ok($"Role Has been Created Successfully {roleName}");

            return BadRequest(result.Errors);
        }
        #endregion

        #region DeleteRole
        [HttpDelete("{roleName}")]
        [EndpointSummary("Delete Role")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("This Role is not Exist.");

            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return Ok($"Role Has been Deleted Successfully {roleName}");

            return BadRequest(result.Errors);
        }
        #endregion
    }
}
