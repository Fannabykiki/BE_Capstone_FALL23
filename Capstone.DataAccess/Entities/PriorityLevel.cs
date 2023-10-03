using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class PriorityLevel
    {
        [Key]
        public Guid LevelId { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
