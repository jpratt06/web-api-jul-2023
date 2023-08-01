using Microsoft.EntityFrameworkCore;

namespace EmployeesHrApi.Data;

public class EmployeeDataContext : DbContext
{
    public EmployeeDataContext(DbContextOptions<EmployeeDataContext> options) : base(options)
    {

    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<HiringRequests> HiringRequests { get; set; }


    //This method returns an IQuerable that knows how to get employees in a department or all of them.
    public IQueryable<Employee> GetEmployeesByDepartment(string department)
    {
        if (department != "All")
        {
            return Employees.Where(e => e.Department == department);
        }
        else
        {
            return Employees;
        }
    }
}