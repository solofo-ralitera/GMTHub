using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GMTHub.Models.SCSTelemetry;

namespace GMTHub.Models
{
    public class SCSSdkClient
    {
        private const int StringSize = 64;
        private const int WheelSize = 16;
        private const int Substances = 25;

        private readonly int[] _offsetAreas =
            {0, 40, 500, 700, 1500, 1640, 2000, 2200, 2300, 4000, 4200, 4300, 4400, 6000};

        private byte[] _data;
        private int _offset;

        private int _offsetArea;

        /// <summary>
        ///     Convert the Shared Memory Byte data structure in a C# object
        /// </summary>
        /// <param name="structureDataBytes">
        ///     byte array from the shared memory
        /// </param>
        /// <returns>
        ///     C# object with game data of the shared memory
        /// </returns>
        public SCSTelemetry Convert(byte[] structureDataBytes)
        {
            _offsetArea = 0;
            SetOffset();


            _data = structureDataBytes;
            var retData = new SCSTelemetry();

            #region FIRST ZONE 

            retData.SdkActive = GetBool();
            GetBoolArray(3);
            retData.Paused = GetBool();
            GetBoolArray(3);
            retData.Timestamp = GetULong();
            retData.SimulationTimestamp = GetULong();
            retData.RenderTimestamp = GetULong();

            NextOffsetArea();

            #endregion

            #region SECOND ZONE

            retData.DllVersion = GetUint();
            retData.GameVersionMajor = GetUint();
            retData.GameVersionMinor = GetUint();
            retData.Game = GetUint(); // 0: unknown, 1: ETS2, 2: ATS
            retData.TelemetryVersionMajor = GetUint();
            retData.TelemetryVersionMinor = GetUint();

            retData.CommonValuesGameTime = GetUint();
            retData.TruckValues_ConstantsValues_MotorValues_ForwardGearCount = GetUint();
            retData.TruckValues_ConstantsValues_MotorValues_ReverseGearCount = GetUint();
            retData.TruckValues_ConstantsValues_MotorValues_RetarderStepCount = GetUint();
            retData.TruckValues_ConstantsValues_WheelsValues_Count = GetUint();
            retData.TruckValues_ConstantsValues_MotorValues_SelectorCount = GetUint();
            SetDeliveryTime(retData, GetUint());
            retData.MaxTrailerCount = GetUint();
            retData.JobValues_CargoValues_UnitCount = GetUint();
            retData.JobValues_PlannedDistanceKm = GetUint();


            retData.TruckValues_CurrentValues_MotorValues_GearValues_HShifterSlot = GetUint();
            retData.TruckValues_CurrentValues_MotorValues_BrakeValues_RetarderLevel = GetUint();
            retData.TruckValues_CurrentValues_LightsValues_AuxFront = GetUint(); // 0: Off, 1: Dimmed, 2: Full
            retData.TruckValues_CurrentValues_LightsValues_AuxRoof = GetUint(); // 0: Off, 1: Dimmed, 2: Full
            retData.TruckValues_CurrentValues_WheelsValues_Substance = GetUintArray(WheelSize);

            retData.TruckValues_ConstantsValues_MotorValues_SlotHandlePosition = GetUintArray(32);
            retData.TruckValues_ConstantsValues_MotorValues_SlotSelectors = GetUintArray(32);

            retData.GamePlay_JobDelivered_DeliveryTime = GetUint();
            var jobStartingTime = new SCSTelemetry.Time(GetUint());
            retData.GamePlay_JobCancelled_Started = jobStartingTime;
            retData.GamePlay_JobDelivered_Started = jobStartingTime;
            var jobFinishingTime = new SCSTelemetry.Time(GetUint());
            retData.GamePlay_JobCancelled_Finished = jobFinishingTime;
            retData.GamePlay_JobDelivered_Finished = jobFinishingTime;

            NextOffsetArea();

            #endregion

            #region THIRD ZONE

            retData.CommonValues_NextRestStop = GetInt();

            retData.TruckValues_CurrentValues_MotorValues_GearValues_Selected = GetInt();
            retData.TruckValues_CurrentValues_DashboardValues_GearDashboards = GetInt();
            retData.TruckValues_ConstantsValues_MotorValues_SlotGear = GetIntArray(32);

            retData.GamePlay_JobDelivered_EarnedXp = GetInt();

            NextOffsetArea();

            #endregion

            #region 4TH ZONE

            retData.CommonValues_Scale = GetFloat();

            retData.TruckValues_ConstantsValues_CapacityValues_Fuel = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_Fuel = GetFloat();
            retData.TruckValues_ConstantsValues_CapacityValues_AdBlue = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_AdBlue = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_AirPressure = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_AirPressureEmergency = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_OilPressure = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_WaterTemperature = GetFloat();
            retData.TruckValues_ConstantsValues_WarningFactorValues_BatteryVoltage = GetFloat();
            retData.TruckValues_ConstantsValues_MotorValues_EngineRpmMax = GetFloat();
            retData.TruckValues_ConstantsValues_MotorValues_DifferentialRation = GetFloat();
            retData.JobValues_CargoValues_Mass = GetFloat();
            retData.TruckValues_ConstantsValues_WheelsValues_Radius = GetFloatArray(WheelSize);
            retData.TruckValues_ConstantsValues_MotorValues_GearRatiosForward = GetFloatArray(24);
            retData.TruckValues_ConstantsValues_MotorValues_GearRatiosReverse = GetFloatArray(8);
            retData.JobValues_CargoValues_UnitMass = GetFloat();

            retData.TruckValues_CurrentValues_DashboardValues_Speed_Value = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_RPM = GetFloat();
            retData.ControlValues_InputValues_Steering = GetFloat();
            retData.ControlValues_InputValues_Throttle = GetFloat();
            retData.ControlValues_InputValues_Brake = GetFloat();
            retData.ControlValues_InputValues_Clutch = GetFloat();
            retData.ControlValues_GameValues_Steering = GetFloat();
            retData.ControlValues_GameValues_Throttle = GetFloat();
            retData.ControlValues_GameValues_Brake = GetFloat();
            retData.ControlValues_GameValues_Clutch = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_CruiseControlSpeed_Value = GetFloat();
            retData.TruckValues_CurrentValues_MotorValues_BrakeValues_AirPressure = GetFloat();
            retData.TruckValues_CurrentValues_MotorValues_BrakeValues_Temperature = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_FuelValue_Amount = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_FuelValue_AverageConsumption = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_FuelValue_Range = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_AdBlue = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_OilPressure = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_OilTemperature = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_WaterTemperature = GetFloat();
            retData.TruckValues_CurrentValues_DashboardValues_BatteryVoltage = GetFloat();
            retData.TruckValues_CurrentValues_LightsValues_DashboardBacklight = GetFloat();
            retData.TruckValues_CurrentValues_DamageValues_Engine = GetFloat();
            retData.TruckValues_CurrentValues_DamageValues_Transmission = GetFloat();
            retData.TruckValues_CurrentValues_DamageValues_Cabin = GetFloat();
            retData.TruckValues_CurrentValues_DamageValues_Chassis = GetFloat();
            retData.TruckValues_CurrentValues_DamageValues_WheelsAvg = GetFloat();


            retData.TruckValues_CurrentValues_DashboardValues_Odometer = GetFloat();
            retData.NavigationValues_NavigationDistance = GetFloat();
            retData.NavigationValues_NavigationTime = GetFloat();
            retData.NavigationValues_SpeedLimit = GetFloat();
            retData.TruckValues_CurrentValues_WheelsValues_SuspDeflection = GetFloatArray(WheelSize);
            retData.TruckValues_CurrentValues_WheelsValues_Velocity = GetFloatArray(WheelSize);
            retData.TruckValues_CurrentValues_WheelsValues_Steering = GetFloatArray(WheelSize);
            retData.TruckValues_CurrentValues_WheelsValues_Rotation = GetFloatArray(WheelSize);
            retData.TruckValues_CurrentValues_WheelsValues_Lift = GetFloatArray(WheelSize);
            retData.TruckValues_CurrentValues_WheelsValues_LiftOffset = GetFloatArray(WheelSize);

            retData.GamePlay_JobDelivered_CargoDamage = GetFloat();
            retData.GamePlay_JobDelivered_DistanceKm = GetFloat();

            retData.GamePlay_RefuelEvent_Amount = GetFloat();

            retData.JobValues_CargoValues_CargoDamage = GetFloat();

            NextOffsetArea();

            #endregion

            #region 5Th ZONE

            retData.TruckValues_ConstantsValues_WheelsValues_Steerable = GetBoolArray(WheelSize);
            retData.TruckValues_ConstantsValues_WheelsValues_Simulated = GetBoolArray(WheelSize);
            retData.TruckValues_ConstantsValues_WheelsValues_Powered = GetBoolArray(WheelSize);
            retData.TruckValues_ConstantsValues_WheelsValues_Liftable = GetBoolArray(WheelSize);

            retData.JobValues_CargoLoaded = GetBool();
            retData.JobValues_SpecialJob = GetBool();

            retData.TruckValues_CurrentValues_MotorValues_BrakeValues_ParkingBrake = GetBool();
            retData.TruckValues_CurrentValues_MotorValues_BrakeValues_MotorBrake = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressure = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_AirPressureEmergency = GetBool();

            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_FuelW = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_AdBlue = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_OilPressure = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_WaterTemperature = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_WarningValues_BatteryVoltage = GetBool();
            retData.TruckValues_CurrentValues_ElectricEnabled = GetBool();
            retData.TruckValues_CurrentValues_EngineEnabled = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_Wipers = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_BlinkerLeftActive = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_BlinkerRightActive = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_BlinkerLeftOn = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_BlinkerRightOn = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_Parking = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_BeamLow = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_BeamHigh = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_Beacon = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_Brake = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_Reverse = GetBool();
            retData.TruckValues_CurrentValues_LightsValues_HazardWarningLights = GetBool();
            retData.TruckValues_CurrentValues_DashboardValues_CruiseControl = GetBool();
            retData.TruckValues_CurrentValues_WheelsValues_OnGround = GetBoolArray(WheelSize);
            retData.TruckValues_CurrentValues_MotorValues_GearValues_HShifterSelector = GetBoolArray(2);

            retData.TruckValues_CurrentValues_DifferentialLock = GetBool();
            retData.TruckValues_CurrentValues_LiftAxle = GetBool();
            retData.TruckValues_CurrentValues_LiftAxleIndicator = GetBool();
            retData.TruckValues_CurrentValues_TrailerLiftAxle = GetBool();
            retData.TruckValues_CurrentValues_TrailerLiftAxleIndicator = GetBool();

            retData.GamePlay_JobDelivered_AutoParked = GetBool();
            retData.GamePlay_JobDelivered_AutoLoaded = GetBool();

            NextOffsetArea();

            #endregion

            #region 6TH ZONE

            retData.TruckValues_Positioning_Cabin = GetFVector();
            retData.TruckValues_Positioning_Head = GetFVector();
            retData.TruckValues_Positioning_Hook = GetFVector();
            var tempPos = new SCSTelemetry.FVector[WheelSize];
            for (var j = 0; j < WheelSize; j++)
            {
                tempPos[j] = new SCSTelemetry.FVector { X = GetFloat() };
            }

            for (var j = 0; j < WheelSize; j++)
            {
                tempPos[j].Y = GetFloat();
            }

            for (var j = 0; j < WheelSize; j++)
            {
                tempPos[j].Z = GetFloat();
            }

            retData.TruckValues_ConstantsValues_WheelsValues_PositionValues = tempPos;


            retData.TruckValues_CurrentValues_AccelerationValues_LinearVelocity = GetFVector();
            retData.TruckValues_CurrentValues_AccelerationValues_AngularVelocity = GetFVector();
            retData.TruckValues_CurrentValues_AccelerationValues_LinearAcceleration = GetFVector();
            retData.TruckValues_CurrentValues_AccelerationValues_AngularAcceleration = GetFVector();
            retData.TruckValues_CurrentValues_AccelerationValues_CabinAngularVelocity = GetFVector();
            retData.TruckValues_CurrentValues_AccelerationValues_CabinAngularAcceleration = GetFVector();

            NextOffsetArea();

            #endregion

            #region 7TH ZONE

            retData.TruckValues_Positioning_CabinOffset = GetFPlacement();
            retData.TruckValues_Positioning_HeadOffset = GetFPlacement();


            NextOffsetArea();

            #endregion

            #region 8TH ZONE 

            SetTruckPosition(retData, GetDPlacement());


            NextOffsetArea();

            #endregion

            #region 9TH ZONE

            retData.TruckValues_ConstantsValues_BrandId = GetString();
            retData.TruckValues_ConstantsValues_Brand = GetString();
            retData.TruckValues_ConstantsValues_Id = GetString();
            retData.TruckValues_ConstantsValues_Name = GetString();
            retData.JobValues_CargoValues_Id = GetString();
            retData.JobValues_CargoValues_Name = GetString();
            retData.JobValues_CityDestinationId = GetString();
            retData.JobValues_CityDestination = GetString();
            retData.JobValues_CompanyDestinationId = GetString();
            retData.JobValues_CompanyDestination = GetString();
            retData.JobValues_CitySourceId = GetString();
            retData.JobValues_CitySource = GetString();
            retData.JobValues_CompanySourceId = GetString();
            retData.JobValues_CompanySource = GetString();
            var tempShift = GetString(16);
            if (tempShift?.Length > 0)
            {
                /*
                 * 0: Unknown
                 * 1: Arcade
                 * 2: Automatic
                 * 3: Manual
                 * 4: HShifter
                 * */
                retData.TruckValues_ConstantsValues_MotorValues_ShifterTypeValue = tempShift;
            }

            retData.TruckValues_ConstantsValues_LicensePlate = GetString();
            retData.TruckValues_ConstantsValues_LicensePlateCountryId = GetString();
            retData.TruckValues_ConstantsValues_LicensePlateCountry = GetString();

            var tempJobMarket = GetString(32);
            if (tempJobMarket?.Length > 0)
            {
                /*
                 * 0: NoValue,
                 * 1: cargo_market,
                 * 2: quick_job,
                 * 3: freight_market,
                 * 4: external_contracts,
                 * 5: external_market
                */
                retData.JobValues_Market = tempJobMarket;
            }

            var tempfineOffence = GetString(32);
            if (tempfineOffence?.Length > 0)
            {
                /*
                /// NoValue                 - No Value from the sdk
                /// Crash                   - Crash with another vehicle
                /// Avoid_sleeping          - driver did not sleep
                /// Wrong_way               - drive on the wrong side of the street
                /// Speeding_camera         - drives to fast at a camera
                /// No_lights               - drives without lights on
                /// Red_signal              - ignores a red signal
                /// Avoid-Weighting         - ignore weighting
                /// Speeding                - drives to fast
                /// Illegal_trailer         - carries a trailer that is not allowed in this area
                /// Avoid_Inspection        - avoid inspection
                /// Illegal_Border_Crossing - illegal border crossing
                /// Hard_Shoulder_Violation - hard shoulder violation
                /// Damaged_Vehicle_Usage   - damaged vehicle usage
                /// Generic                 - some other generic fine
                 * */
                retData.GamePlay_FinedEvent_Offence = tempfineOffence;
            }

            retData.GamePlay_FerryEvent_SourceName = GetString();
            retData.GamePlay_FerryEvent_TargetName = GetString();
            retData.GamePlay_FerryEvent_SourceId = GetString();
            retData.GamePlay_FerryEvent_TargetId = GetString();
            retData.GamePlay_TrainEvent_SourceName = GetString();
            retData.GamePlay_TrainEvent_TargetName = GetString();
            retData.GamePlay_TrainEvent_SourceId = GetString();
            retData.GamePlay_TrainEvent_TargetId = GetString();


            NextOffsetArea();

            #endregion

            #region 10TH ZONE

            retData.JobValues_Income = GetULong();

            NextOffsetArea();

            #endregion

            #region 11TH ZONE

            retData.GamePlay_JobCancelled_Penalty = GetLong();
            retData.GamePlay_JobDelivered_Revenue = GetLong();
            retData.GamePlay_FinedEvent_Amount = GetLong();
            retData.GamePlay_TollgateEvent_PayAmount = GetLong();
            retData.GamePlay_FerryEvent_PayAmount = GetLong();
            retData.GamePlay_TrainEvent_PayAmount = GetLong();

            NextOffsetArea();

            #endregion

            #region 12TH ZONE

            retData.SpecialEventsValues_OnJob = GetBool();
            retData.SpecialEventsValues_JobFinished = GetBool();

            retData.SpecialEventsValues_JobCancelled = GetBool();
            retData.SpecialEventsValues_JobDelivered = GetBool();
            retData.SpecialEventsValues_Fined = GetBool();
            retData.SpecialEventsValues_Tollgate = GetBool();
            retData.SpecialEventsValues_Ferry = GetBool();
            retData.SpecialEventsValues_Train = GetBool();

            retData.SpecialEventsValues_Refuel = GetBool();
            retData.SpecialEventsValues_RefuelPayed = GetBool();

            NextOffsetArea();

            #endregion

            #region 13TH ZONE

            for (var i = 0; i < Substances; i++)
            {
                var tempSubstance = GetString();
                if (tempSubstance.Length != 0)
                {
                    retData.Substances.Add(new SCSTelemetry.Substance { Index = i, Value = tempSubstance });
                }
            }

            NextOffsetArea();

            #endregion

            #region 14TH ZONE

            // retData.TrailerValues = GetTrailers();

            #endregion


            return retData;
        }

