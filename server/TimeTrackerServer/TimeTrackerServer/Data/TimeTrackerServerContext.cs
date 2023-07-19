using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTrackerServer.Models;

namespace TimeTrackerServer.Data
{
    public class TimeTrackerServerContext : DbContext
    {
        public TimeTrackerServerContext (DbContextOptions<TimeTrackerServerContext> options)
            : base(options)
        {
        }

        public DbSet<TimeTrackerServer.Models.Budget> Budget { get; set; } = default!;
    }
}
