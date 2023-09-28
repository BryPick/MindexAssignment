using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if (employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if (originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructureById(string id)
        {
            var employee = GetById(id);

            if (employee == null)
                return null;

            return new ReportingStructure
            {
                Employee = $"{employee.FirstName} {employee.LastName}",
                NumberOfReports = GetDirectReportCount(id)
            };
        }

        public Compensation GetCompensationById(string id)
        {
            var employee = GetById(id);

            if (employee == null)
                return null;

            return _employeeRepository.GetCompensationById(id);
        }

        public Compensation CreateCompensation(String id, Compensation compensation)
        {
            var employee = GetById(id);

            if (employee == null)
                return null;

            // Validates the employee name is correct
            if (!compensation.Employee.Equals($"{employee.FirstName} {employee.LastName}"))
                return null;

            if (compensation != null)
            {
                _employeeRepository.AddCompensation(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        private int GetDirectReportCount(string id)
        {
            var employee = GetById(id);

            /*
             * 1. Check initial direct report count
             * 2. Greater than zero, check nested direct report count
             * 3. Add up nested count
             * 4. Return initial direct report count + nested count
             */
            if (employee.DirectReports.Count > 0)
            {
                var nestedReportCount = 0;

                employee.DirectReports.ForEach(report =>
                {
                    nestedReportCount += GetDirectReportCount(report.EmployeeId);
                });

                return employee.DirectReports.Count + nestedReportCount;
            }

            return 0;
        }
    }
}
