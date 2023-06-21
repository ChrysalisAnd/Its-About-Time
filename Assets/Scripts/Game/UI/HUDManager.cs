using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private TimeKeeper timeKeeper;

    public string TimeTextValue = "HELLO";
    public TextMeshProUGUI Timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Timer.text = timeKeeper.objectiveTime.ToString("0.00"); 
    }
}
