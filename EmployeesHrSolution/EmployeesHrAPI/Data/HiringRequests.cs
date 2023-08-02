namespace EmployeesHrApi.Data;

public class HiringRequests
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string HomeEmail { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string RequestedDepartment { get; set; } = string.Empty;
    public decimal RequiredSalary { get; set; }
    public HiringRequestStatus Status { get; set; }
}

public enum HiringRequestStatus { WaitingForJobAssignment, Hired, Denied } //only valid values. are stored as 0,1,2 in database.