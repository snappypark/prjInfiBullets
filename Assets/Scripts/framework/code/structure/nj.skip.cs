using System;

namespace nj
{
    public struct skip
    {
        int _skipIdx, _skipCount;
        public skip(int skipCount) {
            _skipIdx = 1;
            _skipCount = skipCount;
        }

        public void OnUpdate(Action callback)
        {
            switch (_skipIdx)
            {
                case 0:
                    callback();
                    _skipIdx = _skipCount;
                    break;
                default:
                    --_skipIdx;
                    break;
            }
        }
    }
}
