using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;

        public MainController(ILogger<MainController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            MySqlConnection conn = OpenMySqlConnection();

            MySqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "select * from movies";

            DbDataReader reader = cmd.ExecuteReader();
            // reader.Read();
            List<Movie> moviesList = new List<Movie>();
            while(reader.Read()){
                moviesList.Add(
                    new Movie{
                        Number = Convert.ToInt32(reader[0]), 
                        Id = reader[1].ToString(),
                        ImdbId = reader[2].ToString(),
                        OriginalTitle = reader[3].ToString(),
                        Genres = reader[4].ToString(),
                        Overview = reader[5].ToString(),
                        PosterPath = reader[6].ToString(),
                        ReleaseDate = reader[7].ToString(),
                        Runtime = Convert.ToInt32(reader[8]),
                        VoteAverage = Convert.ToDouble(reader[9]),
                        VoteCount = Convert.ToInt32(reader[10]),
                        Status = reader[11].ToString()
                    }
                );
                // Console.WriteLine($"{reader[0]}-{reader[1]}-{reader[2]}-{reader[3]}");
            }

            return Ok(moviesList);
        }

        [HttpPost("get-movies-by-status")]
        public async Task<IActionResult> GetMoviesByStatus([FromForm]UserStatus userStatus)
        {
            MySqlConnection conn = OpenMySqlConnection();

            MySqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = $"select * from user_status where user_number = {userStatus.UserNumber} AND status = '{userStatus.Status}'";

            DbDataReader reader = cmd.ExecuteReader();
            // reader.Read();
            List<UserStatus> moviesList = new List<UserStatus>();
            while(reader.Read()){
                moviesList.Add(
                    new UserStatus{
                        UserNumber = Convert.ToInt32(reader[0]), 
                        MovieNumber = Convert.ToInt32(reader[1]), 
                        Status = reader[2].ToString(),
                        PosterPath = reader[3].ToString(),
                        OriginalTitle = reader[4].ToString(),
                        Overview = reader[5].ToString()
                    }
                );
            }

            return Ok(moviesList);
        }

        [HttpPost("change-movie-status")]
        public async Task<IActionResult> ChangeMovieStatus([FromForm]UserStatus userStatus)
        {
            var userStatusFromDb = GetUserStatus(userStatus);
            if(userStatusFromDb == null) 
            {
                MySqlCommand cmd = OpenMySqlConnection().CreateCommand();
                cmd.CommandText = $"insert into user_status values ({userStatus.UserNumber}, {userStatus.MovieNumber}, '{userStatus.Status}', '{userStatus.PosterPath}', \"{userStatus.OriginalTitle}\", \"{userStatus.Overview}\")";
                cmd.ExecuteNonQuery();
                return Ok(userStatus);
            }
            else
            {
                MySqlCommand cmd = OpenMySqlConnection().CreateCommand();
                cmd.CommandText = $"update user_status set status = '{userStatus.Status}' where user_number = {userStatus.UserNumber} AND movie_number = {userStatus.MovieNumber}";
                cmd.ExecuteNonQuery();
                return Ok(userStatus);
            }
        }

        [HttpPost("get-user-status")]
        public async Task<IActionResult> GetUserSta([FromForm]UserStatus userStatus)
        {
            var userStatusFromDb = GetUserStatus(userStatus);
            return Ok(userStatusFromDb);
        }

        [HttpGet("get-user-status-info/{userNumber}")]
        public async Task<IActionResult> GetUserStaa(int userNumber)
        {
            // var allRecords = select count(*) from user_status where user_number=67
            // mw = select count(*) from user_status where user_number=67 AND status = 'MW'
            // aw = select count(*) from user_status where user_number=67 AND status = 'AW'
            // return Ok(userStatusFromDb);
            MySqlCommand cmdAllRecords = OpenMySqlConnection().CreateCommand();
            cmdAllRecords.CommandText = $"select count(*) from user_status where user_number={userNumber}";
            DbDataReader reader;
            reader = cmdAllRecords.ExecuteReader();
            reader.Read();
            int allRecords = Convert.ToInt32(reader[0]);
            

            MySqlCommand cmdMWRecords = OpenMySqlConnection().CreateCommand();
            cmdMWRecords.CommandText = $"select count(*) from user_status where user_number={userNumber} AND status = 'MW'";
            DbDataReader reader1;
            reader1 = cmdMWRecords.ExecuteReader();
            reader1.Read();
             int MWRecords = Convert.ToInt32(reader1[0]);

            MySqlCommand cmdAWRecords = OpenMySqlConnection().CreateCommand();
            cmdAWRecords.CommandText = $"select count(*) from user_status where user_number={userNumber} AND status = 'AW'";
            DbDataReader reader2;
            reader2 = cmdAWRecords.ExecuteReader();
            reader2.Read();
             int AWRecords = Convert.ToInt32(reader2[0]);

             return Ok(new {AllRecords = allRecords, MWRecords = MWRecords, AWRecords = AWRecords});
        }

        private UserStatus GetUserStatus(UserStatus userStatus)
        {
            MySqlCommand cmd = OpenMySqlConnection().CreateCommand();
            cmd.CommandText = $"select * from user_status where user_number = {userStatus.UserNumber} AND movie_number = {userStatus.MovieNumber}";
            var userStatusFromDb = new UserStatus();
            try
            {
                DbDataReader reader;
                reader = cmd.ExecuteReader();
                reader.Read();
                userStatusFromDb = new UserStatus {
                    UserNumber = Convert.ToInt32(reader[0]), 
                    MovieNumber = Convert.ToInt32(reader[1]), 
                    Status = reader[2].ToString(),
                    PosterPath = reader[3].ToString(),
                    OriginalTitle = reader[4].ToString(),
                    Overview = reader[5].ToString()
                };
                reader.Close();
            }
            catch (System.Exception ex)
            {
                return null;
            }
            
            return userStatusFromDb;
        }

        
        private Movie GetMovieByNumber(int movieNumber)
        {
            MySqlCommand cmd = OpenMySqlConnection().CreateCommand();
            cmd.CommandText = $"select * from movies where number = {movieNumber}";
            var movie = new Movie();
            try
            {
                DbDataReader reader;
                reader = cmd.ExecuteReader();
                reader.Read();
                movie = new Movie {
                    Number = Convert.ToInt32(reader[0]), 
                    Id = reader[1].ToString(),
                    ImdbId = reader[2].ToString(),
                    OriginalTitle = reader[3].ToString(),
                    Genres = reader[4].ToString(),
                    Overview = reader[5].ToString(),
                    PosterPath = reader[6].ToString(),
                    ReleaseDate = reader[7].ToString(),
                    Runtime = Convert.ToInt32(reader[8]),
                    VoteAverage = Convert.ToDouble(reader[9]),
                    VoteCount = Convert.ToInt32(reader[10]),
                    Status = reader[11].ToString()
                };
                reader.Close();
            }
            catch (System.Exception ex)
            {
                return null;
            }
            
            return movie;
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
