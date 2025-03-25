using System.Threading.Tasks;

namespace SquadGUI.Interfaces;

public interface IHttpService
{
    Task<string> SubmitAsync(string data);
    Task<string> SaveAsync(string data);
    Task<string> Authenticate(string email, string password);
}