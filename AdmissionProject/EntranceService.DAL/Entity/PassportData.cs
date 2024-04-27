using System.ComponentModel.DataAnnotations;


namespace EntranceService.DAL.Entity
{
    public class PassportData
    {
        [Key]
        public Guid OwnerId {  get; set; }
        public int Series {  get; set; }
        public int Number { get; set; }
        public string BirthPlace { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuePlace { get; set; }
    }
}
