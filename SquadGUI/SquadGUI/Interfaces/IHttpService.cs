using System.Collections.Generic;
using System.Threading.Tasks;
using SquadGUI.Assets.Templates;

namespace SquadGUI.Interfaces;

public interface IHttpService
{
    Task<string> SubmitAsync(string data, int id);
    Task<string> SaveAsync(string data, int id);
    Task<string> Authenticate(string email, string password);
    Task<List<ProjectSummary>> GetSummariesAsync(int page = 0, int size = 20, string? status = null);
    Task<int> CreateNewFileId();
    Task<ReportModel?> GetFormByIdAsync(int id);
}