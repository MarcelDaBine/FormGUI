using System.Threading.Tasks;

namespace SquadGUI.Interfaces;

public interface IFileSaver
{
    Task SubmitJsonAsync(string json);
    Task SaveJsonAsync(string json);
}