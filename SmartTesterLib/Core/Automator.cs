﻿//#define debug
using System.Text.Json;

namespace SmartTesterLib
{
    public class Automator//提供Automator顶层数据结构及API（Init, AutoRun），不关心具体的硬件
    {
        public Automator()
        {
        }

        //private string Name { get; set; }
        #region AutoRun
#if false
        public async Task AutoRun(string projectName)//从目录结构中加载Test，控制温箱分组，控制温箱中各实验的同步工作。
        {
            ProjectName = projectName;
            if (!Directory.Exists(GlobalSettings.TestPlanFolderPath))
                Directory.CreateDirectory(GlobalSettings.TestPlanFolderPath);
            if (!Utilities.TestPlanFullCheck(projectName, Chambers, Testers))
            {
                Utilities.WriteLine($"Test Plan pre-check failed!");
                return;
            }

            Utilities.CreateOutputFolderRoot(projectName);

            List<Task> tasks = new List<Task>();
            foreach (var chamber in Chambers)        //Tests按Chamber分组
            {
                Task t = AsyncStartChamberGroup(projectName, chamber);
                tasks.Add(t);
            }
            Utilities.WriteLine($"Automator Start. Waiting.");
            await Task.WhenAll(tasks);
            Utilities.WriteLine("All test done!");
        }


