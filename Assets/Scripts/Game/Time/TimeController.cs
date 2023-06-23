using System.Collections;
using System.Collections.Generic;
using System.Data;
#if UNITY_EDITOR
using UnityEditor.MemoryProfiler;
#endif
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TimeController : MonoBehaviour
{
    public struct RecordCharacter
    {
        public Vector2 position;
        public float rotation;
    }

    public struct RecordBullet
    {
        public Vector2 position;
        public float rotation;
        //public bool state; // true - start, false - end
    }


    public int bulletIndex;

    [SerializeField] private TimeKeeper timeKeeper;
    [SerializeField] private BulletPool bulletPool;

    float[] timeStamps;
    RecordCharacter[,] recordedCharacters; //TODO? rewrite to list. Probaly list of lists. For memory reasons
    RecordBullet[,] recordedBullets;
    int recordMax = 20000; // 4000 secs if fps = 50
    int recordIndex;
    int recordCount;


    [SerializeField] private int framesBetweenRecords;
    private float recordDelay;

    bool wasSteppingBack = false;

    float currentTimeLineEndTime = 0; // for time keeper

    TimeCharacter[] timeCharacters;

    private void Awake()
    {
        timeCharacters = GameObject.FindObjectsOfType<TimeCharacter>();
        recordedCharacters = new RecordCharacter[timeCharacters.Length, recordMax];
        timeStamps = new float[recordMax];
        recordDelay = 0.02f * framesBetweenRecords; // 0.02 because assuming 50 frames

        recordedBullets = new RecordBullet[bulletPool.Size, recordMax];

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

    void Update()
    {

        bool pause = Input.GetKey(KeyCode.UpArrow);
        bool stepBack = Input.GetKey(KeyCode.LeftArrow);
        bool stepForward = Input.GetKey(KeyCode.RightArrow);
        bool slowDown = Input.GetKey(KeyCode.Space);
        if (slowDown) // make it based on events to not check every frame?
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
            TimeCharacter character = timeCharacters[characterIndex];
            character.rigidbody = character.GetComponent<Rigidbody2D>();
            RecordCharacter data = new RecordCharacter(); // maybe make constuctor?
            data.position = character.rigidbody.position;
            data.rotation = character.rigidbody.rotation;
            recordedCharacters[characterIndex, recordCount] = data;
        }
        
        for (int bulletIndex = 0; bulletIndex < bulletPool.Size; bulletIndex++)
        {
            GameObject bullet = bulletPool.bulletPool[bulletIndex];
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            RecordBullet data = new RecordBullet();
            data.position = rb.position;
            data.rotation = rb.rotation;
            recordedBullets[bulletIndex, recordCount] = data;
        }

        timeStamps[recordCount] = timeKeeper.objectiveTime;
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
            bulletPool.ReactivateBullets();
            timeKeeper.lastRecordTime = timeKeeper.objectiveTime;
        }
        if (timeKeeper.IsRecordNeeded(recordDelay))
        {
            RecordData();
            //Debug.Log(recordedData[0, recordCount - 1].position);
        }

        foreach (TimeCharacter timeCharacter in timeCharacters)
        {
            timeCharacter.TimeUpdate();
        }
    }

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
            InterpolateBullets();
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
            InterpolateBullets();
        }
    }

    void InterpolateTimeCharacters()
    {
        for (int characterIndex = 0; characterIndex < timeCharacters.Length; characterIndex++)
        {
            TimeCharacter character = timeCharacters[characterIndex];
            character.rigidbody = character.GetComponent<Rigidbody2D>();
            RecordCharacter data = recordedCharacters[characterIndex, recordIndex];
            RecordCharacter dataNext = recordedCharacters[characterIndex, recordIndex + 1];

            float timeRatio = (timeKeeper.objectiveTime - timeStamps[recordIndex]) / (timeStamps[recordIndex + 1] - timeStamps[recordIndex]);

            character.rigidbody.position = Vector2.Lerp(data.position, dataNext.position, timeRatio);
            character.rigidbody.rotation = InterpolateRotation(data.rotation, dataNext.rotation, timeRatio);
        }
    }
    
    void InterpolateBullets()
    {

        for (int bulletIndex = 0; bulletIndex < bulletPool.Size; bulletIndex++)
        {
            RecordBullet data = recordedBullets[bulletIndex, recordIndex];
            RecordBullet dataNext = recordedBullets[bulletIndex, recordIndex + 1];
            GameObject bullet = bulletPool.bulletPool[bulletIndex];
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            float timeRatio = (timeKeeper.objectiveTime - timeStamps[recordIndex]) / (timeStamps[recordIndex + 1] - timeStamps[recordIndex]);
            rb.position = Vector2.Lerp(data.position, dataNext.position, timeRatio);
            rb.rotation = data.rotation;

            if (data.position == BulletPool.restPosition)
            {
                rb.position = BulletPool.restPosition;
                rb.rotation = 0;
            }
            else if (dataNext.position == BulletPool.restPosition)
            {
                rb.position = BulletPool.restPosition;
                rb.rotation = 0;
            }
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
