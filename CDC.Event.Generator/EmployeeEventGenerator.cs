using System;
using CDC.Event.Generator.Dex;
using CDC.Event.Generator.Extension;
using CDC.Reader.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CDC.Event.Generator
{
    public class EmployeeEventGenerator
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public EmployeeEventGenerator(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public bool GenerateEvent(EmployeeCT employee)
        {
            bool isSuccess = false;

            if(employee.OperationType == 2)
            {
                EmployeeCreated employeeCreated = new EmployeeCreated
                {
                    EventId = Guid.NewGuid(),
                    Environment = "CLOUD1",
                    EventName = nameof(EmployeeCreated),
                    Initializer = "CLOUD1:CDC",
                    Timestamp = DateTime.Now,
                    Employee = MapEmployeeMessageToDexModel(employee)
                };

                if (!DeXEvent.PushToDeX(this._configuration, employeeCreated, this._logger))
                {
                    _logger.LogError("Could not push message to dex");
                }
            }
            else if(employee.OperationType == 4)
            {
                EmployeeUpdated employeeUpdated = new EmployeeUpdated
                {
                    EventId = Guid.NewGuid(),
                    Environment = "CLOUD1",
                    EventName = nameof(EmployeeUpdated),
                    Initializer = "CLOUD1:CDC",
                    Timestamp = DateTime.Now,
                    Employee = MapEmployeeMessageToDexModel(employee)
                };

                if (!DeXEvent.PushToDeX(this._configuration, employeeUpdated, this._logger))
                {
                    _logger.LogError("Could not push message to dex");
                }
            }
            else if(employee.OperationType == 1)
            {
                EmployeeDeleted employeeDeleted = new EmployeeDeleted
                {
                    EventId = Guid.NewGuid(),
                    Environment = "CLOUD1",
                    EventName = nameof(EmployeeDeleted),
                    Initializer = "CLOUD1:CDC",
                    Timestamp = DateTime.Now,
                    Employee = MapEmployeeMessageToDexModel(employee)
                };

                if (!DeXEvent.PushToDeX(this._configuration, employeeDeleted, this._logger))
                {
                    _logger.LogError("Could not push message to dex");
                }
            }

            return isSuccess;
        }

        private Employee MapEmployeeMessageToDexModel(EmployeeCT employee)
        {
            Employee dexEmployee = new Employee()
            {
                GlobalEmployeeId = employee.GlobalEmployeeId,
                Email = employee.Email,
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                HiredDate = employee.HiredDate,
                PhoneNumber = employee.PhoneNumber
            };

            return dexEmployee;
        }
    }
}
