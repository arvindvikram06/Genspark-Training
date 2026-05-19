namespace UnderstandingWebApi.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public Car(int id,string brand,string model,int year)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Year = year;
        }
    }
}