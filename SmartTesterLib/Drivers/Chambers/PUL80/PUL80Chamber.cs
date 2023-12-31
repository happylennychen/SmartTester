﻿//using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmartTesterLib
{
    public class PUL80Chamber : IChamber
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double LowestTemperature { get; set; }
        public double HighestTemperature { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        //[JsonIgnore]
        public IChamberExecutor Executor { get; set; }
        public List<IChannel> PairedChannels { get; set; }
        public TestPlanScheduler TestScheduler { get; set; }
        public TemperatureScheduler TempScheduler { get; set; }
        private byte TempInRangeCounter { get; set; } = 0;

        public PUL80Chamber()
        { }
        public PUL80Chamber(int Id, string Manufacturer, string Name, int HighestTemperature, int LowestTemperature)
        {
            this.Id = Id;
            this.Manufacturer = Manufacturer;
            this.Name = Name;
            this.HighestTemperature = HighestTemperature;
            this.LowestTemperature = LowestTemperature;
        }
        //[JsonConstructor]
        public PUL80Chamber(int id, string manufacturer, string name, double highestTemperature, double lowestTemperature, string ipAddress, int port)
        {
            Id = id;
            Manufacturer = manufacturer;
            Name = name;
            HighestTemperature = highestTemperature;
            LowestTemperature = lowestTemperature;
            IpAddress = ipAddress;
            Port = port;
            Executor = new PUL80Executor();
            TestScheduler = new TestPlanScheduler(this);
            TempScheduler = new TemperatureScheduler();
            if (!Executor.Init(ipAddress, port))
            {
                Utilities.WriteLine("PUL-80 init failed!");
                return;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public bool StartNextUnit()
        {
            bool ret;
            var ctu = TempScheduler.GetCurrentTemp();
            if (ctu != null)
                ctu.Status = TemperatureStatus.PASSED;
            var tUnit = TempScheduler.GetNextTemp();
            if (tUnit == null)
            {
                Utilities.WriteLine($"There's no waiting temperature.");
                return false;
            }

            ret = Executor.Start(tUnit.Target.Value);
            if (!ret)
            {
                Utilities.WriteLine($"Start chamber failed! Please check chamber cable.");
                return ret;
            }
            tUnit.Status = TemperatureStatus.REACHING;

            return true;
        }

        public bool Stop()
        {
            bool ret;
            //var tUnit = TempScheduler.GetCurrentTemp();

            ret = Executor.Stop();
            if (!ret)
            {
                Utilities.WriteLine($"Stop chamber failed! Please check chamber cable.");
                return ret;
            }
            //tUnit.Status = TemperatureStatus.PASSED;
            return true;
        }
        public bool UpdateStatus()
        {
            double temp;
            bool ret = false;
            ret = Executor.ReadTemperature(out temp);
            if (!ret)
            {
                Utilities.WriteLine($"Read Temperature failed! Please check chamber cable.");
                return false;
            }
            var currentTemp = TempScheduler.GetCurrentTemp();
            if (Math.Abs(temp - currentTemp.Target.Value) < 5)
            {
                TempInRangeCounter++;
                Utilities.WriteLine($"Temperature reach target. Counter: {TempInRangeCounter}");
            }
            else
            {
                TempInRangeCounter = 0;
                Utilities.WriteLine($"Temperature leave target. Counter: {TempInRangeCounter}");
            }
            if (TempInRangeCounter < 30)
            {
                currentTemp.Status = TemperatureStatus.REACHING;
            }
            else
            {
                currentTemp.Status = TemperatureStatus.REACHED;
                //timer.Change(Timeout.Infinite, 1000);
            }
            return true;
        }

        public void Assamble()
        {
            throw new NotImplementedException();
        }
    }
}