        internal void SetDeliveryTime(SCSTelemetry retData, uint deliveryTime)
        {
            retData.DeliveryTime = deliveryTime;
            if (retData.CommonValuesGameTime > 0 && retData.CommonValuesGameTime < 4000000000 && deliveryTime > 0)
            {
                retData.RemainingDeliveryTime = (int)(deliveryTime - retData.CommonValuesGameTime);
            }
            else
            {
                retData.RemainingDeliveryTime = 0;
            }
        }

        internal void SetTruckPosition(SCSTelemetry retData, SCSTelemetry.DPlacement position)
        {
            retData.TruckValues_CurrentValues_PositionValue = position;
            retData.TruckValues_Positioning_TruckPosition = position;
        }

        private bool GetBool()
        {
            var temp = _data[_offset];
            _offset++;
            return temp > 0;
        }

        private uint GetUint()
        {
            while (_offset % 4 != 0)
            {
                _offset++;
            }

            var temp = (uint)((_data[_offset + 3] << 24) |
                               (_data[_offset + 2] << 16) |
                               (_data[_offset + 1] << 8) |
                               _data[_offset]);
            _offset += 4;
            return temp;
        }

        private float GetFloat()
        {
            while (_offset % 4 != 0)
            {
                _offset++;
            }

            var temp = new[] { _data[_offset], _data[_offset + 1], _data[_offset + 2], _data[_offset + 3] };
            _offset += 4;
            return BitConverter.ToSingle(temp, 0);
        }

