using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App_tracker.Models;
using ClosedXML.Excel;
using System.IO;
using System.Diagnostics;

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
            if (door == null)
                return NotFound("Door not found.");

            container.BayId = bayId;
            container.DoorId = doorId;

            _context.Update(container);

            await _context.SaveChangesAsync();

            _context.ChangeTracker.LazyLoadingEnabled = true;

            return NoContent();
        }

        // DELETE: api/Container/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContainer(int id)
        {
            var container = await _context.Containers.FindAsync(id);
            if (container == null)
            {
                return NotFound("Container data not found.");
            }

            _context.Containers.Remove(container);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Route("{startDate}")]
        public async Task<IActionResult> DownloadFile(DateTime startDate)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;

            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = new XLWorkbook();

                var firstDay = startDate;
                var secondDay = startDate.AddDays(1);
                var thirdDay = startDate.AddDays(2);
                var fourthDay = startDate.AddDays(3);

                var worksheet1 = workbook.Worksheets.Add(firstDay.ToString("dd.MM.yy"));
                var worksheet2 = workbook.Worksheets.Add(secondDay.ToString("dd.MM.yy"));
                var worksheet3 = workbook.Worksheets.Add(thirdDay.ToString("dd.MM.yy"));
                var worksheet4 = workbook.Worksheets.Add(fourthDay.ToString("dd.MM.yy"));

                var worksheetForContainers = workbook.Worksheets.Add("Containers");

                AddTableHeaders(worksheet1);
                AddTableHeaders(worksheet2);
                AddTableHeaders(worksheet3);
                AddTableHeaders(worksheet4);
                AddTableHeaders(worksheetForContainers);

                var containers = await _context.Containers
                    .AsNoTracking()
                    .Include(c => c.Bay)
                    .Include(c => c.Door)
                    .Include(c => c.ContainerComments)
                    .Include(c => c.ContainerSuppliers)
                        .ThenInclude(cs => cs.Supplier)
                    .Where(c => c.ArrivalDate >= startDate)
                    .ToListAsync();

                var containerTypes = containers.Where(c => c.TypeId == 1);
                var containerTypesDayOne = containerTypes.Where(ct => ct.ArrivalDate == firstDay);
                var containerTypesDayTwo = containerTypes.Where(ct => ct.ArrivalDate == secondDay);
                var containerTypesDayThree = containerTypes.Where(ct => ct.ArrivalDate == thirdDay);
                var containerTypesDayFour = containerTypes.Where(ct => ct.ArrivalDate == fourthDay);

                var liveTipTypes = containers.Where(c => c.TypeId == 2);
                var liveTipTypesDayOne = liveTipTypes.Where(ltc => ltc.ArrivalDate == firstDay);
                var liveTipTypesDayTwo = liveTipTypes.Where(ltc => ltc.ArrivalDate == secondDay);
                var liveTipTypesDayThree = liveTipTypes.Where(ltc => ltc.ArrivalDate == thirdDay);
                var liveTipTypesDayFourth = liveTipTypes.Where(ltc => ltc.ArrivalDate == fourthDay);

                AddContainerSheetHeaderDate(worksheetForContainers, firstDay);
                AddTableCells(worksheetForContainers, containerTypesDayOne);
                AddContainerSheetHeaderDate(worksheetForContainers, secondDay);
                AddTableCells(worksheetForContainers, containerTypesDayTwo);
                AddContainerSheetHeaderDate(worksheetForContainers, thirdDay);
                AddTableCells(worksheetForContainers, containerTypesDayThree);
                AddContainerSheetHeaderDate(worksheetForContainers, fourthDay);
                AddTableCells(worksheetForContainers, containerTypesDayFour);

                AddTableCells(worksheet1, liveTipTypesDayOne);
                AddTableCells(worksheet2, liveTipTypesDayTwo);
                AddTableCells(worksheet3, liveTipTypesDayThree);
                AddTableCells(worksheet4, liveTipTypesDayFourth);

                worksheet1.Columns(2, 13).AdjustToContents();
                worksheet2.Columns(2, 13).AdjustToContents();
                worksheet3.Columns(2, 13).AdjustToContents();
                worksheet4.Columns(2, 13).AdjustToContents();
                worksheetForContainers.Columns(2, 13).AdjustToContents();

                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                _context.ChangeTracker.LazyLoadingEnabled = false;

                return this.File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                    // By setting a file download name the framework will
                    // automatically add the attachment Content-Disposition header
                    fileDownloadName: "ERSheet.xlsx"
                );
            }
        }
        private void AddTableHeaders(IXLWorksheet worksheet)
        {
            AddTableHeaderTextAndStyles(worksheet.Cell("B2"), "Ref Num");
            AddTableHeaderTextAndStyles(worksheet.Cell("C2"), "Exp Pallets");
            AddTableHeaderTextAndStyles(worksheet.Cell("D2"), "Exp Units");
            AddTableHeaderTextAndStyles(worksheet.Cell("E2"), "Exp Arrival Time");
            AddTableHeaderTextAndStyles(worksheet.Cell("F2"), "Bay");
            AddTableHeaderTextAndStyles(worksheet.Cell("G2"), "Supplier");
            AddTableHeaderTextAndStyles(worksheet.Cell("H2"), "Door");
            AddTableHeaderTextAndStyles(worksheet.Cell("I2"), "Act Pallets");
            AddTableHeaderTextAndStyles(worksheet.Cell("J2"), "Act Units");
            AddTableHeaderTextAndStyles(worksheet.Cell("K2"), "Arrival Time");
            AddTableHeaderTextAndStyles(worksheet.Cell("L2"), "Comments");
            AddTableHeaderTextAndStyles(worksheet.Cell("M2"), "Checksheet");
        }

        private void AddTableHeaderTextAndStyles(IXLCell cell, string text)
        {
            cell.SetValue(text);

            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.OutsideBorderColor = XLColor.Black;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 102, 255);
        }

        private void AddContainerSheetHeaderDate(IXLWorksheet worksheet, DateTime title)
        {
            int rowIndex = worksheet.LastRowUsed().RowNumber() + 1;

            var cell = worksheet.Cell(rowIndex, "G");
            cell.SetValue(title);

            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 102, 255);
            cell.Style.Font.Bold = true;

            IXLRange range = worksheet.Range($"B{rowIndex}:M{rowIndex}");

            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorderColor = XLColor.Black;

            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorderColor = XLColor.Black;

            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorderColor = XLColor.Black;
        }

        private void AddTableCells(IXLWorksheet worksheet, IEnumerable<Containers> containers)
        {
            int rowIndex = worksheet.LastRowUsed().RowNumber() + 1;
            foreach(var container in containers)
            {
                AddTableCell(worksheet, container, rowIndex);
                rowIndex++;
            }

        }

        private void AddTableCell(IXLWorksheet worksheet, Containers container, int rowIndex)
        {
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "B"), container.RefNum);
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "C"), container.ExpNumOfPallets);
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "D"), container.ExpNumOfUnits);
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "E"), container.ExpTimeOfArrival);


            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "F"), container.Bay?.Bay);
            if (container.Bay == null)
                worksheet.Cell(rowIndex, "F").Style.Fill.BackgroundColor = XLColor.Red;
            else if (container.StatusId == 4)
                worksheet.Cell(rowIndex, "F").Style.Fill.BackgroundColor = XLColor.LightGreen;
            else
                worksheet.Cell(rowIndex, "F").Style.Fill.BackgroundColor = XLColor.Yellow;


            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "G"), FormatSuppliers(container.ContainerSuppliers.ToList()));

            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "H"), container.Door?.Door);
            if (container.StatusId != 4 && container.Door != null)
                worksheet.Cell(rowIndex, "H").Style.Fill.BackgroundColor = XLColor.Yellow;
            else if (container.StatusId == 4)
                worksheet.Cell(rowIndex, "H").Style.Fill.BackgroundColor = XLColor.Red;

            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "I"), container.ActNumOfPallets);
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "J"), container.ActNumOfUnits);
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "K"), container.ActTimeOfArrival);

            string formattedComments = "";
            foreach(var comment in container.ContainerComments)
            {
                formattedComments += " - " + comment.Comment + Environment.NewLine;
            }
            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "L"), formattedComments);
            worksheet.Cell(rowIndex, "L").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            AddTableCellTextAndStyles(worksheet.Cell(rowIndex, "M"), true);
        }

        private void AddTableCellTextAndStyles(IXLCell cell, string text)
        {
            cell.SetValue(text);

            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorderColor = XLColor.Black;

            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorderColor = XLColor.Black;

            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
        }

        private void AddTableCellTextAndStyles(IXLCell cell, TimeSpan? text)
        {
            cell.SetValue(text);

            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorderColor = XLColor.Black;

            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorderColor = XLColor.Black;

            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
        }

        private void AddTableCellTextAndStyles(IXLCell cell, int? text)
        {
            cell.SetValue(text);

            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorderColor = XLColor.Black;

            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorderColor = XLColor.Black;

            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
        }

        private void AddTableCellTextAndStyles(IXLCell cell, bool text)
        {
            cell.SetValue(text);

            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorderColor = XLColor.Black;

            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorderColor = XLColor.Black;

            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
        }

        private string FormatSuppliers(List<ContainerSuppliers> containerSuppliers)
        {
            var formattedSuppliers = "";
            for (var i = 0; i < containerSuppliers.Count(); i++)
            {
                if (i == containerSuppliers.Count() - 1)
                {
                    formattedSuppliers += containerSuppliers[i].Supplier?.Supplier;
                } else
                {
                    formattedSuppliers += containerSuppliers[i].Supplier?.Supplier + " - ";
                }
            }
            return formattedSuppliers;
        }
    }
}
 