namespace ExpenseTracker.DTOs;

public class DailyReportDto
{
    public DateOnly Date { get; set; }
    public decimal TotalIncomes { get; set; }
    public decimal TotalExpenses { get; set; }
    public List<OperationDto>  Operations { get; set; }
}