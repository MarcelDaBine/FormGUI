using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ReportModel
{
    [JsonPropertyName("date")]
    public string FormDate { get; set; }

    [JsonPropertyName("time")]
    public string FormTime { get; set; }

    [JsonPropertyName("shooter")]
    public string Shooter { get; set; }

    [JsonPropertyName("recorder")]
    public string Recorder { get; set; }

    [JsonPropertyName("temperatureC")]
    public double CelsiusText { get; set; }

    [JsonPropertyName("temperatureF")]
    public double FahrenheitText { get; set; }

    [JsonPropertyName("humidity")]
    public double HumidityText { get; set; }

    [JsonPropertyName("lotNo")]
    public double LotNo { get; set; }

    [JsonPropertyName("client")]
    public string Client { get; set; }

    [JsonPropertyName("reportNumber")]
    public string ReportNumberText { get; set; }

    [JsonPropertyName("sampleNumber")]
    public string SampleNumberText { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("model")]
    public string SelectedModel { get; set; }

    [JsonPropertyName("size")]
    public string SelectedSize { get; set; }

    [JsonPropertyName("mass")]
    public string SelectedMass { get; set; }

    [JsonPropertyName("grams")]
    public double MassGrams { get; set; }

    [JsonPropertyName("pounds")]
    public double MassPounds { get; set; }

    [JsonPropertyName("condition")]
    public string SelectedCondition { get; set; }

    [JsonPropertyName("optionalInfoText")]
    public string OptionalInfoText { get; set; }

    [JsonPropertyName("inputRowsInfo")]
    public List<string> InputRowsInfo { get; set; }

    [JsonPropertyName("projectile")]
    public string SelectedProjectile { get; set; }

    [JsonPropertyName("powder")]
    public string SelectedPowder { get; set; }

    [JsonPropertyName("barrel")]
    public string SelectedBarrel { get; set; }

    [JsonPropertyName("sensor")]
    public string SelectedRangeConfig { get; set; }

    [JsonPropertyName("shotSpacing")]
    public string ShotSpacing { get; set; }

    [JsonPropertyName("witnessPanel")]
    public string WitnessPanel { get; set; }

    [JsonPropertyName("obliquity")]
    public string Obliquity { get; set; }

    [JsonPropertyName("backingMaterial")]
    public string BackingMaterial { get; set; }

    [JsonPropertyName("inputRowsProc")]
    public List<string> InputRowsProc { get; set; }

    [JsonPropertyName("v50ValueM")]
    public double MeanValueMs { get; set; }

    [JsonPropertyName("v50ValueFt")]
    public double MeanValueFt { get; set; }

    [JsonPropertyName("highPartialM")]
    public double HighPartialMs { get; set; }

    [JsonPropertyName("highPartialFt")]
    public double HighPartialFt { get; set; }

    [JsonPropertyName("lowCompleteM")]
    public double LowCompleteMs { get; set; }

    [JsonPropertyName("lowCompleteFt")]
    public double LowCompleteFt { get; set; }

    [JsonPropertyName("mixedResultsM")]
    public string MixedResultsMs { get; set; }

    [JsonPropertyName("mixedResultsFt")]
    public string MixedResultsFt { get; set; }

    [JsonPropertyName("gapM")]
    public string GapMs { get; set; }

    [JsonPropertyName("gapFt")]
    public string GapFt { get; set; }

    [JsonPropertyName("rangeResultsM")]
    public double RangeResultsMs { get; set; }

    [JsonPropertyName("rangeResultsFt")]
    public double RangeResultsFt { get; set; }

    [JsonPropertyName("v50MinFt")]
    public double V50MinFt { get; set; }

    [JsonPropertyName("v50MinM")]
    public double V50MinMs { get; set; }

    [JsonPropertyName("deltaVM")]
    public double DeltaVMs { get; set; }

    [JsonPropertyName("deltaVFt")]
    public double DeltaVFt { get; set; }

    [JsonPropertyName("percentageFt")]
    public double PercentageFt { get; set; }

    [JsonPropertyName("percentageM")]
    public double PercentageMs { get; set; }

    [JsonPropertyName("expandedUncertainty")]
    public string ExpandedUncertainty { get; set; }

    [JsonPropertyName("decisionRule")]
    public string DecisionRule { get; set; }

    [JsonPropertyName("data")]
    public List<ShotDataRow> Data { get; set; }

    public class ShotDataRow
    {
        [JsonPropertyName("load")] public string Load { get; set; }
        [JsonPropertyName("trackID")] public string TrackID { get; set; }
        [JsonPropertyName("strVelFt")] public double StrVelFt { get; set; }
        [JsonPropertyName("ppCp")] public string PpCp { get; set; }
        [JsonPropertyName("notes")] public string Notes { get; set; }
        [JsonPropertyName("position")] public string Position { get; set; }
    }
}
