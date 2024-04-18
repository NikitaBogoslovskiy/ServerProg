namespace RazorPages.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<Movie>? Movies { get; set; }
        public List<MoviePerson>? MoviePersons { get; set; }
    }
}
