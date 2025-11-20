using Domain.Models;
using Domain.ServiceInterfaces;
using ExpenseTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize (Roles = "user")]
public class OperationController : ControllerBase
{
   private readonly IOperationService _service;

   public OperationController(IOperationService service)
   {
      _service = service;
   }

   [HttpGet]
   [Route("GetOperations")]
   public async Task<IActionResult> GetOperations()
   {
      var operations = await _service.GetAllOperations();
      if (operations == null)
      {
         return NotFound("No operations found in database");
      }
      return Ok(operations);
   }

   [HttpGet]
   [Route("GetOperationsByCategory")]
   public async Task<IActionResult> GetOperationsByCategory(Guid categoryId)
   {
      var operations = await _service.GetAllByCategory(categoryId);
      if (operations == null)
         return NotFound("No operations found for this category");
      return Ok(operations);
   }

   [HttpGet]
   [Route("GetById/{id}")]
   public async Task<IActionResult> GetOperationById(Guid id)
   {
      var operation = await _service.GetOperationById(id);
      if (operation == null)
      {
         return NotFound("Operation not found");
      }
      return Ok(operation);
   }

   [HttpGet]
   [Route("GetByDate/{date}")]
   public async Task<IActionResult> GetOperation(DateOnly date)
   {
      var operations = await _service.GetOperationsByDate(date);
      if (operations == null)
      {
         return NotFound("No operations for the given date");
      }
      return Ok(operations);
   }

   [HttpPut]
   [Route("UpdateById/{id}")]
   public async Task<IActionResult> UpdateOperation(Guid id,[FromBody]OperationDto operationDto)
   {
      var updated = await _service.UpdateOperationAsync(id,new Operation
      {
         Id = id,
         Date = operationDto.Date,
         Description = operationDto.Description,
         Amount = operationDto.Amount,
         CategoryId = operationDto.CategoryId
      });
      if (updated == null)
      {
         return NotFound($"Operation with id {id} not found.");
      }
      return Ok(updated);
   }

   [HttpPost]
   [Route("AddOperation")]
   public async Task<ActionResult<Operation>> CreateOperation([FromBody] OperationDto operationDTO)
   {
      if (operationDTO == null)
         return BadRequest("Operation data must be provided.");
      var created = await _service.CreateOperation(new Operation
      { 
         Date = operationDTO.Date,
         Amount = operationDTO.Amount,
         Description = operationDTO.Description,
         CategoryId = operationDTO.CategoryId,
      });
      if (created == null)
         return BadRequest("Operation not created.");
      return CreatedAtAction(nameof(GetOperationById), new { id = created.Id }, created);
   }

   [HttpDelete]
   [Route("Delete/{id}")]
   public async Task<IActionResult> DeleteOperation(Guid id)
   {
      var deleted = await _service.DeleteOperation(id);
      if (!deleted)
      {
         return NotFound($"Operation with id {id} not found.");
      }
      return NoContent();
   }
}