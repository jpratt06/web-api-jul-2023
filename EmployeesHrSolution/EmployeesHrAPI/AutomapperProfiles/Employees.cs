using AutoMapper;
using EmployeesHrApi.Data;
using EmployeesHrApi.Models;

namespace EmployeesHrApi.AutomapperProfiles
{
    public class Employees : Profile
    {
        // Employee => EmployeeDetailsResposeModel
        public Employees()
        {
            CreateMap<Employee, EmployeeDetailsResponseModel>()
                .ForMember(dest => dest.PhoneExtension, opt => opt.MapFrom(src => src.PhoneExtensions));

            CreateMap<Employee, EmployeesSummaryResponseModel>();
        }
    }
}
