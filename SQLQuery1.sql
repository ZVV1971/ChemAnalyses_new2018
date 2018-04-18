CREATE VIEW [SaltAnalysisView] AS
SELECT [IDSaltAnalysis], [Sample].[IDSample], [Sample].LabNumber
[WetWeight], [MagnesiumTitre],
[MagnesiumAliquote], [MagnesiumTrilonTitre],[KaliumCalibration],
[AnalysisDate], [KaliumDiapason], [KaliumConcentration],
[SulfatesCrucibleEmptyWeight], [SulfatesCrucibleFullWeight], 
[ResiduumCrucibleFullWeight], [ResiduumCrucibleEmptyWeight],
[HumidityCrucibleEmptyWeight], [HumidityCrucibleWetSampleWeight], 
[HumidityCrucibleDry110SampleWeight], [HumidityCrucibleDry180SampleWeight], 
[CalciumTitre], [ChlorumTitre], [BromumTitre], [KaliumValue], [CarbonatesTitre], 
[HydrocarbonatesTitre],  [CalciumTrilonTitre], [CalciumAliquote], [BromumAliquote],
[SulfatesAliquote], [KaliumVolume], [HgCoefficient], [BromumBlank], [BromumStandardTitre],
[SulfatesBlank],  [ChlorumAliquote]
FROM [SaltAnalysis] INNER JOIN [Sample] ON [SaltAnalysis].IDSample = [Sample].IDSample