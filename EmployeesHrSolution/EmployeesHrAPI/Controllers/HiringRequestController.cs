using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeesHrApi.Data;
using EmployeesHrApi.HttpAdapters;
using EmployeesHrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeesHrApi.Controllers;



// Dont do this either.[ApiController] //Automatically sends down in json format a list of errors instead of using return ModelState.
public class HiringRequestsController : ControllerBase
{
    private readonly EmployeeDataContext _context;
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _mapperConfig;
    private readonly TelecomHttpAdapter _telecomHttp;

    public HiringRequestsController(EmployeeDataContext context, IMapper mapper, MapperConfiguration mapperConfig, TelecomHttpAdapter telecomHttp)
    {
        _context = context;
        _mapper = mapper;
        _mapperConfig = mapperConfig;
        _telecomHttp = telecomHttp;
    }

    [HttpPost("/approved-hiring-requests")]
    public async Task<ActionResult> ApproveHiringRequestAsync([FromBody] HiringRequestResponseModel request)
    {
        var id = int.Parse(request.Id);
        if (request.Status != HiringRequestStatus.WaitingForJobAssignment)
        {
            return BadRequest("Can only deny pending assignments");
        }
        var savedHiringRequest = await _context.HiringRequests.Where(h => h.Id == id)
            .SingleOrDefaultAsync();

        if (savedHiringRequest == null)
        {
            return BadRequest();
        }
        else
        {
            if (savedHiringRequest.Status != HiringRequestStatus.WaitingForJobAssignment)
            {
                return BadRequest();
            }

            savedHiringRequest.Status = HiringRequestStatus.Hired;

            var newHireRequest = new NewHireRequestModel
            {
                Id = id,
                Department = request.RequestedDepartment,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            var teleComInfo = await _telecomHttp.GetTelecomInfoForNewHire(newHireRequest);
            if (teleComInfo == null)
            {
                throw new ArgumentNullException("The Api Done Crashed");
            }

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Department = request.RequestedDepartment,
                Salary = request.RequiredSalary,
                Email = teleComInfo.EmailAddress,
                PhoneExtensions = teleComInfo.PhoneExtension
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return NoContent(); // or the mapped hiring request.
        }
    }

    [HttpPost("/denied-hiring-requests")]
    public async Task<ActionResult> DenyHiringRequestAsync([FromBody] HiringRequestResponseModel request)
    {
        var id = int.Parse(request.Id);
        if (request.Status != HiringRequestStatus.WaitingForJobAssignment)
        {
            return BadRequest("Can only deny pending assignments");
        }
        var savedHiringRequest = await _context.HiringRequests.Where(h => h.Id == id)
            .SingleOrDefaultAsync();



        if (savedHiringRequest == null)
        {
            return BadRequest();
        }
        else
        {
            if (savedHiringRequest.Status != HiringRequestStatus.WaitingForJobAssignment)
            {
                return BadRequest();
            }

            savedHiringRequest.Status = HiringRequestStatus.Denied;
            await _context.SaveChangesAsync();
            return NoContent(); // or the mapped hiring request.
        }
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
        //return Ok(newHiringRequest);
        var response = _mapper.Map<HiringRequestResponseModel>(newHiringRequest);
        return CreatedAtRoute("hiring-request#gethiringrequestbyidasync", new { id = response.Id }, response);
    }

    [HttpGet("/hiring-requests/{id:int}", Name = "hiring-request#gethiringrequestbyidasync")]
    public async Task<ActionResult> GetHiringRequestByIdAsync(int id)
    {
        var hiringRequest = await _context.HiringRequests
            .Where(e => e.Id == id)
            .ProjectTo<HiringRequestResponseModel>(_mapperConfig)
            .SingleOrDefaultAsync();

        if (hiringRequest is not null)
        {
            return Ok(hiringRequest);
        }
        else
        {
            return NotFound();
        }
    }
}