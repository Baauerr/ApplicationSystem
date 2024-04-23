using System.ComponentModel.DataAnnotations;


namespace EntranceService.DAL.Entity
{
    public class PassportData
    {
        [Key]
        public Guid ownerId {  get; set; }
        public int series {  get; set; }
        public int number { get; set; }
        public string birthPlace { get; set; }
        public DateTime issueDate { get; set; }
        public string issuePlace { get; set; }
    }
}
