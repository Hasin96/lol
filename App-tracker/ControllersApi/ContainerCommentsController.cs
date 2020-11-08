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
    public class ContainerCommentsController : ControllerBase
    {
        private readonly AppointmentTrackerContext _context;

        public ContainerCommentsController(AppointmentTrackerContext context)
        {
            _context = context;
        }

        // DELETE: api/ContainerComments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ContainerComments>> DeleteContainerComments(int id)
        {
            var containerComments = await _context.ContainerComments.FindAsync(id);
            if (containerComments == null)
            {
                return NotFound("Record not found, may have already been deleted. Try refreshing the page.");
            }

            _context.ContainerComments.Remove(containerComments);
            await _context.SaveChangesAsync();

            return containerComments;
        }
    }
}
