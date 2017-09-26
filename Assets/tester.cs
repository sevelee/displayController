using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class tester : MonoBehaviour {
    DisplayController myController;
    bool[][] values;

	// Use this for initialization
	void Start () {
        myController = new DisplayController();
        string[] portList = SerialPort.GetPortNames();
        for (int i = 0; i < portList.Length; i++)
        {
            Debug.Log(portList[i]);
        }

        myController.Init("COM8", 115200);

        values = new bool[myController.HitterNumber][];
        for (int i = 0; i < myController.HitterNumber; i++)
        {
            values[i] = new bool[8];
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 50), "move test"))
        {
            myController.sendMove(2, 2);
        }
        if (GUI.Button(new Rect(210, 10, 200, 50), "hit test"))
        {
            int[] temp;
            temp = new int[] { 1, 1, 1, 1, 1, 1 };
            myController.sendData(temp);
        }

        // 上下左右
        if (GUI.Button(new Rect(10, 200, 50, 50), "left"))
        {
            myController.sendMove(1, 0);
        }
        if (GUI.Button(new Rect(110, 200, 50, 50), "right"))
        {
            myController.sendMove(-1, 0);
        }
        if (GUI.Button(new Rect(60, 150, 50, 50), "up"))
        {
            myController.sendMove(0, -1);
        }
        if (GUI.Button(new Rect(60, 200, 50, 50), "down"))
        {
            myController.sendMove(0, 1);
        }

        int hitterStartX = 50;
        int hitterStartY = 300;

        for (int i = 0; i < myController.HitterNumber; i++)
        {
            int startX = hitterStartX + i * 100;
            for (int j = 0; j < 4; j++)
            {
                values[i][j] = GUI.Toggle(new Rect(startX, hitterStartY + j * 20, 20, 20), values[i][j], "");
            }
            for (int j = 0; j < 4; j++)
            {
                values[i][j + 4] = GUI.Toggle(new Rect(startX + 20, hitterStartY + j * 20, 20, 20), values[i][j + 4], "");
            }
        }

        if (GUI.Button(new Rect(hitterStartX, hitterStartY + 100, 150, 20), "发送电磁铁测试数据"))
        {
            myController.sendDebugData(values);
        }
    }
}