        private double GetDouble()
        {
            while (_offset % 4 != 0)
            {
                _offset++;
            }

            var temp = new[] {
                                 _data[_offset], _data[_offset + 1], _data[_offset + 2], _data[_offset + 3],
                                 _data[_offset + 4], _data[_offset + 5], _data[_offset + 6], _data[_offset + 7]
                             };
            _offset += 8;
            return BitConverter.ToDouble(temp, 0);
        }

        private int GetInt()
        {
            while (_offset % 4 != 0)
            {
                _offset++;
            }

            var temp = (_data[_offset + 3] << 24) |
                       (_data[_offset + 2] << 16) |
                       (_data[_offset + 1] << 8) |
                       _data[_offset];
            _offset += 4;
            return temp;
        }

        private byte[] GetSubArray(int length)
        {
            var ret = new byte[length];
            for (var i = 0; i < length; i++)
            {
                ret[i] = _data[_offset + i];
            }

            _offset += length;
            return ret;
        }


        private void NextOffsetArea()
        {
            _offsetArea++;
            SetOffset();
        }

        private void SetOffset()
        {
            // Debug Fix?
            if (_offsetArea >= _offsetAreas.Length)
            {
                return;
            }

            _offset = _offsetAreas[_offsetArea];
        }

        private void SetOffset(int off) => _offset += off;

