﻿using System;
using System.Net.Sockets;

namespace SmartTesterLib
{
    public class PUL80Executor : IChamberExecutor
    {
        private static TcpClient? tcpClient;
        private static NetworkStream? stream;

        public bool Init(string ipAddress, int port)
        {
            return OpenTcp(ipAddress, 1000, port);
        }

        private bool OpenTcp(string ipAddress, int connectTimeout, int port)
        {
            tcpClient = new TcpClient();
            var result = tcpClient.BeginConnect(ipAddress, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(connectTimeout);
            if (!success)
            {
                tcpClient.Close();
                return false;
            }
            tcpClient.EndConnect(result);

            stream = tcpClient.GetStream();
            stream.ReadTimeout = connectTimeout;
            return true;
        }

        //public bool ReadStatus(out ChamberStatus status)
        //{
        //    status = ChamberStatus.UNKNOWN;
        //    var value = Read(HongZhanPUL80Constant.OPERATION_ADDRESS);
        //    if (value == HongZhanPUL80Constant.STOP)
        //        status = ChamberStatus.STOP;
        //    else if (value == 3)
        //        status = ChamberStatus.RUN;
        //    return true;
        //}

        public bool ReadTemperature(out double temperature)
        {
            short value;
            bool ret;
            ret = Read(HongZhanPUL80Constant.TEMPERATURE_ADDRESS, out value);
            temperature = (double)value / 10.0;
            return ret;
        }

        //public double TargetTemperature()
        //{
        //    var value = Read(HongZhanPUL80Constant.TARGET_TEMPERATURE_ADDRESS);
        //    return (double)value / 10.0;
        //}

        public bool Start(double temperature)
        {
            bool ret;
            ret = Write(HongZhanPUL80Constant.TARGET_TEMPERATURE_ADDRESS, (Int16)(temperature * 10));
            if (ret != true)
                return ret;
            ret = Write(HongZhanPUL80Constant.OPERATION_ADDRESS, HongZhanPUL80Constant.START);
            if (ret != true)
                return ret;
            return true;
        }

        public bool Stop()
        {
            bool ret;
            ret = Write(HongZhanPUL80Constant.OPERATION_ADDRESS, HongZhanPUL80Constant.STOP);
            return ret;
        }

        private static byte[] GetResponse(NetworkStream stream)
        {
            string str = string.Empty;
            byte[] buffer = new byte[11];
            var bt = stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        private bool Read(byte addr, out Int16 iValue)
        {
            try
            {
                byte[] actionCmd =
                    {
                    0x00, 0x00,     //transaction identifier (Index)
                    0x00, 0x00,     //protocal identifier (TCP)
                    0x00, 0x06,     //length
                    0x01,           //unit identifier
                    0x03,           //function code
                    0x00, addr,     //address
                    0x00, 0x01
                };
                stream.Write(actionCmd, 0, actionCmd.Length);
                byte[] buffer = new byte[12];
                stream.Read(buffer, 0, buffer.Length);
                byte[] value = new byte[2] { buffer[10], buffer[9] };
                iValue = BitConverter.ToInt16(value, 0);
                return true;
            }
            catch (Exception e)
            {
                iValue = 0;
                return false;
            }
        }
        private bool Write(byte addr, Int16 value)
        {
            try
            {
                byte[] v = BitConverter.GetBytes(value);
                byte[] actionCmd =
                    {
                    0x00, 0x00,     //transaction identifier (Index)
                    0x00, 0x00,     //protocal identifier (TCP)
                    0x00, 0x09,     //length
                    0x01,           //unit identifier
                    0x10,           //function code
                    0x00, addr,     //sv temperature address
                    0x00, 0x01,     //quantity
                    0x02,           //byte count
                    v[1], v[0]    //0:stop 1:start
                };
                stream.Write(actionCmd, 0, actionCmd.Length);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public static class HongZhanPUL80Constant
    {
        public const byte TEMPERATURE_ADDRESS = 0;
        public const byte TARGET_TEMPERATURE_ADDRESS = 43;
        public const byte OPERATION_ADDRESS = 47;
        public const Int16 START = 1;
        public const Int16 STOP = 0;
    }
}