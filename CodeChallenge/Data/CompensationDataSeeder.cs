using CodeChallenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class CompensationDataSeeder
    {
        private EmployeeContext _employeeContext;
        private const String COMPENSATION_SEED_DATA_FILE = "resources/CompensationSeedData.json";

        public CompensationDataSeeder(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task Seed()
        {
            if(!_employeeContext.Compensations.Any())
            {
                List<Compensation> compensations = LoadCompensations();
                _employeeContext.Compensations.AddRange(compensations);

                await _employeeContext.SaveChangesAsync();
            }
        }

        private List<Compensation> LoadCompensations()
        {
            using (FileStream fs = new FileStream(COMPENSATION_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Compensation> compensations = serializer.Deserialize<List<Compensation>>(jr);

                return compensations;
            }
        }
    }
}
