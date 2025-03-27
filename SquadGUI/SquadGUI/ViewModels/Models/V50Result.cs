using System.Collections.Generic;

namespace SquadGUI.ViewModels.Models;

public class V50Result {
    public double? V50ValueM { get; set; }
    public double? V50ValueFt { get; set; }
    
    public double? HighPartialM { get; set; }
    public double? HighPartialFt { get; set; }
    public double? LowCompleteM { get; set; }
    public double? LowCompleteFt { get; set; }
    
    public double? MixedResultM { get; set; }
    public double? MixedResultFt { get; set; }
    
    public double? GapM { get; set; }
    public double? GapFt { get; set; }
    
    public double? RangeResultsM { get; set; }
    public double? RangeResultsFt { get; set; }
    
    public double? V50MinM { get; set; }
    public double? V50MinFt { get; set; }
    
    public double? DeltaVM { get; set; }
    public double? DeltaFt { get; set; }
    
    public double? PercentageValue => V50MinM is > 0 && DeltaVM.HasValue ? (DeltaVM.Value / V50MinM.Value) * 100 : null;
    
    public string? ExpandedUncertainty { get; set; }
    public string? DecisionRule { get; set; }
    
    public IList<Shot> UserShots { get; set; } = new List<Shot>();
    
    public V50CalculationStatus Status { get; set; }
    public string? StatusMessage { get; set; }
    
    public enum V50CalculationStatus {
        NotCalculated,
        Success,
        InsufficientData,
        ExcessiveSpread,
        Error
    }
    
}