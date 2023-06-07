using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{

    public struct RecordedData
    {
        public Vector2 position;
        public float rotation;
    }

    RecordedData[,] recordedData;
    int recordMax = 100000; // 27 mins if fps = 60 and recording every frame
    int recordIndex;
    int recordCount;
    bool wasSteppingBack = false;

    TimeControlledCharacter[] timeCharacters;

    private void Awake()
    {
        timeCharacters = GameObject.FindObjectsOfType<TimeControlledCharacter>();
        recordedData = new RecordedData[timeCharacters.Length, recordMax];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool pause = Input.GetKey(KeyCode.UpArrow);
        bool stepBack = Input.GetKey(KeyCode.LeftArrow);
        bool stepForward = Input.GetKey(KeyCode.RightArrow);
        if (stepBack)
        {
            wasSteppingBack = true;
            if (recordIndex > 0)
            {
                recordIndex--;
                for (int characterIndex = 0; characterIndex < timeCharacters.Length; characterIndex++)
                {
                    TimeControlledCharacter character = timeCharacters[characterIndex];
                    character.rigidbody = character.GetComponent<Rigidbody2D>();
                    RecordedData data = recordedData[characterIndex, recordIndex];
                    character.rigidbody.position = data.position;
                    character.rigidbody.rotation = data.rotation;
                }
            }

        }
        else if (pause && stepForward)
        {
            wasSteppingBack = true;
            if (recordIndex < recordCount - 1)
            {
                recordIndex++;
                for (int characterIndex = 0; characterIndex < timeCharacters.Length; characterIndex++)
                {
                    TimeControlledCharacter character = timeCharacters[characterIndex];
                    character.rigidbody = character.GetComponent<Rigidbody2D>();
                    RecordedData data = recordedData[characterIndex, recordIndex];
                    character.rigidbody.position = data.position;
                    character.rigidbody.rotation = data.rotation;
                }
            }
        }
        else if (!pause && !stepBack)
        {
            if (wasSteppingBack)
            {
                wasSteppingBack = false;
                recordCount = recordIndex;
            }
            for (int characterIndex = 0;  characterIndex < timeCharacters.Length; characterIndex++)
            {
                TimeControlledCharacter character = timeCharacters[characterIndex];
                character.rigidbody = character.GetComponent<Rigidbody2D>();
                RecordedData data = new RecordedData();
                data.position = character.rigidbody.position;
                data.rotation = character.rigidbody.rotation;
                recordedData[characterIndex, recordCount] = data;
            }
            recordCount++;
            recordIndex = recordCount;

            foreach(TimeControlledCharacter timeCharacter in timeCharacters)
            {
                timeCharacter.TimeUpdate();
                Debug.Log("Working");
            }
        }
    }
}
