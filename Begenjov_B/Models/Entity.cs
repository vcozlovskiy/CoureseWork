namespace Begenjov_B.Models.Account
{
    public class Entity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public User Ovner { get; set; }
        public DateTime PublesherDate { get; set; }
    }
}
