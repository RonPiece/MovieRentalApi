using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using hw4.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class DBservices
{
    public DBservices() { }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the appsettings.json 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString("myProjDB");
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }

    //--------------------------------------------------------------------------------------------------
    // Create the SqlCommand
    //--------------------------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {
        SqlCommand cmd = new SqlCommand()
        {
            Connection = con,
            CommandText = spName,
            CommandTimeout = 10,
            CommandType = CommandType.StoredProcedure
        };

        if (paramDic != null)
        {
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // Methods
    //--------------------------------------------------------------------------------------------------
    
    public int InsertMovie(Movie movie)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@Url", movie.Url },
            { "@PrimaryTitle", movie.PrimaryTitle },
            { "@Description", movie.Description },
            { "@PrimaryImage", movie.PrimaryImage },
            { "@Year", movie.Year },
            { "@ReleaseDate", movie.ReleaseDate },
            { "@Language", movie.Language },
            { "@Budget", movie.Budget },
            { "@GrossWorldwide", movie.GrossWorldwide },
            { "@Genres", movie.Genres },
            { "@IsAdult", movie.IsAdult },
            { "@RuntimeMinutes", movie.RuntimeMinutes },
            { "@AverageRating", movie.AverageRating },
            { "@NumVotes", movie.NumVotes },
            { "@PriceToRent", movie.PriceToRent }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_InsertMovie", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public int InsertUser(User user)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@Name", user.Name },
            { "@Password", user.Password },
            { "@Email", user.Email },
            { "@Active", user.Active }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_InsertUser", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public User Login(string email, string plainPassword)
    {
        email = email.Trim();
        plainPassword = plainPassword.Trim();

        SqlConnection con;
        SqlCommand cmd;
        User user = null;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@Email", email }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_Login", con, paramDic);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string hashedPassword = reader["Password"].ToString();
                if (BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword))
                {
                    user = new User
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = hashedPassword,
                        Active = (bool)reader["Active"], 
                        DeletedAt = reader["DeletedAt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DeletedAt"]
                    };
                }
            }

            reader.Close();
            return user;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public int RentMovie(int userId, int movieId, DateTime rentStart, DateTime rentEnd)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@UserId", userId },
            { "@MovieId", movieId },
            { "@RentStart", rentStart },
            { "@RentEnd", rentEnd }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_RentMovie", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public int TransferRental(int rentalId, string toUserEmail)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@RentalId", rentalId },
            { "@ToUserEmail", toUserEmail }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_TransferRental", con, paramDic);

        try
        {
            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    public int UpdateUser(int userId, User user)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@UserId", userId },
            { "@Name", user.Name },
            { "@Password", user.Password },
            { "@Email", user.Email }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_UpdateUser", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public int DeleteUser(int userId)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteUser", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public int DeleteMovie(int movieId)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@MovieId", movieId }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteMovie", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    public int DeleteRental(int userId, int movieId)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@UserId", userId },
            { "@MovieId", movieId }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteRental", con, paramDic);

        try
        {
            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }
    }

    public List<Movie> GetRentedMovies(int userId)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Movie> rentedMovies = new List<Movie>();

        try
        {
            con = connect("myProjDB");  
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetRentedMovies", con, paramDic);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();  

            // Read the data from the SqlDataReader
            while (reader.Read())
            {
                Movie movie = new Movie
                {
                    Id = (int)reader["Id"],
                    Url = reader["Url"].ToString(),
                    PrimaryTitle = reader["PrimaryTitle"].ToString(),
                    Description = reader["Description"].ToString(),
                    PrimaryImage = reader["PrimaryImage"].ToString(),
                    Year = (int)reader["Year"],
                    ReleaseDate = ((DateTime)reader["ReleaseDate"]).Date,
                    Language = reader["Language"].ToString(),
                    Budget = Convert.ToDouble(reader["Budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                    Genres = reader["Genres"].ToString(),
                    IsAdult = (bool)reader["IsAdult"],
                    RuntimeMinutes = (int)reader["RuntimeMinutes"],
                    AverageRating = Convert.ToSingle(reader["AverageRating"]),
                    NumVotes = (int)reader["NumVotes"],
                    PriceToRent = Convert.ToDouble(reader["PriceToRent"]),
                    RentalCount = (int)reader["RentalCount"],
                    RentStart = reader["RentStart"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["RentStart"],
                    RentEnd = reader["RentEnd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["RentEnd"],
                    TotalPrice = reader["TotalPrice"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["TotalPrice"]),
                    RentalId = (int)reader["RentalId"],
                };
                rentedMovies.Add(movie);
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();  
            }
        }

        return rentedMovies;
    }

    public (List<Movie> Movies, int TotalCount) GetAllMovies(int page, int pageSize)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Movie> movies = new List<Movie>();
        int totalCount = 0;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@Page", page },
            { "@PageSize", pageSize }
        };
        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetAllMovies", con, paramDic);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Movie movie = new Movie
                {
                    Id = (int)reader["Id"],
                    Url = reader["Url"].ToString(),
                    PrimaryTitle = reader["PrimaryTitle"].ToString(),
                    Description = reader["Description"].ToString(),
                    PrimaryImage = reader["PrimaryImage"].ToString(),
                    Year = (int)reader["Year"],
                    ReleaseDate = (DateTime)reader["ReleaseDate"],
                    Language = reader["Language"].ToString(),
                    Budget = Convert.ToDouble(reader["Budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                    Genres = reader["Genres"].ToString(),
                    IsAdult = (bool)reader["IsAdult"],
                    RuntimeMinutes = (int)reader["RuntimeMinutes"],
                    AverageRating = Convert.ToSingle(reader["AverageRating"]),
                    NumVotes = (int)reader["NumVotes"],
                    PriceToRent = Convert.ToDouble(reader["PriceToRent"]),
                    RentalCount = (int)reader["RentalCount"]
                };
                movies.Add(movie);
            }

            // Move to next result set for total count
            if (reader.NextResult() && reader.Read())
            {
                totalCount = (int)reader["TotalCount"];
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return (movies, totalCount);
    }

    public Movie GetMovieById(int movieId)
    {
        SqlConnection con;
        SqlCommand cmd;
        Movie movie = null;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@MovieId", movieId }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMovieById", con, paramDic);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                movie = new Movie
                {
                    Id = (int)reader["Id"],
                    Url = reader["Url"].ToString(),
                    PrimaryTitle = reader["PrimaryTitle"].ToString(),
                    Description = reader["Description"].ToString(),
                    PrimaryImage = reader["PrimaryImage"].ToString(),
                    Year = (int)reader["Year"],
                    ReleaseDate = (DateTime)reader["ReleaseDate"],
                    Language = reader["Language"].ToString(),
                    Budget = Convert.ToDouble(reader["Budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                    Genres = reader["Genres"].ToString(),
                    IsAdult = (bool)reader["IsAdult"],
                    RuntimeMinutes = (int)reader["RuntimeMinutes"],
                    AverageRating = Convert.ToSingle(reader["AverageRating"]),
                    NumVotes = (int)reader["NumVotes"],
                    PriceToRent = Convert.ToDouble(reader["PriceToRent"]),
                    RentalCount = (int)reader["RentalCount"]
                };
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return movie;
    }

    public List<Movie> GetMoviesByTitle(string title)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Movie> movies = new List<Movie>();

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@Title", title }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesByTitle", con, paramDic);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Movie movie = new Movie
                {
                    Id = (int)reader["Id"],
                    Url = reader["Url"].ToString(),
                    PrimaryTitle = reader["PrimaryTitle"].ToString(),
                    Description = reader["Description"].ToString(),
                    PrimaryImage = reader["PrimaryImage"].ToString(),
                    Year = (int)reader["Year"],
                    ReleaseDate = (DateTime)reader["ReleaseDate"],
                    Language = reader["Language"].ToString(),
                    Budget = Convert.ToDouble(reader["Budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                    Genres = reader["Genres"].ToString(),
                    IsAdult = (bool)reader["IsAdult"],
                    RuntimeMinutes = (int)reader["RuntimeMinutes"],
                    AverageRating = Convert.ToSingle(reader["AverageRating"]),
                    NumVotes = (int)reader["NumVotes"],
                    PriceToRent = Convert.ToDouble(reader["PriceToRent"]),
                    RentalCount = (int)reader["RentalCount"]
                };
                movies.Add(movie);
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return movies;
    }

    public List<Movie> GetMoviesByReleaseDate(DateTime startDate, DateTime endDate)
    {
        SqlConnection con;
        SqlCommand cmd;
        List<Movie> movies = new List<Movie>();

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@StartDate", startDate.Date },
            { "@EndDate", endDate.Date }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesByReleaseDate", con, paramDic);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Movie movie = new Movie
                {
                    Id = (int)reader["Id"],
                    Url = reader["Url"].ToString(),
                    PrimaryTitle = reader["PrimaryTitle"].ToString(),
                    Description = reader["Description"].ToString(),
                    PrimaryImage = reader["PrimaryImage"].ToString(),
                    Year = (int)reader["Year"],
                    ReleaseDate = (DateTime)reader["ReleaseDate"],
                    Language = reader["Language"].ToString(),
                    Budget = Convert.ToDouble(reader["Budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                    Genres = reader["Genres"].ToString(),
                    IsAdult = (bool)reader["IsAdult"],
                    RuntimeMinutes = (int)reader["RuntimeMinutes"],
                    AverageRating = Convert.ToSingle(reader["AverageRating"]),
                    NumVotes = (int)reader["NumVotes"],
                    PriceToRent = Convert.ToDouble(reader["PriceToRent"]),
                    RentalCount = (int)reader["RentalCount"]
                };
                movies.Add(movie);
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return movies;
    }

    public List<User> GetAllUsers()
    {
        SqlConnection con;
        SqlCommand cmd;
        List<User> users = new List<User>();

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        cmd = CreateCommandWithStoredProcedureGeneral("SP_GetAllUsers", con, null);

        try
        {
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                User user = new User
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Password = reader["Password"].ToString(),
                    Active = (bool)reader["Active"],
                    DeletedAt = reader["DeletedAt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DeletedAt"] // <-- Add this line
                };
                users.Add(user);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
                con.Close();
        }

        return users;
    }

    public int UpdateUserActiveStatus(int userId, bool isActive)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>
        {
            { "@UserId", userId },
            { "@IsActive", isActive }
        };

        cmd = CreateCommandWithStoredProcedureGeneral("SP_UpdateUserActiveStatus", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }
}