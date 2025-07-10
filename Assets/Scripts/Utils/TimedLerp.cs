using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedLerp
{
    private float mLerpTime;
    private Vector2 mStart;
    private Vector2 mEnd;

    private float mStartTime;
    private bool  mLerpEnded = true;

    public TimedLerp(float timeInSeconds, float _unusedRate = 0f)
    {
        mLerpTime = Mathf.Max(0.0001f, timeInSeconds); // 防止除零
    }

    public void SetLerpParms(float timeInSeconds, float _unusedRate = 0f)
    {
        mLerpTime = Mathf.Max(0.0001f, timeInSeconds);
    }

    public void BeginLerp(Vector2 start, Vector2 end)
    {
        mStart     = start;
        mEnd       = end;
        mStartTime = Time.realtimeSinceStartup;
        mLerpEnded = false;
    }

    public Vector2 UpdateLerp()
    {
        if (mLerpEnded) return mEnd;

        float t = (Time.realtimeSinceStartup - mStartTime) / mLerpTime;
        if (t >= 1f)
        {
            mLerpEnded = true;
            return mEnd;
        }
        return Vector2.Lerp(mStart, mEnd, t);
    }

    public bool LerpIsActive() { return !mLerpEnded; }
}
