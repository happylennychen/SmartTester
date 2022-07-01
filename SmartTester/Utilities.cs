using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTester
{
    public static class Utilities
    {
        public static void FileConvert(string filePath)
        {
            uint indexOffset = 0;
            uint timeOffset = 0;
            double lastCapacity = 0;
            double capacityOffset = 0;
            double totalCapacityOffset = 0;
            bool isNewStep = false;
            ActionMode lastMode = ActionMode.REST;
            using (FileStream rawFile = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader rawReader = new StreamReader(rawFile))
                {
                    using (FileStream stdFile = new FileStream(Path.ChangeExtension(filePath, "csv"), FileMode.Create))
                    {
                        using (StreamWriter stdWriter = new StreamWriter(stdFile))
                        {
                            stdWriter.WriteLine("Index,Time(mS),Mode,Current(mA),Voltage(mV),Temperature(degC),Capacity(mAh),Total Capacity(mAh),Status");
                            while (rawReader.Peek() != -1)
                            {
                                var line = rawReader.ReadLine();
                                StandardRow stdRow = new StandardRow(line);
                                if (isNewStep)
                                {
                                    if (lastMode == stdRow.Mode)
                                        capacityOffset = lastCapacity;
                                }
                                stdRow.Index = ++indexOffset;
                                stdRow.TimeInMS += timeOffset;
                                stdRow.Capacity += capacityOffset;
                                stdRow.TotalCapacity = stdRow.Capacity + totalCapacityOffset;
                                stdWriter.WriteLine(stdRow.ToString());
                                if (stdRow.Status == RowStatus.STOP)
                                {
                                    timeOffset = stdRow.TimeInMS;
                                    lastMode = stdRow.Mode;
                                    lastCapacity = stdRow.Capacity;
                                    totalCapacityOffset = stdRow.TotalCapacity;
                                    isNewStep = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void FileConvert(List<string> filePaths, string newFilePath)
        {
            uint indexOffset = 0;
            uint timeOffset = 0;
            double lastCapacity = 0;
            double capacityOffset = 0;
            double totalCapacityOffset = 0;
            bool isNewStep = false;
            ActionMode lastMode = ActionMode.REST;
            using (FileStream stdFile = new FileStream(newFilePath, FileMode.Create))
            {
                using (StreamWriter stdWriter = new StreamWriter(stdFile))
                {
                    foreach (var filePath in filePaths)
                    {
                        using (FileStream rawFile = new FileStream(filePath, FileMode.Open))
                        {
                            using (StreamReader rawReader = new StreamReader(rawFile))
                            {
                                stdWriter.WriteLine("Index,Time(mS),Mode,Current(mA),Voltage(mV),Temperature(degC),Capacity(mAh),Total Capacity(mAh),Status");
                                while (rawReader.Peek() != -1)
                                {
                                    var line = rawReader.ReadLine();
                                    StandardRow stdRow = new StandardRow(line);
                                    if (isNewStep)
                                    {
                                        if (lastMode == stdRow.Mode)
                                            capacityOffset = lastCapacity;
                                    }
                                    stdRow.Index = ++indexOffset;
                                    stdRow.TimeInMS += timeOffset;
                                    stdRow.Capacity += capacityOffset;
                                    stdRow.TotalCapacity = stdRow.Capacity + totalCapacityOffset;
                                    stdWriter.WriteLine(stdRow.ToString());
                                    if (stdRow.Status == RowStatus.STOP)
                                    {
                                        timeOffset = stdRow.TimeInMS;
                                        lastMode = stdRow.Mode;
                                        lastCapacity = stdRow.Capacity;
                                        totalCapacityOffset = stdRow.TotalCapacity;
                                        isNewStep = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
