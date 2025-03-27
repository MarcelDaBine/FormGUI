using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ReportModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("shooter")]
    public string? Shooter { get; set; }

    [JsonPropertyName("recorder")]
    public string? Recorder { get; set; }

    [JsonPropertyName("temperatureC")]
    public double? TemperatureC { get; set; }

    [JsonPropertyName("temperatureF")]
    public double? TemperatureF { get; set; }

    [JsonPropertyName("humidity")]
    public double? Humidity { get; set; }

    [JsonPropertyName("lotNo")]
    public double? LotNo { get; set; }

    [JsonPropertyName("client")]
    public string? Client { get; set; }

    [JsonPropertyName("reportNumber")]
    public string? ReportNumber { get; set; }

    [JsonPropertyName("sampleNumber")]
    public string? SampleNumber { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; }

    [JsonPropertyName("mass")]
    public string? Mass { get; set; }

    [JsonPropertyName("grams")]
    public double? Grams { get; set; }

    [JsonPropertyName("pounds")]
    public double? Pounds { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    [JsonPropertyName("optionalInfoText")]
    public string? OptionalInfoText { get; set; }

    [JsonPropertyName("inputRowsInfo")]
    public List<string>? InputRowsInfo { get; set; }

    [JsonPropertyName("projectile")]
    public string? Projectile { get; set; }

    [JsonPropertyName("powder")]
    public string? Powder { get; set; }

    [JsonPropertyName("barrel")]
    public string? Barrel { get; set; }

    [JsonPropertyName("sensor")]
    public string? Sensor { get; set; }

    [JsonPropertyName("shotSpacing")]
    public string? ShotSpacing { get; set; }

    [JsonPropertyName("witnessPanel")]
    public string? WitnessPanel { get; set; }

    [JsonPropertyName("obliquity")]
    public string? Obliquity { get; set; }

    [JsonPropertyName("backingMaterial")]
    public string? BackingMaterial { get; set; }

    [JsonPropertyName("inputRowsProc")]
    public List<string>? InputRowsProc { get; set; }

    [JsonPropertyName("v50ValueM")]
    public double? V50ValueM { get; set; }

    [JsonPropertyName("v50ValueFt")]
    public double? V50ValueFt { get; set; }

    [JsonPropertyName("highPartialM")]
    public double? HighPartialM { get; set; }

    [JsonPropertyName("highPartialFt")]
    public double? HighPartialFt { get; set; }

    [JsonPropertyName("lowCompleteM")]
    public double? LowCompleteM { get; set; }

    [JsonPropertyName("lowCompleteFt")]
    public double? LowCompleteFt { get; set; }

    [JsonPropertyName("mixedResultsM")]
    public double? MixedResultsM { get; set; }

    [JsonPropertyName("mixedResultsFt")]
    public double? MixedResultsFt { get; set; }

    [JsonPropertyName("gapM")]
    public double? GapM { get; set; }

    [JsonPropertyName("gapFt")]
    public double? GapFt { get; set; }

    [JsonPropertyName("rangeResultsM")]
    public double? RangeResultsM { get; set; }

    [JsonPropertyName("rangeResultsFt")]
    public double? RangeResultsFt { get; set; }

    [JsonPropertyName("v50MinM")]
    public double? V50MinM { get; set; }

    [JsonPropertyName("v50MinFt")]
    public double? V50MinFt { get; set; }

    [JsonPropertyName("deltaVM")]
    public double? DeltaVM { get; set; }

    [JsonPropertyName("deltaVFt")]
    public double? DeltaVFt { get; set; }

    [JsonPropertyName("percentageM")]
    public double? PercentageM { get; set; }

    [JsonPropertyName("percentageFt")]
    public double? PercentageFt { get; set; }

    [JsonPropertyName("expandedUncertainty")]
    public double? ExpandedUncertainty { get; set; }

    [JsonPropertyName("decisionRule")]
    public string? DecisionRule { get; set; }

    [JsonPropertyName("data")]
    public List<ShotDataRow>? Data { get; set; }

    public class ShotDataRow
    {
        [JsonPropertyName("load")]
        public string? Load { get; set; }

        [JsonPropertyName("trackId")]
        public string? TrackId { get; set; }

        [JsonPropertyName("strVelM")]
        public double? StrVelM { get; set; }

        [JsonPropertyName("strVelFt")]
        public double? StrVelFt { get; set; }

        [JsonPropertyName("ppCp")]
        public string? PpCp { get; set; }

        [JsonPropertyName("used")]
        public bool? Used { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }
    }
}
