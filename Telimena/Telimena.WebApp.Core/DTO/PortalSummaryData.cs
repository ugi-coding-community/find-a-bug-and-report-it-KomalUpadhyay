﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telimena.WebApp.Core.DTO
{
    using Models;

    public class PortalSummaryData
    {
        public int TotalUsersCount { get; set; }
        public int UsersActiveInLast24Hrs { get; set; }
        public TelimenaUser NewestUser { get; set; }
        public TelimenaUser LastActiveUser { get; set; }
    }
}