using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using App_tracker.ViewModels;
using App_tracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App_tracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppointmentTrackerContext _context;

        public HomeController(ILogger<HomeController> logger, AppointmentTrackerContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new CreateContainerViewModel();
            viewModel.ContainerTypes = await _context.ContainerTypes.Select(ct => new ContainerTypes { Id = ct.Id, Type = ct.Type }).ToListAsync();
            viewModel.ContainerDepartments = await _context.ContainerDepartments.Select(cd => new ContainerDepartments { Id = cd.Id, Department = cd.Department }).ToListAsync();
            viewModel.Bays = await _context.Bays.Select(b => new SelectListItem() { Value = b.Id.ToString(), Text = b.Bay.ToString() }).ToListAsync();
            viewModel.Comments = new List<ContainerComments>();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContainerViewModel containerVM)
        {
            var container = containerVM.Container;

            //if (ModelState.IsValid)
            //{
            //    _context.Add(movie);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(movie);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
