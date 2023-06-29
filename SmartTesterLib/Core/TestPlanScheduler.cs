using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartTester
{
    public class TestPlanScheduler
    {
        private List<TestRound> TestRoundList { get; set; }
        public bool IsCompleted 
        {
            get 
            {
                return !TestRoundList.Any(tr => tr.Status == RoundStatus.WAITING);
            } 
        }

        public void AppendTestRound(TestRound round)
        {
            round.Status = RoundStatus.WAITING;
            TestRoundList.Add(round);
        }
        public void InsertTestRound(TestRound round, int index)
        {
            round.Status = RoundStatus.WAITING;
            TestRoundList.Insert(index, round);
        }
        public void MoveTestRound(int original, int destination)    //需要测试
        {
            TestRound tr = TestRoundList[original];
            TestRoundList.Remove(tr);
            TestRoundList.Insert(destination, tr);
        }
        public TestRound GetRunningTestRound()
        {
            return TestRoundList.Single(tr => tr.Status == RoundStatus.RUNNING);
        }
        public TestRound GetFirstWaitingTestRound()
        {
            return TestRoundList.First(tr => tr.Status == RoundStatus.WAITING);
        }
        public void SkipTestRound(int index)
        {
            TestRoundList[index].Status = RoundStatus.SKIPPED;
        }
        public void SkipTestRound(TestRound round)
        {
            round.Status = RoundStatus.SKIPPED;
        }
        public void RunTestRound(int index)
        {
            TestRoundList[index].Status = RoundStatus.RUNNING;
        }
        public void RunTestRound(TestRound round)
        {
            round.Status = RoundStatus.RUNNING;
        }
        public void CompleteTestRound(int index)
        {
            TestRoundList[index].Status = RoundStatus.COMPLETED;
        }
        public void CompleteTestRound(TestRound round)
        {
            round.Status = RoundStatus.COMPLETED;
        }
    }
}