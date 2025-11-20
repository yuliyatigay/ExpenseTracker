using Domain.ServiceInterfaces;
using ExpenseTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize (Roles = "user")]
public class ReportController : ControllerBase
{
    private readonly IOperationService _operationService;

    public ReportController(IOperationService operationService)
    {
        _operationService = operationService;
    }

    [HttpGet]
    [Route(("DailyReport/{date}"))]

    public async Task<IActionResult> GetDaylyReport(DateOnly date)
    {
        var operationsFromDb = await _operationService.GetOperationsByDate(date);
        if (operationsFromDb == null || !operationsFromDb.Any())
            return NotFound("No reports for the given date");
        
        var listOperations = operationsFromDb.Select(op =>
            new OperationDto()
            {
                Date = date,
                Amount = op.Amount,
                Description = op.Description,
                CategoryId = op.CategoryId,
                Category = op.Category==null? null : new CategoryDto
                {
                    Name = op.Category.Name,
                    CategoryTypes = op.Category.CategoryTypes
                }
            }).ToList();
        var dailyReport = new DailyReportDto
        {
            Date = date,
            TotalIncomes = _operationService.GetTotalIncomes(operationsFromDb),
            TotalExpenses = _operationService.GetTotalExpenses(operationsFromDb),
            Operations = listOperations
        };
        return Ok(dailyReport);
    }

    [HttpGet]
    [Route("PeriodReport/{startDate}/{endDate}")]
    public async Task<IActionResult> GetPeriodReport(DateOnly startDate, DateOnly endDate)
    {
        var operationsFromDb =await _operationService.GetOperationsByPeriod(startDate, endDate);
        if (operationsFromDb == null || !operationsFromDb.Any())
            return NotFound("No reports for the given date");
        
        var listOperations = operationsFromDb
            .Select(op => new OperationDto
            {
                Description = op.Description,
                Amount = op.Amount,
                Date = op.Date,
                CategoryId = op.CategoryId,
                Category = op.Category == null? null : new CategoryDto
                {
                    Name = op.Category.Name,
                    CategoryTypes = op.Category.CategoryTypes
                },
            });
        var periodReport = new PeriodReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            Operations = listOperations.ToList(),
            TotalExpenses = _operationService.GetTotalExpenses(operationsFromDb),
            TotalIncomes = _operationService.GetTotalIncomes(operationsFromDb)
        };
        return Ok(periodReport);
    }
}