﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbIntegrationTestHelpers;
using NUnit.Framework;
using Telimena.WebApp.Controllers.Api;
using Telimena.WebApp.Controllers.Api.V1;
using Telimena.WebApp.Core.Models;
using Telimena.WebApp.Infrastructure.Database;
using Telimena.WebApp.Infrastructure.Repository.FileStorage;
using Telimena.WebApp.Infrastructure.UnitOfWork.Implementation;
using TelimenaClient;

namespace Telimena.Tests
{
    [TestFixture]
    public class ToolkitDataRegistrationTests : IntegrationTestsContextNotShared<TelimenaContext>
    {
        protected override Action SeedAction =>
            () =>
            {
                TelimenaDbInitializer.SeedUsers(this.Context);
                this.Context.SaveChanges();
            };

        [Test]
        public async Task TestToolkitDataAssignment()
        {
            TelemetryUnitOfWork unit = new TelemetryUnitOfWork(this.Context, new AssemblyStreamVersionReader());
            Assert.AreEqual(0, unit.ToolkitData.Get().Count());

            TelemetryController sut = new TelemetryController(unit);
            UserInfo userInfo = Helpers.GetUserInfo(Helpers.GetName("NewGuy"));
                var apps = await Helpers.SeedInitialPrograms(this.Context, 1, "TestProg", new string[0]);
                var apps2 = await Helpers.SeedInitialPrograms(this.Context, 1, "OtherProg", new string[0]);

            TelemetryInitializeRequest request = new TelemetryInitializeRequest(apps[0].Value)

            {
                ProgramInfo = Helpers.GetProgramInfo(Helpers.GetName("TestProg")),
                TelimenaVersion = "1.3.0.0",
                UserInfo = userInfo
            };

            request.ProgramInfo.HelperAssemblies = new List<AssemblyInfo>
            {
                new AssemblyInfo {Name = "Helper_" + Helpers.GetName("TestProg") + ".dll", VersionData = new VersionData("0.0.0.1", "3.0.0")}
            };

            await sut.Initialize(request);

            TelimenaToolkitData toolkitData = unit.ToolkitData.Single();
            Assert.AreEqual("1.3.0.0", toolkitData.Version);

            Helpers.GetProgramAndUser(this.Context, "TestProg", "NewGuy", out Program prg, out ClientAppUser usr);

            Assert.AreEqual("1.3.0.0", prg.PrimaryAssembly.GetLatestVersion().ToolkitData.Version);

            int testProgPrimaryAssId = prg.PrimaryAssembly.Id;
            //now, different assembly  will use same toolkit version
            request = new TelemetryInitializeRequest(apps2[0].Value)

            {
                ProgramInfo = Helpers.GetProgramInfo(Helpers.GetName("OtherProg"), version: new VersionData("2.0.0", "3.0.0")),
            TelimenaVersion = "1.3.0.0",
                UserInfo = userInfo
            };

            await sut.Initialize(request);

            toolkitData = unit.ToolkitData.Single();
            Assert.AreEqual("1.3.0.0", toolkitData.Version);

            Helpers.GetProgramAndUser(this.Context, "OtherProg", "NewGuy", out prg, out usr);
            int otherProgPrimaryAssId = prg.PrimaryAssembly.Id;

            Assert.AreEqual("1.3.0.0", prg.PrimaryAssembly.GetLatestVersion().ToolkitData.Version);

            //now a new version of an assembly will use the same toolkit version
            request.ProgramInfo = Helpers.GetProgramInfo(Helpers.GetName("OtherProg"), version: new VersionData("3.0.0", "4.0.0"));

            await sut.Initialize(request);

            toolkitData = unit.ToolkitData.Single();
            Assert.AreEqual("1.3.0.0", toolkitData.Version);

            Helpers.GetProgramAndUser(this.Context, "OtherProg", "NewGuy", out prg, out usr);

            Assert.AreEqual("1.3.0.0", prg.PrimaryAssembly.GetLatestVersion().ToolkitData.Version);


            //now an even newer version of an assembly will use a new toolkit version
            request.ProgramInfo = Helpers.GetProgramInfo(Helpers.GetName("OtherProg"), version: new VersionData("4.0.0", "5.0.0"));
            request.TelimenaVersion = "4.5.0.0";

            await sut.Initialize(request);

            toolkitData = unit.ToolkitData.Single(x => x.Id == 1);
            TelimenaToolkitData newToolkitData = unit.ToolkitData.Single(x => x.Id == 2);
            Assert.AreEqual("1.3.0.0", toolkitData.Version);
            Assert.AreEqual("4.5.0.0", newToolkitData.Version);

            Helpers.GetProgramAndUser(this.Context, "OtherProg", "NewGuy", out prg, out usr);

            Assert.AreEqual("4.5.0.0", prg.PrimaryAssembly.GetLatestVersion().ToolkitData.Version);
        }
    }
}