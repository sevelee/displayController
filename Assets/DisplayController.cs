//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class DisplayController{
    string serialPortName;
    int length, width;
    int hitterNumber = 6;
    int[,] lastPicture;
    int[,] picture;
    SerialPort myPort;

    //
    Thread receiveThread;
    Thread sendThread;

    public int Length
    { 
        get { return length; }
    }

    public int Width
    {
        get { return width; }
    }

    public int[,] LastPicture
    {
        get { return lastPicture; }
    }

    public int[,] Picture
    {
        get { return picture; }
        set { picture = value; }
    }

    public void Init(string SerialPortName, int speed)
    {
        serialPortName = SerialPortName;
        myPort = new SerialPort(serialPortName, speed, Parity.None, 8, StopBits.One);
        myPort.ReadTimeout = 1000;
        myPort.Open();

        if (myPort.IsOpen)
        {
            Debug.Log("port opened.");
        }
    }

    /// <summary>
    /// 设置显示大小
    /// </summary>
    /// <param length="x"></param>
    /// <param width="y"></param>
    public void SetSize(int x, int y)
    {
        length = x;
        width = y;
    }

    /// <summary>
    /// 将hitter的数据压缩一下
    /// </summary>
    void convertData (int[,] data)
    {

        return;
    }
}
