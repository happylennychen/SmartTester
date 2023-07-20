using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTester
{

    public enum SBSCommand
    {
        ManufacturerAccess = 0x00,
        RemainingCapacityAlarm = 0x01,
        RemainingTimeAlarm = 0x02,
        BatteryMode = 0x03,
        AtRate = 0x04,
        AtRateTimeToFull = 0x05,
        AtRateTimeToEmpty = 0x06,
        AtRateOK = 0x07,
        Temperature = 0x08,
        Voltage = 0x09,
        Current = 0x0a,
        AverageCurrent = 0x0b,
        MaxError = 0x0c,
        RelativeStateOfCharge = 0x0d,
        AbsoluteStateOfCharge = 0x0e,
        RemainingCapacity = 0x0f,
        FullChargeCapacity = 0x10,
        RunTimeToEmpty = 0x11,
        AverageTimeToEmpty = 0x12,
        AverageTimeToFull = 0x13,
        ChargingCurrent = 0x14,
        ChargingVoltage = 0x15,
        BatteryStatus = 0x16,
        CycleCount = 0x17,
        //DesignCapacity
        //DesignVoltage
        //SpecificationInfo
        //ManufactureDate
        //SerialNumber
        Temperature2 = 0x44,
        Temperature3 = 0x45,
        Temperature4 = 0x46,
        CellVoltage1 = 0x60,
        CellVoltage2 = 0x61,
        CellVoltage3 = 0x62,
        CellVoltage4 = 0x63,
        CellVoltage5 = 0x64,
        CellVoltage6 = 0x65,
        CellVoltage7 = 0x66,
        CellBalanceStatus = 0x6a,
        PackModelID = 0x80,
        FirmwareVersion = 0x81,
        FirmwareChecksum = 0x82,
        NumberofCells = 0x83,
        PackStatus = 0x84,
        SafetyStatus = 0x85,
        PackVoltage = 0x8d,
        AveragePowerWithDesignVoltage = 0x90,
        AveragePowerWithReaVoltage = 0x91,

    }
}
