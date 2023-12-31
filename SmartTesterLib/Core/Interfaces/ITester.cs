﻿using System.Collections.Generic;

namespace SmartTesterLib
{
    public interface ITester: IAssamble
    {
        int Id { get; set; }
        //void Start(int index);
        //void Stop(int index);
        List<IChannel> Channels { get; set; }
        string Name { get; set; }
        ITesterExecutor Executor { get; set; }
    }
}