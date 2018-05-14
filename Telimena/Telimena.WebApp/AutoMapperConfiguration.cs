﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Telimena.WebApp
{
    using AutoMapper;
    using Client;
    using Infrastructure.DTO;
    using Infrastructure.Repository;
    using WebApi.Controllers;

    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
                    cfg.AddProfiles(new[] {
                        typeof(AutoMapperWebProfile),
                        typeof(AutoMapperDomainProfile)
                    }));
        }
        public static void Validate()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }


    public class AutoMapperWebProfile : Profile
    {
        public AutoMapperWebProfile()
        {
            this.CreateMap<ProgramInfo, ProgramInfoDto>();
            this.CreateMap<UserInfo, UserInfoDto >();
        }
    }
}