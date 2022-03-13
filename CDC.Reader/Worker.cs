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

namespace CDC.Reader.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICDCProcessLogRepository _processLogRepository;

        private byte[] lastProcessedLSN = null;

        public Worker(ILogger<Worker> logger, IEmployeeRepository employeeRepository, ICDCProcessLogRepository processLogRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            _processLogRepository = processLogRepository;
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
                    lastLog = new CDCProcessLogs();
                    lastLog.TableName = TableNames.CDCProcessLogs;
                    lastLog.TimeStamp = DateTime.Now;
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

                // TODO : determine what events to dispatch based on OperationType
                // TODO : In the scenario that multiple instances of this worker are running
                //        we need to determine the last record pulled off the cdc table
                //        and insert that into another table for tracking. Don't cross paths
                //        between instances of the worker service.

                // TODO : After processing is completed, delete the records? Or let a SQL Agent
                //        job come in later for cleanup?
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
            }
        }
    }
}
