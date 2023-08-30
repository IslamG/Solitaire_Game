using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// A timer
/// </summary>
public class Timer : MonoBehaviour
{
    #region Fields
    [SerializeField]
    TextMeshProUGUI TimerTitle;

    public static bool IsTimed;

    // timer duration
    public float TotalTime = 0;
    // timer execution
    public float ElapsedTime = 0;
    bool running = true;
    // support for Finished property
    bool started = false;
    #endregion

    //void Update() { TotalTime += Time.deltaTime; TimerTitle.text = TotalTime.ToString(); }

    public void InitializeTimer()
    {
        IsTimed = OptionsManager.TimedGame;
        TotalTime = 0;
    }
    private void DeductPoints()//Per update? late update?
    {
        if (!IsTimed) return;
        if (OptionsManager.DrawCount is not DrawType.Single) return;
        if (ElapsedTime < 10) return;

        //ElapsedTime++;
        if (ElapsedTime >= 10)
        {
            ElapsedTime = 0;
            //points -2;
        }
    }

    #region Properties

    /// <summary>
    /// Sets the duration of the timer
    /// The duration can only be set if the timer isn't currently running
    /// </summary>
    /// <value>duration</value>
    public float Duration
    {
        set
        {
            if (!running)
            {
                TotalTime = value;
            }
        }
    }

    /// <summary>
    /// Gets whether or not the timer has finished running
    /// This property returns false if the timer has never been started
    /// </summary>
    /// <value>true if finished; otherwise, false.</value>
    public bool Finished
    {
        get { return started && !running; } 
    }

    /// <summary>
    /// Gets whether or not the timer is currently running
    /// </summary>
    /// <value>true if running; otherwise, false.</value>
    public bool Running
    {
        get { return running; }
    }

    /// <summary>
    /// Gets ho wmany seconds are left on the timer
    /// </summary>
    /// <value>seconds left</value>
    public float SecondsLeft
    {
        get 
        {
            if (running)
            {
                return TotalTime - ElapsedTime; 
            }
            else
            {
                return 0;
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {   
        // update timer and check for finished
        if (running)
        {
            ElapsedTime += Time.deltaTime;
            TotalTime += Time.deltaTime;
            //if (ElapsedTime >= TotalTime)
            //{
            //    running = false;
            //}
        }
        TimerTitle.text = ((int)TotalTime).ToString();
    }

    /// <summary>
    /// Runs the timer
    /// Because a timer of 0 duration doesn't really make sense,
    /// the timer only runs if the total seconds is larger than 0
    /// This also makes sure the consumer of the class has actually 
    /// set the duration to something higher than 0
    /// </summary>
    public void Run()
    {   
        // only run with valid duration
        if (TotalTime > 0)
        {
            started = true;
            running = true;
            ElapsedTime = 0;
        }
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    public void Stop()
    {
        started = false;
        running = false;
    }

    /// <summary>
    /// Adds the given number of seconds to the timer
    /// </summary>
    /// <param name="seconds">time to add</param>
    public void AddTime(float seconds)
    {
        TotalTime += seconds;
    }

    #endregion
}
