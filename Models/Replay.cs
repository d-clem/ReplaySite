namespace ReplaySite.Models
{
    public class Replay
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int WeaponId { get; set; }
        public int MapId { get; set; }	
        public int ModeId { get; set; }
        public int UserId { get; set; }
    }
}