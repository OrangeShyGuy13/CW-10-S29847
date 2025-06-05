namespace CW10S29847.Models.DTO;

public class GetResponseWithParamsDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<GetTripDto> Trips { get; set; }
}