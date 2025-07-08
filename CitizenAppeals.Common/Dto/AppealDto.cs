namespace CitizenAppeals.Shared.Dto
{
    public class AppealDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; } 
        public string? AppealNumber { get; set; }
        public DateTime AppealDate { get; set; }
        public string? Executors { get; set; } 
        public string? AppealLink { get; set; }
        public int ViolationType { get; set; }
        public string? Result { get; set; }
    }
}
