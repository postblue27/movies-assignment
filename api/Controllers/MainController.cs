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

        public static MySqlConnection
                 GetDBConnection(string host, int port, string database, string username, string password)
        {
            // Connection String.
            String connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password;

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            MySqlConnection conn = new MySqlConnection(_config.GetSection("MySqlConnectionString").Value);
            try
            {
                Console.WriteLine("Openning Connection ...");

                conn.Open();

                Console.WriteLine("Connection successful!");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

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
    }
}
