using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    public class TimedCallback
    {
        public Action startCallback;

        public Action callback;

        public float timeOfEnd;

        public bool finished = true;

        private float timedAmount;

        public bool flexbool = false;

        public TimedCallback(Action _callback, float _ms, Action _startCallback)
        {
            callback = _callback;
            startCallback = _startCallback;
            timedAmount = _ms/1000;
        }

        public TimedCallback(Action _callback, int seconds)
        {
            callback = _callback;
            timedAmount = seconds;
        }

        public void Start()
        {
            startCallback?.Invoke();
            timeOfEnd = Time.instance.time + timedAmount;
            finished = false;
            flexbool = false;
        }
        public void End()
        {
            finished = true;
        }

        private void OnTimedEvent()
        {
            callback?.Invoke();
            finished = true;
        }

        public void UpdateTimeToNow()
        {
            timeOfEnd = Time.instance.time;
        }

        public void Update()
        {
            if (finished)
                return;

            if (timeOfEnd <= Time.instance.time)
            {
                OnTimedEvent();
            }
        }
    }
}
