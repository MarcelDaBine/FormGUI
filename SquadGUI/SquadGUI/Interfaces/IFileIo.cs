using System.Threading.Tasks;

namespace SquadGUI.Interfaces;

public interface IFileIo
{
    Task SubmitJsonAsync(string json);
    Task SaveJsonAsync(string json);
    Task<T?> OpenJsonAsync<T>();
}