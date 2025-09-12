using System.Diagnostics;
using System.Globalization;
using System.Printing;
using HASS.Agent.Models.Internal;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing printer-related info
    /// </summary>
    public class PrintersSensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "printers";

        private bool _errorPrinted = false;

        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new();

        public PrintersSensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 20, id)
        {
            _updateInterval = updateInterval ?? 20;

            UpdateSensorValues();
        }

        private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
        {
            if (!Sensors.ContainsKey(sensorId))
                Sensors.Add(sensorId, sensor);
            else
                Sensors[sensorId] = sensor;
        }

        public sealed override void UpdateSensorValues()
        {
            try
            {
                var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);
                var deviceName = SharedHelperFunctions.GetSafeDeviceName();

                var printerInfo = GetPrinterInfo();

                var printersCountEntityName = $"{parentSensorSafeName}_printers_count";
                var printersCountId = $"{Id}_printers_count";
                var printersCountSensor = new DataTypeIntSensor(_updateInterval, printersCountEntityName, "Printers Count", printersCountId, string.Empty, "measurement", "mdi:printer", string.Empty, EntityName);
                printersCountSensor.SetState(printerInfo.PrintQueues.Count);
                AddUpdateSensor(printersCountId, printersCountSensor);

                var defaultQueueEntityName = $"{parentSensorSafeName}_default_queue";
                var defaultQueueId = $"{Id}_default_queue";
                var defaultQueueSensor = new DataTypeStringSensor(_updateInterval, defaultQueueEntityName, "Default Queue", defaultQueueId, string.Empty, "mdi:printer", string.Empty, EntityName);
                defaultQueueSensor.SetState(printerInfo.DefaultQueue);
                AddUpdateSensor(defaultQueueId, defaultQueueSensor);

                var defaultQueueJobsEntityName = $"{parentSensorSafeName}_default_queue_jobs";
                var defaultQueueJobsId = $"{Id}_default_queue_jobs";
                var defaultQueueJobsSensor = new DataTypeIntSensor(_updateInterval, defaultQueueJobsEntityName, "Default Queue Jobs", defaultQueueJobsId, string.Empty, "measurement", "mdi:printer", string.Empty, EntityName);
                defaultQueueJobsSensor.SetState(printerInfo.DefaultQueueJobs);
                AddUpdateSensor(defaultQueueJobsId, defaultQueueJobsSensor);

                foreach (var printer in printerInfo.PrintQueues)
                {
                    var printerQueueInfo = JsonConvert.SerializeObject(printer, Formatting.Indented);
                    var printerEntityName = $"{parentSensorSafeName}_{SharedHelperFunctions.GetSafeValue(printer.Name)}";
                    var printerId = $"{Id}_{SharedHelperFunctions.GetSafeValue(printer.Name)}";
                    var printerSensor = new DataTypeIntSensor(_updateInterval, printerEntityName, $"{printer.Name}", printerId, string.Empty, "measurement", "mdi:printer", string.Empty, EntityName, true);

                    printerSensor.SetState(printer.Jobs);
                    printerSensor.SetAttributes(printerQueueInfo);
                    AddUpdateSensor(printerId, printerSensor);
                }

                // optionally reset error flag
                if (_errorPrinted)
                    _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted)
                    return;

                _errorPrinted = true;

                Log.Fatal(ex, "[PRINTERS] [{name}] Error while fetching audio info: {err}", EntityName, ex.Message);
            }
        }

        private PrinterInfo GetPrinterInfo()
        {
            var printerInfo = new PrinterInfo();

            try
            {
                var errors = false;

                try
                {
                    using var localPrintServer = new LocalPrintServer();
                    localPrintServer.Refresh();

                    using var defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                    defaultPrintQueue.Refresh();

                    printerInfo.DefaultQueue = defaultPrintQueue.Name;
                    printerInfo.DefaultQueueJobs = defaultPrintQueue.NumberOfJobs;

                    // fetch all queues
                    foreach (var queue in localPrintServer.GetPrintQueues())
                    {
                        if (queue == null)
                            continue;

                        try
                        {
                            queue.Refresh();

                            var queueInfo = new PrintQueueInfo
                            {
                                Name = queue.Name,
                                Jobs = queue.NumberOfJobs,
                                Location = queue.Location,
                                Driver = queue.QueueDriver.Name
                            };

                            using var jobs = queue.GetPrintJobInfoCollection();
                            foreach (var job in jobs)
                            {
                                if (job == null)
                                    continue;

                                try
                                {
                                    job.Refresh();

                                    var queueJob = new PrintJobInfo
                                    {
                                        Name = job.Name,
                                        Submitter = job.Submitter,
                                        Status = job.JobStatus.ToString(),
                                        NumberOfPages = job.NumberOfPages,
                                        NumberOfPagesPrinted = job.NumberOfPagesPrinted,
                                        PositionInPrintQueue = job.PositionInPrintQueue
                                    };

                                    queueInfo.PrintJobs.Add(queueJob);
                                }
                                catch (Exception ex)
                                {
                                    if (!_errorPrinted)
                                        Log.Fatal(ex, "[PRINTERS] [{name}] [{queue}] Exception while retrieving job: {err}", EntityName, queue.Name ?? "-", ex.Message);

                                    errors = true;
                                }
                                finally
                                {
                                    job.Dispose();
                                }
                            }

                            // status
                            queueInfo.Status = queue.QueueStatus.ToString();
                            queueInfo.AveragePagesPerMinute = queue.AveragePagesPerMinute;
                            queueInfo.HasPaperProblem = queue.HasPaperProblem;
                            queueInfo.IsBusy = queue.IsBusy;
                            queueInfo.IsDoorOpened = queue.IsDoorOpened;
                            queueInfo.IsInError = queue.IsInError;
                            queueInfo.IsInitializing = queue.IsInitializing;
                            queueInfo.IsNotAvailable = queue.IsNotAvailable;
                            queueInfo.IsOffline = queue.IsOffline;
                            queueInfo.IsIOActive = queue.IsIOActive;
                            queueInfo.IsOutOfMemory = queue.IsOutOfMemory;
                            queueInfo.IsOutOfPaper = queue.IsOutOfPaper;
                            queueInfo.IsPaperJammed = queue.IsPaperJammed;
                            queueInfo.IsOutputBinFull = queue.IsOutputBinFull;
                            queueInfo.IsManualFeedRequired = queue.IsManualFeedRequired;
                            queueInfo.IsPaused = queue.IsPaused;
                            queueInfo.IsPendingDeletion = queue.IsPendingDeletion;
                            queueInfo.IsPowerSaveOn = queue.IsPowerSaveOn;
                            queueInfo.IsPrinting = queue.IsPrinting;
                            queueInfo.IsProcessing = queue.IsProcessing;
                            queueInfo.IsPublished = queue.IsPublished;
                            queueInfo.IsQueued = queue.IsQueued;
                            queueInfo.IsTonerLow = queue.IsTonerLow;
                            queueInfo.IsWaiting = queue.IsWaiting;
                            queueInfo.IsWarmingUp = queue.IsWarmingUp;
                            queueInfo.NeedUserIntervention = queue.NeedUserIntervention;
                            queueInfo.PrintingIsCancelled = queue.PrintingIsCancelled;

                            // ready to add
                            printerInfo.PrintQueues.Add(queueInfo);
                        }
                        catch (Exception ex)
                        {
                            if (!_errorPrinted)
                                Log.Fatal(ex, "[PRINTERS] [{name}] [{queue}] Exception while retrieving queue: {err}", EntityName, queue.Name ?? "-", ex.Message);

                            errors = true;
                        }
                        finally
                        {
                            queue.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!_errorPrinted)
                        Log.Fatal(ex, "[PRINTERS] [{name}] Exception while retrieving info: {err}", EntityName, ex.Message);

                    errors = true;
                }

                // only print errors once
                if (errors && !_errorPrinted)
                {
                    _errorPrinted = true;
                    return printerInfo;
                }

                // optionally reset error flag
                if (_errorPrinted)
                    _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted)
                    return printerInfo;

                _errorPrinted = true;

                Log.Fatal(ex, "[PRINTERS] [{name}] Fatal exception while getting info: {err}", EntityName, ex.Message);
            }

            return printerInfo;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
