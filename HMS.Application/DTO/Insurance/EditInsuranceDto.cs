namespace HMS.Application.DTO.Insurance
{
    public class EditInsuranceDto
    {
        public int Id { get; set; }

        public string ProviderName { get; set; }

        public string PolicyNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
