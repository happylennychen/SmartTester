using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    public class Channel : IChannel
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public Tester Tester { get; set; }
        //public bool IsRunning { get; private set; }
        //public bool IsStarted { get; set; }
        //public Timer Timer { get; set; }
        private DataLogger dataLogger { get; set; }
        public StandardRow GetData()
        {
            DateTime now = DateTime.Now;
            string time = now.ToString("yyyy-MM-dd HH:mm:ss fff");
            string fakeData = $"{Tester.Name} {Name} Fake data at {time}\n";
            Console.Write($"{Tester.Name} {Name} get data:{fakeData}");
            //dataLogger.AddData(fakeData);
            return new StandardRow();
        }

        public void SetStep(Step step)
        {
            Console.WriteLine($"{Tester.Name} {Name} set step:\n{step.ToString()}");
        }

        public void Start()
        {
            //if (IsRunning)
            //{
            //    Console.WriteLine("Already running! Aborted.");
            //    return;
            //}
            //IsRunning = true;
            dataLogger = new DataLogger(1, GetFileName());
            Console.WriteLine($"{Tester.Name} {Name} start");
        }

        private string GetFileName()
        {
            return $"{Tester.Name}-{Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
        }

        public void Stop()
        {
            //if (!IsRunning)
            //{
            //    Console.WriteLine("Already stopped! Aborted.");
            //    return;
            //}
            //IsRunning = false;
            dataLogger.Close();
            Console.WriteLine($"{Tester.Name} {Name} stopped in Stop().");
        }

        public void LogData(string log)
        {
            dataLogger.AddData(log);
        }
    }
}
