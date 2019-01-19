﻿using System;
using System.Collections.Generic;
using Telimena.WebApp.Core.Models;

namespace Telimena.WebApp.Core.DTO
{
    public class TelemetrySummaryDto
    {
        [Obsolete("For serialization")]
        public TelemetrySummaryDto()
        {
        }

        public TelemetrySummaryDto(TelemetrySummary telemetrySummary, List<string> propertiesToInclude)
        {
            if (propertiesToInclude.Contains(nameof(this.UserName)))
            {
                this.UserName = telemetrySummary.ClientAppUser.UserName;
            }
            if (propertiesToInclude.Contains(nameof(this.UserGuid)))
            {
                this.UserGuid = telemetrySummary.ClientAppUser.Guid;
            }
            if (propertiesToInclude.Contains(nameof(this.LastReported)))
            {
                this.LastReported = telemetrySummary.LastTelemetryUpdateTimestamp;
            }
            if (propertiesToInclude.Contains(nameof(this.SummaryCount)))
            {
                this.SummaryCount = telemetrySummary.SummaryCount;
            }
        }

        public Guid? UserGuid { get; set; }

        public string UserName { get; set; }

        public List<TelemetryDetailDto> Details { get; set; }  
        public DateTimeOffset? LastReported { get; set; }
        public int? SummaryCount { get; set; }
    }
}