﻿namespace Telimena.WebApi.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Client;
    using DotNetLittleHelpers;
    using WebApp.Core.Models;
    using WebApp.Infrastructure.UnitOfWork;

    public class StatisticsHelperService
    {
        private readonly IStatisticsUnitOfWork _work;

        public StatisticsHelperService(IStatisticsUnitOfWork work)
        {
            this._work = work;
        }

        public async Task<ClientAppUser> GetUserInfoOrAddIfNotExists(UserInfo userDto)
        {
            ClientAppUser user = await this._work.ClientAppUsers.FirstOrDefaultAsync(x => x.UserName == userDto.UserName);
            if (user == null)
            {
                user = Mapper.Map<ClientAppUser>(userDto);
                user.RegisteredDate = DateTime.UtcNow;
                this._work.ClientAppUsers.Add(user);
            }

            return user;
        }

        public async Task<Function> GetFunctionOrAddIfNotExists(string functionName, Program program)
        {
            Function func = await this._work.Functions.FirstOrDefaultAsync(x => x.Name == functionName && x.Program.Name == program.Name);
            if (func == null)
            {
                func = new Function()
                {
                    Name = functionName,
                    Program = program,
                    ProgramId = program.Id
                };
                this._work.Functions.Add(func);
            }

            return func;
        }

        public async Task<Program> GetProgramOrAddIfNotExists(ProgramInfo requestProgramInfo)
        {
            Program program = await this._work.Programs.FirstOrDefaultAsync(x => x.Name == requestProgramInfo.Name);
            if (program == null)
            {
                program = Mapper.Map<Program>(requestProgramInfo);
                this._work.Programs.Add(program);
            }
            StatisticsHelperService.EnsureVersionIsRegistered(program.PrimaryAssembly, requestProgramInfo.PrimaryAssembly.Version);

            if (requestProgramInfo.HelperAssemblies.AnyAndNotNull())
            {
                foreach (AssemblyInfo helperAssembly in requestProgramInfo.HelperAssemblies)
                {
                    ProgramAssembly existingAssembly = program.ProgramAssemblies.FirstOrDefault(x => x.Name == helperAssembly.Name);
                    if (existingAssembly == null)
                    {
                        existingAssembly = Mapper.Map<ProgramAssembly>(helperAssembly);
                        program.ProgramAssemblies.Add(existingAssembly);
                    }

                    StatisticsHelperService.EnsureVersionIsRegistered(existingAssembly, helperAssembly.Version);
                }
            }

            await this.EnsureDeveloperSet(requestProgramInfo, program);
            return program;
        }

        private async Task EnsureDeveloperSet(ProgramInfo info, Program program)
        {
            if (program.DeveloperAccount == null && info.DeveloperId != null)
            {
                var dev = await this._work.Developers.FirstOrDefaultAsync(x => x.Id == info.DeveloperId);
                if (dev != null)
                {
                    program.DeveloperAccount = dev;
                }
            }
        }

        /// <summary>
        /// Verifies that the version of the program is added to the list of versions
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
    }
}