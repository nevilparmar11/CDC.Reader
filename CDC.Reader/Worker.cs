using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CDC.Reader.Core.Enums;
using CDC.Reader.Infrastructure.Repository.Interface;
using CDC.Reader.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CDC.Reader.Core.Extensions;
using CDC.Event.Generator;
using Microsoft.Extensions.Configuration;

namespace CDC.Reader.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICDCProcessLogRepository _processLogRepository;
        private readonly IConfiguration _configuration;
        private EmployeeEventGenerator employeeEventGenerator;

        private byte[] lastProcessedLSN = null;

        public Worker(ILogger<Worker> logger, IEmployeeRepository employeeRepository, ICDCProcessLogRepository processLogRepository, IConfiguration configuration)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            _processLogRepository = processLogRepository;
            _configuration = configuration;
            employeeEventGenerator = new EmployeeEventGenerator(_logger, _configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await GetChanges();
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task GetChanges()
        {
            try
            {
                CDCProcessLogs lastLog = await _processLogRepository.GetCDCProcessLog(TableName: TableNames.CDCProcessLogs);

                if(lastLog == null)
                {
                    lastLog = new CDCProcessLogs
                    {
                        TableName = TableNames.CDCProcessLogs,
                        TimeStamp = DateTime.Now
                    };
                }

                var changes = await _employeeRepository.GetCDCEmployeeRecords(lastLog?.LSN.ConvertToString());

                if(changes.ToList().Count > 0)
                {
                    LogChanges(changes);
                    lastLog.LSN = lastProcessedLSN;
                    int result = await _processLogRepository.SaveCDCProcessLog(lastLog);
                } else
                {
                    _logger.LogInformation("All events have been processed, no new events found!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging employee change event: {ex.Message}");
            }
        }

        private void LogChanges(IEnumerable<EmployeeCT> changes)
        {
            _logger.LogInformation($"Length Of the Changes: {changes.ToList().Count} ");

            foreach (EmployeeCT emp in changes)
            {
                _logger.LogInformation(emp.FirstName + " " + emp.LastName + " " + emp.PhoneNumber);
                lastProcessedLSN = emp.Start_LSN;
                if(emp.OperationType != 3)
                {
                    employeeEventGenerator.GenerateEvent(emp);
                }
            }
        }
    }
}
