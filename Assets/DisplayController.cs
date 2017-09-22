using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController{
    string SerialPort;
    int length, width;
    int[,] LastPicture;
    int[,] Picture;

    public int Length
    {
        get { return length; }
    }

    public int Width
    {
        get { return width; }
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

    void showPicture()
    {
        return;
    }
}
