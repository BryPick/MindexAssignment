using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        ReportingStructure GetReportingStructureById(String id);
        Compensation GetCompensationById(String id);
        Compensation CreateCompensation(String id, Compensation compensation);
    }
}
