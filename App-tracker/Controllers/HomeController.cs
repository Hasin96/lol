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

        // Seeds to calculate if the current date is a in the 4 day work week and what it is.
        private DateTime _dayOne = new DateTime(2020, 11, 14);
        private DateTime _dayTwo = new DateTime(2020, 11, 15);
        private DateTime _dayThree = new DateTime(2020, 11, 16);
        private DateTime _dayFour = new DateTime(2020, 11, 17);

        public HomeController(ILogger<HomeController> logger, AppointmentTrackerContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(ContainersViewModel viewModel = null)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            DateTime startOf4DayWeek = new DateTime();
            bool askForStartDate = true;

            if (viewModel?.CustomStartDate == null)
            {
                //var today = DateTime.Today;
                var today = new DateTime(2020, 11, 30);

                var difference = (today - _dayOne).Days % 8;
                var difference2 = (today - _dayTwo).Days % 8;
                var difference3 = (today - _dayThree).Days % 8;
                var difference4 = (today - _dayFour).Days % 8;

                if (difference == 0)
                {
                    startOf4DayWeek = today;
                    askForStartDate = false;
                }
                else if (difference2 == 0)
                {
                    startOf4DayWeek = today.AddDays(-1);
                    askForStartDate = false;
                }
                else if (difference3 == 0)
                {
                    startOf4DayWeek = today.AddDays(-2);
                    askForStartDate = false;
                }
                else if (difference4 == 0)
                {
                    startOf4DayWeek = today.AddDays(-3);
                    askForStartDate = false;
                }
            } else
            {
                askForStartDate = false;
                startOf4DayWeek = (DateTime)(viewModel?.CustomStartDate.Value);
            }

            var containers = _context.Containers
                .AsNoTracking()
                .Include(c => c.Bay)
                .Include(c => c.ContainerComments)
                .Include(c => c.Type)
                .Include(c => c.ContainerSuppliers)
                    .ThenInclude(cs => cs.Supplier)
                .Where(c => c.ArrivalDate >= startOf4DayWeek && c.ArrivalDate < startOf4DayWeek.AddDays(5));

            if (string.IsNullOrEmpty(viewModel?.ReferenceNumber) == false)
                containers = containers.Where(c => c.RefNum.Contains(viewModel.ReferenceNumber));

            if (viewModel?.TypeId != 0)
                containers = containers.Where(c => c.TypeId == viewModel.TypeId);

            if (viewModel?.DepartmentId != 0)
                containers = containers.Where(c => c.DepartmentId == viewModel.DepartmentId);

            if (viewModel?.Bay != null)
                containers = containers.Where(c => c.Bay.Bay == viewModel.Bay);

            if (string.IsNullOrEmpty(viewModel?.Supplier) == false)
                containers = containers.Where(c => c.ContainerSuppliers.Any(cs => cs.Supplier.Supplier.Equals(viewModel.Supplier)));

            if (viewModel != null)
            {
                viewModel.ContainerStatuses = await _context.ContainerStatus.AsNoTracking().ToListAsync();
                viewModel.ContainerTypes = await _context.ContainerTypes.AsNoTracking().ToListAsync();
                viewModel.ContainerDepartments = await _context.ContainerDepartments.AsNoTracking().ToListAsync();
                viewModel.Containers = await containers.ToListAsync();
                viewModel.Bays = await _context.Bays.AsNoTracking().Select(b => new SelectListItem() { Value = b.Id.ToString(), Text = b.Bay.ToString() }).ToListAsync();
                viewModel.Doors = await _context.Doors.AsNoTracking().Select(d => new SelectListItem() { Value = d.Id.ToString(), Text = d.Door.ToString() }).ToListAsync();
                viewModel.PromptForStartDate = askForStartDate;
                viewModel.StartOf4DayWeekDate = startOf4DayWeek;
            } else
            {
                viewModel = new ContainersViewModel()
                {
                    ContainerStatuses = await _context.ContainerStatus.AsNoTracking().ToListAsync(),
                    ContainerTypes = await _context.ContainerTypes.AsNoTracking().ToListAsync(),
                    ContainerDepartments = await _context.ContainerDepartments.AsNoTracking().ToListAsync(),
                    Containers = await containers.ToListAsync(),
                    Bays = await _context.Bays.AsNoTracking().Select(b => new SelectListItem() { Value = b.Id.ToString(), Text = b.Bay.ToString() }).ToListAsync(),
                    Doors = await _context.Doors.AsNoTracking().Select(d => new SelectListItem() { Value = d.Id.ToString(), Text = d.Door.ToString() }).ToListAsync(),
                    PromptForStartDate = askForStartDate,
                    StartOf4DayWeekDate = startOf4DayWeek,
                };
            }

            _context.ChangeTracker.LazyLoadingEnabled = true;

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            return View("AppointmentForm", await CreateViewModelWithPopulatedFormInputs());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContainerViewModel containerVM)
        {

            if (ModelState.IsValid == false)
            {
                return View(await CreateViewModelWithPopulatedFormInputs());
            }

            int? itemD = null;
            if (containerVM.ExpNumOfUnits != null && containerVM.ActNumOfUnits != null)
                itemD = containerVM.ExpNumOfUnits.Value - containerVM.ActNumOfUnits.Value;

            var container = new Containers()
            {
                RefNum = containerVM.RefNum,
                ExpTimeOfArrival = containerVM.ExpTimeOfArrival,
                ExpNumOfUnits = containerVM.ExpNumOfUnits,
                ExpNumOfPallets = containerVM.ExpNumOfPallets,
                ActTimeOfArrival = containerVM.ActTimeOfArrival,
                ActNumOfUnits = containerVM.ActNumOfUnits,
                ActNumOfPallets = containerVM.ActNumOfPallets,
                ArrivalDate = DateTime.Today,
                DepartmentId = containerVM.ContainerDepartmentId,
                TypeId = containerVM.ContainerTypeId,
                BayId = containerVM.BayId,
                DoorId = containerVM.DoorId,
                StatusId = containerVM.ContainerStatusId,
                ItemDiscrepancy = (containerVM.ExpNumOfUnits.HasValue && containerVM.ActNumOfUnits.HasValue) ? (containerVM.ExpNumOfUnits.Value - containerVM.ActNumOfUnits.Value) : (int?)null
            };

            _context.Add(container);

            await _context.SaveChangesAsync();

            foreach (var comment in containerVM.Comments)
            {
                var containerComment = new ContainerComments()
                {
                    ContainerId = container.Id,
                    Comment = comment.Comment
                };

                _context.Add(containerComment);
            }

            if (containerVM.SupplierIds?.Count() > 0)
            {
                foreach(var supplierId in containerVM.SupplierIds)
                {
                    var containerSuppliers = new ContainerSuppliers()
                    {
                        ContainerId = container.Id,
                        SupplierId = supplierId.Value
                    };

                    _context.Add(containerSuppliers);
                }
                
            }

            await _context.SaveChangesAsync();
                
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            if (id == null)
                return BadRequest("Not a valid container id");

            var container = await _context.Containers.Include(c => c.ContainerComments).Include(c => c.ContainerSuppliers).Where(c => c.Id == id).Select(c =>
            new CreateContainerViewModel
            {
                ContainerId = c.Id,
                RefNum = c.RefNum,
                ExpTimeOfArrival = c.ExpTimeOfArrival.Value,
                ExpNumOfPallets = c.ExpNumOfPallets.Value,
                ExpNumOfUnits = c.ExpNumOfUnits.Value,
                ActTimeOfArrival = c.ActTimeOfArrival.Value,
                ActNumOfPallets = c.ActNumOfPallets.Value,
                ActNumOfUnits = c.ActNumOfUnits.Value,
                SupplierIds = c.ContainerSuppliers.Select(cs => (int?)cs.SupplierId).ToList(),
                BayId = c.BayId,
                DoorId = c.DoorId,
                ContainerTypeId = c.TypeId,
                ContainerDepartmentId = c.DepartmentId,
                ContainerStatusId = c.StatusId,
                Comments = c.ContainerComments.ToList(),
                })
                .FirstOrDefaultAsync();

            if (container == null)
                return NotFound("Container record not found");

            container.ContainerTypes = await _context.ContainerTypes.AsNoTracking().Select(ct => new ContainerTypes { Id = ct.Id, Type = ct.Type }).ToListAsync();
            container.ContainerDepartments = await _context.ContainerDepartments.AsNoTracking().Select(cd => new ContainerDepartments { Id = cd.Id, Department = cd.Department }).ToListAsync();
            container.Bays = await _context.Bays.AsNoTracking().Select(b => new SelectListItem() { Value = b.Id.ToString(), Text = b.Bay.ToString() }).ToListAsync();
            container.Doors = await _context.Doors.AsNoTracking().Select(d => new SelectListItem() { Value = d.Id.ToString(), Text = d.Door }).ToListAsync();
            container.Suppliers = await _context.Suppliers.AsNoTracking().OrderBy(s => s.Supplier).Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Supplier }).ToListAsync();
            container.Statuses = await _context.ContainerStatus.AsNoTracking().Select(cs => new SelectListItem() { Value = cs.Id.ToString(), Text = cs.Status }).ToListAsync();

            return View("AppointmentForm", container);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CreateContainerViewModel containerVM)
        {
            if (ModelState.IsValid == false)
            {
                return View(await CreateViewModelWithPopulatedFormInputs());
            }

            var container = await _context.Containers.FindAsync(containerVM.ContainerId);

            if (container == null)
                return NotFound("Container record not found");

            container.RefNum = containerVM.RefNum;
            container.ExpTimeOfArrival = containerVM.ExpTimeOfArrival;
            container.ExpNumOfUnits = containerVM.ExpNumOfUnits;
            container.ExpNumOfPallets = containerVM.ExpNumOfPallets;
            container.ActTimeOfArrival = containerVM.ActTimeOfArrival;
            container.ActNumOfUnits = containerVM.ActNumOfUnits;
            container.ActNumOfPallets = containerVM.ActNumOfPallets;
            container.ArrivalDate = DateTime.Today;
            container.DepartmentId = containerVM.ContainerDepartmentId;
            container.TypeId = containerVM.ContainerTypeId;
            container.BayId = containerVM.BayId;
            container.DoorId = containerVM.DoorId;
            container.StatusId = containerVM.ContainerStatusId;
            container.ItemDiscrepancy = (containerVM.ExpNumOfUnits.HasValue && containerVM.ActNumOfUnits.HasValue) ? (containerVM.ExpNumOfUnits.Value - containerVM.ActNumOfUnits.Value) : (int?)null;

            _context.Update(container);

            foreach (var comment in containerVM.Comments)
            {
                if (comment.Id == 0)
                {
                    var containerComment = new ContainerComments()
                    {
                        ContainerId = container.Id,
                        Comment = comment.Comment
                    };

                    _context.Add(containerComment);
                } else if (comment.Id != 0)
                {
                    _context.Update(comment);
                }
            }

            if (containerVM.SupplierIds.Count() > 0)
            {
                var savedSuppliers = await _context.ContainerSuppliers.Where(cs => cs.ContainerId == container.Id).ToListAsync();
                var suppliersToDel = new List<ContainerSuppliers>(savedSuppliers);
                    
                foreach (var supplierId in containerVM.SupplierIds)
                {
                    bool addThisSupplier = true;
                    foreach (var savedSupplier in savedSuppliers)
                    {
                        if (savedSupplier.SupplierId == supplierId)
                        {
                            addThisSupplier = false;
                            suppliersToDel.Remove(savedSupplier);
                        }
                    }

                    if (addThisSupplier)
                    {
                        var containerSuppliers = new ContainerSuppliers()
                        {
                            ContainerId = container.Id,
                            SupplierId = supplierId.Value
                        };

                        _context.Add(containerSuppliers);
                    }
                    
                }

                _context.RemoveRange(suppliersToDel);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

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

        private async Task<CreateContainerViewModel> CreateViewModelWithPopulatedFormInputs()
        {
            var containerVM = new CreateContainerViewModel();
            containerVM.ContainerTypes = await _context.ContainerTypes.AsNoTracking().Select(ct => new ContainerTypes { Id = ct.Id, Type = ct.Type }).ToListAsync();
            containerVM.ContainerDepartments = await _context.ContainerDepartments.AsNoTracking().Select(cd => new ContainerDepartments { Id = cd.Id, Department = cd.Department }).ToListAsync();
            containerVM.Bays = await _context.Bays.AsNoTracking().Select(b => new SelectListItem() { Value = b.Id.ToString(), Text = b.Bay.ToString() }).ToListAsync();
            containerVM.Doors = await _context.Doors.AsNoTracking().Select(d => new SelectListItem() { Value = d.Id.ToString(), Text = d.Door }).ToListAsync();
            containerVM.Suppliers = await _context.Suppliers.AsNoTracking().OrderBy(s => s.Supplier).Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Supplier }).ToListAsync();
            containerVM.Statuses = await _context.ContainerStatus.AsNoTracking().Select(cs => new SelectListItem() { Selected = cs.Id == 1 ? true : false, Value = cs.Id.ToString(), Text = cs.Status }).ToListAsync();

            return containerVM;
        }
    }
}
