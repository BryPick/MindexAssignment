using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            // Included DirectReports, was returning null prior to update
            return _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
        
        public Compensation GetCompensationById(String id)
        {
            // Perform join on Employees (FirstName/LastName) props and Compensations.Employee where id = {id}
            return _employeeContext.Employees.Join(
                _employeeContext.Compensations,
                employees => $"{employees.FirstName} {employees.LastName}",
                compensations => compensations.Employee,
                (employees, compensations) => new
                {
                    employees.EmployeeId,
                    compensations.Employee,
                    compensations.Salary,
                    compensations.EffectiveDate,
                })
                .Where(x => x.EmployeeId == id)
                .Select(x => new Compensation
                {
                    Employee = x.Employee,
                    Salary = x.Salary,
                    EffectiveDate = x.EffectiveDate,
                })
                .FirstOrDefault();
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }
    }
}