        private string GetString(int length = StringSize)
        {
            var area = GetSubArray(length);
            return Encoding.UTF8.GetString(area).Replace('\0', ' ').Trim();
        }

        private uint[] GetUintArray(int length)
        {
            var res = new uint[length];
            for (var i = 0; i < length; i++)
            {
                res[i] = GetUint();
            }

            return res;
        }

        private int[] GetIntArray(int length)
        {
            var res = new int[length];
            for (var i = 0; i < length; i++)
            {
                res[i] = GetInt();
            }

            return res;
        }

        private float[] GetFloatArray(int length)
        {
            var res = new float[length];
            for (var i = 0; i < length; i++)
            {
                res[i] = GetFloat();
            }

            return res;
        }

        private bool[] GetBoolArray(int length)
        {
            var res = new bool[length];
            for (var i = 0; i < length; i++)
            {
                res[i] = GetBool();
            }

            return res;
        }

        private SCSTelemetry.FVector GetFVector() => new SCSTelemetry.FVector
        { X = GetFloat(), Y = GetFloat(), Z = GetFloat() };

        private SCSTelemetry.FVector[] GetFVectorArray(int length)
        {
            var tempPos = new SCSTelemetry.FVector[length];
            for (var j = 0; j < length; j++)
            {
                tempPos[j] = new SCSTelemetry.FVector { X = GetFloat() };
            }

            for (var j = 0; j < length; j++)
            {
                tempPos[j].Y = GetFloat();
            }

            for (var j = 0; j < length; j++)
            {
                tempPos[j].Z = GetFloat();
            }

            return tempPos;
        }

