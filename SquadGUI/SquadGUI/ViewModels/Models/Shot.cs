namespace SquadGUI.ViewModels.Models;

public class Shot {
    private PenetrationLevel _ppCp;
    private ShotPosition _position;
    
    public double Load { get; set; }
    public string? TrackId { get; set; }
    public double VelocityFt { get; set; }
    public double VelocityM { get; set; }
    public bool Used { get; set; }
    public string? Notes { get; set; }
    
    public string PpCp => _ppCp.ToString();
    public string Position => _position.ToString();
}