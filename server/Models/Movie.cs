namespace hw4.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PrimaryTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PrimaryImage { get; set; } = string.Empty;
        public int Year { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Language { get; set; } = string.Empty;
        public double Budget { get; set; }
        public double GrossWorldwide { get; set; }
        public string Genres { get; set; } = string.Empty;
        public bool IsAdult { get; set; }
        public int RuntimeMinutes { get; set; }
        public float AverageRating { get; set; }
        public int NumVotes { get; set; }
        public double PriceToRent { get; set; }
        public int RentalCount { get; set; }
        public DateTime? RentStart { get; set; }
        public DateTime? RentEnd { get; set; }
        public double? TotalPrice { get; set; }
        public int? RentalId { get; set; }

        // Empty constructor
        public Movie() { }

        // Constructor with parameters
        public Movie(int id, string url, string primaryTitle, string description, string primaryImage,
                     int year, DateTime releaseDate, string language, double budget, double grossWorldwide,
                     string genres, bool isAdult, int runtimeMinutes, float averageRating, int numVotes)
        {
            Id = id;
            Url = url;
            PrimaryTitle = primaryTitle;
            Description = description;
            PrimaryImage = primaryImage;
            Year = year;
            ReleaseDate = releaseDate;
            Language = language;
            Budget = budget;
            GrossWorldwide = grossWorldwide;
            Genres = genres;
            IsAdult = isAdult;
            RuntimeMinutes = runtimeMinutes;
            AverageRating = averageRating;
            NumVotes = numVotes;
        }


        // Instance methods
        public int InsertMovie()
        {
            DBservices dbs = new DBservices();
            return dbs.InsertMovie(this);
        }

        public int DeleteMovie()
        {
            DBservices dbs = new DBservices();
            return dbs.DeleteMovie(this.Id);
        }

        // Static methods because not creating an instance of Movie

        public static (List<Movie> Movies, int TotalCount) GetAllMovies(int pageNum, int pageSize)
        {
            DBservices dbs = new DBservices();
            return dbs.GetAllMovies(pageNum, pageSize);
        }

        public static List<Movie> GetMoviesByTitle(string title)
        {
            DBservices dbs = new DBservices();
            return dbs.GetMoviesByTitle(title);
        }

        public static List<Movie> GetMoviesByReleaseDate(DateTime startDate, DateTime endDate)
        {
            DBservices dbs = new DBservices();
            return dbs.GetMoviesByReleaseDate(startDate, endDate);
        }

        public static Movie? GetMovieById(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.GetMovieById(id);
        }

        ////-----------------------------------------------------------Methods-----------------------------------------------------------//

        //// Method to insert a new movie into the list
        //public bool Insert()
        //{
        //    if (Id == 0)
        //    {
        //        Id = nextId++;
        //    }
        //    // Check for duplicates based on ID or PrimaryTitle
        //    foreach (var movie in MoviesList)
        //    {
        //        if (movie.Id == Id || movie.PrimaryTitle == PrimaryTitle)
        //        {
        //            Console.WriteLine($"Duplicate movie detected: ID = {Id}, Title = {PrimaryTitle}");
        //            return false;
        //        }
        //    }

        //    Console.WriteLine($"Adding movie: {PrimaryTitle}, ID: {Id}");
        //    MoviesList.Add(this);
        //    return true;
        //}

        //// Method to read all movies in the list
        //public static List<Movie> Read()
        //{
        //    Console.WriteLine($"Movies in collection: {MoviesList.Count}");
        //    foreach (var movie in MoviesList)
        //    {
        //        Console.WriteLine($"Movie: {movie.PrimaryTitle}, ID: {movie.Id}");
        //    }
        //    return MoviesList;
        //}

        //// Method to get a movie by its ID
        //public static List<Movie> GetByTitle(string title)
        //{
        //    List<Movie> selectedList = new List<Movie>();
        //    foreach (Movie m in MoviesList)
        //    {
        //        if (m.PrimaryTitle.ToLower().Contains(title.ToLower()))
        //        {
        //            selectedList.Add(m);
        //        }
        //    }
        //    return selectedList;
        //}

        //// Method to get movies released within a specific date range
        //public static List<Movie> GetByReleaseDate(DateTime startDate, DateTime endDate)
        //{
        //    List<Movie> selectedList = new List<Movie>();
        //    foreach (Movie m in MoviesList)
        //    {
        //        if (m.ReleaseDate.Date >= startDate.Date && m.ReleaseDate.Date <= endDate.Date)
        //        {
        //            selectedList.Add(m);
        //        }
        //    }
        //    return selectedList;
        //}

        //// Method to delete a movie by its ID
        //public static bool Delete(int id)
        //{
        //    foreach (var movie in MoviesList)
        //    {
        //        if (movie.Id == id)
        //        {
        //            MoviesList.Remove(movie);
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}