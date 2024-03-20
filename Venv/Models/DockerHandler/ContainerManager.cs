﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Venv.Models.DockerHandler.Interfaces;
using Venv.Services;

namespace Venv.Models.DockerHandler
{
    public class ContainerManager
    {
        private readonly ISshClient _sshClient;
        private readonly ShipDataService _shipDataService;
        private readonly List<string> failedEvents = new List<string>();

        public ContainerManager(ISshClient ssh, ShipDataService shipDataService)
        {
            _sshClient = ssh;
            _shipDataService = shipDataService;
        }

        public Task ListenForEventsAsync(DPU? singleDpu)
        {
            return Task.Run(() => ProcessEvents(singleDpu));
        }
        public void ProcessEvents(DPU? singleDpu)
        {
            //Single DPU
            if (singleDpu != null)
            {
                _shipDataService.UpdateDpuStatus(singleDpu.Number, "Receiving Command");
                while (!_shipDataService.IsDpuInFinalState(singleDpu))
                {
                    string? eventMessage = ReadNextEvent();
                    if (!string.IsNullOrEmpty(eventMessage))
                    {
                        HandleEvent(eventMessage);
                    }
                }
            }
            //Multiple DPU's
            else
            {
                foreach (var dpu in _shipDataService.GetSelectedDpus())
                {
                    _shipDataService.UpdateDpuStatus(dpu.Number, "Receiving Command");
                }
                while (!_shipDataService.AreAllDpusInFinalState())
                {
                    string? eventMessage = ReadNextEvent();
                    if (!string.IsNullOrEmpty(eventMessage))
                    {
                        HandleEvent(eventMessage);
                    }
                }
            }

        }
        private string? ReadNextEvent()
        {
            var reader = _sshClient.GetStandardOutput();
            if (reader != null)
            {
                Task<string?> readLineTask = Task.Run(() => reader.ReadLine());
                bool completed = readLineTask.Wait(TimeSpan.FromSeconds(1)); // 5-second timeout
                if (completed && readLineTask.Result != null )
                {
                    return readLineTask.Result;
                }
                else
                {
                    // Handle timeout or no data available
                    return null;
                }
            }
            return null;
        }
        private void HandleEvent(string eventMessage)
        {
            var cleanMessage = Regex.Replace(eventMessage, @"\u001b\[\d+m", string.Empty);

            var match = Regex.Match(cleanMessage, @"Container (DPU\d+)\s+([a-zA-Z]+)");
            if (match.Success)
            {
                string status = match.Groups[2].Value;
                string containerName = match.Groups[1].Value;

                var numberMatch = Regex.Match(containerName, @"\d+");
                if (int.TryParse(numberMatch.Value, out int dpuNum))
                {
                    _shipDataService.UpdateDpuStatus(dpuNum, status);
                }
                else
                {
                    Console.WriteLine($"Failed to parse DPU number {dpuNum}");
                }
            }
            else
            {
                failedEvents.Add(eventMessage);
            }
        }

    }
}

