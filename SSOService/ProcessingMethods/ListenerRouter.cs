using PowerTrackEnterprise.Core.Handlers;
using PowerTrackEnterprise.Core.ProcessingMethods.ProcessDestination;
using PowerTrackEnterprise.Core.ProcessingMethods.ProcessListeners;
using PowerTrackEnterprise.Data.Relational.Models;
using PowerTrackEnterprise.EnumLibrary.Enums;
using Remote.DataMarshal.Service.OperationalModels;
using System;
using System.Net;
using System.Threading;

namespace Remote.DataMarshal.Service.ProcessingMethods
{
    public class ListenerRouter
    {
        private static ListenerManifestDTO _listenerManifest;

        public static Thread Router(ListenerManifestDTO listenManifest)
        {
            try
            {
                _listenerManifest = listenManifest;
                var threadName = $"{listenManifest.ProcessName} | {listenManifest.ProcessCode}";

                // switch to determine source type
                switch (listenManifest.PortListenProcessor)
                {
                    case PortListenProcessor.Native:
                        return new Thread(() =>
                        {
                            // initiate the port listener and pass a call back for processing of received data.
                            var listenerNative = new ListenerNative();

                            listenerNative.StartListening(IPAddress.Parse(listenManifest.BindingIp),
                                listenManifest.BindingPort,
                                threadName,
                                (content) => { ProcessData(content); });
                        })
                        {
                            Name = threadName
                        };

                    case PortListenProcessor.Snpp:
                        return null;

                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Log(e);
                DBLogHandler.LogException(e);

                return null;
            }
        }

        /// <summary>
        /// Inject Custom Processing Code Here
        /// </summary>
        /// <param name="data">Data Received from the Socket</param>
        public static void ProcessData(string data)
        {
            try
            {
                if (data.Length < 1)
                {
                    LogHandler.LogWarn(
                        $"{_listenerManifest.ProcessName}|{_listenerManifest.ProcessCode} >>> No Data to Push at this time.");
                    DBLogHandler.LogWarn(
    $"{_listenerManifest.ProcessName}|{_listenerManifest.ProcessCode} >>> No Data to Push at this time.");

                    return;
                }

                LogHandler.LogInfo($"{_listenerManifest.ProcessCode} has {data.Length} characters for push at this time");

                var processDestinationResult =
                        ApplicationProcessingInterface.Push(
                            _listenerManifest.DestinationDefinition, Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                _listenerManifest.ProcessCode,
                                Data = data
                            }));

                if (processDestinationResult)
                {
                    var lastRun = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    _listenerManifest.LastRun = lastRun;
                    DataMarshalService.UpdateListenerManifest(_listenerManifest);
                    LogHandler.LogInfo(
                        $"{_listenerManifest.ProcessName}|{_listenerManifest.ProcessCode} >>> Pushed Message Successfully and updated Manifest to {lastRun}.");
                }
                else
                {
                    LogHandler.LogWarn(
                        $"{_listenerManifest.ProcessName}|{_listenerManifest.ProcessCode} >>> Failed on Data Push. Session will be retried");
                    DBLogHandler.LogWarn(
    $"{_listenerManifest.ProcessName}|{_listenerManifest.ProcessCode} >>> Failed on Data Push. Session will be retried");


                    // Todo: Handle Failed Messages
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Log(e);
                DBLogHandler.LogException(e);

            }
        }
    }
}
