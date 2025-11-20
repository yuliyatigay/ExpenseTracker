using Domain.Models;

namespace Domain.ServiceInterfaces;

public interface IJwtService
{
    string GenerateJwtToken(Account account);
}