        private async Task AsyncStartOneRound(IChamber chamber, List<Recipe> testsInOneRound)
        {
            Utilities.WriteLine($"Start Chamber Group for {chamber.Name}. Thread {CurrentThread.ManagedThreadId}, {CurrentThread.IsThreadPoolThread}");
            Utilities.CreateOutputFolder(chamber);
            List<KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>>> sections = GetTestSections(testsInOneRound);   //sections是每个温度点下的测试的集合
            var channels = testsInOneRound.Select(o => o.Channel).ToList();
            bool ret;
            foreach (var channel in channels)
            {
                //channel.Tester.Stop(channel.Index);     //先停止前面的实验。
                channel.Stop();
                channel.Chamber = chamber;              //指定使用的chamber
                //channel.DataLogger.Folder = outputFolder;
            }
            foreach (var ts in sections)
            {
                TargetTemperature targetT = ts.Key;
                ret = chamber.Executor.Start(targetT.Temperature);
                if (!ret)
                {
                    Utilities.WriteLine($"Start chamber failed! Please check chamber cable.");
                    return;
                }
                Utilities.WriteLine($"Start {targetT.Temperature} deg for {chamber.Name}");
                if (targetT.IsCritical)
                {
                    Utilities.WriteLine($"Wait for {targetT.Temperature} deg ready");
                    ret = await WaitForChamberReady(chamber, targetT.Temperature);
                    if (ret == false)
                    {
                        Utilities.WriteLine("Chamber control failed.");
                        return;
                    }
                }
                var dic = ts.Value; //dic.key
                //var channels = dic.Keys.ToList();
                foreach (var channel in channels)
                {
                    var steps = dic[channel];
                    channel.StepsForOneTempPoint = steps;
                    //channel.Tester.Start(channel.Index);
                    channel.Start();
                }

                Utilities.WriteLine($"Wait for all channels done. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
                if (!await WaitForAllChannelsDone(channels))
                {
                    var errChannels = channels.Where(ch => ch.Status != ChannelStatus.COMPLETED);
                    foreach (var ch in errChannels)
                    {
                        Utilities.WriteLine($"Something went wrong. CH{ch.Index} Status:{ch.Status}");
                    }
                    //return;
                }
                else
                {
                    //如果顺利完成，是否需要进行文件转换？还是等所有section都结束了再一起转换？
                }
            }
            Utilities.WriteLine($"Stop Chamber Group for {chamber.Name}. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
            ret = chamber.Executor.Stop();
            if (!ret)
            {
                Utilities.WriteLine($"Stop chamber failed! Please check chamber cable.");
                return;
            }
            //await Task.Delay(1000);
            Utilities.WriteLine($"Start file converting.");
            foreach (var test in testsInOneRound)
            {
                test.Channel.GenerateFile();
            }
        }

        private async Task AsyncStartChamberGroup(string projectName, IChamber chamber)
        {
            GlobalSettings.ChamberRoundIndex[chamber] = 1;
            while (true)
            {
                List<Recipe> tests;
                if (Utilities.LoadTestsForOneRound(projectName, Chambers, Testers, chamber, GlobalSettings.ChamberRoundIndex[chamber], out tests))
                {
                    if (Utilities.ChamberGroupTestCheck(tests))
                    {
                        Utilities.WriteLine($"Round {GlobalSettings.ChamberRoundIndex[chamber]}");
                        var folderPath = $@"{GlobalSettings.TestPlanFolderPath}R{GlobalSettings.ChamberRoundIndex[chamber]}";

                        Utilities.WriteLine($"Main function run in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                        await AsyncStartOneRound(chamber, tests);
                        Utilities.WriteLine($"Round {GlobalSettings.ChamberRoundIndex[chamber]} programs in {folderPath} completed!");
                        GlobalSettings.ChamberRoundIndex[chamber]++;
                    }
                }
                else
                {
                    Utilities.WriteLine($"All rounds finished.");
                    break;
                }
            }
        }

        private async Task<bool> WaitForAllChannelsDone(List<IChannel> channels)
        {
            while (!channels.All(ch => ch.Status != ChannelStatus.RUNNING))
            {
                //Utilities.WriteLine($"Not all channels stoped, wait for more 5 seconds. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
                await Task.Delay(10000);
            }
            if (channels.All(ch => ch.Status == ChannelStatus.COMPLETED))
                return true;
            else
                return false;
        }

        private List<KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>>> GetTestSections(List<Recipe> testsInOneChamber)
        {
            List<KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>>> output = new List<KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>>>();
            List<TargetTemperature> tts = GetTemperaturePoints(testsInOneChamber);
            if (tts.Count == 1)
            {
                TargetTemperature tt = new TargetTemperature();
                tt.Temperature = 25;
                tt.IsCritical = true;
                Dictionary<IChannel, List<Step>> dic = new Dictionary<IChannel, List<Step>>();
                foreach (var t in testsInOneChamber)
                {
                    dic.Add(t.Channel, t.Steps);
                }
                KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>> pair = new KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>>(tt, dic);
                output.Add(pair);
            }
            else
                foreach (var tt in tts)
                {
                    Dictionary<IChannel, List<Step>> dic = new Dictionary<IChannel, List<Step>>();
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
                            Utilities.WriteLine("Not ready.");
                        }
                        else
                        {
                            Utilities.WriteLine("Not ready.");
                        }
                        dic.Add(t.Channel, steps);
                    }
                    KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>> pair = new KeyValuePair<TargetTemperature, Dictionary<IChannel, List<Step>>>(tt, dic);
                    output.Add(pair);
                }
            return output;
        }

        private async Task<bool> WaitForChamberReady(IChamber chamber, double temperature)
        {
#if debug
            await Task.Delay(500);
            Utilities.WriteLine($"Chamber Ready!");
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
                    Utilities.WriteLine($"Cannot reach target temperature in {waitingTime} minutes!");
                    return false;
                }
                ret = chamber.Executor.ReadTemperature(out temp);
                if(!ret)
                {
                    Utilities.WriteLine($"Read Temperature failed! Please check chamber cable.");
                    return false;
                }
                Utilities.WriteLine($"Read Temperature:{temp} in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                //if (!chamber.Executor.ReadStatus(out chamberStatus))    //偶尔读出786？？？
                //return;
                await Task.Delay(1000);
                if (Math.Abs(temp - temperature) < 5)
                {
                    tempInRangeCounter++;
                    Utilities.WriteLine($"Temperature reach target. Counter: {tempInRangeCounter} in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                }
                else
                {
                    tempInRangeCounter = 0;
                    Utilities.WriteLine($"Temperature leave target. Counter: {tempInRangeCounter} in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                }
            }
            while (tempInRangeCounter < 30 /*|| chamberStatus != ChamberStatus.HOLD*/);    //chamber temperature tolerrance is 5?
            return true;
#endif
        }

        private List<TargetTemperature> GetTemperaturePoints(Recipe recipe)
        {
            var temps = recipe.Steps.Select(st => st.Temperature).ToList();
            List<TargetTemperature> uniqueTemps = new List<TargetTemperature>();    //去掉连续重复的温度点
            TargetTemperature lastTemp = null;
            foreach (var temp in temps)
            {
                if (uniqueTemps.Count == 0)
                {
                    uniqueTemps.Add(temp);
                    lastTemp = temp;
                }
                else
                {
                    if (temp.IsCritical != lastTemp.IsCritical || temp.Temperature == lastTemp.Temperature)
                    {
                        uniqueTemps.Add(temp);
                        lastTemp = temp;
                    }
                }
            }
            return uniqueTemps;
        }
#endif
        #endregion
        #region New Method
        public void PutChannelsInChamber(IEnumerable<IChannel> channels, IChamber chamber)
        {
            chamber.PairedChannels = channels.ToList();
            foreach (var ch in channels)
            {
                ch.ContainingChamber = chamber;
            }
        }
        public async Task AsyncStartChambers(IEnumerable<IChamber> chambers)
        {
            Utilities.CreateOutputFolderRoot();
            List<Task> tasks = new List<Task>();
            foreach (var chamber in chambers)        //Tests按Chamber分组
            {
                Task t = AsyncStartChamber(chamber);
                tasks.Add(t);
            }
            Utilities.WriteLine($"Automator Start. Waiting.");
            await Task.WhenAll(tasks);
            Utilities.WriteLine("All test done!");
        }
        public async Task AsyncStartChamber(IChamber chamber)  //执行单个Chamber中的多轮测试
        {
            while (!chamber.TestScheduler.IsCompleted)
            {
                //List<Recipe> tests;
                //if (Utilities.LoadTestsForOneRound(projectName, Chambers, Testers, chamber, GlobalSettings.ChamberRoundIndex[chamber], out tests))
                //{
                //    if (Utilities.ChamberGroupTestCheck(tests))
                //    {
                //        Utilities.WriteLine($"Round {GlobalSettings.ChamberRoundIndex[chamber]}");
                //        var folderPath = $@"{GlobalSettings.TestPlanFolderPath}R{GlobalSettings.ChamberRoundIndex[chamber]}";

                //        Utilities.WriteLine($"Main function run in thread {CurrentThread.ManagedThreadId}, pool:{CurrentThread.IsThreadPoolThread}");
                //        await AsyncStartOneRound(chamber, tests);
                //        Utilities.WriteLine($"Round {GlobalSettings.ChamberRoundIndex[chamber]} programs in {folderPath} completed!");
                //        GlobalSettings.ChamberRoundIndex[chamber]++;
                //    }
                //}
                var tr = chamber.TestScheduler.GetFirstWaitingTestRound();
                await AsyncStartOneRound(chamber, tr);

            }
            Utilities.WriteLine($"{chamber.Name} All rounds finished.");
        }
        public async Task AsyncStartOneRound(IChamber chamber, TestRound tr)     //Chamber的scheduler已经ready
        {
            foreach (var item in tr.ChannelRecipes)
            {
                var ch = item.Key;
                var rec = item.Value;
                Utilities.WriteLine($"{ch.Tester.Name} {ch.Name}: {rec.Name}");
                if (!AssignRecipeToChannel(ch, rec))
                {
                    Utilities.WriteLine($"{rec.Name} cannot be assigned to {ch.ContainingChamber.Name} because of temperature conflict.");
                }
            }
            tr.Status = RoundStatus.RUNNING;
            Utilities.WriteLine($"Start Chamber Group for {chamber.Name}.");
            var assignedChannels = chamber.PairedChannels.Where(ch => ch.Status == ChannelStatus.ASSIGNED).ToList();
            bool ret;
            foreach (var channel in assignedChannels)
            {
                channel.Stop();
            }
            while (!chamber.TempScheduler.IsCompleted)
            {
                ret = chamber.StartNextUnit();
                if (!ret)
                {
                    Utilities.WriteLine($"Start chamber failed! Please check.");
                    return;
                }
                var curTemp = chamber.TempScheduler.GetCurrentTemp();
                if (curTemp == null)
                {
                    Utilities.WriteLine($"Start chamber failed! Please check.");
                    return;
                }
                var target = curTemp.Target;
                Utilities.WriteLine($"Start {target.Value} deg for {chamber.Name}");
                if (target.IsCritical)
                {
                    Utilities.WriteLine($"Wait for {target.Value} deg ready");
                    ret = await WaitForChamberReady(chamber);
                    if (ret == false)
                    {
                        Utilities.WriteLine("Chamber control failed.");
                        return;
                    }
                }
                foreach (var channel in assignedChannels)
                {
                    //channel.StepsForOneTempPoint = steps;
                    channel.SetStepsForOneTempPoint();
                    channel.Start();
                }

                Utilities.WriteLine($"Wait for all channels done.");
                if (!await WaitForAllChannelsDone(assignedChannels))
                {
                    var errChannels = assignedChannels.Where(ch => ch.Status != ChannelStatus.COMPLETED);
                    foreach (var ch in errChannels)
                    {
                        Utilities.WriteLine($"Something went wrong. CH{ch.Index} Status:{ch.Status}");
                    }
                    //return;
                }
                else
                {
                    //如果顺利完成，是否需要进行文件转换？还是等所有section都结束了再一起转换？
                }
            }
            Utilities.WriteLine($"Stop Chamber Group for {chamber.Name}.");
            ret = chamber.Stop();
            if (!ret)
            {
                Utilities.WriteLine($"Stop chamber failed! Please check chamber cable.");
                return;
            }
            Utilities.WriteLine($"Start file converting.");
            foreach (var channel in assignedChannels)
            {
                channel.GenerateFile();
            }
            tr.Status = RoundStatus.COMPLETED;
        }

        private bool AssignRecipeToChannel(IChannel channel, SmartTesterRecipe recipe) //不仅把recipe指派给了channel，同时更新了温箱的温度列表,同时绑定了recipe.Steps中的TemperatureUnit到chamber.TempScheduler中的TemperatureUnit
        {
            //var ts = recipe.GetUniqueTemperaturePoints();
            if (channel.ContainingChamber.TempScheduler.IsTemperatureSchedulerCompatible(recipe.GetUniqueTemperaturePoints()))
            {
                channel.ContainingChamber.TempScheduler.UpdateTemperatureScheduler(ref recipe); //同时绑定了recipe.Steps中的TemperatureUnit到chamber.TempScheduler中的TemperatureUnit
                channel.Recipe = recipe;
                channel.Status = ChannelStatus.ASSIGNED;
                //recipe.Channel = channel;
                //if (channel.Chamber != null)
                //    recipe.Chamber = channel.Chamber;
                return true;
            }
            else
                return false;
        }
        private async Task<bool> WaitForChamberReady(IChamber chamber)
        {
            while (chamber.TempScheduler.GetCurrentTemp().Status != TemperatureStatus.REACHED)
            {
                await Task.Delay(1000);
                chamber.UpdateStatus();
            }
            return true;
        }
        private async Task<bool> WaitForAllChannelsDone(List<IChannel> channels)
        {
            while (!channels.All(ch => ch.Status != ChannelStatus.RUNNING))
            {
                //Utilities.WriteLine($"Not all channels stoped, wait for more 5 seconds. Thread {CurrentThread.ManagedThreadId},{CurrentThread.IsThreadPoolThread}");
                await Task.Delay(10000);
            }
            if (channels.All(ch => ch.Status == ChannelStatus.COMPLETED))
                return true;
            else
                return false;
        }
        #endregion
    }
}