using Microsoft.AspNetCore.Mvc;
using JuntoChallenge.Domain.Entities;
using JuntoChallenge.Application.Interfaces;
using JuntoChallenge.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using JuntoChallenge.Application.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol.Plugins;
using System.Text.Json;
using JuntoChallenge.Infrastructure.Migrations;

namespace JuntoChallenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public UsersController(IUserService userService, ILogService logService)
        {
            _userService = userService;
            _logService = logService;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var response = _userService.GetUsers(pageNumber, pageSize);

                await _logService.LogAsync("OK", $"|DATA : pageNumber = {pageNumber}, pageSize = {pageSize} |");
                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : pageNumber = {pageNumber}, pageSize = {pageSize} | {ex.Message}", ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                var user = _userService.GetUser(id);

                if (user == null)
                {
                    await _logService.LogAsync("NotFound", $"|DATA : ID = {id} |");
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : ID = {id} | {ex.Message}", ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UpdateUserDTO user)
        {
            try
            {
                var oldUser = _userService.GetUser(id);

                if (oldUser == null)
                {
                    await _logService.LogAsync("NotFound", $"|DATA : ID = {id}, {JsonSerializer.Serialize(user)} | User not found!");
                    return NotFound(new { message = "User not found!" });
                }

                var updateUser = await _userService.UpdateUser(id, user);

                if (updateUser == null)
                {
                    await _logService.LogAsync("NotFound", $"|DATA : ID = {id}, {JsonSerializer.Serialize(user)} | User not found!");
                    return NotFound(new { message = "User not found!" });
                }
                else
                {
                    await _logService.LogAsync("OK", $"|DATA : ID = {id}, {JsonSerializer.Serialize(user)} | User with Id:{updateUser.Id} updated!");
                    return Ok(new { message = $"User with Id:{updateUser.Id} updated!" });
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : ID = {id}, {JsonSerializer.Serialize(user)} | {ex.Message}", ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(PostUserDTO user)
        {
            try
            {

                var postUser = await _userService.PostUser(user);

                if (postUser != null)
                {
                    if (postUser.Id != 0)
                    {
                        await _logService.LogAsync("OK", $"|DATA : {JsonSerializer.Serialize(user)} | User saved!");
                        return CreatedAtAction(nameof(GetUser), new { id = postUser.Id }, postUser);
                    }
                }

                await _logService.LogAsync("BadRequest", $"|DATA : {JsonSerializer.Serialize(user)} | User doesn't saved!");
                return BadRequest(new { message = "User doesn't saved!" });
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : {JsonSerializer.Serialize(user)} | {ex.Message}", ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Users/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var delUser = await _userService.DeleteUser(id);

                if (delUser == true)
                {
                    await _logService.LogAsync("OK", $"|DATA : ID = {id} | User with Id:{id} was deleted!");
                    return Ok(new { message = $"User with Id:{id} was deleted!" });
                }
                else
                {
                    await _logService.LogAsync("BadRequest", $"|DATA : ID = {id} | User not deleted!");
                    return BadRequest(new { message = $"User not deleted!" });
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : ID = {id} | {ex.Message}", ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("/api/Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            try
            {
                var response = _userService.Login(login);

                await _logService.LogAsync("OK", $"|DATA : {JsonSerializer.Serialize(login)} |");
                return Ok(response);
            }
            catch(Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : {JsonSerializer.Serialize(login)} | {ex.Message}", ex);
                return BadRequest(new {message = ex.Message});
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/api/UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordUserDTO userPass)
        {
            try
            {
                var response = await _userService.UpdatePassword(userPass);
                if (response == true)
                {
                    await _logService.LogAsync("OK", $"|DATA : {JsonSerializer.Serialize(userPass)} | Password from username {userPass.Username} was successfully changed!");
                    return Ok(new { message = $"Password from username {userPass.Username} was successfully changed!" });
                }
                else
                {
                    await _logService.LogAsync("BadRequest", $"|DATA : {JsonSerializer.Serialize(userPass)} | Password from username {userPass.Username} was not changed!");
                    return BadRequest(new { message = $"Password from username {userPass.Username} was not changed!" });
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("BadRequest", $"|DATA : {JsonSerializer.Serialize(userPass)} | {ex.Message}", ex);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}