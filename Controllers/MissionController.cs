using AlignAPI.DB;
using AlignAPI.DB.Entities;
using AlignAPI.dto;
using AlignAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace AlignAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MissionController : Controller
    {
        private readonly MissionContext _context;
        private readonly GeocodingHelper _geocodingHelper;

        public MissionController(MissionContext context, GeocodingHelper geocodingHelper)
        {
            _context = context;
            _geocodingHelper = geocodingHelper;
        }

        [HttpPost]
        public async Task<ActionResult<GetMission>> Post(PostMission mission)
        {
            try
            {
                var newMission = new Mission()
                {
                    Address = mission.Address,
                    Country = mission.Country,
                    Agent = mission.Agent,
                    Date = mission.Date
                };
                var targetCoordinates = await _geocodingHelper.GetCoordinatesAsync(newMission.Address);

                newMission.Location = new Point(targetCoordinates.Longitude, targetCoordinates.Latitude) { SRID = 4326 };

                _context.Missions.Add(newMission);
                await _context.SaveChangesAsync();

                var result = await Get(newMission.Id);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetMission>> Get(int id)
        {
            try
            {
                var mission = await _context.Missions
                    .AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new GetMission
                    {
                        Id = m.Id,
                        Agent = m.Agent,
                        Address = m.Address,
                        Country = m.Country,
                        Date = m.Date,
                    })
                    .FirstOrDefaultAsync();

                if (mission == null)
                {
                    return NotFound();
                }

                return Ok(mission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet()]
        public async Task<ActionResult<List<GetMission>>> GetAll()
        {
            try
            {
                var missions = await _context.Missions
                    .AsNoTracking()
                    .Select(x => new GetMission
                    {
                        Id = x.Id,
                        Agent = x.Agent,
                        Address = x.Address,
                        Country = x.Country,
                        Date = x.Date,
                    })
                    .ToListAsync();

                if (!missions.Any())
                {
                    return NotFound();
                }

                return Ok(missions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("~/countries-by-isolation")]
        public async Task<ActionResult<string>> GetCountriesByIsolation()
        {
            try
            {
                var missions = await _context.Missions.AsNoTracking().ToListAsync();
                var isolationStats = missions.GroupBy(m => m.Country)
                    .Select(g => new
                    {
                        Country = g.Key,
                        IsolatedAgents = g.GroupBy(m => m.Agent).Count(a => a.Count() == 1)
                    })
                    .OrderByDescending(i => i.IsolatedAgents)
                    .FirstOrDefault();

                return Ok(isolationStats != null ? isolationStats.Country : "No data");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("~/find-closest")]
        public async Task<ActionResult<GetMission>> FindClosest([FromBody] string targetLocation)
        {
            try
            {
                Point targetPoint;

                if (IsCoordinates(targetLocation, out double latitude, out double longitude))
                {
                    targetPoint = new Point(longitude, latitude) { SRID = 4326 };
                }
                else
                {
                    var targetCoordinates = await _geocodingHelper.GetCoordinatesAsync(targetLocation);
                    targetPoint = new Point(targetCoordinates.Longitude, targetCoordinates.Latitude) { SRID = 4326 };
                }

                var closestMission = await _context.Missions
                    .AsNoTracking()
                    .OrderBy(m => m.Location.Distance(targetPoint))
                    .Select(m => new GetMission
                    {
                        Id = m.Id,
                        Agent = m.Agent,
                        Address = m.Address,
                        Country = m.Country,
                        Date = m.Date,
                    })
                    .FirstOrDefaultAsync();

                if (closestMission == null)
                {
                    return NotFound();
                }

                return Ok(closestMission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsCoordinates(string input, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;

            var parts = input.Split(',');

            if (parts.Length == 2 &&
                double.TryParse(parts[0].Trim(), out latitude) &&
                double.TryParse(parts[1].Trim(), out longitude))
            {
                return true;
            }

            return false;
        }
    }
}
