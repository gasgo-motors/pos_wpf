using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public partial class VehicleModel
    {
        public string DriveTypeString
        {
            get
            {
                switch (DriveType.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "Rear Wheel Drive";
                    case 2:
                        return "Front Wheel Drive";
                    case 3:
                        return "All Wheel Drive";
                    case 4:
                        return "10x4/6";
                    case 5:
                        return "10x8/6";
                    case 6:
                        return "2x2";
                    case 7:
                        return "4x2";
                    case 8:
                        return "4x4";
                    case 9:
                        return "6x2";
                    case 10:
                        return "6x2/2/4";
                    case 11:
                        return "6x2/4";
                    case 12:
                        return "6x4";
                    case 13:
                        return "6x6";
                    case 14:
                        return "8x2/4";
                    case 15:
                        return "8x2/6";
                    case 16:
                        return "8x4";
                    case 17:
                        return "8x4/4";
                    case 18:
                        return "8x6/4";
                    case 19:
                        return "8x8";
                    case 20:
                        return "8x8/4";
                    default:
                        return "";
                }
            }
        }

        public string EngineTypeString
        {
            get
            {
                switch (EngineType.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "Diesel";
                    case 2:
                        return "Petrol";
                    case 3:
                        return "Gas";
                    case 4:
                        return "Petrol/Gas";
                    case 5:
                        return "Electric";
                    case 6:
                        return "Hybrid (Petrol/Electric)";
                    case 7:
                        return "Hybrid (Diesell/Electric)";
                    case 8:
                        return "Hydrogen";
                    case 9:
                        return "Ethanol";
                    case 10:
                        return "LPG";
                    case 11:
                        return "Petrol/Natural Gas (CNG)";
                    default:
                        return "";
                }
            }
        }

        public string TransmissionString
        {
            get
            {
                switch (Transmission.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "Automatic transmission";
                    case 2:
                        return "Steptronic";
                    case 3:
                        return "Manual gearbox 4-Speed";
                    case 4:
                        return "Manual gearbox 5-Speed";
                    case 5:
                        return "Manual gearbox 6-Speed";
                    case 6:
                        return "Manual gearbox 7-Speed";
                    case 7:
                        return "Automatic Transmission 4-speed";
                    case 8:
                        return "Automatic Transmission 5-speed";
                    case 9:
                        return "Automatic Transmission 6-speed";
                    case 10:
                        return "Fully Automatic";
                    case 11:
                        return "Manual Transmission";
                    default:
                        return "";
                }
            }
        }

        public string FuelMixtureFormationString
        {
            get
            {
                switch (FuelMixtureFormation.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "Direct Injection";
                    case 2:
                        return "Intake Manifold Injection/Carburettor";
                    case 3:
                        return "Fuel Mixture Formation";
                    case 4:
                        return "Manifold Injection/Direct Injection";
                    case 5:
                        return "Prechamber Engine";
                    default:
                        return "";
                }
            }
        }

        public string BrakeSystemString
        {
            get
            {
                switch (BrakeSystem.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "-ABS";
                    case 2:
                        return "+ABS";
                    case 3:
                        return "Hydraulic-Mechanical";
                    case 4:
                        return "Hydraulic";
                    case 5:
                        return "Mechanical";
                    default:
                        return "";
                }
            }
        }

        public string BrakeTypeString
        {
            get
            {
                switch (BrakeType.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "Disc/Drum";
                    case 2:
                        return "Disc/Disc";
                    default:
                        return "";
                }
            }
        }

        public string CatalyticConverterTypeString
        {
            get
            {
                switch (CatalyticConverterType.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "Without Catalytic Convertor";
                    case 2:
                        return "With Catalytic Convertor";
                    case 3:
                        return "With 3 Way Catalyst";
                    case 4:
                        return "With Diesel Catalyst Oxi-Cat";
                    case 5:
                        return "With 2 Way Catalyst";
                    default:
                        return "";
                }
            }
        }

        public string AirConditionString
        {
            get
            {
                switch (AirCondition.Value)
                {
                    case 0:
                        return " ";
                    case 1:
                        return "With Air Condition";
                    case 2:
                        return "Without Air Condition";
                    default:
                        return "";
                }
            }
        }

    }
}
