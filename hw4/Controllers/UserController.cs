using hw4.DTO;
using hw4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hw4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        // POST api/User/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid user object.");

            if (!Models.User.IsValidEmail(dto.Email))
                return BadRequest("Invalid email format.");

            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create a new User object and set Active/DeletedAt
            User newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Active = true,           // Always active for new users
                DeletedAt = null         // Always null for new users
            };

            int result = newUser.InsertUser();
            if (result == -1)
                return BadRequest("Email already exists.");
            if (result > 0)
                return Ok(new { message = "User registered successfully.", name = newUser.Name, email = newUser.Email });
            else
                return BadRequest("Failed to register user.");
        }

        // POST api/User/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest("Invalid login data.");

            User loggedInUser = Models.User.Login(request.Email, request.Password);

            if (loggedInUser != null && loggedInUser.Active)
                return Ok(new { id = loggedInUser.Id, name = loggedInUser.Name, email = loggedInUser.Email });

            // If user is found but inactive
            if (loggedInUser != null && !loggedInUser.Active)
                return Unauthorized(new { message = "User is inactive" });

            // If user not found or password incorrect
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // PUT api/User/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserRequest updatedUser)
        {
            if (updatedUser == null)
                return BadRequest("Invalid user object.");

            if (!Models.User.IsValidEmail(updatedUser.Email))
                return BadRequest("Invalid email format.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            User userToUpdate = new User
            {
                Id = id, // From route
                Name = updatedUser.Name,
                Email = updatedUser.Email,
                Password = hashedPassword,
                Active = updatedUser.Active
            };

            int result = userToUpdate.UpdateUser();

            if (result > 0)
                return Ok(new { id = userToUpdate.Id, name = userToUpdate.Name, email = userToUpdate.Email });
            else
                return NotFound("User not found.");
        }


        // DELETE api/User/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User userToDelete = new User { Id = id };

            int result = userToDelete.DeleteUser();

            if (result > 0)
                return Ok("User deleted successfully.");
            else
                return NotFound("User not found.");
        }

        // GET api/User
        [HttpGet]
        public IActionResult GetAll()
        {
            // Use 'Models' because to avoid ambiguity with the User property
            /*This is a common issue in ASP.NET Core controllers because Controller base class already has a User property,
             * which conflicts with your model class name. Using the fully qualified name with namespace resolves this ambiguity.*/
            var users = Models.User.GetAllUsers();
            return Ok(users);
        }

        // PUT api/User/{id}/active
        [HttpPut("{id}/active")]
        public IActionResult UpdateUserActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                User user = new User { Id = id };

                int result = user.UpdateUserActiveStatus(isActive);

                if (result > 0)
                    return Ok(result);
                else
                    return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
// old controller
////------------------------------------------------------
////OLD CONTROLLER
////------------------------------------------------------
//{
//    // GET: api/<UserController>
//    [HttpGet]
//    public IEnumerable<User> Get()
//    {
//        User user = new User();
//        return user.Read();
//    }

//    // GET api/<UserController>/5
//    [HttpGet("{id}")]
//    public string Get(int id)
//    {
//        return "value";
//    }

//    // POST api/<UserController>
//    //[HttpPost]
//    //public bool Post([FromBody] User user)
//    //{
//    //    return user.Insert();
//    //}

//    // POST api/User/register
//    [HttpPost("register")]
//    public IActionResult Register([FromBody] User user)
//    {
//        bool result = user.Register(user.Name, user.Email, user.Password);
//        return Ok(result); // returns JSON: true or false
//    }

//    // POST api/User/login
//    [HttpPost("login")]
//    public IActionResult Login([FromBody] JsonElement data)
//    {

//        // Extract email and password from the JSON payload
//        string email = data.GetProperty("email").GetString();
//        string password = data.GetProperty("password").GetString();

//        // Call the static Login method
//        User loggedInUser = Models.User.Login(email, password);


//        if (loggedInUser != null)
//        {
//            // Return the user's name and email
//            return Ok(new { name = loggedInUser.Name, email = loggedInUser.Email });
//        }

//        // If login fails, return Unauthorized
//        return Unauthorized(new { message = "Invalid email or password." });
//    }

//    // PUT api/<UserController>/5
//    [HttpPut("{id}")]
//    public void Put(int id, [FromBody] string value)
//    {
//    }

//    // DELETE api/<UserController>/5
//    [HttpDelete("{id}")]
//    public void Delete(int id)
//    {
//    }
//}
