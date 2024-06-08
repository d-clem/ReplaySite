namespace ReplaySite.Models
{
    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TwitterName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string LastToken { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, TwitterName: {TwitterName}, Login: {Login}";
        }

    }

}