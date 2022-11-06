using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GMTHub.Models.SCSTelemetry;

namespace GMTHub.Models
{
    public class SCSTelemetry
    {
        // https://etcars.readthedocs.io/en/master/thedata.html

        public bool SdkActive { get; internal set; }
        public bool Paused { get; internal set; }
        public ulong Timestamp { get; internal set; }
        public ulong SimulationTimestamp { get; internal set; }
        public ulong RenderTimestamp { get; internal set; }

        public uint DllVersion { get; internal set; }
        public uint GameVersionMajor { get; internal set; }
        public uint GameVersionMinor { get; internal set; }
        public uint Game { get; internal set; }
        public uint TelemetryVersionMajor { get; internal set; }
        public uint TelemetryVersionMinor { get; internal set; }

        public uint CommonValuesGameTime { get; internal set; }
        public uint TruckValues_ConstantsValues_MotorValues_ForwardGearCount { get; internal set; }
        public uint TruckValues_ConstantsValues_MotorValues_ReverseGearCount { get; internal set; }
        public uint TruckValues_ConstantsValues_MotorValues_RetarderStepCount { get; internal set; }
        public uint TruckValues_ConstantsValues_WheelsValues_Count { get; internal set; }
        public uint TruckValues_ConstantsValues_MotorValues_SelectorCount { get; internal set; }

        public uint DeliveryTime { get; internal set; }
        public int RemainingDeliveryTime { get; internal set; }

        public uint MaxTrailerCount { get; internal set; }
        public uint JobValues_CargoValues_UnitCount { get; internal set; }
        public uint JobValues_PlannedDistanceKm { get; internal set; }

        public uint TruckValues_CurrentValues_MotorValues_GearValues_HShifterSlot { get; internal set; }
        public uint TruckValues_CurrentValues_MotorValues_BrakeValues_RetarderLevel { get; internal set; }
        public uint TruckValues_CurrentValues_LightsValues_AuxFront { get; internal set; }
        public uint TruckValues_CurrentValues_LightsValues_AuxRoof { get; internal set; }
        public uint[] TruckValues_CurrentValues_WheelsValues_Substance { get; internal set; }

        public uint[] TruckValues_ConstantsValues_MotorValues_SlotHandlePosition { get; internal set; }
        public uint[] TruckValues_ConstantsValues_MotorValues_SlotSelectors { get; internal set; }

        public uint GamePlay_JobDelivered_DeliveryTime { get; internal set; }
        public Time GamePlay_JobCancelled_Started { get; internal set; }
        public Time GamePlay_JobDelivered_Started { get; internal set; }
        public Time GamePlay_JobCancelled_Finished { get; internal set; }
        public Time GamePlay_JobDelivered_Finished { get; internal set; }

        public int CommonValues_NextRestStop { get; internal set; }
        public int TruckValues_CurrentValues_MotorValues_GearValues_Selected { get; internal set; }
        public int TruckValues_CurrentValues_DashboardValues_GearDashboards { get; internal set; }
        public int[] TruckValues_ConstantsValues_MotorValues_SlotGear { get; internal set; }
        public int GamePlay_JobDelivered_EarnedXp { get; internal set; }


        public float CommonValues_Scale { get; internal set; }

