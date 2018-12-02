﻿namespace TelimenaClient
{
    /// <summary>
    /// Class DefaultToolkitNames.
    /// </summary>
    public static class DefaultToolkitNames
    {
        /// <summary>
        /// The updater file name
        /// </summary>
        public static string UpdaterFileName {get;} =  "Updater.exe";

        /// <summary>
        /// The file name of the package trigger updater
        /// </summary>
        public static string PackageTriggerUpdaterFileName {get;} =  "PackageTriggerUpdater.exe";
        /// <summary>
        /// The zipped package file name
        /// </summary>
        public static string ZippedPackageName {get;} =  "Telimena.Client.zip";
        /// <summary>
        /// The telimena assembly name
        /// </summary>
        public static string TelimenaAssemblyName {get;} =  "Telimena.Client.dll";

        /// <summary>
        /// The default updater internal name
        /// </summary>
        public static string UpdaterInternalName {get;} =  "TelimenaStandaloneUpdater";

        /// <summary>
        /// The default package trigger updater name
        /// </summary>
        public static string PackageTriggerUpdaterInternalName {get;} =  "TelimenaPackageUpdater";

        /// <summary>
        /// The telimena system dev team
        /// </summary>
        public static string TelimenaSystemDevTeam {get;} =  "TelimenaSystemDevTeam";

    }
}