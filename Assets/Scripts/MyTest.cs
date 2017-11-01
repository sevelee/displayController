using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using System.IO;

public class MyTest : MonoBehaviour {
    public SerialPort m_port;
    private string m_serialPortName;
    private int m_height;
    private int m_width;
    private bool[,] m_lastPicture;
    private bool[,] m_currentPicture;
    bool m_getResponse;
    private int m_hitterNumber;
    private int m_x;
    private int m_y;
    Thread m_receiveThread;
    Thread m_sendThread;

    public void Init(string serialPortName, int speed)
    {
        m_serialPortName = serialPortName;
        //m_port = new SerialPort(m_serialPortName, speed, Parity.None, 8, StopBits.One);
        //m_port.ReadTimeout = 1000;
        //m_port.Open();
        m_getResponse = true;
        //if (m_port.IsOpen)
        //{
        //    Debug.Log("Port open successful!");
        //}
        m_width = 120;
        m_height = 60;
        m_hitterNumber = 6;
        m_lastPicture = new bool[m_width, m_height];
        m_currentPicture = new bool[m_width, m_height];
        m_x = m_y = 0;
    }

    private void CheckThread()
    {
        while(true)
        {
            try
            {
                string str = m_port.ReadLine();
                Debug.Log(str);
                if(str == "o")
                {
                    m_getResponse = true;
                    return;
                }
            }
            catch(Exception)
            {
            }
        }
    }

    public void SendMove(int x, int y)
    {
        m_x += x;
        m_y += y;
        Thread thread;
        if(m_getResponse)
        {
            m_port.Write("Move: " + x + ", " + y);
            m_getResponse = false;
            thread = new Thread(CheckThread);
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private void SendData(int[] data)
    {
        Thread thread;
        if (m_getResponse)
        {
            string str = "Hit,";
            for (int i = 0; i < data.Length; i++)
            {
                str += data[i] + ",";
            }
            m_port.Write(str);
            m_getResponse = false;
            thread = new Thread(CheckThread);
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private int ConvertData(int[] orignal)
    {
        int result = 0;
        for(int i = 0; i < orignal.Length; i++)
        {
            if (orignal[i] > 0)
                result |= (1 << i);
        }
        return result;
    }

    private void SendDebugData(bool[,] values)
    {
        int[] data = new int[m_hitterNumber];
        for (int i = 0; i < m_hitterNumber; i++)
        {
            int[] orignal = new int[8];
            for (int j = 0; j < 8; j++)
            {
                if (values[i,j])
                {
                    orignal[j] = 1;
                }
                else
                {
                    orignal[j] = 0;
                }
            }
            data[i] = ConvertData(orignal);
        }
        SendData(data);
    }

    private bool UpdatePicture(string imageName)
    {
        Texture2D temp = Resources.Load("Images/" + imageName, typeof(Texture2D)) as Texture2D;
        if (temp.width < m_width || temp.height < m_height)
            return false;
        Debug.Log(temp.width + ", " + temp.height);
        m_lastPicture = m_currentPicture;
        for(int i = 0; i < m_width; i++)
        {
            for(int j = 0; j < m_height; j++)
            {
                m_currentPicture[i, j] = temp.GetPixel(i, m_height - 1 - j).r > 0.5f ? true : false;
            }
        }
        return true;
    }

    private void DisplayCurrentPicture()
    {
        bool[,] data = new bool[m_width, m_height];
        Texture2D t = Resources.Load("Images/Image1", typeof(Texture2D)) as Texture2D;
        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
                data[i, j] = m_lastPicture[i, j] ^ m_currentPicture[i, j];
                if (data[i, j])
                    t.SetPixel(i, j, new Color(0, 0, 0));
                else
                    t.SetPixel(i, j, new Color(1, 1, 1));
            }
        }
        bool[,] values = new bool[m_hitterNumber, 8];
        int column, row;
        for(int num = 0; num < 20; num++)
        {
            row = num / 10;
            column = num % 10;
            for (int i = 0; i < m_hitterNumber; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (row % 2 == 0)
                    {
                        values[i, j] = data[2 * column + 20 * i + j / 4, row * 4 + j % 4];
                    }
                    else
                    {
                        values[i, j] = data[2 * (9 - column) + 20 * i + j / 4, row * 4 + j % 4];
                    }
                }
            }
            //SendDebugData(values);
            //if (column == 9 && num != 149)
            //{
            //    SendMove(0, 4);
            //}
            //else if (row % 2 == 0)
            //{
            //    SendMove(2, 0);
            //}
            //else
            //{
            //    SendMove(-2, 0);
            //}
        }
    }

    // Use this for initialization
    void Start()
    {
        //Init("dafd", 0);
        //if (UpdatePicture("t"))
        //    DisplayCurrentPicture();

        Texture2D t = Resources.Load("Images/test", typeof(Texture2D)) as Texture2D;
        for(int i = 0; i < t.width; i++)
        {
            for (int j = 0; j < t.height; j++)
                Debug.Log(t.GetPixel(i, j));
        }
    }
}
