//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class DisplayController{
    string serialPortName;
    int height, width;
    int moveHeight, moveWidth;
    int hitterNumber = 6;
    int[,] lastPicture;
    int[,] picture;
    public SerialPort myPort;
    bool getResponse;

    int nowX, nowY;

    //
    Thread receiveThread;
    Thread sendThread;

    public int HitterNumber
    {
        get { return hitterNumber; }
    }

    public int Height
    { 
        get { return height; }
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

        width = 120;
        height = 60;

        lastPicture = new int[width, height];
        picture = new int[width, height];
        moveWidth = width / 6;
        moveHeight = height / 4;
        nowX = nowY = 0;
    }

    public void Init(string SerialPortName, int speed, int _width, int _height)
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

        width = _width;
        height = _height;

        lastPicture = new int[width, height];
        picture = new int[width, height];
        moveWidth = width / 6 / 2;
        moveHeight = height / 4;
        nowX = nowY = 0;
    }

    /// <summary>
    /// 发送移动位（相对），回头改成pravate
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void sendMove(int x, int y)
    {
        nowX += x;
        nowY += y;
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
    /// 输入一个bool型的数组，然后发送到硬件
    /// </summary>
    /// <param name="values"></param>
    public void sendDebugData(bool[][] values)
    {
        int[] sData = new int[hitterNumber];
        for (int i = 0; i < hitterNumber; i++)
        {
            int[] OriData;
            OriData = new int[8];
            for (int j = 0; j < 8; j++)
            {
                if (values[i][j])
                {
                    OriData[j] = 1;
                }
                else
                {
                    OriData[j] = 0;
                }
            }
            sData[i] = convertData(OriData);
        }
        sendData(sData);
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
    /// 输入一个数组，输出一个编码后的int 
    /// </summary>
    /// <param name="data"></param>
    private int convertData(int[] data)
    {
        int _result = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > 0)
            {
                _result |= (1 << i);
            }
        }
        return _result;
    }

    public void UpdataPicture()
    {
        stupidUpdate();
    }

    private void stupidUpdate()
    {
        // -1左面, 1右面
        int positionFlag = -1;
        for (int i = 0; i < moveHeight; i++)
        {
            // 如果不是第一行的话，先向下移动4格
            if (i > 0)
            {
                sendMove(0, 4);
            }
            int startH = i * 4;
            int[,] xorData;
            xorData = new int[width, 4];
            bool needMoveAndHit = false;
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    xorData[j, k] = lastPicture[j, startH + k] ^ picture[j, startH + k];
                    if (xorData[j,k] > 0)
                    {
                        needMoveAndHit = true;
                    }
                }
            }
            if (needMoveAndHit)
            {
                if (positionFlag == -1)
                {
                    int lastX = 0;
                    int nowX = 0;
                    bool needMoveX = false;
                    for (int j = 0; j < moveWidth; j++)
                    {
                        int[][] oriData;
                        oriData = new int[hitterNumber][];
                        for (int k = 0; k < hitterNumber; k++)
                        {
                            oriData[k] = new int[8];
                        }
                        needMoveX = false;
                        for (int k = 0; k < hitterNumber; k++)
                        {
                            for (int _i = 0; _i < 4; _i++)
                            {
                                oriData[k][_i] = xorData[j * 2, _i];
                                needMoveX |= (oriData[k][_i] == 1);
                            }
                            for (int _i = 0; _i < 4; _i++)
                            {
                                oriData[k][_i + 4] = xorData[j * 2 + 1, _i];
                                needMoveX |= (oriData[k][_i + 4] == 1);
                            }
                        }
                        if (needMoveX)
                        {
                            nowX = j * 2;
                            sendMove(nowX - lastX, 0);
                            int[] _sendData;
                            _sendData = new int[hitterNumber];
                            for (int k = 0; k < hitterNumber; k++)
                            {
                                _sendData[k] = convertData(oriData[k]);
                            }
                            sendData(_sendData);
                        }
                    }
                    sendMove(width / hitterNumber - 1 - nowX, 0);
                    positionFlag = 1;
                }
                else if (positionFlag == 1)
                {
                    int lastX = width / hitterNumber - 1;
                    int nowX = lastX;
                    bool needMoveX = false;
                    for (int j = moveWidth - 1; j >= 0; j--)
                    {
                        int[][] oriData;
                        oriData = new int[hitterNumber][];
                        for (int k = 0; k < hitterNumber; k++)
                        {
                            oriData[k] = new int[8];
                        }
                        needMoveX = false;
                        for (int k = 0; k < hitterNumber; k++)
                        {
                            for (int _i = 0; _i < 4; _i++)
                            {
                                oriData[k][_i] = xorData[j * 2, _i];
                                needMoveX |= (oriData[k][_i] == 1);
                            }
                            for (int _i = 0; _i < 4; _i++)
                            {
                                oriData[k][_i + 4] = xorData[j * 2 + 1, _i];
                                needMoveX |= (oriData[k][_i + 4] == 1);
                            }
                        }
                        if (needMoveX)
                        {
                            nowX = j * 2;
                            sendMove(nowX - lastX, 0);
                            int[] _sendData;
                            _sendData = new int[hitterNumber];
                            for (int k = 0; k < hitterNumber; k++)
                            {
                                _sendData[k] = convertData(oriData[k]);
                            }
                            sendData(_sendData);
                        }
                    }
                    sendMove(0 - nowX, 0);
                    positionFlag = -1;
                }
            }
        }

        //复原到0,0
        if (positionFlag == 1)
        {
            sendMove(-(width / hitterNumber - 1), -(height - 4));
        }
        else
        {
            sendMove(0, - (height - 4));
        }
    }
}