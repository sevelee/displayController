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

        myController.Init("COM8", 115200);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
    }
}
