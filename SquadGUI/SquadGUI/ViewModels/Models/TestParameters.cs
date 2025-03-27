using System;

namespace SquadGUI.ViewModels.Models;

public class TestParameters {
    public string? LotNumber { get; set; }
    public string? Client { get; set; }
    public string? ReportNumber { get; set; }
    public string? SampleNumber { get; set; }
    public string? Description { get; set; }
    
    public DateTimeOffset? TestDate { get; set; }
    public TimeSpan? TestTime { get; set; }
    
    public double? TemperatureC { get; set; }
    public double? TemperatureF { get; set; }
    public double? Humidity { get; set; }
    
    public string? Model { get; set; }
    public string? Size { get; set; }
    
    public string? MassType { get; set; }
    public double? MassG { get; set; }
    public double? MassLb { get; set; }
    
    public string? Condition { get; set; }
    
    public string? Projectile { get; set; }
    public string? Powder { get; set; }
    public string? Barrel { get; set; }
    public string? RangeConfiguration { get; set; }
    public string? ShotSpacing { get; set; }
    public string? WitnessPanel { get; set; }
    public double? Obliquity { get; set; }
    public string? BackingMaterial { get; set; }
    
    public string? Shooter { get; set; }
    public string? Recorder { get; set; }
    
    public string? OptionalInfo { get; set; }
    public string[]? SampleInfo { get; set; }
    public string[]? StandardsInfo { get; set; }
}