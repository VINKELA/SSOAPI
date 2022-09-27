using PowerTrackEnterprise.Core.Handlers;
using PowerTrackEnterprise.Core.Models;
using PowerTrackEnterprise.Core.ProcessingMethods.ProcessDatabase;
using PowerTrackEnterprise.Core.ProcessingMethods.ProcessDestination;
using PowerTrackEnterprise.Core.Utility;
using PowerTrackEnterprise.Data.Relational.Models;
using PowerTrackEnterprise.EnumLibrary.Enums;
using Remote.DataMarshal.Service.OperationalModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Remote.DataMarshal.Service.ProcessingMethods
{
    public class ProcessRouter
    {
        public static void Router(MarshalManifestDTO marshalManifest, DateTime signalTime)
        {
            try
            {
                var data = new List<object>();
                var lastExecution = signalTime;
                var lastRun = marshalManifest.LastRun != null ? DateTime.ParseExact(marshalManifest.LastRun, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) :
                                 DateTime.Now;
                var date = DateTime.Now;
                if (date.Subtract(lastRun).TotalMinutes < marshalManifest.DelayTimeInMinutes)
                {
                    if (marshalManifest.RepeatMonthlyPull)
                    {
                        var repeatAt = marshalManifest.RepeatPullStartTimeInMinutes;
                        if (repeatAt == null) return;
                        var restartDate = lastRun.AddMinutes(-(double)repeatAt);
                        marshalManifest.LastRun = restartDate.ToString("yyyyMMddHHmmssfff");
                    }
                    return;
                }
                // switch to determine source type
                switch ((ProcessSourceType)marshalManifest.SourceType)
                {
                    case ProcessSourceType.Database:
                        using (var dataBaseMarshal =
                            new ProcessDatabaseMarshal(
                                Newtonsoft.Json.JsonConvert.DeserializeObject<SourceDefinitionDatabaseDTO>(
                                    Newtonsoft.Json.JsonConvert.SerializeObject(marshalManifest.SourceDefinition))))
                        {
                            data = dataBaseMarshal.PullData(marshalManifest.LastRun,
                                signalTime, marshalManifest.SeekMode, marshalManifest.IntervalOffset);

                            lastExecution = dataBaseMarshal.LastExecution;
                        }

                        break;
                }

                if (!data.Any())
                {
                    var message = $"{marshalManifest.ProcessName}|{marshalManifest.ProcessCode} >>> No Data to Push at this time.";
                    marshalManifest.LastRun = lastExecution.ToString("yyyyMMddHHmmssfff");
                    DataMarshalService.UpdateMarshalManifest(marshalManifest);
                    LogHandler.LogWarn(message);
                    return;
                }
                foreach (var dataChunk in Transforms.ListChunk(data, 1000))
                {
                    var processDestinationResult =
                        ApplicationProcessingInterface.Push(
                            marshalManifest.DestinationDefinition, Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                marshalManifest.ProcessCode,
                                Data = dataChunk
                            }));

                    if (processDestinationResult)
                    {
                        marshalManifest.LastRun = lastExecution.ToString("yyyyMMddHHmmssfff");
                        DataMarshalService.UpdateMarshalManifest(marshalManifest);
                        LogHandler.LogInfo(
                            $"{marshalManifest.ProcessName}|{marshalManifest.ProcessCode} >>> Pushed {dataChunk.Count} Data Items Successfully and updated Manifest to {lastExecution.ToString("yyyyMMddHHmmssfff")}.");
                    }
                    else
                    {
                        var error = $"{marshalManifest.ProcessName}|{marshalManifest.ProcessCode} >>> Failed on Data Push. Session will be retried";
                        LogHandler.LogWarn(error);
                        SendErrorMessages(marshalManifest, null, error);
                        break; // we are breaking - this means others in the iterating list won't be run, despite the fact that they could be successful
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Log(e);
                SendErrorMessages(marshalManifest, e);
            }
        }
        private static void SendErrorMessages(ManifestDefinitionDTO manifest, Exception e = null, string error = null)
        {
            //first time message was sent or interval exceeded
            if (manifest.LastErrorEmailTime == null ||
                DateTime.Now.Subtract(manifest.LastErrorEmailTime.Value).TotalMinutes > manifest.ErrorMessageIntervalInMinutes
                )
            {
                if (e != null)
                    DBLogHandler.LogException(e);
                else DBLogHandler.LogError(error);
                manifest.LastErrorEmailTime = DateTime.Now;
            }
        }

    }
}
