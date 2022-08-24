//#define debug
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace SmartTester
{
    public class Automator
    {
        public List<Chamber> Chambers { get; set; }
        public List<Tester> Testers { get; set; }
        public List<Test> Tests { get; set; }

        public async Task Start(List<Test> tests)
        {
            Console.WriteLine($"Automator Start. Begin in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
            var testsGroupedbyChamber = tests.GroupBy(t => t.Chamber);
            List<Task> tasks = new List<Task>();
            foreach (var testGroup in testsGroupedbyChamber)        //Tests按Chamber分组
            {
                var chamber = testGroup.Key;
                var testsInOneChamber = testGroup.ToList();
                if (!ChamberGroupTestCheck(testsInOneChamber))
                {
                    return;
                }
                Task t = AsyncStartChamberGroup(chamber, testsInOneChamber);
                tasks.Add(t);
            }
            Console.WriteLine($"Automator Start. Waiting. Thread {CurrentThread.ManagedThreadId}, {CurrentThread.IsThreadPoolThread}");
            await Task.WhenAll(tasks);
            Console.WriteLine("All test done!");
        }

        private async Task AsyncStartChamberGroup(Chamber chamber, List<Test> testsInOneChamber)
        {
            Console.WriteLine($"Start Chamber Group for {chamber.Name}. Thread {CurrentThread.ManagedThreadId}, {CurrentThread.IsThreadPoolThread}");
            Utilities.CreateOutputFolder();
            List<KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>> sections = GetTestSections(testsInOneChamber);   //sections是每个温度点下的测试的集合
            var channels = testsInOneChamber.Select(o => o.Channel).ToList();
            bool ret;
            foreach (var channel in channels)
            {
                channel.Tester.Stop(channel.Index);     //先停止前面的实验。
                //channel.DataLogger.Folder = outputFolder;
            }
            foreach (var ts in sections)
            {
                TargetTemperature targetT = ts.Key;
                ret = chamber.Executor.Start(targetT.Temperature);
                if (!ret)
                {
                    Console.WriteLine($"Start chamber failed! Please check chamber cable.");
                    return;
                }
                Console.WriteLine($"Start {targetT.Temperature} deg for {chamber.Name}");
                if (targetT.IsCritical)
                {
                    Console.WriteLine($"Wait for {targetT.Temperature} deg ready");
                    ret = await WaitForChamberReady(chamber, targetT.Temperature);
                    if (ret == false)
                    {
                        Console.WriteLine("Chamber control failed.");
                        return;
                    }
                }
                var dic = ts.Value; //dic.key
                //var channels = dic.Keys.ToList();
                foreach (var channel in channels)
                {
                    var steps = dic[channel];
                    channel.FullSteps = steps;
                    channel.Tester.Start(channel.Index);
                }

                Console.WriteLine($"Wait for all channels done. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
                if (!await WaitForAllChannelsDone(channels))
                {
                    var errChannels = channels.Where(ch => ch.Status != ChannelStatus.IDLE);
                    foreach (var ch in errChannels)
                    {
                        Console.WriteLine($"Something went wrong. CH{ch.Index} Status:{ch.Status}");
                    }
                    //return;
                }
                else
                {
                    //如果顺利完成，是否需要进行文件转换？还是等所有section都结束了再一起转换？
                }
            }
            Console.WriteLine($"Stop Chamber Group for {chamber.Name}. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
            ret = chamber.Executor.Stop();
            if (!ret)
            {
                Console.WriteLine($"Stop chamber failed! Please check chamber cable.");
                return;
            }
            //await Task.Delay(1000);
            Console.WriteLine($"Start file converting.");
            foreach (var test in testsInOneChamber)
            {
                test.Channel.GenerateFile(test.Steps);
            }
        }

        private async Task<bool> WaitForAllChannelsDone(List<Channel> channels)
        {
            while (!channels.All(ch => ch.Status != ChannelStatus.RUNNING))
            {
                //Console.WriteLine($"Not all channels stoped, wait for more 5 seconds. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
                await Task.Delay(10000);
            }
            if (channels.All(ch => ch.Status == ChannelStatus.IDLE))
                return true;
            else
                return false;
        }

        private List<KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>> GetTestSections(List<Test> testsInOneChamber)
        {
            List<KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>> output = new List<KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>>();
            List<TargetTemperature> tts = GetTemperaturePoints(testsInOneChamber);
            if (tts.Count == 1)
            {
                TargetTemperature tt = new TargetTemperature();
                tt.Temperature = 25;
                tt.IsCritical = true;
                Dictionary<Channel, List<Step>> dic = new Dictionary<Channel, List<Step>>();
                foreach (var t in testsInOneChamber)
                {
                    dic.Add(t.Channel, t.Steps);
                }
                KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>> pair = new KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>(tt, dic);
                output.Add(pair);
            }
            else
                foreach (var tt in tts)
                {
                    Dictionary<Channel, List<Step>> dic = new Dictionary<Channel, List<Step>>();
                    foreach (var t in testsInOneChamber)
                    {
                        List<Step> steps = null;
                        if (output.Count == 0)  //添加第一个温度点对应的工步
                        {
                            var firstStep = t.Steps.First();
                            //if (firstStep.Action.Mode == ActionMode.CC_CV_CHARGE)
                            steps = new List<Step>() { firstStep };
                        }
                        else if (output.Count == 1) //添加第二个温度点对应的工步
                        {
                            //var firstStep = t.Steps.First();
                            //if (firstStep.Action.Mode == ActionMode.CC_CV_CHARGE)
                            //{
                            steps = new List<Step>(t.Steps.Where(s => s.Index != 1).OrderBy(s => s.Index));
                            //}
                        }
                        else if (output.Count == 2)//添加第三个温度点对应的工步
                        {
                            Console.WriteLine("Not ready.");
                        }
                        else
                        {
                            Console.WriteLine("Not ready.");
                        }
                        dic.Add(t.Channel, steps);
                    }
                    KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>> pair = new KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>(tt, dic);
                    output.Add(pair);
                }
            return output;
        }

        private async Task<bool> WaitForChamberReady(Chamber chamber, double temperature)
        {
#if debug
            await Task.Delay(500); 
            Console.WriteLine($"Chamber Ready!");
            return true;
#else
            byte tempInRangeCounter = 0;
            double temp;
            bool ret;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int waitingTime = 15;
            do
            {
                if(sw.Elapsed.TotalMinutes > waitingTime)
                {
                    Console.WriteLine($"Cannot reach target temperature in {waitingTime} minutes!");
                    return false;
                }
                ret = chamber.Executor.ReadTemperature(out temp);
                if(!ret)
                {
                    Console.WriteLine($"Read Temperature failed! Please check chamber cable.");
                    return false;
                }
                Console.WriteLine($"Read Temperature:{temp} in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                //if (!chamber.Executor.ReadStatus(out chamberStatus))    //偶尔读出786？？？
                //return;
                await Task.Delay(1000);
                if (Math.Abs(temp - temperature) < 5)
                {
                    tempInRangeCounter++;
                    Console.WriteLine($"Temperature reach target. Counter: {tempInRangeCounter} in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                }
                else
                {
                    tempInRangeCounter = 0;
                    Console.WriteLine($"Temperature leave target. Counter: {tempInRangeCounter} in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                }
            }
            while (tempInRangeCounter < 30 /*|| chamberStatus != ChamberStatus.HOLD*/);    //chamber temperature tolerrance is 5?
            return true;
#endif
        }

        private List<TargetTemperature> GetTemperaturePoints(List<Test> testsInOneChamber)  //现在暂时就两个点
        {
            var targetTemperature = testsInOneChamber.First().DischargeTemperature;
            if (targetTemperature != 25)
                return new List<TargetTemperature>() { new TargetTemperature() { Temperature = 25, IsCritical = true }, new TargetTemperature() { Temperature = (targetTemperature), IsCritical = false } };
            else
                return new List<TargetTemperature>() { new TargetTemperature() { Temperature = 25, IsCritical = true } };
        }

        public static bool ChamberGroupTestCheck(List<Test> tests)
        {
            if (tests.GroupBy(t => t.DischargeTemperature).Count() != 1)
            {
                Console.WriteLine("Error. No unified discharge temperature.");
                return false;
            }
            if (tests.GroupBy(t => t.Channel).Where(g => g.Count() > 1).Count() > 0)
            {
                Console.WriteLine("Error. Multiple tests used same channel(s).");
                return false;
            }
            return true;
        }
    }
}