//#define UseFileInsteadOfConsole
#define debug
using SmartTesterLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace SmartTester
{
    class Program//调用automator提供的API，提供基本的操作界面。后期可以用图形界面替代。
    {
        [STAThread]
        static void Main(string[] args)
        {
#if UseFileInsteadOfConsole
            Utilities.CreateConsoleFolder();
            string consoleOuputFile = Path.Combine(GlobalSettings.ConsoleFolderPath, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
            FileStream fs = new FileStream(consoleOuputFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            var tempOut = Console.Out;
            Console.SetOut(sw);
#endif

            Automator amtr = new Automator();
            //TestPlanScheduler scheduler = new TestPlanScheduler();
            string configurationFilePath = @"D:\Lenny\Tasks\O2Micro\Smart Tester\Original Code\SmartTester\SmartTester\Configuration.json";
            if(!amtr.InitHW(configurationFilePath))
                return;
#if debug
            amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch=>ch.Index<=4), amtr.Chambers[0]);
            amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch=>ch.Index>4), amtr.Chambers[1]);
            #region Chamber1
            string chamber1folder = @"D:\Lenny\Tasks\O2Micro\Smart Tester\Original Code\SmartTester\SmartTester\Test Plan\2Chambers1Tester_2\PUL-80\";
            var  chm1_r1_ch3= Utilities.LoadRecipeFromFile(@$"{chamber1folder}R1\17208Auto\CH3\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch1 = Utilities.LoadRecipeFromFile(@$"{chamber1folder}R2\17208Auto\CH1\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch2 = Utilities.LoadRecipeFromFile(@$"{chamber1folder}R2\17208Auto\CH2\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch3 = Utilities.LoadRecipeFromFile(@$"{chamber1folder}R2\17208Auto\CH3\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch4 = Utilities.LoadRecipeFromFile(@$"{chamber1folder}R2\17208Auto\CH4\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            Dictionary<IChannel, SmartTesterRecipe> channelRecipesForR1 = new Dictionary<IChannel, SmartTesterRecipe>();
            channelRecipesForR1.Add(amtr.Testers[0].Channels[0], chm1_r1_ch3);

            TestRound round1 = new TestRound(channelRecipesForR1);
            amtr.Chambers[0].TestScheduler.AppendTestRound(round1);

            Dictionary<IChannel, SmartTesterRecipe> channelRecipesForR2 = new Dictionary<IChannel, SmartTesterRecipe>();
            channelRecipesForR2.Add(amtr.Testers[0].Channels[0], chm1_r2_ch1);
            
            TestRound round2 = new TestRound(channelRecipesForR2);
            amtr.Chambers[0].TestScheduler.AppendTestRound(round2);
            #endregion

            #region Chamber2
            string chamber2folder = @"D:\Lenny\Tasks\O2Micro\Smart Tester\Original Code\SmartTester\SmartTester\Test Plan\2Chambers1Tester_2\PUL-82\";
            var chm2_r1_ch5 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r1_ch6 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r1_ch7 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r1_ch8 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            var chm2_r2_ch5 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r2_ch6 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r2_ch7 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r2_ch8 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            var chm2_r3_ch5 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r3_ch6 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r3_ch7 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r3_ch8 = Utilities.LoadRecipeFromFile(@$"{chamber2folder}R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            var r1 = new Dictionary<IChannel, SmartTesterRecipe>();
            r1.Add(amtr.Testers[0].Channels[4], chm2_r1_ch5);
            r1.Add(amtr.Testers[0].Channels[5], chm2_r1_ch6);
            r1.Add(amtr.Testers[0].Channels[6], chm2_r1_ch7);
            r1.Add(amtr.Testers[0].Channels[7], chm2_r1_ch8);
            var R1 = new TestRound(r1);
            amtr.Chambers[1].TestScheduler.AppendTestRound(R1);

            var r2 = new Dictionary<IChannel, SmartTesterRecipe>();
            r2.Add(amtr.Testers[0].Channels[4], chm2_r2_ch5);
            r2.Add(amtr.Testers[0].Channels[5], chm2_r2_ch6);
            r2.Add(amtr.Testers[0].Channels[6], chm2_r2_ch7);
            r2.Add(amtr.Testers[0].Channels[7], chm2_r2_ch8);
            var R2 = new TestRound(r2);
            amtr.Chambers[1].TestScheduler.AppendTestRound(R2);

            var r3 = new Dictionary<IChannel, SmartTesterRecipe>();
            r3.Add(amtr.Testers[0].Channels[4], chm2_r3_ch5);
            r3.Add(amtr.Testers[0].Channels[5], chm2_r3_ch6);
            r3.Add(amtr.Testers[0].Channels[6], chm2_r3_ch7);
            r3.Add(amtr.Testers[0].Channels[7], chm2_r3_ch8);
            var R3 = new TestRound(r3);
            amtr.Chambers[1].TestScheduler.AppendTestRound(R3);//*/
            #endregion
#else
            amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch => ch.Index == 1), amtr.Chambers[0]);
            var chm1_r1_ch3 = Utilities.LoadRecipeFromFile(@"E:\Smart Tester Test\test\25Deg-Charge Test.testplan");
            Dictionary<IChannel, SmartTesterRecipe> channelRecipesForR1 = new Dictionary<IChannel, SmartTesterRecipe>();
            channelRecipesForR1.Add(amtr.Testers[0].Channels[0], chm1_r1_ch3);

            TestRound round1 = new TestRound(channelRecipesForR1);
            amtr.Chambers[0].TestScheduler.AppendTestRound(round1);
#endif

            Task task = amtr.AsyncStartChambers();
            Monitor monitor = new Monitor(amtr);
            monitor.Run();
            task.Wait();
#if UseFileInsteadOfConsole
            sw.Close();
            fs.Close();
            Console.SetOut(tempOut);
            Utilities.WriteLine($"Demo program completed! Please check {consoleOuputFile} for the details.");
#endif
            Utilities.WriteLine($"Demo program completed!");
            Console.ReadLine();
        }
    }
}
