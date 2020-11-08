using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App_tracker.Models;

namespace App_tracker.ControllersApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly AppointmentTrackerContext _context;

        public ContainersController(AppointmentTrackerContext context)
        {
            _context = context;
        }

        // PUT: api/Containers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{newStatusId}/{containerId}")]
        public async Task<IActionResult> UpdateContainerStatus(int newStatusId, int containerId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            var containerStatus = await _context.ContainerStatus.FindAsync(newStatusId);

            if (containerStatus == null)
                return NotFound("New status not found");

            var container = await _context.Containers.FindAsync(containerId);

            if (container == null)
                return NotFound("Container not found");

            container.StatusId = containerStatus.Id;

            _context.Update(container);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.LazyLoadingEnabled = true;

            return NoContent();
        }

        [HttpPut("{containerId}")]
        public async Task<IActionResult> ActivateContainer(int containerId, [FromForm] int bayId, [FromForm] int doorId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            var container = await _context.Containers.FindAsync(containerId);
            if (container == null)
                return NotFound("Container not found.");

            var bay = await _context.Bays.FindAsync(bayId);
            if (bay == null)
                return NotFound("Bay not found.");

            var door = await _context.Doors.FindAsync(doorId);
            if (bay == null)
                return NotFound("Door not found.");

            container.BayId = bayId;
            container.DoorId = doorId;

            _context.Update(container);

            await _context.SaveChangesAsync();

            _context.ChangeTracker.LazyLoadingEnabled = true;

            return NoContent();
        }
    }
}
 