﻿#pragma warning disable 1591
using System;

namespace Telimena.WebApp.Models.ProgramStatistics
{
    public class ProgramStatisticsViewModel
    {
        public string ProgramName { get; set; }
        public Guid TelemetryKey { get; set; }
    }

    public class SequenceHistoryViewModel
    {
        public string ProgramName { get; set; }
        public Guid TelemetryKey { get; set; }

        public string SequenceId { get; set; }
    }
}