        private SCSTelemetry.DVector GetDVector() => new SCSTelemetry.DVector
        { X = GetDouble(), Y = GetDouble(), Z = GetDouble() };

        private SCSTelemetry.Euler GetEuler() => new SCSTelemetry.Euler
        { Heading = GetFloat(), Pitch = GetFloat(), Roll = GetFloat() };

        private SCSTelemetry.Euler GetDEuler() =>
            new SCSTelemetry.Euler
            { Heading = (float)GetDouble(), Pitch = (float)GetDouble(), Roll = (float)GetDouble() };

        private SCSTelemetry.FPlacement GetFPlacement() => new SCSTelemetry.FPlacement
        { Position = GetFVector(), Orientation = GetEuler() };

        private SCSTelemetry.DPlacement GetDPlacement() => new SCSTelemetry.DPlacement
        { Position = GetDVector(), Orientation = GetDEuler() };

        private long GetLong()
        {
            var temp = new[] {
                                 _data[_offset], _data[_offset + 1], _data[_offset + 2], _data[_offset + 3],
                                 _data[_offset + 4], _data[_offset + 5], _data[_offset + 6], _data[_offset + 7]
                             };
            _offset += 8;
            return BitConverter.ToInt64(temp, 0);
        }

        private ulong GetULong()
        {
            var temp = new[] {
                                 _data[_offset], _data[_offset + 1], _data[_offset + 2], _data[_offset + 3],
                                 _data[_offset + 4], _data[_offset + 5], _data[_offset + 6], _data[_offset + 7]
                             };
            _offset += 8;
            return BitConverter.ToUInt64(temp, 0);
        }

