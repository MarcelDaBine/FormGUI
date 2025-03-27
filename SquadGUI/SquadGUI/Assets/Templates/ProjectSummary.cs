using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SquadGUI.Assets.Templates;

public class ProjectSummary
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastEdited { get; set; }
    public string? ClientName { get; set; }
}

public class ProjectSummaryItem
{
    public ProjectSummary Model { get; }
    public ICommand OpenCommand { get; }

    public ProjectSummaryItem(ProjectSummary model, ICommand openCommand)
    {
        Model = model;
        OpenCommand = openCommand;
    }
}


public class SummaryResponse
{
    public List<ProjectSummary> Content { get; set; } = new();
}
