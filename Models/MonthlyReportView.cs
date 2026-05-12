public class MonthlyReportViewModel
{
    public string SystemName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public decimal UptimeHour { get; set; }
    public decimal DowntimeHour { get; set; }
    public decimal Availability { get; set; }
    public string Note { get; set; } = string.Empty;
}