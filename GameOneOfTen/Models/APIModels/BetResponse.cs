namespace GameOneOfTen.Models.APIModels
{
    public class BetResponse
    {
        public required int PlayerId { get; set; }
        public required string Status { get; set; }
        public required int Points { get; set; }
    }
}
