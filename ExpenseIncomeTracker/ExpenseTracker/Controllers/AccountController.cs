using Domain.Models;
using Domain.ServiceInterfaces;
using ExpenseTracker.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var account = new Account
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PasswordHash = request.Password
        };
        try
        {
            await _accountService.RegisterAsync(account);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        try
        {
            var result = await _accountService.LoginAsync(login.UserName, login.Password);
            return Ok(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        
    }
}