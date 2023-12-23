// Example of Models/Domain/UserNotification.cs
namespace auth.Models.Domain
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsNotified { get; set; }

        // Other properties related to notifications...
    }
}
