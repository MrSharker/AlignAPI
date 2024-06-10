using AlignAPI.DB.Entities;
using AlignAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Reflection;

namespace AlignAPI.DB
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MissionContext(serviceProvider.GetRequiredService<DbContextOptions<MissionContext>>()))
            {
                if (context.Missions.Any())
                {
                    return;
                }
                var geocodingHelper = serviceProvider.GetRequiredService<GeocodingHelper>();
                var missions = new List<Mission> {
                    new Mission
                    {
                        Agent = "007",
                        Country = "Brazil",
                        Address = "Avenida Vieira Souto 168 Ipanema, Rio de Janeiro",
                        Date = DateTime.Parse("1995-12-17T21:45:17")
                    },
                    new Mission
                    {
                        Agent = "005",
                        Country = "Poland",
                        Address = "Rynek Glowny 12, Krakow",
                        Date = DateTime.Parse("2011-04-05T17:05:12")
                    },
                    new Mission
                    {
                        Agent = "007",
                        Country = "Morocco",
                        Address = "27 Derb Lferrane, Marrakech",
                        Date = DateTime.Parse("2001-01-01T00:00:00")
                    },
                    new Mission
                    {
                        Agent = "005",
                        Country = "Brazil",
                        Address = "Rua Roberto Simonsen 122, Sao Paulo",
                        Date = DateTime.Parse("1986-05-05T08:40:23")
                    },
                    new Mission
                    {
                        Agent = "011",
                        Country = "Poland",
                        Address = "swietego Tomasza 35, Krakow",
                        Date = DateTime.Parse("1997-09-07T19:12:53")
                    },
                    new Mission
                    {
                        Agent = "003",
                        Country = "Morocco",
                        Address = "Rue Al-Aidi Ali Al-Maaroufi, Casablanca",
                        Date = DateTime.Parse("2012-08-29T10:17:05")
                    },
                    new Mission
                    {
                        Agent = "008",
                        Country = "Brazil",
                        Address = "Rua tamoana 418, tefe",
                        Date = DateTime.Parse("2005-11-10T13:25:13")
                    },
                    new Mission
                    {
                        Agent = "013",
                        Country = "Poland",
                        Address = "Zlota 9, Lublin",
                        Date = DateTime.Parse("2002-10-17T10:52:19")
                    },
                    new Mission
                    {
                        Agent = "002",
                        Country = "Morocco",
                        Address = "Riad Sultan 19, Tangier",
                        Date = DateTime.Parse("2017-01-01T17:00:00")
                    },
                    new Mission
                    {
                        Agent = "009",
                        Country = "Morocco",
                        Address = "atlas marina beach, agadir",
                        Date = DateTime.Parse("2016-12-01T21:21:21")
                    }
                };
                foreach (var mission in missions)
                {
                    var coordinates = await geocodingHelper.GetCoordinatesAsync(mission.Address);
                    mission.Location = new Point(coordinates.Longitude, coordinates.Latitude) { SRID = 4326 }; 
                    await Task.Delay(1000);
                }

                context.Missions.AddRange(missions);
                await context.SaveChangesAsync();
            }
        }
    }
}
