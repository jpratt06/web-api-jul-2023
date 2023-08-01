using AutoMapper;
using EmployeesHrApi.Data;
using EmployeesHrApi.Models;
using Microsoft.AspNetCore.Mvc;



namespace EmployeesHrApi.Controllers;


// Dont do this either.[ApiController] //Automatically sends down in json format a list of errors instead of using return ModelState.
public class HiringRequestsController : ControllerBase
{
    private readonly EmployeeDataContext _context;
    private readonly IMapper _mapper;

    public HiringRequestsController(EmployeeDataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("/hiring-requests")]
    public async Task<ActionResult> AddHiringRequestAsync([FromBody] HiringRequestCreateRequest request)
    {
        //.Net creates the input model instance and fills it with input data.
        // 3. Return 201 Created Status Code
        //  -- Add Header "Location" - with the Url of the new resource. 
        // - http://localhost:1337/hiring-request


        //if (!ModelState.IsValid) //don't use this.
        //{
        //    return BadRequest(ModelState); //don't return modelstate. It's not very useful.
        //}

        // 1. Validate it a little? - if it isn't valid, send them a 400 (Bad Request)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400
        }
        // 2. Save it to the database.
        //var newHiringRequest = new HiringRequests
        //{
        //    FirstName = request.FirstName,
        //    LastName = request.LastName,
        //    HomeEmail = request.HomeEmail,
        //    HomePhone = request.HomePhone,
        //    RequestedDepartment = request.RequestedDepartment,
        //    RequiredSalary = request.RequiredSalary,
        //    Status = HiringRequestStatus.WaitingForJobAssignment
        //};
        var newHiringRequest = _mapper.Map<HiringRequests>(request);
        _context.HiringRequests.Add(newHiringRequest);
        await _context.SaveChangesAsync();
        // 3. Return a 201 Created Status Code 
        //   - Add Header "Location" - with the Url of the new resource.
        //   - Return them a copy of the new resource
        return Ok(newHiringRequest);
    }
}