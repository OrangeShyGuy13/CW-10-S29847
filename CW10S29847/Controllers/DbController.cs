using CW10S29847.Models.DTO;
using CW10S29847.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CW10S29847.Controllers;


[ApiController]
[Route("[controller]/api")]
public class DbController(IDbService dbService) : ControllerBase
{
    [HttpGet("/trips")]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var trips = await dbService.GetTrips(page, pageSize);
            return Ok(trips);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("/trips/{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] PutClientTripDto dto)
    {
        try
        {
            await dbService.AssignClientToTrip(idTrip, dto);
            return Ok(new { Message = "Client assigned to trip successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("/clients/{idClient}")]
    public async Task<IActionResult> DeleteClientFromTrip(int idClient)
    {
        try
        {
            await dbService.DeleteClient(idClient);
            return Ok(new { Message = "Client deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}