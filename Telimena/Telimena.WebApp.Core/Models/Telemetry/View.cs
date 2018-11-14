﻿using System.Collections.Generic;
using System.Linq;
using DotNetLittleHelpers;

namespace Telimena.WebApp.Core.Models
{
    public class View : ProgramComponent
    {
        public virtual RestrictedAccessList<ViewTelemetrySummary> TelemetrySummaries { get; set; } = new RestrictedAccessList<ViewTelemetrySummary>();

        public override IReadOnlyList<TelemetrySummary> GetTelemetrySummaries() => this.TelemetrySummaries.AsReadOnly();

        public override TelemetrySummary AddTelemetrySummary(int clientAppUserId)
        {
            ViewTelemetrySummary summary = new ViewTelemetrySummary()
            {
                ClientAppUserId = clientAppUserId,
                View = this
            };
            ((List<ViewTelemetrySummary>)this.TelemetrySummaries).Add(summary);
            return summary;
        }
    }
}