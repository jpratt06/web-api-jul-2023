using Microsoft.AspNetCore.Mvc;

namespace EmployeesHrAPI.Controllers;

public class EmployeesController : ControllerBase
{
    //GET /employees
    [HttpGet("/employees")]
    public async Task<ActionResult> GetEmployeesAsync()
    {
        return Ok("Hello World");
    }

    //GET /departments
    [HttpGet("/departments")]
    public async Task<ActionResult> GetDepartmentsAsync()
    {
        var departments = new List<string> { "DEV", "HR", "SALES", "QA" };
        return Ok(departments);
    }

}
