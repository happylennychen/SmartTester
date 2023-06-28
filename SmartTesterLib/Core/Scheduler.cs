using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SmartTester
{
    public class Scheduler
    {
        //public DebugTester Tester { get; set; }
        private Timer mainTimer { get; set; }
        private int _counter { get; set; } = 0;
        private Stopwatch mainWatch { get; set; }
        public int _count { get; set; }
        private int _slice { get; set; }
        private int _margin { get; set; }
        private int _threshold { get; set; }
        private Token[] TokenArrary { get; set; }
        public Scheduler(int count)
        {
            _count = count;
            _slice = 1000 / count;
            _margin = 25;
            _threshold = 10;
            mainWatch = new Stopwatch();
            TokenArrary = new Token[count];
        }
        public void RegisterToken(Token token)
        {
            TokenArrary[token.Index - 1] = token;
        }
        public void Activate()
        {
            mainTimer = new Timer(_ => MainCounter(), null, 100, 0);
            mainWatch.Start();
        }
        public void Deactivate()
        {
            mainWatch.Stop();
        }

        private void MainCounter()
        {
            long data;
            do
            {
                data = mainWatch.ElapsedMilliseconds % _slice;       //slice是将1S分成多少份之后，每一份的长度，data是在这一份中的游标
            }
            while (data > _threshold);          //data大于threshold表示脱离目标时间点，会被锁住，小于threshold表示刚刚到达目标时间点
            mainTimer.Change(_slice - _margin, 0);   //重新设定下个定时器入口，入口会比目标时间点提前margin。

            var counter = _counter % _count;    //counter在0，_count-1之间不停循环
            //var channel = Tester.Channels.SingleOrDefault(ch => ch.Index == counter + 1);
            var token = GetToken(counter + 1);
            if (token.ShouldTimerStart && !token.IsTimerStart)     //应该开启且还没开启
            {
                token.Callback();
                token.IsTimerStart = true;
            }
            _counter++;
            if (_counter == _count * 10)
            {
                _counter = 0;
            }
        }

        private Token GetToken(int id)
        {
            return TokenArrary.SingleOrDefault(t => t.Index == id);
        }
    }
}