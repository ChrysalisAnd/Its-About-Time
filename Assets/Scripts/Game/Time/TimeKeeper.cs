using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{


    public float objectiveTime { get; set; }

    public float lastRecordTime { get; set; }

    void Start()
    {
        lastRecordTime = -1f;
        objectiveTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimeUpdate(float deltaTime)
    {
        objectiveTime += deltaTime;
        if (objectiveTime < 0)
        {
            objectiveTime = 0;
        }
    }

    public bool IsRecordNeeded(float recordDelay)
    {
        if (objectiveTime > lastRecordTime + recordDelay)
        {
            lastRecordTime = objectiveTime;
            //Debug.Log("recording: " + objectiveTime);
            return true;
        }

        return false;
    }
}
