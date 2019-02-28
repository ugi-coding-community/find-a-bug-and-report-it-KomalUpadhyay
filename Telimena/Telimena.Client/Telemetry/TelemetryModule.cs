﻿using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;

namespace TelimenaClient
{
    /// <inheritdoc />
    public partial class TelemetryModule : ITelemetryModule
    {
        /// <summary>
        ///     Asynchronous Telimena methods
        /// </summary>
        public TelemetryModule(ITelimenaProperties telimenaProperties)
        {
            this.telimenaProperties = telimenaProperties;
        }

        private readonly ITelimenaProperties telimenaProperties;

        /// <summary>
        /// Gets the telemetry client.
        /// </summary>
        /// <value>The telemetry client.</value>
        public TelemetryClient TelemetryClient { get; private set; }

        /// <inheritdoc />

        public void SendAllDataNow()
        {
            try
            {
                this.TelemetryClient.Flush();
            }
            catch (Exception)
            {
                if (!this.telimenaProperties.SuppressAllErrors)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Initializes the telemetry client.
        /// </summary>
        public void InitializeTelemetryClient()
        {
            TelemetryClientBuilder builder = new TelemetryClientBuilder(this.telimenaProperties);
            this.TelemetryClient = builder.GetClient();
        }

        /// <summary>
        /// Initializes the telemetry client.
        /// </summary>
        [Obsolete("For tests only")]
        internal void InitializeTelemetryClient(ITelemetryChannel channel)
        {
            TelemetryClientBuilder builder = new TelemetryClientBuilder(this.telimenaProperties);
#pragma warning disable 618
            this.TelemetryClient = builder.GetClient(channel);
#pragma warning restore 618
        }


     
    }
}