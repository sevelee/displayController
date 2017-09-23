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
    public SerialPort myPort;
    bool getResponse;

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
        getResponse = true;

        if (myPort.IsOpen)
        {
            Debug.Log("port opened.");
        }
    }

    /// <summary>
    /// 发送移动位（相对），回头改成pravate
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void sendMove(int x, int y)
    {
        Thread rThread;
        // 如果上一次通讯得到了回复（即，arduino完成了动作），再发送这次动作。
        if (getResponse)
        {
            myPort.Write("Move," + x + "," + y);
            getResponse = false;
            rThread = new Thread(check_r_thread);
            rThread.IsBackground = true;
            rThread.Start();
        }
    }

    /// <summary>
    /// 发送一组sData
    /// </summary>
    /// <param name="sData"></param>
    public void sendData(int[] sData)
    {
        Thread rThread;
        if (getResponse)
        {
            string sendString = "Hit,";
            for (int i = 0; i < sData.Length; i++)
            {
                sendString += sData[i] + ",";
            }
            myPort.Write(sendString);
            getResponse = false;
            rThread = new Thread(check_r_thread);
            rThread.IsBackground = true;
            rThread.Start();
        }
    }

    /// <summary>
    /// 查看arduino是否进行了回复，Move和Hit都可以用。
    /// </summary>
    private void check_r_thread()
    {
        while (true)
        {
            try
            {
                string strRec = myPort.ReadLine();
                Debug.Log(strRec);
                if (strRec == "o")
                {
                    getResponse = true;
                    return;
                }
            }
            catch (Exception)
            {
            }
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
