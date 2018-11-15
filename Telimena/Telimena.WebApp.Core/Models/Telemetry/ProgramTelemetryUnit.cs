﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Telimena.WebApp.Core.Models
{

    public class ProgramTelemetryUnit : TelemetryUnit
    {
        public virtual ProgramTelemetryDetail TelemetryDetail { get; set; }
    }
}