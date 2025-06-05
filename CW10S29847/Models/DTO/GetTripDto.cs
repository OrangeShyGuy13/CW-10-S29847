namespace CW10S29847.Models.DTO;

public class GetTripDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<GetCountryDto> Countries { get; set; }
    public List<GetClientDto> Clients { get; set; }
}