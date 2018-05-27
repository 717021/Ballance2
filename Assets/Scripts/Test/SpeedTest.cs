using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTest : MonoBehaviour {

    public bool isStart = true;

    public static bool isTesting = false;
    public static float allTime = 0;

    void Start () {
		
	}
	void Update () {
        if (isTesting)
        {
            allTime += Time.deltaTime;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (isStart)
            {
                if (!isTesting)
                {
                    allTime = 0;
                    isTesting = true;
                    GlobalMediator.Log("Test started!");
                }
            }
            else
            {
                if (isTesting)
                {
                    isTesting = false;
                    GlobalMediator.Log("Test ended!");
                    GlobalMediator.Log("All sec is " + allTime + " s");
                }
            }
        }
    }
}
