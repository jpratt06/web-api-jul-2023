using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeesHrApi.AutomapperProfiles;
using EmployeesHrApi.Data;
using EmployeesHrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeesHrAPI.Controllers;

public class EmployeesController : ControllerBase
{
    private readonly EmployeeDataContext _context;
    private readonly ILogger<EmployeesController> _logger;
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _config;

    public EmployeesController(EmployeeDataContext context, ILogger<EmployeesController> logger, IMapper mapper, MapperConfiguration config)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        _config = config;
    }





    [HttpGet("/employees/{employeeId:int}/salary")]
    public async Task<ActionResult> GetAnEmployeesSalaryAsync(int employeeId)
    {
        var salary = await _context.GetActiveEmployees()
            .Where(e => e.Id == employeeId)
            .Select(e => e.Salary)
            .SingleOrDefaultAsync();

        if (salary == 0)
        {
            return NotFound();
        }
        else
        {
            var response = new EmployeeSalaryInformationResponse { Salary = salary };
            return Ok(response);
        }
    }

    [HttpDelete("/employees/{id:int}")]
    public async Task<ActionResult> FireEmployeeAsync(int id)
    {
        var employee = await _context.Employees.Where(e => e.Id == id && e.Fired == false).SingleOrDefaultAsync();
        if (employee is not null)
        {
            employee.Fired = true;
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }

    //GET /employees/{employeeId:int}
    [HttpGet("/employees/{employeeId:int}")] //:int causes controller isto not be created if user inputs a non int value. Called a route constraint. Nothing gets logged, nothing is executed.
    public async Task<ActionResult> GetAnEmployeeAsync(int employeeId)
    {
        _logger.LogInformation("Got the following employeeId {0}", employeeId)
;
        var employee = await _context.GetActiveEmployees()
            .Where(e => e.Id == employeeId)
            .ProjectTo<EmployeeDetailsResponseModel>(_config)
            //.Select(e => new EmployeeDetailsResponseModel // Employee => EmployeeDetailsResposeModel
            //{
            //    Id = e.Id.ToString(),
            //    FirstName = e.FirstName,
            //    LastName = e.LastName,
            //    Department = e.Department,
            //    Email = e.Email,
            //    PhoneExtension = e.PhoneExtensions
            //})
            .SingleOrDefaultAsync();

        if (employee is null)
        {
            return NotFound(); // 404
        }
        else
        {
            return Ok(employee);
        }
    }

    //GET /employees
    [HttpGet("/employees")]
    public async Task<ActionResult<EmployeesResponseModel>> GetEmployeesAsync([FromQuery] string department = "All")
    {
        var employees = await _context.GetEmployeesByDepartment(department)
            .ProjectTo<EmployeesSummaryResponseModel>(_config)
                        //.Select(emp => new EmployeesSummaryResponseModel
                        //{
                        //    Id = emp.Id.ToString(),
                        //    FirstName = emp.FirstName,
                        //    LastName = emp.LastName,
                        //    Department = emp.Department,
                        //    Email = emp.Email})
                        .ToListAsync(); //actually runs the query here.

        var response = new EmployeesResponseModel
        {
            Employees = employees,
            ShowingDepartment = department
        };
        return Ok(response);
    }
}