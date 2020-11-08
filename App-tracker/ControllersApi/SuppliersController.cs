using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App_tracker.Models;
using System.Net;

namespace App_tracker.ControllersApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly AppointmentTrackerContext _context;

        public SuppliersController(AppointmentTrackerContext context)
        {
            _context = context;
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostSuppliers([FromForm] Suppliers suppliers)
        {

            if (suppliers.Supplier == null)
                return BadRequest("No supplier name supplied");

            var supplierName = suppliers.Supplier.Trim().ToLower();
            var duplicateSupplier = _context.Suppliers.Any(s => s.Supplier.Trim().ToLower() == supplierName);

            if (duplicateSupplier)
            {
                return Conflict("This supplier is already saved. Please select it using the select list on the form.");
            }

            _context.Suppliers.Add(suppliers);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetSuppliers", new { id = suppliers.Id }, suppliers);
        }
    }

}
