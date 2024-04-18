namespace NotificationService.DAL.Entity
{
    public class Notification
    {
        public Guid Id { get; set; }
        public required string NotificationType { get; set; }
        public required string NotificationBody { get; set; }
        public required string NotificationRecipient { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
