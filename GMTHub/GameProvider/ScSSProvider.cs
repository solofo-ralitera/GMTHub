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

        public TelemetryData GetData()
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
            if (telemetryData.Paused)
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
                rpm_max = (ushort)telemetryData.TruckValues_ConstantsValues_MotorValues_EngineRpmMax,
                speed_ms = (ushort)Math.Abs(telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value),
                speed_kph = (ushort)(Math.Abs(telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value) * 3.6f),
                speed_Mph = (ushort)(Math.Abs(telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value) * 2.25f),

                gear = (sbyte)telemetryData.TruckValues_CurrentValues_DashboardValues_GearDashboards,
                odometer = telemetryData.TruckValues_CurrentValues_DashboardValues_Odometer,

                oilTemperature = telemetryData.TruckValues_CurrentValues_DashboardValues_OilTemperature,
                oilPressure = telemetryData.TruckValues_CurrentValues_DashboardValues_OilPressure,
                oilPressure_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_OilPressure,

                waterTemperature = telemetryData.TruckValues_CurrentValues_DashboardValues_WaterTemperature,
                waterTemperature_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_WaterTemperature,

                adblue = telemetryData.TruckValues_CurrentValues_DashboardValues_AdBlue,
                adblue_capacity = telemetryData.TruckValues_ConstantsValues_CapacityValues_AdBlue,
                adblue_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AdBlue,
                adblue_pct = (byte) (telemetryData.TruckValues_CurrentValues_DashboardValues_AdBlue * 100 / telemetryData.TruckValues_ConstantsValues_CapacityValues_AdBlue),

                airPressure = telemetryData.TruckValues_CurrentValues_MotorValues_BrakeValues_AirPressure,
                airPressure_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressure 
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressureEmergency,

                batteryVoltage = telemetryData.TruckValues_CurrentValues_DashboardValues_BatteryVoltage,
                batteryVoltage_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_BatteryVoltage,

                fuel = telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_Amount,
                fuel_averageConsumption = telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_AverageConsumption,
                fuel_range = telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_Range,
                fuel_capacity = telemetryData.TruckValues_ConstantsValues_CapacityValues_Fuel,
                fuel_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_FuelW,
                fuel_pct = (byte) (telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_Amount * 100 / telemetryData.TruckValues_ConstantsValues_CapacityValues_Fuel),

                warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_WaterTemperature
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AdBlue
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressure
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressureEmergency
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_BatteryVoltage
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_OilPressure
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_FuelW,

                electric_on = telemetryData.TruckValues_CurrentValues_ElectricEnabled,
                engine_on = telemetryData.TruckValues_CurrentValues_EngineEnabled,

                parkingBrake = telemetryData.TruckValues_CurrentValues_MotorValues_BrakeValues_ParkingBrake,

                blinkerLeft = telemetryData.TruckValues_CurrentValues_LightsValues_BlinkerLeftOn,
                blinkerRight = telemetryData.TruckValues_CurrentValues_LightsValues_BlinkerRightOn,
                hazardLight = telemetryData.TruckValues_CurrentValues_LightsValues_HazardWarningLights,

                speed_limit = telemetryData.NavigationValues_SpeedLimit,

                // TODO calculated warning
                // si frein parkin activé et accelerateur et vitesse != N
                // Si differential lock et vitesse elevé
                // Si depassement limitation vitesse

            };
        }
    }
}
