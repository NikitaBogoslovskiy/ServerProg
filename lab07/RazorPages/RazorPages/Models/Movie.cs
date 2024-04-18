namespace RazorPages.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Budget { get; set; }
        public string? Homepage { get; set; }
        public string? Overview { get; set; }
        public string? Popularity { get; set; }
        public string? ReleaseDate { get; set; }
        public string? Revenue { get; set; }
        public string? Runtime { get; set; }
        public string? MovieStatus { get; set; }
        public string? Tagline { get; set; }
        public string? VoteAverage { get; set; }
        public string? VoteCount { get; set; }

        public List<Person>? Persons { get; set; }
        public List<MoviePerson>? MoviePersons { get; set; }
    }
}
