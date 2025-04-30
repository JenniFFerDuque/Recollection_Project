using System.Net;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Recolections.cs.DTOs;
using Services.Users;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //Inyección de dependencias
        private readonly IUserService _userService;

        //Constructor
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        //CRUD:

        //CREATE
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateUser(user);
            return createdUser is not null
                ? CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser)
                : BadRequest("Error al crear el usuario");
        }

        //READ
        [HttpGet("{id}", Name = "GetUserById")] //Obtener un usuario por su id
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            return user is not null ? Ok(user) : NotFound();
        }

        [HttpGet(Name = "GetAllUsers")] //Obtener todos los usuarios
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return users is not null && users.Any() ? Ok(users) : NoContent();
        }

        // Updated UpdateUser method to fix the CS7036 error
        [HttpPut("{id}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO updateData)
        {
            //Validar que el id coincide con la url
            if (id != updateData.UserId)
                return BadRequest("ID does not match");

            //Validar que el usuario existe
            var existingUser = await _userService.GetUserById(id);
            if (existingUser is null)
            {
                return NotFound();
            }
            
            var updatedUser = new User
            {
                UserId = updateData.UserId,
                Email = updateData.Email ?? existingUser.Email,
                Password = updateData.Password ?? existingUser.Password
            };

            // llamar al servicio para actualizar el usuario
            var result = await _userService.UpdateUser(existingUser, updatedUser);
            return result is not null ? Ok(result) : StatusCode((int)HttpStatusCode.NotModified);
        }

        //DELETE
        [HttpDelete("{id}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //Validar que el usuario existe
            var getUser = await _userService.GetUserById(id);

            if (getUser is null)
            {
                return NotFound();
            }

            var deleted = await _userService.DeleteUser(id);
            return deleted ? Ok() : StatusCode((int)HttpStatusCode.NotModified);
        }
    }
}
