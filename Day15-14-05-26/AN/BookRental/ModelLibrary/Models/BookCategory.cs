using System.Collections.Generic;

namespace ModelLibrary.Models
{
    public class BookCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; } = new List<Book>();

        public override string ToString()
        {
            return $"ID: {CategoryId} | Name: {CategoryName}";
        }
    }
}
