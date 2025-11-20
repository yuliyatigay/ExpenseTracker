namespace ExpenseTracker.DTOs;

public class PeriodReportDto
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalIncomes { get; set; }
    public decimal TotalExpenses { get; set; }
    public List<OperationDto> Operations { get; set; }
}