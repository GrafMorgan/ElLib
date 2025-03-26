using System;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

namespace ElLib
{
    public class Timer
    {
        #region NonStatic fields & Methods

        public UnityEvent OnTimesUp = new UnityEvent();
        private float _time;
        private bool isSingleUsed;
        bool isScaled;
        public bool isActive { get; private set; }

        private float _passedTime;
        public Timer(float time, bool IsScaled = true)
        {
            isScaled = IsScaled;
            _time = time;
            isActive = true;
        }
        public void Update(bool isScaled = true)
        {
            if (isActive == true)
            {
                if (isScaled)
                    _passedTime += Time.deltaTime;
                else
                    _passedTime += Time.unscaledDeltaTime;
                if (_passedTime >= _time)
                {
                    OnTimesUp?.Invoke();
                }
            }
        }
        public float GetTime()
        {
            return _passedTime;
        }
        public void Reset()
        {
            _passedTime = 0;
        }
        #endregion

    }
}