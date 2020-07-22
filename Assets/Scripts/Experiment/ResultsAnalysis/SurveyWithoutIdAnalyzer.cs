﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    class SurveyWithoutIdAnalyzer : SurveyAnalyzerBase
    {
        public SurveyWithoutIdAnalyzer(ResultAnalysisConfiguration configuration) : base(configuration)
        {

        }

        private int unasignedSurveyIndex;
        protected override string GetTargetDirectory(List<string> cells, string[] allDirectories)
        {
            var startDateString = cells[1];
            DateTime surveyStartDateTime;
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en");
            var styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal;
            if (!DateTime.TryParse(startDateString, culture, styles, out surveyStartDateTime))
            {
                UnityEngine.Debug.LogError("Cannot parse date.");
                return null;
            }
            TimeSpan? shortestDifference = null;
            string shortestDifferenceDirectory = null;
            foreach (var directory in allDirectories)
            {
                var rawDataPath = directory + "/" + Configuration.ProcessedRawDataFileName;
                if (!File.Exists(rawDataPath))
                {
                    continue;
                }
                var experimentEndDateTime = GetTestEndDateTime(rawDataPath);
                if (experimentEndDateTime == null)
                {
                    continue;
                }
                // We allow negative values, as time on server and local PC might not be in sync.
                var difference = surveyStartDateTime - experimentEndDateTime.Value;
                if (!shortestDifference.HasValue || difference.Duration() < shortestDifference)
                {
                    shortestDifference = difference.Duration();
                    shortestDifferenceDirectory = directory;
                }
            }
            UnityEngine.Debug.Log($"Shortest difference found: {shortestDifference}");
            UnityEngine.Debug.Log($"Shortest difference directory: {shortestDifferenceDirectory}");
            if (shortestDifference.Value.Minutes <= 2)
            {
                return shortestDifferenceDirectory;
            }
            else
            {
                return Configuration.ResultsRootDirectory + Configuration.UnassignedSurveyFolder + (unasignedSurveyIndex++).ToString();
            }

        }

        private DateTime? GetTestEndDateTime(string rawDataPath)
        {
            // Find the last row and get it's time stamp. This should get us the approximate time when the user closed the game.
            string lastLine = null;
            using (StreamReader sr = new StreamReader(rawDataPath))
            {
                string currentLine = sr.ReadLine();
                while (currentLine != null)
                {
                    lastLine = currentLine;
                    currentLine = sr.ReadLine();
                }
            }
            if (lastLine == null)
            {
                return null;
            }
            var lastLineCells = lastLine.Split(';');
            var timeStampString = lastLineCells[2];
            var timeStampLong = long.Parse(timeStampString);
            // We add two hours to get to the proper timezone.
            return DateTime.FromFileTimeUtc(timeStampLong).AddHours(2);
        }
    }
}