        /*private SCSTelemetry.Trailer[] GetTrailers()
        {
            var trailer = new SCSTelemetry.Trailer[10];
            //TODO : only 1 for old game versions
            for (var i = 0; i < 10; i++)
            {
                trailer[i] = GetTrailer();
            }

            return trailer;
        }

        private SCSTelemetry.Trailer GetTrailer()
        {
            var trailer = new SCSTelemetry.Trailer();

            #region bool Region

            trailer.WheelsConstant.Steerable = GetBoolArray(WheelSize);
            trailer.WheelsConstant.Simulated = GetBoolArray(WheelSize);
            trailer.WheelsConstant.Powered = GetBoolArray(WheelSize);
            trailer.WheelsConstant.Liftable = GetBoolArray(WheelSize);
            trailer.Wheelvalues.OnGround = GetBoolArray(WheelSize);
            trailer.Attached = GetBool();
            SetOffset(3);

            #endregion First Zone 0 - 83

            #region uint Region

            trailer.Wheelvalues.Substance = GetUintArray(WheelSize);
            trailer.WheelsConstant.Count = GetUint();

            #endregion Second Zone 84 - 151

            #region float Region

            trailer.DamageValues.Cargo = GetFloat();
            trailer.DamageValues.Chassis = GetFloat();
            trailer.DamageValues.Wheels = GetFloat();
            trailer.Wheelvalues.SuspDeflection = GetFloatArray(WheelSize);
            trailer.Wheelvalues.Velocity = GetFloatArray(WheelSize);
            trailer.Wheelvalues.Steering = GetFloatArray(WheelSize);
            trailer.Wheelvalues.Rotation = GetFloatArray(WheelSize);
            trailer.Wheelvalues.Lift = GetFloatArray(WheelSize);
            trailer.Wheelvalues.LiftOffset = GetFloatArray(WheelSize);

            trailer.WheelsConstant.Radius = GetFloatArray(WheelSize);

            #endregion Third Zone 152 - 611

            #region floatvector Region

            trailer.AccelerationValues.LinearVelocity = GetFVector();
            trailer.AccelerationValues.AngularVelocity = GetFVector();
            trailer.AccelerationValues.LinearAcceleration = GetFVector();
            trailer.AccelerationValues.AngularAcceleration = GetFVector();

            trailer.Hook = GetFVector();

            trailer.WheelsConstant.PositionValues = GetFVectorArray(WheelSize);

            #endregion 4Th Zone 612 - 863

            #region double placement Region

            trailer.Position = GetDPlacement();

            #endregion 5Th 864 - 911

            #region string Region

            trailer.Id = GetString();
            trailer.CargoAccessoryId = GetString();
            trailer.BodyType = GetString();
            trailer.BrandId = GetString();
            trailer.Brand = GetString();
            trailer.Name = GetString();
            trailer.ChainType = GetString();
            trailer.LicensePlate = GetString();
            trailer.LicensePlateCountry = GetString();
            trailer.LicensePlateCountryId = GetString();

            #endregion 6th Zone 912 - 1551

            return trailer;
        }*/
    }
}
