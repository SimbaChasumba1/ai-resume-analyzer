using backend.Models;

namespace backend.Services;

public interface IJwtService
{
    string Generate(User user);
}
