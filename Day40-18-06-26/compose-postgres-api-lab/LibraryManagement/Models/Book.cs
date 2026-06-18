namespace LibraryManagement.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public int AvailableCopies { get; set; }

        public Book()
        {
        }

        public Book(string title, string author, string isbn, int publishedYear, int availableCopies)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            PublishedYear = publishedYear;
            AvailableCopies = availableCopies;
        }
    }
}