using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public struct RecordedData
    {
        public Vector2 position;
        public float rotation;
    }

    [SerializeField] private TimeKeeper timeKeeper;

    float[] timeStamps;
    RecordedData[,] recordedData; //TODO? rewrite to list. Probaly list of lists. For memory reasons
    int recordMax = 100000; // 27 mins if fps = 60 and recording every frame
    int recordIndex;
    int recordCount;
    int prevRecordIndex; // to know correct interval in records for interpolation

    [SerializeField] private int framesBetweenRecords;
    private float recordDelay;

    bool wasSteppingBack = false;

    float currentTimeLineEndTime = 0; // for time keeper

    TimeControlledCharacter[] timeCharacters;

    private void Awake()
    {
        timeCharacters = GameObject.FindObjectsOfType<TimeControlledCharacter>();
        recordedData = new RecordedData[timeCharacters.Length, recordMax];
        timeStamps = new float[recordMax];
        recordDelay = 0.02f * framesBetweenRecords; // 0.02 because assuming 50 frames
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    void ChangeTimeScale()
    {
        float slowdownFactor = 0.1f;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    void NormalTimeScale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    // Update is called once per frame
    void Update()
    {

        bool pause = Input.GetKey(KeyCode.UpArrow);
        bool stepBack = Input.GetKey(KeyCode.LeftArrow);
        bool stepForward = Input.GetKey(KeyCode.RightArrow);
        bool slowDown = Input.GetKey(KeyCode.Space);
        if (slowDown)
        {
            ChangeTimeScale();
        }
        else
        {
            NormalTimeScale();
        }
        if (stepBack)
        {
            StepBack();
        }
        else if (pause && stepForward)
        {
            StepForward();
        }
        else if (!pause && !stepBack)
        {
            NormalTimeFlow();
        }
    }
    void RecordData()
    {
        for (int characterIndex = 0; characterIndex < timeCharacters.Length; characterIndex++)
        {
            TimeControlledCharacter character = timeCharacters[characterIndex];
            character.rigidbody = character.GetComponent<Rigidbody2D>();
            RecordedData data = new RecordedData(); // maybe make constuctor?
            data.position = character.rigidbody.position;
            data.rotation = character.rigidbody.rotation;
            recordedData[characterIndex, recordCount] = data;
            timeStamps[recordCount] = timeKeeper.objectiveTime;
        }
        recordCount++;
        recordIndex = recordCount;
    }

    void NormalTimeFlow()
    {
        timeKeeper.TimeUpdate(Time.deltaTime);
        currentTimeLineEndTime = timeKeeper.objectiveTime;
        if (wasSteppingBack)
        {
            wasSteppingBack = false;
            
            recordCount = recordIndex;
            timeKeeper.lastRecordTime = timeKeeper.objectiveTime;
        }
        if (timeKeeper.IsRecordNeeded(recordDelay))
        {
            RecordData();
            //Debug.Log(recordedData[0, recordCount - 1].position);
        }

        foreach (TimeControlledCharacter timeCharacter in timeCharacters)
        {
            timeCharacter.TimeUpdate();
        }
    }
    /*
    void WipeTailTimeLine()
    {
        for (int characterIndex = 0; characterIndex < timeCharacters.Length; characterIndex++)
        {
            for (int i = recordIndex + 1; i < recordCount; i++)
            {
                recordedData[characterIndex, i] = 0;
            }
            TimeControlledCharacter character = timeCharacters[characterIndex];
            character.rigidbody = character.GetComponent<Rigidbody2D>();
            RecordedData data = new RecordedData(); // maybe make constuctor?
            data.position = character.rigidbody.position;
            data.rotation = character.rigidbody.rotation;
            recordedData[characterIndex, recordCount] = data;
            timeStamps[recordCount] = timeKeeper.objectiveTime;
        }
    }*/
    void StepBack()
    {
        timeKeeper.TimeUpdate(Time.deltaTime * (-1));
        if (recordIndex > 0) // could be special behavior when in (0,1) = first 0.2 seconds e.g. || (recordCount - 1, recordCount)
        {
            if (wasSteppingBack == false) // TODO: there is a bug when you are near first second and you rapidly tap rewind, index goes below 0
            {
                recordIndex--;
                wasSteppingBack = true;
                timeKeeper.objectiveTime = timeStamps[recordIndex]; // hack, but rapidly typing teleports you
            }
            if (timeKeeper.objectiveTime <= timeStamps[recordIndex]) // potentially could skip more than one frame if game froze and Time.deltaTime is too big
            { // maximum deltaTime is 0.333 (https://docs.unity3d.com/Manual/TimeFrameManagement.html) which might be bigger than time between records
                recordIndex--;
                Debug.Log(recordIndex);
            }
            InterpolateTimeCharacters();
        }
    }
    void StepForward()
    {
        if (timeKeeper.objectiveTime + Time.deltaTime < currentTimeLineEndTime)
        {
            timeKeeper.TimeUpdate(Time.deltaTime);
        }
        wasSteppingBack = true; // probably isn't needed (its already true)
        if (recordIndex < recordCount - 1)
        {
            if (timeKeeper.objectiveTime >= timeStamps[recordIndex + 1])
            {
                recordIndex++;
                Debug.Log(recordIndex);
            }
            InterpolateTimeCharacters();
        }
    }

    void InterpolateTimeCharacters()
    {
        for (int characterIndex = 0; characterIndex < timeCharacters.Length; characterIndex++)
        {
            TimeControlledCharacter character = timeCharacters[characterIndex];
            character.rigidbody = character.GetComponent<Rigidbody2D>();
            RecordedData data = recordedData[characterIndex, recordIndex];
            RecordedData dataNext = recordedData[characterIndex, recordIndex + 1];

            float timeRatio = (timeKeeper.objectiveTime - timeStamps[recordIndex]) / (timeStamps[recordIndex + 1] - timeStamps[recordIndex]);

            character.rigidbody.position = Vector2.Lerp(data.position, dataNext.position, timeRatio);
            character.rigidbody.rotation = InterpolateRotation(data.rotation, dataNext.rotation, timeRatio);
        }
    }

    float InterpolateRotation(float rot1, float rot2, float ratio)
    {
        float rotRes = 0;
        if ((rot1 > 90 && rot2 < -90) || (rot1 < -90 && rot2 > 90))
        {
            if (rot2 < -90)
            {
                rot2 += 360;
            }
            else
            {
                rot1 += 360;
            }
        }
        rotRes = Mathf.Lerp(rot1, rot2, ratio);
        if (rotRes > 180)
        {
            rotRes -= 360;
        }
        return rotRes;
    }
}
