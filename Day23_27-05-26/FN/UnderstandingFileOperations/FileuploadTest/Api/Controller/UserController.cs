using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _service.GetUsers();

            return Ok(users);
        }


        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var createdUser = await _service.CreateUser(user);

            return Ok(createdUser);
        }


        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var path = await _service.ExportUsersToExcel();

            return Ok(new
            {
                FilePath = path
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            await _service.UploadUsersFromExcel(file);

            return Ok(new
            {
                Message = "Users uploaded successfully"
            });
        }
    }
}