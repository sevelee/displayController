using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class tester : MonoBehaviour {
    DisplayController myController;

	// Use this for initialization
	void Start () {
        myController = new DisplayController();
        string[] portList = SerialPort.GetPortNames();
        for (int i = 0; i < portList.Length; i++)
        {
            Debug.Log(portList[i]);
        }

        myController.Init("COM3", 115200);
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
    }
}
