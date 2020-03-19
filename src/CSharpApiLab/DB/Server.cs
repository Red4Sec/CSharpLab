using System.ComponentModel.DataAnnotations;

namespace CSharpApiLab.DB
{
    public class Server
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Ip { get; set; } = "127.0.0.1";
    }
}