        public float TruckValues_ConstantsValues_CapacityValues_Fuel { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_Fuel { get; internal set; }
        public float TruckValues_ConstantsValues_CapacityValues_AdBlue { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_AdBlue { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_AirPressure { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_AirPressureEmergency { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_OilPressure { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_WaterTemperature { get; internal set; }
        public float TruckValues_ConstantsValues_WarningFactorValues_BatteryVoltage { get; internal set; }
        public float TruckValues_ConstantsValues_MotorValues_EngineRpmMax { get; internal set; }
        public float TruckValues_ConstantsValues_MotorValues_DifferentialRation { get; internal set; }
        public float JobValues_CargoValues_Mass { get; internal set; }
        public float[] TruckValues_ConstantsValues_WheelsValues_Radius { get; internal set; }
        public float[] TruckValues_ConstantsValues_MotorValues_GearRatiosForward { get; internal set; }
        public float[] TruckValues_ConstantsValues_MotorValues_GearRatiosReverse { get; internal set; }
        public float JobValues_CargoValues_UnitMass { get; internal set; }

        public float TruckValues_CurrentValues_DashboardValues_Speed_Value { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_RPM { get; internal set; }
        public float ControlValues_InputValues_Steering { get; internal set; }
        public float ControlValues_InputValues_Throttle { get; internal set; }
        public float ControlValues_InputValues_Brake { get; internal set; }
        public float ControlValues_InputValues_Clutch { get; internal set; }
        public float ControlValues_GameValues_Steering { get; internal set; }
        public float ControlValues_GameValues_Throttle { get; internal set; }
        public float ControlValues_GameValues_Brake { get; internal set; }
        public float ControlValues_GameValues_Clutch { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_CruiseControlSpeed_Value { get; internal set; }
        public float TruckValues_CurrentValues_MotorValues_BrakeValues_AirPressure { get; internal set; }
        public float TruckValues_CurrentValues_MotorValues_BrakeValues_Temperature { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_FuelValue_Amount { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_FuelValue_AverageConsumption { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_FuelValue_Range { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_AdBlue { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_OilPressure { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_OilTemperature { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_WaterTemperature { get; internal set; }
        public float TruckValues_CurrentValues_DashboardValues_BatteryVoltage { get; internal set; }
        public float TruckValues_CurrentValues_LightsValues_DashboardBacklight { get; internal set; }
        public float TruckValues_CurrentValues_DamageValues_Engine { get; internal set; }
        public float TruckValues_CurrentValues_DamageValues_Transmission { get; internal set; }
        public float TruckValues_CurrentValues_DamageValues_Cabin { get; internal set; }
        public float TruckValues_CurrentValues_DamageValues_Chassis { get; internal set; }
        public float TruckValues_CurrentValues_DamageValues_WheelsAvg { get; internal set; }


        public float TruckValues_CurrentValues_DashboardValues_Odometer { get; internal set; }
        public float NavigationValues_NavigationDistance { get; internal set; }
        public float NavigationValues_NavigationTime { get; internal set; }
        public float NavigationValues_SpeedLimit { get; internal set; }
        public float[] TruckValues_CurrentValues_WheelsValues_SuspDeflection { get; internal set; }
        public float[] TruckValues_CurrentValues_WheelsValues_Velocity { get; internal set; }
        public float[] TruckValues_CurrentValues_WheelsValues_Steering { get; internal set; }
        public float[] TruckValues_CurrentValues_WheelsValues_Rotation { get; internal set; }
        public float[] TruckValues_CurrentValues_WheelsValues_Lift { get; internal set; }
        public float[] TruckValues_CurrentValues_WheelsValues_LiftOffset { get; internal set; }

        public float GamePlay_JobDelivered_CargoDamage { get; internal set; }
        public float GamePlay_JobDelivered_DistanceKm { get; internal set; }

        public float GamePlay_RefuelEvent_Amount { get; internal set; }

        public float JobValues_CargoValues_CargoDamage { get; internal set; }

        public bool[] TruckValues_ConstantsValues_WheelsValues_Steerable { get; internal set; }
        public bool[] TruckValues_ConstantsValues_WheelsValues_Simulated { get; internal set; }
        public bool[] TruckValues_ConstantsValues_WheelsValues_Powered { get; internal set; }
        public bool[] TruckValues_ConstantsValues_WheelsValues_Liftable { get; internal set; }

        public bool JobValues_CargoLoaded { get; internal set; }
        public bool JobValues_SpecialJob{ get; internal set; }

        public bool TruckValues_CurrentValues_MotorValues_BrakeValues_ParkingBrake { get; internal set; }
        public bool TruckValues_CurrentValues_MotorValues_BrakeValues_MotorBrake { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressure { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressureEmergency { get; internal set; }

        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_FuelW { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_AdBlue { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_OilPressure { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_WaterTemperature { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_WarningValues_BatteryVoltage { get; internal set; }
        public bool TruckValues_CurrentValues_ElectricEnabled { get; internal set; }
        public bool TruckValues_CurrentValues_EngineEnabled { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_Wipers { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_BlinkerLeftActive { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_BlinkerRightActive { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_BlinkerLeftOn { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_BlinkerRightOn { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_Parking { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_BeamLow { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_BeamHigh { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_Beacon { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_Brake { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_Reverse { get; internal set; }
        public bool TruckValues_CurrentValues_LightsValues_HazardWarningLights { get; internal set; }
        public bool TruckValues_CurrentValues_DashboardValues_CruiseControl { get; internal set; }
        public bool[] TruckValues_CurrentValues_WheelsValues_OnGround { get; internal set; }
        public bool[] TruckValues_CurrentValues_MotorValues_GearValues_HShifterSelector { get; internal set; }

        public bool TruckValues_CurrentValues_DifferentialLock { get; internal set; }
        public bool TruckValues_CurrentValues_LiftAxle { get; internal set; }
        public bool TruckValues_CurrentValues_LiftAxleIndicator { get; internal set; }
        public bool TruckValues_CurrentValues_TrailerLiftAxle { get; internal set; }
        public bool TruckValues_CurrentValues_TrailerLiftAxleIndicator { get; internal set; }

        public bool GamePlay_JobDelivered_AutoParked { get; internal set; }
        public bool GamePlay_JobDelivered_AutoLoaded { get; internal set; }


        public FVector TruckValues_Positioning_Cabin { get; internal set; }
        public FVector TruckValues_Positioning_Head { get; internal set; }
        public FVector TruckValues_Positioning_Hook { get; internal set; }
        public FVector[] TruckValues_ConstantsValues_WheelsValues_PositionValues { get; internal set; }
        public FVector TruckValues_CurrentValues_AccelerationValues_LinearVelocity { get; internal set; }
        public FVector TruckValues_CurrentValues_AccelerationValues_AngularVelocity { get; internal set; }
        public FVector TruckValues_CurrentValues_AccelerationValues_LinearAcceleration { get; internal set; }
        public FVector TruckValues_CurrentValues_AccelerationValues_AngularAcceleration { get; internal set; }
        public FVector TruckValues_CurrentValues_AccelerationValues_CabinAngularVelocity { get; internal set; }
        public FVector TruckValues_CurrentValues_AccelerationValues_CabinAngularAcceleration { get; internal set; }

        public FPlacement TruckValues_Positioning_CabinOffset { get; internal set; }
        public FPlacement TruckValues_Positioning_HeadOffset { get; internal set; }

        public DPlacement TruckValues_CurrentValues_PositionValue { get; internal set; }
        public DPlacement TruckValues_Positioning_TruckPosition { get; internal set; }


        public string TruckValues_ConstantsValues_BrandId { get; internal set; }
        public string TruckValues_ConstantsValues_Brand { get; internal set; }
        public string TruckValues_ConstantsValues_Id { get; internal set; }
        public string TruckValues_ConstantsValues_Name { get; internal set; }
        public string JobValues_CargoValues_Id { get; internal set; }
        public string JobValues_CargoValues_Name { get; internal set; }
        public string JobValues_CityDestinationId { get; internal set; }
        public string JobValues_CityDestination { get; internal set; }
        public string JobValues_CompanyDestinationId { get; internal set; }
        public string JobValues_CompanyDestination { get; internal set; }
        public string JobValues_CitySourceId { get; internal set; }
        public string JobValues_CitySource { get; internal set; }
        public string JobValues_CompanySourceId { get; internal set; }
        public string JobValues_CompanySource { get; internal set; }
        public string TruckValues_ConstantsValues_MotorValues_ShifterTypeValue { get; internal set; }
        public string TruckValues_ConstantsValues_LicensePlate { get; internal set; }
        public string TruckValues_ConstantsValues_LicensePlateCountryId { get; internal set; }
        public string TruckValues_ConstantsValues_LicensePlateCountry { get; internal set; }
        public string JobValues_Market { get; internal set; }
        public string GamePlay_FinedEvent_Offence { get; internal set; }
        public string GamePlay_FerryEvent_SourceName { get; internal set; }
        public string GamePlay_FerryEvent_TargetName { get; internal set; }
        public string GamePlay_FerryEvent_SourceId { get; internal set; }
        public string GamePlay_FerryEvent_TargetId { get; internal set; }
        public string GamePlay_TrainEvent_SourceName { get; internal set; }
        public string GamePlay_TrainEvent_TargetName { get; internal set; }
        public string GamePlay_TrainEvent_SourceId { get; internal set; }
        public string GamePlay_TrainEvent_TargetId { get; internal set; }

        public ulong JobValues_Income { get; internal set; }

        public long GamePlay_JobCancelled_Penalty { get; internal set; }
        public long GamePlay_JobDelivered_Revenue { get; internal set; }
        public long GamePlay_FinedEvent_Amount { get; internal set; }
        public long GamePlay_TollgateEvent_PayAmount { get; internal set; }
        public long GamePlay_FerryEvent_PayAmount { get; internal set; }
        public long GamePlay_TrainEvent_PayAmount { get; internal set; }

        public bool SpecialEventsValues_OnJob { get; internal set; }
        public bool SpecialEventsValues_JobFinished { get; internal set; }

        public bool SpecialEventsValues_JobCancelled { get; internal set; }
        public bool SpecialEventsValues_JobDelivered { get; internal set; }
        public bool SpecialEventsValues_Fined { get; internal set; }
        public bool SpecialEventsValues_Tollgate { get; internal set; }
        public bool SpecialEventsValues_Ferry { get; internal set; }
        public bool SpecialEventsValues_Train { get; internal set; }

        public bool SpecialEventsValues_Refuel { get; internal set; }
        public bool SpecialEventsValues_RefuelPayed { get; internal set; }

        public List<Substance> Substances = new List<Substance>();

        public class Substance
        {
            /// <summary>
            ///     Index of the substance in-game
            /// </summary>
            public int Index { get; internal set; }

            /// <summary>
            ///     Name of the substance
            /// </summary>
            public string Value { get; internal set; }
        }

        public class DPlacement
        {
            /// <summary>
            ///     Represents a Position
            /// </summary>
            public DVector Position { get; internal set; }

            /// <summary>
            ///     Represents a Orientation
            /// </summary>
            public Euler Orientation { get; internal set; }
        }

        public class DVector
        {
            /// <summary>
            ///     X Coordinate of the Vector
            ///     In local space point right
            ///     In local space points east
            /// </summary>
            public double X { get; internal set; }

            /// <summary>
            ///     Y Coordinate of the Vector
            ///     In local space points up
            ///     In world space points ip
            /// </summary>
            public double Y { get; internal set; }

            /// <summary>
            ///     Z Coordinate of the Vector
            ///     In local space points backwards
            ///     In world space points south
            /// </summary>
            public double Z { get; internal set; }
        }

        public class FVector
        {
            /// <summary>
            ///     X Coordinate of the Vector
            ///     In local space point right
            ///     In local space points east
            /// </summary>
            public float X { get; internal set; }

            /// <summary>
            ///     Y Coordinate of the Vector
            ///     In local space points up
            ///     In world space points ip
            /// </summary>
            public float Y { get; internal set; }

            /// <summary>
            ///     Z Coordinate of the Vector
            ///     In local space points backwards
            ///     In world space points south
            /// </summary>
            public float Z { get; internal set; }
        }

        public class Euler
        {
            /// About: Heading
            /// Stored in unit range where <0,1) corresponds to <0,360).
            /// The angle is measured counterclockwise in horizontal plane when looking
            /// from top where 0 corresponds to forward(north), 0.25 to left(west),
            /// 0.5 to backward(south) and 0.75 to right(east).
            ///
            /// About: Pitch
            /// Stored in unit range where <-0.25,0.25> corresponds to <-90,90>.
            /// The pitch angle is zero when in horizontal direction,
            /// with positive values pointing up(0.25 directly to zenith),
            /// and negative values pointing down(-0.25 directly to nadir).
            ///
            /// About: Roll
            /// Stored in unit range where <-0.5,0.5> corresponds to <-180,180>.
            /// The angle is measured in counterclockwise when looking in direction of
            /// the roll axis.

            /// <summary>
            /// Heading
            /// </summary>
            ///
            /// <!----> **INFORMATION** <!---->
            /// Stored in unit range where <0,1) corresponds to <0,360).
            /// The angle is measured counterclockwise in horizontal plane when looking
            /// from top where 0 corresponds to forward(north), 0.25 to left(west),
            /// 0.5 to backward(south) and 0.75 to right(east).
            /// <!----> **INFORMATION** <!---->
            public float Heading { get; internal set; }

            /// <summary>
            /// Pitch
            /// </summary>
            ///
            /// <!----> **INFORMATION** <!---->
            /// Stored in unit range where <-0.25,0.25> corresponds to <-90,90>.
            /// The pitch angle is zero when in horizontal direction,
            /// with positive values pointing up(0.25 directly to zenith),
            /// and negative values pointing down(-0.25 directly to nadir).
            /// <!----> **INFORMATION** <!---->
            public float Pitch { get; internal set; }

            /// <summary>
            /// Roll
            /// </summary>
            ///
            /// <!----> **INFORMATION** <!---->
            /// Stored in unit range where <-0.5,0.5> corresponds to <-180,180>.
            /// The angle is measured in counterclockwise when looking in direction of
            /// the roll axis.
            /// <!----> **INFORMATION** <!---->
            public float Roll { get; internal set; }
        }

        public class FPlacement
        {
            /// <summary>
            ///     Represents a position
            /// </summary>
            public FVector Position { get; internal set; }

            /// <summary>
            ///     Represents a orientation
            /// </summary>
            public Euler Orientation { get; internal set; }
        }

        public class Time
        {
            public Time(uint i) => Value = i;

            public Time() { }

            /// <summary>
            ///     Represented in number of in-game minutes
            /// </summary>
            public uint Value { get; internal set; }

            /// <summary>
            ///     Represented in data of in-game minutes
            /// </summary>
            public DateTime Date => MinutesToDate(Value);

            public static implicit operator Time(uint i) => new Time(i);

            public static Time operator -(Time a, Time b) => new Time(a.Value - b.Value);
        }

        internal static DateTime MinutesToDate(uint minutes) => new DateTime((long)minutes * 10000000 * 60, DateTimeKind.Utc);


    }
}
