using UnityEngine;

public struct delay
{
    bool _onceAfterTime;
    public float duration;
    public float halfDuration;
    float _durationOver;
    float _nextTime;
    public delay(float duration_, bool onceAfterTime_ = true)
    {
        _onceAfterTime = onceAfterTime_;
        duration = duration_;
        halfDuration = duration_*0.5f;
        _durationOver = 1 / duration;
        _nextTime = 0;
    }

    public void Reset()
    {
        _onceAfterTime = true;
        _nextTime = Time.time + duration;
    }

    public void ResetRandGap(float duration_)
    {
        _onceAfterTime = true;
        duration = duration_;
        halfDuration = duration_*0.5f;
        _durationOver = 1 / duration_;
        _nextTime = Time.time + Random.Range(0, duration_);
    }

    public void Reset(float duration_)
    {
        _onceAfterTime = true;
        duration = duration_;
        halfDuration = duration_*0.5f;
        _durationOver = 1 / duration_;
        _nextTime = Time.time + duration_;
    }

    public void End()
    {
        _nextTime = 0;
    }

    public bool IsEnd()
    {
        return Time.time > _nextTime;
    }

    public bool InTime()
    {
        return Time.time < _nextTime;
    }

    public bool InHalfTime()
    {
        return Time.time < _nextTime - halfDuration;
    }

    public bool InTime(float percent)
    {
        return Time.time < _nextTime - duration*percent;
    }


    public float Ratio10()
    {
        return Mathf.Clamp01((_nextTime - Time.time)* _durationOver);
    }

    public float Ratio01()
    {
        return 1-Mathf.Clamp01((_nextTime - Time.time) * _durationOver);
    }

    public bool afterOnceTime()
    {
        if (_onceAfterTime && Time.time > _nextTime)
        {
            _onceAfterTime = false;
            return true;
        }
        return false; 
    }
}
