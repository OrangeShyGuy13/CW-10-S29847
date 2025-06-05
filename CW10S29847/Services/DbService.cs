using CW10S29847.Data;
using CW10S29847.Models;
using CW10S29847.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace CW10S29847.Services;

public interface IDbService
{
    public Task<GetResponseWithParamsDto> GetTrips(int page, int pageSize);
    public Task AssignClientToTrip(int idTrip, PutClientTripDto dto);
    public Task DeleteClient(int idClient);
}

public class DbService(MasterContext data): IDbService
{
    public async Task<GetResponseWithParamsDto> GetTrips(int page, int pageSize)
    {
        var query = data.Trips
            .OrderByDescending(t => t.DateFrom)
            .AsQueryable().Select(t => new GetTripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries
                    .Select(ct => new GetCountryDto
                    {
                        Name = ct.Name
                    }).ToList(),
                Clients = t.ClientTrips
                    .Select(ct => new GetClientDto()
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    }).ToList()
            });

        var totalTrips = await query.CountAsync();
        var allPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var trips = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new GetResponseWithParamsDto()
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Trips = trips
        };
    }
    public async Task AssignClientToTrip(int idTrip, PutClientTripDto dto)
    {
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);

        if (trip == null)
            throw new Exception("Trip not found.");
        if (trip.DateFrom < DateTime.Now)
            throw new Exception("Cannot assign client to a finished trip.");

        var client = await data.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);

        if (client == null)
        {
            client = new Client()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            data.Clients.Add(client);
            await data.SaveChangesAsync();
        }
        else
        {
            if (await data.ClientTrips.FirstOrDefaultAsync(ct =>
                    ct.IdClient == client.IdClient && ct.IdTrip == idTrip) == null)
            {
                throw new Exception("Client already on this trip");
            }
        }
        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        data.ClientTrips.Add(clientTrip);
        await data.SaveChangesAsync();
    }
    public async Task DeleteClient(int idClient)
    {
        var client = await data.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            throw new Exception("Client not found.");
        if (client.ClientTrips.Any())
            throw new Exception("Can't delete, client assigned to one or more trips.");

        data.Clients.Remove(client);
        await data.SaveChangesAsync();
    }

}