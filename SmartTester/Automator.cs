using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTester
{
    public class Automator
    {
        public List<Chamber> Chambers { get; set; }
        public List<Tester> Testers { get; set; }
        public List<Test> Tests { get; set; }

        public async Task Start(List<Test> tests)
        {
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
            await Task.WhenAll(tasks);
            Console.WriteLine("All test done!");
        }

        private async Task AsyncStartChamberGroup(Chamber chamber, List<Test> testsInOneChamber)
        {
            Console.WriteLine($"Start Chamber Group for {chamber.Name}");
            List<KeyValuePair<TargetTemperature, Dictionary<Channel, List<Step>>>> sections = GetTestSections(testsInOneChamber);   //sections是每个温度点下的测试的集合
            var channels = testsInOneChamber.Select(o => o.Channel).ToList();
            foreach (var ts in sections)
            {
                TargetTemperature targetT = ts.Key;
                chamber.Executor.Start(targetT.Temperature);
                Console.WriteLine($"Start {targetT.Temperature} deg for {chamber.Name}");
                if (targetT.IsCritical)
                {
                    Console.WriteLine($"Wait for {targetT.Temperature} deg ready");
                    await WaitForChamberReady(chamber, targetT.Temperature);
                }
                var dic = ts.Value; //dic.key
                //var channels = dic.Keys.ToList();
                foreach (var channel in channels)
                {
                    var steps = dic[channel];
                    channel.FullSteps = steps;
                    channel.Tester.Start(channel.Index);
                }

                Console.WriteLine($"Wait for all channels done.");
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
            Console.WriteLine($"Stop Chamber Group for {chamber.Name}");
            chamber.Executor.Stop();
            foreach (var test in testsInOneChamber)
            {
                test.Channel.GenerateFile(test.Steps);
            }
        }

        private async Task<bool> WaitForAllChannelsDone(List<Channel> channels)
        {
            while (!channels.All(ch => ch.Status != ChannelStatus.RUNNING))
            {
                await Task.Delay(5000);
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

        private async Task WaitForChamberReady(Chamber chamber, double temperature)
        {
            byte tempInRangeCounter = 0;
            double temp;
            do
            {
                temp = chamber.Executor.ReadTemperature();
                //if (!chamber.Executor.ReadStatus(out chamberStatus))    //偶尔读出786？？？
                //return;
                await Task.Delay(10000);
                if (Math.Abs(temp - temperature) < 5)
                {
                    tempInRangeCounter++;
                    Console.WriteLine($"Temperature reach target. Counter: {tempInRangeCounter}");
                }
                else
                {
                    tempInRangeCounter = 0;
                    Console.WriteLine($"Temperature leave target. Counter: {tempInRangeCounter}");
                }
            }
            while (tempInRangeCounter < 3 /*|| chamberStatus != ChamberStatus.HOLD*/);    //chamber temperature tolerrance is 5?
        }

        private List<TargetTemperature> GetTemperaturePoints(List<Test> testsInOneChamber)  //现在暂时就两个点
        {
            return new List<TargetTemperature>() { new TargetTemperature() { Temperature = 25, IsCritical = true }, new TargetTemperature() { Temperature = (testsInOneChamber.First().DischargeTemperature), IsCritical = false } };
        }

        private bool ChamberGroupTestCheck(List<Test> tests)
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