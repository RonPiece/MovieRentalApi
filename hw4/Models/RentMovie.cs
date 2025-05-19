using System;
using System.Collections.Generic;

namespace hw4.Models
{
    public class RentMovie
    {
        public int RentalId { get; set; } 
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }

        // Instance methods
        public int RentMovieMethod()
        {
            DBservices dbs = new DBservices();
            return dbs.RentMovie(UserId, MovieId, RentStart, RentEnd);
        }

        public int DeleteRental()
        {
            DBservices dbs = new DBservices();
            return dbs.DeleteRental(UserId, MovieId);
        }

        public int TransferRental(string toUserEmail)
        {
            DBservices dbs = new DBservices();
            return dbs.TransferRental(RentalId, toUserEmail);
        }

        // Static methods
        public static List<Movie> GetRentedMovies(int userId)
        {
            DBservices dbs = new DBservices();
            return dbs.GetRentedMovies(userId);
        }
    }
}