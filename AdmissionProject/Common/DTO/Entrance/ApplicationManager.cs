namespace Common.DTO.Entrance
{
    public class TakeApplication
    {
        public Guid ApplicationId { get; set; }
        public Guid ManagerId { get; set; }
    }

    public class RefuseApplication: TakeApplication
    {
    }
}
