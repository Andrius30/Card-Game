using System;
using UnityEngine;

namespace Andrius.Core.Timers
{

    public class Timer
    {
        float startTime;
        bool isDone;
        bool ignoreTimeScale;
        Action onDone;

        public Timer(float startTime, Action onDone, bool ignoreTimeScale, bool isDone = false)
        {
            this.startTime = startTime;
            this.isDone = isDone;
            this.onDone = onDone;
            this.ignoreTimeScale = ignoreTimeScale;
        }

        public void StartTimer()
        {
            if (isDone) return;
            if (startTime > 0)
            {
                if (ignoreTimeScale)
                {
                    startTime -= Time.unscaledDeltaTime;
                }
                else
                {
                    startTime -= Time.deltaTime;
                }
            }
            else
            {
                startTime = 0;
                isDone = true;
                onDone?.Invoke();
            }
        }
        public void SetTimer(float time, bool done)
        {
            this.startTime = time;
            this.isDone = done;
        }
        public bool IsDone() => isDone;
        public float GetCurrentTime() => startTime;
    }
}
