using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    public class Tester : ITester
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public List<Channel> Channels { get; set; }
        private Timer mainTimer;
        private Timer[] Timers { get; set; }
        //private bool isMainTimerRunning;
        //private Timer[] timers;
        private DateTime[] startTimes;
        private int _counter = 0;
        private bool[] _shouldTimerStart { get; set; }
        private bool[] _isTimerStart { get; set; }
        private Stopwatch[] stopwatchs { get; set; }
        private Stopwatch mainWatch { get; set; }
        public Tester(string name, int channelNumber)
        {
            Name = name;
            Channels = new List<Channel>();
            startTimes = new DateTime[channelNumber];
            stopwatchs = new Stopwatch[channelNumber];
            Timers = new Timer[channelNumber];
            mainWatch = new Stopwatch();

            for (int i = 1; i <= channelNumber; i++)
            {
                Channel ch = new Channel();
                ch.Name = $"Ch{i}";
                ch.Index = i;
                Channels.Add(ch);
                ch.Tester = this;
                //ch.Timer = new Timer(_ => TimerOperation(i), null, Timeout.Infinite, 0);
                Timers[i - 1] = new Timer(WorkerCallback, i - 1, Timeout.Infinite, Timeout.Infinite);
                stopwatchs[i - 1] = new Stopwatch();
            }
            mainTimer = new Timer(_ => MainCounter(), null, 100, 0);
            _shouldTimerStart = new bool[channelNumber];
            _isTimerStart = new bool[channelNumber];
            mainWatch.Start();
        }

        private void WorkerCallback(object i)
        {
            int index = (int)i;
            var startPoint = stopwatchs[index].ElapsedMilliseconds % 1000;
            long data;
            do
            {
                data = stopwatchs[index].ElapsedMilliseconds % 1000;
                //Console.WriteLine($"Read data from channel {index + 1}. Start point is {startPoint}, data is {data}");
            }
            while (data > 100);

            var timer = Timers[index];
            var enable = _shouldTimerStart[index];
            if (enable) //开启下一次计时
            {
                timer.Change(950, 0);
            }
            string gap = string.Empty;
            for (int j = 0; j < index; j++)
            {
                gap += " ";
            }
            Console.WriteLine($"Read data from channel {gap}{index + 1}. Start point is {startPoint}, data is {data}, Time is {DateTime.Now.ToString("HH:mm:ss fff")}");
        }

        private void MainCounter()
        {
            var startPoint = mainWatch.ElapsedMilliseconds % 125;
            long data;
            do
            {
                data = mainWatch.ElapsedMilliseconds % 125;
            }
            while (data > 10);
            mainTimer.Change(100, 0);
            //Console.WriteLine($"Main Counter Start point is {startPoint}, data is {data}, next delay is {100}");

            var counter = _counter;
            var timer = Timers[counter];
            if (_shouldTimerStart[counter] && !_isTimerStart[counter])     //应该开启且还没开启
            {
                Console.WriteLine($"Start channel {counter + 1}");
                stopwatchs[counter].Start();
                timer.Change(980, 0);
                _isTimerStart[counter] = true;
            }
            _counter++;
            if (_counter == Channels.Count)
                _counter = 0;
        }

        public void SetStep(Step step, int index)
        {
            var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            channel.SetStep(step);
        }

        public void Start(int index)
        {
            //var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            //channel.Start();
            //channel.IsStarted = true;
            _shouldTimerStart[index - 1] = true;
        }

        private void TimerOperation(int index)
        {
            var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            var result = channel.GetData();
            if (!GetIsRunning(result, startTimes[index - 1]))
            {
                channel.Stop();
            }
        }

        private bool GetIsRunning(string readData, DateTime startTime)
        {
            var startIndex = readData.IndexOf("at ") + 3;
            var timeString = readData.Substring(startIndex, 19);
            var now = DateTime.Parse(timeString);
            if ((now - startTime) > TimeSpan.FromSeconds(10))
                return false;
            else
                return true;
        }

        public void Stop(int index)
        {
            //var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            //channel.Stop();
            _shouldTimerStart[index - 1] = false;

            Console.WriteLine($"Stop channel {index - 1 + 1}");
            stopwatchs[index - 1].Reset();
            Timers[index - 1].Change(Timeout.Infinite, Timeout.Infinite);
            _isTimerStart[index - 1] = false;
        }

        public string GetData(int index)
        {
            throw new NotImplementedException();
        }
    }
}
