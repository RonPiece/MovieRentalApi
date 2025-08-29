using hw4.DTO;
using hw4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace hw4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        // POST: api/Movie
        [HttpPost]
        public IActionResult Post([FromBody] Movie movie)
        {
            if (movie == null)
                return BadRequest("Invalid movie object.");

            int result = movie.InsertMovie(); // Use instance method

            if (result == -1)
                return BadRequest("Duplicate movie detected (ID or Title).");
            if (result > 0)
                return Ok(movie);
            else
                return BadRequest("Failed to insert movie.");
        }

        // DELETE: api/Movie/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Movie movie = Movie.GetMovieById(id);
            if (movie == null)
                return NotFound("Movie not found.");

            int result = movie.DeleteMovie(); 
            if (result > 0)
                return Ok(result);
            else
                return NotFound("Movie not found or could not be deleted.");
        }

        // DELETE: api/Movie/rent/{userId}/{movieId}
        [HttpDelete("rent/{userId}/{movieId}")]
        public IActionResult DeleteRental(int userId, int movieId)
        {
            try
            {
                RentMovie rental = new RentMovie { UserId = userId, MovieId = movieId };
                int result = rental.DeleteRental();

                if (result > 0)
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Movie/rent
        [HttpPost("rent")]
        public IActionResult Rent([FromBody] RentRequest request)
        {
            if (request == null)
                return BadRequest("Invalid rent request.");

            // --- Backend Validation ---
            // 1. Fetch the movie to check its release date
            var movieToRent = Movie.GetMovieById(request.MovieId);
            if (movieToRent == null)
            {
                return NotFound("Movie not found.");
            }

            // 2. Check if the movie's release date is in the future
            if (movieToRent.ReleaseDate.Date > DateTime.Now.Date)
            {
                return BadRequest("Cannot rent a movie that has not been released yet.");
            }
            // --- End Validation ---

            // Map DTO to model
            RentMovie rental = new RentMovie
            {
                UserId = request.UserId,
                MovieId = request.MovieId,
                RentStart = request.RentStart,
                RentEnd = request.RentEnd
            };

            try
            {
                int result = rental.RentMovieMethod();
                if (result > 0)
                    return Ok(result);
                else
                    return BadRequest("Failed to rent movie.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Movie/transferRental
        [HttpPost("transferRental")]
        public IActionResult TransferRental([FromBody] TransferRentalRequest request)
        {
            try
            {
                RentMovie rental = new RentMovie { RentalId = request.RentalId };
                int result = rental.TransferRental(request.ToUserEmail);

                if (result > 0)
                    return Ok(result);
                else
                    return BadRequest("Transfer failed. Check if the recipient exists and does not already have this rental.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Movie/rented/{userId}
        [HttpGet("rented/{userId}")]
        public IActionResult GetRentedMovies(int userId)
        {
            try
            {
                var rentedMovies = RentMovie.GetRentedMovies(userId);

                if (rentedMovies != null && rentedMovies.Count > 0)
                    return Ok(rentedMovies);
                else
                    return NotFound("No rented movies found for this user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET: api/Movie
        [HttpGet]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (movies, totalCount) = Movie.GetAllMovies(page, pageSize);
            return Ok(new { movies, totalCount });
        }

        // GET: api/Movie/search?title=...
        [HttpGet("search")]
        public IActionResult SearchByTitle([FromQuery] string title)
        {
            var movies = Movie.GetMoviesByTitle(title);
            return Ok(movies);
        }

        // GET: api/Movie/searchByDate?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet("searchByDate")]
        public IActionResult SearchByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var movies = Movie.GetMoviesByReleaseDate(startDate, endDate);
            return Ok(movies);
        }

        // GET: api/Movie/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var movie = Movie.GetMovieById(id);
            if (movie != null)
                return Ok(movie);
            else
                return NotFound();
        }

    }
}