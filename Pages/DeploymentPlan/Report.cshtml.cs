using ACE_Deployment_Tracking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.Globalization;

public class ReportModel : PageModel
{
    private readonly AppDbContext _context;

    public ReportModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? SelectedMonth { get; set; }
    public decimal TotalMonthHours { get; set; }
    public List<MonthlyReportViewModel> Reports { get; set; } = new();

    public async Task OnGetAsync()
    {
        DateTime selectedDate;

        if (string.IsNullOrEmpty(SelectedMonth))
        {
            selectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    
            SelectedMonth = selectedDate.ToString("yyyy-MM");
        }
        else
        {
            selectedDate = DateTime.Parse(SelectedMonth + "-01");
        }
    
        var fromDate = new DateTime(selectedDate.Year, selectedDate.Month, 1);
    
        var toDate = fromDate.AddMonths(1);
    
        int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
    
        decimal totalHours = daysInMonth * 24;
        TotalMonthHours = totalHours;

        Reports = await _context.DeploymentSchedules
            .Where(x => x.PlannedDate >= fromDate &&
                        x.PlannedDate < toDate)
            .GroupBy(x => x.SystemName)
            .Select(g => new MonthlyReportViewModel
            {
                SystemName = g.Key,
                TotalHours = totalHours,
    
                DowntimeHour = g.Sum(x => x.ActualHour ?? 0),
    
                UptimeHour = totalHours - g.Sum(x => x.ActualHour ?? 0),
    
                Availability =
                    ((totalHours - g.Sum(x => x.ActualHour ?? 0))
                    / totalHours) * 100
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnGetExportAsync()
    {
        await OnGetAsync();

        var monthText = DateTime.Parse(SelectedMonth + "-01")
            .ToString("yyyy MMMM", CultureInfo.InvariantCulture);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Availability Report");

        ws.Range("A1:G1").Merge();
        ws.Cell("A1").Value = $"System Availability Details [{monthText}]";
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 14;
        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#9999FF");

        string[] headers =
        {
        "#", "System Name", "Total Hours in Month",
        "Uptime Hours", "Downtime Hours", "Availability (%)", "Note"
    };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(2, i + 1).Value = headers[i];
        }

        ws.Range("A2:G2").Style.Fill.BackgroundColor = XLColor.FromHtml("#D5DBE3");
        ws.Range("A2:G2").Style.Font.Bold = true;
        ws.Range("A2:G2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var systems = new List<string>
    {
        "ACE IS - Contract & Project Management System",
        "ACE IS - Integrated Accounting Systems",
        "ACE IS - Human Resource Management Systems",
        "Bitrix 24 CRM System",
        "Backlog System",
        "ACE Issue Tracking System"
    };

        int row = 3;
        int no = 1;

        foreach (var system in systems)
        {
            var report = Reports.FirstOrDefault(x => x.SystemName == system);

            decimal totalHours = report?.TotalHours ?? TotalMonthHours;
            decimal downtime = report?.DowntimeHour ?? 0;
            decimal uptime = report?.UptimeHour ?? TotalMonthHours;
            decimal availability = report?.Availability ?? 100;

            ws.Cell(row, 1).Value = no;
            ws.Cell(row, 2).Value = system;
            ws.Cell(row, 3).Value = totalHours;
            ws.Cell(row, 4).Value = uptime;
            ws.Cell(row, 5).Value = downtime;
            ws.Cell(row, 6).Value = availability / 100;
            ws.Cell(row, 7).Value = "At 99%";

            ws.Cell(row, 6).Style.NumberFormat.Format = "0.00%";

            var rowColor = no <= 3 ? "#E4F0DA" : "#DFE8F5";
            ws.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.FromHtml(rowColor);

            row++;
            no++;
        }

        var usedRange = ws.Range(1, 1, row - 1, 7);
        usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        usedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        ws.Columns().AdjustToContents();
        ws.Column(2).Width = 45;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"System_Availability_Report_{SelectedMonth}.xlsx"
        );
    }
}