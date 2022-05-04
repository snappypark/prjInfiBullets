using UnityEngine;

namespace UnityEngineEx
{
    [System.Serializable]
    public class VecDamp
    {
        [SerializeField] float _smoothTime = 0.1f;
        [SerializeField] float maxSpeed = 500;
        Vector3 _posCurrent;
        Vector3 _posTarget;
        Vector3 _vel;

        public Vector3 Cur { get { return _posCurrent; } }
        public Vector3 Target { get { return _posTarget; } }

        public void SetSmoothTime(float smoothTime_)
        {
            _smoothTime = smoothTime_;
        }
        public void Init(Vector3 current )
        {
            _vel = Vector3.zero;
               _posCurrent = current;
            _posTarget = current;
        }

        public void SetNewTarget(Vector3 newTarget)
        {
            _posTarget = newTarget;
        }

        public bool UpdateUntilCoincide()
        {
            if ((_posTarget - _posCurrent).sqrMagnitude < 0.000001f)
                return false;
            _posCurrent = Vector3.SmoothDamp(_posCurrent, _posTarget,
                ref _vel, _smoothTime, maxSpeed * Time.smoothDeltaTime);

            return true;
        }
    }
}
