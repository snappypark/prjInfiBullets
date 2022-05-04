using UnityEngine;

namespace nj
{
    public struct time
    {
        float _curTime;
        float _durationOver;
        public time(float duration_)
        {
            _curTime = 0;
            _durationOver = 1 / duration_;
        }

        public void Reset()
        {
            _curTime = Time.time;
        }

        public void Reset(float newDuration_)
        {
            _curTime = Time.time;
            _durationOver = 1 / newDuration_;
        }

        public float Ratio01() { return Mathf.Clamp01((Time.time - _curTime) * _durationOver); }
        public float Ratio10() { return 1 - Mathf.Clamp01((Time.time - _curTime) * _durationOver); }
    }
    
    public struct timer
    {
        public nj.time time;
        float _nextTime;
        float _duration;
        public timer(float duration_)
        {
            _duration = duration_;
            _nextTime = duration_;
            time = new time(duration_);
        }

        public void Reset()
        {
            _nextTime = Time.time + _duration;
            time.Reset();
        }

        public void Reset(float newDuration_)
        {
            time.Reset(newDuration_);
            _duration = newDuration_;
            _nextTime = Time.time + _duration;
        }

        public bool IsEnd() { return Time.time > _nextTime; }
        public bool InDuration() { return Time.time < _nextTime; }
    }

    public struct timeFlager
    {
        public nj.timer timer;
        bool _onceAfterTime;
        public timeFlager(float duration_)
        {
            timer = new timer(duration_);
            _onceAfterTime = true;
        }

        public void Reset()
        {
            _onceAfterTime = true;
            timer.Reset();
        }

        public void Reset(float duration_)
        {
            _onceAfterTime = true;
            timer.Reset(duration_);
        }

        public bool afterOnceTime()
        {
            if (_onceAfterTime && timer.IsEnd())
            {
                _onceAfterTime = false;
                return true;
            }
            return false;
        }
    }
}
