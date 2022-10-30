using GMTHub.MemoryProviders;
using GMTHub.Models;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.GameProvider
{
    internal class ScSSProvider : IGameProvider
    {
        private const string DefaultSharedMemoryMap = "Local\\SCSTelemetry";
        protected MemoryMappedFileProvider dataProvider;
        protected SCSSdkClient sdk;
        protected SCSTelemetry telemetryData;

        public ScSSProvider()
        {
            sdk = new SCSSdkClient();
            dataProvider = new MemoryMappedFileProvider();
        }

        public bool Init()
        {
            dataProvider.Connect(DefaultSharedMemoryMap);
            if (!dataProvider.Hooked)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Error: dataProvider hooked");
                Console.ResetColor();
                return false;
            }
            return true;
        }

        public TelemetryData Loop()
        {
            telemetryData = sdk.Convert(dataProvider.Update());
            // check if sdk is not running OR game paused
            if (!telemetryData.SdkActive)
            {
                // if sdk not active we don't need to do something
                ConsoleLog.Error("The game or the SCS-Telemetry is not running");
                return new TelemetryData
                {
                    notfilled = true
                };
            }
            if(telemetryData.Paused)
            {
                return new TelemetryData
                {
                    notfilled = true
                };
            }
 
            return new TelemetryData
            {
                notfilled = false,
                rpm = (ushort)telemetryData.TruckValues_CurrentValues_DashboardValues_RPM,
                rpm_max = (ushort) telemetryData.TruckValues_ConstantsValues_MotorValues_EngineRpmMax,
                speed = (short)telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value,
                gear = (sbyte)telemetryData.TruckValues_CurrentValues_DashboardValues_GearDashboards
            };
        }
    }
}
