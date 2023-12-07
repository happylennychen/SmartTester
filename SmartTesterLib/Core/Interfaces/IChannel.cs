namespace SmartTesterLib
{
    public interface IChannel
    {
        int Id { get; set; }
        int Index { get; set; }
        string Name { get; set; }
        SmartTesterRecipe Recipe { get; set; }
        SmartTesterStep CurrentStep { get; set; }
        //List<Step> StepsForOneTempPoint { get; set; }
        ChannelStatus Status { get; set; }
        ITester Tester { get; set; }
        IChamber? ContainingChamber { get; set; }
        void GenerateFile();
        void Reset();
        void Stop();
        void Start();
        void SetStepsForOneTempPoint();
        CutOffBehavior GetCutOffBehavior(SmartTesterStep currentStep, IRow row);
        SmartTesterStep GetNewTargetStep(SmartTesterStep currentStep, List<SmartTesterStep> fullSteps, double temperature, IRow row);
    }
}