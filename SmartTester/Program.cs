using SmartTesterLib;
using Spectre.Console;

namespace SmartTester
{
    class Program//调用automator提供的API，提供基本的操作界面。后期可以用图形界面替代。
    {
        const string CREATE_STR = "[yellow]Create[/]";
        [STAThread]
        static void Main(string[] args)
        {
            string testPlanFolder = @"D:\Lenny\Tasks\O2Micro\Smart Tester\SmartTester\SmartTester\Test Plan";
            List<string> recipeFiles = Directory.EnumerateFiles(testPlanFolder, "*.testplan").ToList();
            //Utilities.CreateOutputFolderRoot();
            string configurationPath = @"D:\Lenny\Tasks\O2Micro\Smart Tester\SmartTester\SmartTester\Configuration.json";

            Automator amtr = new Automator();
            if (amtr == null)
                return;
            List<IChamber> chambers;
            Utilities.LoadChambersFromFile(configurationPath, out chambers);
            List<ITester> testers;
            Utilities.LoadTestersFromFile(configurationPath, out testers);
            SpectreMonitor monitor = new SpectreMonitor(chambers);
            monitor.Run();
            bool bQuit = false;
            while (!bQuit)
            {
                var cmd = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title($"\n[green]Welcome to SmartTester![/]" +
                $"\n[green]Test Plan Folder: {testPlanFolder}[/]" +
                $"\n[green]Configuration Folder: {configurationPath}[/]" +
                $"\n[green]Output Folder: {GlobalSettings.OutputFolder}[/]")
                .AddChoices(new[] {
                    "Setup Chambers",
                    "Setup Test Rounds",
                    "Run Tests",
                    "Quit"
                }
                    )
                );
                switch (cmd)
                {
                    case "Setup Chambers":
                        SetupChambers(chambers, testers, amtr);
                        break;
                    case "Setup Test Rounds":
                        SetupTestRounds(chambers, recipeFiles);
                        break;
                    case "Run Tests":
                        Task task = amtr.AsyncStartChambers(chambers);
                        break;
                    case "Quit": bQuit = true; break;
                    default: break;
                }
            }
            AnsiConsole.WriteLine($"Demo program completed!");
            Console.ReadLine();
        }

        private static void SetupTestRounds(List<IChamber> chambers, List<string> recipeFiles)
        {
            var selectedChamber = SpecifyChamber(chambers);
            //if (selectedChamber.TestScheduler.TestRoundList.Count == 0)
            //{
            //    if (!AnsiConsole.Confirm("There's no test round yet, create a new one?"))
            //        return;
            //    var selectedChannel = SpecifyChannel(selectedChamber.Channels);
            //    var selectedRecipe = SpecifyRecipe(recipeFiles);
            //    selectedChamber.TestScheduler.AppendTestRound(new TestRound(new Dictionary<IChannel, SmartTesterRecipe> { { selectedChannel, selectedRecipe } }));
            //}
            //else
            //{
            //    TestRound tr = SpecifyTestRound(selectedChamber.TestScheduler.TestRoundList.Where(tr => tr.Status == RoundStatus.WAITING));
            //    var selectedChannel = SpecifyChannel(selectedChamber.Channels);
            //    var selectedRecipe = SpecifyRecipe(recipeFiles);
            //    tr.ChannelRecipes.Add(selectedChannel, selectedRecipe);
            //}
            var trl = selectedChamber.TestScheduler.TestRoundList;
            var selectedTRStr = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title($"Select an existed test round, or create a new one:")
                .AddChoices(trl.Select(i => $"Round {trl.IndexOf(i)}")
                .Append(CREATE_STR))
                );
            if (selectedTRStr == CREATE_STR)
            {
                CreateTestRound(selectedChamber, recipeFiles);
            }
            else
            {
                var tr = trl.Single(i => $"Round {trl.IndexOf(i)}" == selectedTRStr);
                EditTestRound(tr, selectedChamber, recipeFiles);
            }
        }

        private static void EditTestRound(TestRound tr, IChamber selectedChamber, List<string> recipeFiles)
        {
            bool bQuit = false;
            while (!bQuit)
            {
                var cmd = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("")
                .AddChoices(new[] {
                    "Create Channel-Recipe pair",
                    "Quit"
                }
                    )
                );
                switch (cmd)
                {
                    case "Create Channel-Recipe pair":
                        var selectedChannel = SpecifyChannel(selectedChamber.PairedChannels);
                        var selectedRecipe = SpecifyRecipe(recipeFiles);
                        tr.AppendChannelRecipePair(selectedChannel, selectedRecipe);
                        break;
                    case "Quit": bQuit = true; break;
                    default: break;
                }
            }
            if (tr.ChannelRecipes.Count > 0)
                selectedChamber.TestScheduler.AppendTestRound(tr);
        }

        private static void CreateTestRound(IChamber selectedChamber, IEnumerable<string> recipeFiles)
        {
            TestRound tr = new TestRound(selectedChamber.PairedChannels);
            bool bQuit = false;
            while (!bQuit)
            {
                var cmd = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("")
                .AddChoices(new[] {
                    "Create Channel-Recipe pair",
                    "Quit"
                }
                    )
                );
                switch (cmd)
                {
                    case "Create Channel-Recipe pair":
                        var selectedChannel = SpecifyChannel(selectedChamber.PairedChannels);
                        var selectedRecipe = SpecifyRecipe(recipeFiles);
                        tr.AppendChannelRecipePair(selectedChannel, selectedRecipe);
                        break;
                    case "Quit": bQuit = true; break;
                    default: break;
                }
            }
            if (tr.ChannelRecipes.Count > 0)
                selectedChamber.TestScheduler.AppendTestRound(tr);
        }

        private static TestRound SpecifyTestRound(IEnumerable<TestRound> testRounds)
        {
            return SpecifyItem<TestRound>(testRounds, "Test Round");
        }

        private static SmartTesterRecipe SpecifyRecipe(IEnumerable<string> recipeFiles)
        {
            var recipeFileShort = SpecifyItem<string>(recipeFiles.Select(rf => Path.GetFileName(rf)), "RecipeFile");
            var recipeFile = recipeFiles.Single(rf => Path.GetFileName(rf) == recipeFileShort);
            return Utilities.LoadRecipeFromFile(recipeFile);
        }

        private static IChannel SpecifyChannel(List<IChannel> channels)
        {
            return SpecifyItem<IChannel>(channels, "Channel");
        }

        private static void SetupChambers(List<IChamber> chambers, List<ITester> testers, Automator amtr)
        {
            var selectedChamber = SpecifyChamber(chambers);

            var channelsStr = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                .Title("Which channels are in the selected chamber?")
                .PageSize(10)
                .NotRequired()
                .MoreChoicesText("[grey](Move up and down to reveal more channels)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a channel, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(testers.SelectMany(t => t.Channels.Select(c => c.Tester.Name + " " + c.Name)))
                );

            List<IChannel> selectedChannels = testers.SelectMany(t => t.Channels.Where(c => channelsStr.Contains(c.Tester.Name + " " + c.Name))).ToList();
            amtr.PutChannelsInChamber(selectedChannels, selectedChamber);
        }

        private static T SpecifyItem<T>(IEnumerable<T> items, string itemName)
        {
            var selectedItemStr = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title($"Select {itemName}:")
                .AddChoices(items.Select(i => i.ToString()))
                );
            var selectedItem = items.Single(i => i.ToString() == selectedItemStr);
            return selectedItem;
        }

        private static IChamber SpecifyChamber(List<IChamber> chambers)
        {
            return SpecifyItem<IChamber>(chambers, "Chamber");
        }
    }
}
