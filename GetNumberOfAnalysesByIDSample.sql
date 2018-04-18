SELECT [Sample].IDSample, COUNT(IDSaltAnalysis) AS c, LabNumber, SamplingDate, [Description]
FROM [Sample] 
LEFT JOIN  SaltAnalysis ON [Sample].[IDSample] = [SaltAnalysis].[IDSample]
GROUP BY [Sample].IDSample, LabNumber, SamplingDate, [Description]