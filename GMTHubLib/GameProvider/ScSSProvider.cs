using GMTHubLib.MemoryProviders;
using GMTHubLib.Models;
using GMTHubLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GMTHubLib.GameProvider
{
    public class ScSSProvider : IGameProvider
    {
        private const string DefaultSharedMemoryMap = "Local\\SCSTelemetry";
        protected MemoryMappedFileProvider dataProvider;
        protected SCSSdkClient sdk;
        protected SCSTelemetry telemetryData;

        Blinker Blinker;

        public ScSSProvider()
        {
            sdk = new SCSSdkClient();
            dataProvider = new MemoryMappedFileProvider();
        }

        public string GetGameName()
        {
            return "Ets2";
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

        public void SetBlinker(Blinker blinker)
        {
            Blinker = blinker;
        }

        public TelemetryData GetData()
        {
            telemetryData = sdk.Convert(dataProvider.Update());
            // check if sdk is not running OR game paused
            if (!telemetryData.SdkActive)
            {
                // if sdk not active we don't need to do something
                ConsoleLog.Error("The game or the SCS-Telemetry is not running");
                Task.Delay(2000);
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
                blinker = Blinker,

                rpm = (ushort)telemetryData.TruckValues_CurrentValues_DashboardValues_RPM,
                rpm_max = (ushort)telemetryData.TruckValues_ConstantsValues_MotorValues_EngineRpmMax,
                speed_ms = Math.Abs(telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value),
                speed_kph = (int) (Math.Abs(telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value) * 3.6f),
                speed_Mph = (int) (Math.Abs(telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value) * 2.25f),

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
                adblue_pct = (ushort) (telemetryData.TruckValues_CurrentValues_DashboardValues_AdBlue * 100 / telemetryData.TruckValues_ConstantsValues_CapacityValues_AdBlue),

                airPressure = telemetryData.TruckValues_CurrentValues_MotorValues_BrakeValues_AirPressure,
                airPressure_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressure 
                    || telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressureEmergency,

                batteryVoltage = telemetryData.TruckValues_CurrentValues_DashboardValues_BatteryVoltage,
                batteryVoltage_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_BatteryVoltage,

                fuel = telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_Amount,
                fuel_averageConsumption = telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_AverageConsumption * 100,
                fuel_range = telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_Range,
                fuel_capacity = telemetryData.TruckValues_ConstantsValues_CapacityValues_Fuel,
                fuel_warning = telemetryData.TruckValues_CurrentValues_DashboardValues_WarningValues_FuelW,
                fuel_pct = (ushort) (telemetryData.TruckValues_CurrentValues_DashboardValues_FuelValue_Amount * 100 / telemetryData.TruckValues_ConstantsValues_CapacityValues_Fuel),

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
                motorBrake = telemetryData.TruckValues_CurrentValues_MotorValues_BrakeValues_MotorBrake,
                wipers = telemetryData.TruckValues_CurrentValues_DashboardValues_Wipers,
                brake = telemetryData.TruckValues_CurrentValues_MotorValues_BrakeValues_MotorBrake || telemetryData.TruckValues_CurrentValues_LightsValues_Brake,

                blinkerLeft = telemetryData.TruckValues_CurrentValues_LightsValues_BlinkerLeftOn,
                blinkerRight = telemetryData.TruckValues_CurrentValues_LightsValues_BlinkerRightOn,
                hazardLight = telemetryData.TruckValues_CurrentValues_LightsValues_HazardWarningLights,
                backLight = telemetryData.TruckValues_CurrentValues_LightsValues_DashboardBacklight,

                speedLimit_ms = telemetryData.NavigationValues_SpeedLimit,
                speedLimit_kph = telemetryData.NavigationValues_SpeedLimit * 3.6f,
                speedLimit_Mph = telemetryData.NavigationValues_SpeedLimit * 2.25f,
                speedLimit_warning = telemetryData.NavigationValues_SpeedLimit > 0 && telemetryData.TruckValues_CurrentValues_DashboardValues_Speed_Value > telemetryData.NavigationValues_SpeedLimit,

                cargoMass = telemetryData.JobValues_CargoValues_Mass,
                cargoMass_ton = telemetryData.JobValues_CargoValues_Mass / 1000,
                distance = telemetryData.NavigationValues_NavigationDistance / 1000,

                beaconLight = telemetryData.TruckValues_CurrentValues_ElectricEnabled && telemetryData.TruckValues_CurrentValues_LightsValues_Beacon,
                beamLowLight = telemetryData.TruckValues_CurrentValues_ElectricEnabled && telemetryData.TruckValues_CurrentValues_LightsValues_BeamLow,
                beamHighLight = telemetryData.TruckValues_CurrentValues_ElectricEnabled && telemetryData.TruckValues_CurrentValues_LightsValues_BeamLow && telemetryData.TruckValues_CurrentValues_LightsValues_BeamHigh,
                brakeLight = telemetryData.TruckValues_CurrentValues_ElectricEnabled && telemetryData.TruckValues_CurrentValues_LightsValues_Brake,
                cruiseControl_on = telemetryData.TruckValues_CurrentValues_DashboardValues_CruiseControl,
                cruiseControl_value = telemetryData.TruckValues_CurrentValues_DashboardValues_CruiseControlSpeed_Value,
                differentialLock = telemetryData.TruckValues_CurrentValues_DifferentialLock,
                parkingLight = telemetryData.TruckValues_CurrentValues_ElectricEnabled && telemetryData.TruckValues_CurrentValues_LightsValues_Parking,
                reverseLight = telemetryData.TruckValues_CurrentValues_ElectricEnabled && telemetryData.TruckValues_CurrentValues_LightsValues_Reverse,
                auxFront = telemetryData.TruckValues_CurrentValues_LightsValues_AuxFront,
                auxRoof = telemetryData.TruckValues_CurrentValues_LightsValues_AuxRoof,
                // TODO calculated warning
                // si frein parkin activé et accelerateur et vitesse != N
                // Si differential lock et vitesse elevé
                // Si depassement limitation vitesse

                damage_cabin = telemetryData.TruckValues_CurrentValues_DamageValues_Cabin * 100,
                damage_chassis = telemetryData.TruckValues_CurrentValues_DamageValues_Chassis * 100,
                damage_engine = telemetryData.TruckValues_CurrentValues_DamageValues_Engine * 100,
                damage_transmission = telemetryData.TruckValues_CurrentValues_DamageValues_Transmission * 100,
                damage_wheels = telemetryData.TruckValues_CurrentValues_DamageValues_WheelsAvg * 100,
                damage_trailer = telemetryData.JobValues_CargoValues_CargoDamage * 100,

                cargo_name = telemetryData.JobValues_CargoValues_Name,
                cargo_destination = telemetryData.JobValues_CityDestination,
                cargo_remaining_time = telemetryData.RemainingDeliveryTime,
            };
        }
    }
}
