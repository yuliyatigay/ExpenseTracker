using Domain.Models;
using Domain.ServiceInterfaces;
using ExpenseTracker.Controllers;
using ExpenseTracker.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenseTrackerTests.ControllerTests;

public class ReportControllerTest
{
    private readonly Mock<IOperationService>  _mockOperationService;
    private readonly ReportController  _reportController;
    private readonly List<Operation> _operations;
    private readonly List<Category>  _categories;

    public ReportControllerTest()
    {
        _mockOperationService = new Mock<IOperationService>();
        _reportController = new ReportController(_mockOperationService.Object);
        _categories = new List<Category>
        {
            new Category() {
                Name = "Income", CategoryTypes = CategoryTypes.Income,
                Id = Guid.NewGuid()},
            new Category() {
                Name = "Expense", CategoryTypes = CategoryTypes.Expense,
                Id = Guid.NewGuid()}
        };
        _operations = new List<Operation>
        {
            new Operation
            {
                Id = Guid.NewGuid(), Date = new DateOnly(2025, 1, 1),
                Amount = 100, Description = "operation 1", CategoryId = _categories[0].Id,
                Category = _categories[0]
            },
            new Operation
            {
                Id = Guid.NewGuid(), Date = new DateOnly(2025, 2, 1),
                Amount = 200, Description = "operation 2", CategoryId = _categories[1].Id,
                Category = _categories[1]
            }
        };

    }

    [Fact]
    public async Task GetDailyReport_ReturnsCorrectReport()
    {
        var date = new DateOnly(2025, 1, 1);
        _mockOperationService.Setup(o => o.GetOperationsByDate(date)).
            ReturnsAsync(new List<Operation>{_operations[0]});
        var reportOperations = new List<Operation>{_operations[0]};
        _mockOperationService.Setup(s => s.GetTotalIncomes(reportOperations))
            .Returns(reportOperations.Where(op => op.Category?.CategoryTypes == CategoryTypes.Income)
                .Sum(op => op.Amount));
        _mockOperationService.Setup(s => s.GetTotalExpenses(reportOperations))
            .Returns(reportOperations.Where(op => op.Category?.CategoryTypes == CategoryTypes.Expense)
                .Sum(op => op.Amount));
        var report = await _reportController.GetDaylyReport(date) as OkObjectResult;
        var dto = Assert.IsType<DailyReportDto>(report.Value);
        
        Assert.Equal(date, dto.Date);
        Assert.Equal(100, dto.TotalIncomes);
        Assert.Equal(0, dto.TotalExpenses);
    }

    [Fact]
    public async Task GetDailyReport_ServiceReturnedNull_ReturnsNotFound()
    {
        var date = new DateOnly(2025, 3, 3);
        _mockOperationService.Setup(o => o.GetOperationsByDate(date))
            .ReturnsAsync((IEnumerable<Operation>)null!);
        
        var result = await _reportController.GetDaylyReport(date);
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal("No reports for the given date", notFound.Value);
    }

    [Fact]
    public async Task GetPeriodReport_ReturnsCorrectReport()
    {
        var start =  new DateOnly(2025, 1, 1);
        var end =  new DateOnly(2025, 2, 1);
        _mockOperationService.Setup(o => 
            o.GetOperationsByPeriod(start, end)).
            ReturnsAsync(_operations);
        _mockOperationService.Setup(s => s.GetTotalIncomes(_operations))
            .Returns(_operations.Where(op => op.Category?.CategoryTypes == CategoryTypes.Income)
                .Sum(op => op.Amount));
        _mockOperationService.Setup(s => s.GetTotalExpenses(_operations))
            .Returns(_operations.Where(op => op.Category?.CategoryTypes == CategoryTypes.Expense)
                .Sum(op => op.Amount));
        var report = await _reportController.GetPeriodReport(start, end) as OkObjectResult;
        var dto = Assert.IsType<PeriodReportDto>(report.Value);
        
        Assert.Equal(100, dto.TotalIncomes);
        Assert.Equal(200, dto.TotalExpenses);
    }
    [Fact]
    public async Task GetPeriodReport_ServiceReturnedNull_ReturnsNotFound()
    {
        var start = new DateOnly(2025, 3, 3);
        var end =  new DateOnly(2025, 4, 3);
        _mockOperationService.Setup(o => o.GetOperationsByPeriod(start, end)).
            ReturnsAsync((List<Operation>)null!);
        
        var result = await _reportController.GetPeriodReport(start, end);
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal("No reports for the given date", notFound.Value);
    }
    
}