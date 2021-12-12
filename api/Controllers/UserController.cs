namespace api.Controllers
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using MySql.Data.MySqlClient;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
            
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var user = GetUserByUsername(username);
            if(user == null)
            {
                return NotFound("User not found");
            }
            
            return Ok(user);
        }
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromForm]User userToSignIn)
        {
            var userFromDb = GetUserByUsername(userToSignIn.Username);
            if(userFromDb == null)
            {
                return NotFound("User not found");
            }
            if(userToSignIn.Password == userFromDb.Password) {
                return Ok(new {Number = userFromDb.Number, Name = userFromDb.Name, Username = userFromDb.Username});
            }
            
            return BadRequest("Username or password is incorrect.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm]User userToRegister)
        {
            var userFromDb = GetUserByUsername(userToRegister.Username);
            if(userFromDb != null)
            {
                return BadRequest("User already exists");
            }
            RegisterUser(userToRegister);
            return Ok(userToRegister);
        }

        private User GetUserByUsername(string username) 
        {
            MySqlCommand cmd = OpenMySqlConnection().CreateCommand();
            cmd.CommandText = $"select * from users where username = \"{username}\"";
            var user = new User();
            try
            {
                DbDataReader reader;
                reader = cmd.ExecuteReader();
                reader.Read();
                user = new User{
                    Number = Convert.ToInt32(reader[0]),
                    Name = reader[1].ToString(),
                    Username = reader[2].ToString(),
                    Password = reader[3].ToString()
                };
                reader.Close();
            }
            catch (System.Exception ex)
            {
                return null;
            }
            
            return user;
        }

        private void RegisterUser(User userToRegister) 
        {
            MySqlCommand cmd = OpenMySqlConnection().CreateCommand();
            cmd.CommandText = $"insert into users values (0, '{userToRegister.Name}', '{userToRegister.Username}', '{userToRegister.Password}')";
            cmd.ExecuteNonQuery();
        }

        private MySqlConnection OpenMySqlConnection()
        {
            var _conn = new MySqlConnection(_config.GetSection("MySqlConnectionString").Value);
            try
            {
                Console.WriteLine("Openning Connection ...");

                _conn.Open();

                Console.WriteLine("Connection successful!");
                return _conn;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return null;
        }
    }
}