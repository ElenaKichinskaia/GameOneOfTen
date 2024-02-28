using System.ComponentModel.DataAnnotations.Schema;

namespace GameOneOfTen.Models
{
    [Table("BetHistory")]
    public class BetHistory
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public required int Value { get; set; }
        public required int Number { get; set; }
        public BetStatus Result { get; set; }
        public required DateTime Date { get; set; }

        public virtual Player Player  { get;set;}

    }
}
