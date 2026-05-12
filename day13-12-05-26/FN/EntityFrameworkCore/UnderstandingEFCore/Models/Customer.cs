using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace UnderstandingEFCoreApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime DateOfBirth { get; set; }


        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Phone: {Phone}, Email: {Email}, DateOfBirth: {DateOfBirth}";
        }
    }
}