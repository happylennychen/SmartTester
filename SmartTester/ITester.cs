﻿namespace SmartTester
{
    public interface ITester
    {
        void SetStep(Step step, int index);
        void Start(int index);
        void Stop(int index);
        string GetData(int index);
    }
}