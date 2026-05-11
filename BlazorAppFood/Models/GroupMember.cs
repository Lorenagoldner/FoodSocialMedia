namespace BlazorAppFood.Models
{
    public class GroupMember
    {
        public int Id_User { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserPhoto { get; set; }
        public bool IsAdmin { get; set; }
    }
}
