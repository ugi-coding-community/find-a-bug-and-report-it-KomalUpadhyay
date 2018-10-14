﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetLittleHelpers;
using Telimena.WebApp.Core.Models;
using Telimena.WebApp.Infrastructure.UnitOfWork;
using TelimenaClient;

namespace Telimena.WebApp.Controllers.Api
{
    public class StatisticsHelperService
    {
        public StatisticsHelperService(IStatisticsUnitOfWork work)
        {
            this._work = work;
        }

        private readonly IStatisticsUnitOfWork _work;

        /// <summary>
        ///     Verifies that the version of the program is added to the list of versions
        /// </summary>
        /// <param name="programAssembly"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static void EnsureVersionIsRegistered(ProgramAssembly programAssembly, string version)
        {
            if (programAssembly.Versions.AnyAndNotNull())
            {
                programAssembly.AddVersion(version);
            }
            else
            {
                programAssembly.SetLatestVersion(version);
            }
        }

        public async Task<Function> GetFunctionOrAddIfNotExists(string functionName, Program program)
        {
            Function func = await this._work.Functions.FirstOrDefaultAsync(x => x.Name == functionName && x.Program.Name == program.Name);
            if (func == null)
            {
                func = new Function {Name = functionName, Program = program, ProgramId = program.Id};
                this._work.Functions.Add(func);
            }

            return func;
        }

        public async Task<Program> GetProgramOrAddIfNotExists(RegistrationRequest requestProgramInfo)
        {
            Program program = await this._work.Programs.FirstOrDefaultAsync(x => x.Name == requestProgramInfo.ProgramInfo.Name);
            if (program == null)
            {
                program = Mapper.Map<Program>(requestProgramInfo.ProgramInfo);
                this._work.Programs.Add(program);
            }

            EnsureVersionIsRegistered(program.PrimaryAssembly, requestProgramInfo.ProgramInfo.PrimaryAssembly.Version);

            if (requestProgramInfo.ProgramInfo.HelperAssemblies.AnyAndNotNull())
            {
                foreach (AssemblyInfo helperAssembly in requestProgramInfo.ProgramInfo.HelperAssemblies)
                {
                    ProgramAssembly existingAssembly = program.ProgramAssemblies.FirstOrDefault(x => x.Name == helperAssembly.Name);
                    if (existingAssembly == null)
                    {
                        existingAssembly = Mapper.Map<ProgramAssembly>(helperAssembly);
                        program.ProgramAssemblies.Add(existingAssembly);
                    }

                    EnsureVersionIsRegistered(existingAssembly, helperAssembly.Version);
                }
            }

            await this.AssignToolkitVersion(program.PrimaryAssembly, requestProgramInfo.ProgramInfo.PrimaryAssembly.Version
                , requestProgramInfo.TelimenaVersion);
            await this.EnsureDeveloperSet(requestProgramInfo.ProgramInfo, program);
            return program;
        }

        public async Task<ClientAppUser> GetUserInfoOrAddIfNotExists(UserInfo userDto, string ip)
        {
            ClientAppUser user = await this._work.ClientAppUsers.FirstOrDefaultAsync(x => x.UserName == userDto.UserName);
            if (user == null)
            {
                user = Mapper.Map<ClientAppUser>(userDto);
                user.RegisteredDate = DateTime.UtcNow;
                user.IpAddresses.Add(ip);
                this._work.ClientAppUsers.Add(user);
            }
            else
            {
                if (!user.IpAddresses.Contains(ip))
                {
                    user.IpAddresses.Add(ip);
                }
            }
            return user;
        }

        private async Task AssignToolkitVersion(ProgramAssembly programAssembly, string programVersion, string toolkitVersion)
        {
            TelimenaToolkitData toolkitData = await this._work.ToolkitData.FirstOrDefaultAsync(x => x.Version == toolkitVersion);
            AssemblyVersion assemblyVersion = programAssembly.GetVersion(programVersion);
            if (toolkitData == null)
            {
                toolkitData = new TelimenaToolkitData(toolkitVersion);
            }

            assemblyVersion.ToolkitData = toolkitData;
        }

        private async Task EnsureDeveloperSet(ProgramInfo info, Program program)
        {
            if (program.DeveloperAccount == null && info.DeveloperId != null)
            {
                DeveloperAccount dev = await this._work.Developers.FirstOrDefaultAsync(x => x.Id == info.DeveloperId);
                if (dev != null)
                {
                    program.DeveloperAccount = dev;
                }
            }
        }
    }
}