using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public float currentTimeScale, timeStopMultiplier;
    public float TimeScale { get {return currentTimeScale; } set { currentTimeScale = value; } }
    private float standardTimeScale;
    [HideInInspector]
    public bool isTimeStoped;

    public void Start()
    {
        isTimeStoped = false;
        standardTimeScale = Time.fixedDeltaTime;
        resetTimeScale();
        updateTimeScale(timeStopMultiplier);
    }

    private void Update()
    {
        if (isTimeStoped)
        {
            updateTimeScale(timeStopMultiplier);
        }
        else if (!isTimeStoped && TimeScale != standardTimeScale )
        {
            resetTimeScale();
        }
    }
    public void enableTimeStop()
    {
        isTimeStoped = !isTimeStoped;
    }
    private void updateTimeScale(float multiplyBy)
    {
        TimeScale = Mathf.Lerp(TimeScale, multiplyBy, Time.deltaTime*2);
    }
    public void resetTimeScale()
    {
        TimeScale = standardTimeScale;
    }

}
