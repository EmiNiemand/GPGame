using UnityEngine;

namespace LevelLogic
{
    public class Timer
    {
        private float currentTime;
        private bool active = false;
        
        public bool UpdateTime(float value)
        {
            if (!active) return false;

            currentTime += value;
            return true;
        }

        public void Activate() { currentTime=0; active=true; }
        public void Pause() { active = false; }
        public void Resume() { active = true; }

        public override string ToString()
        {
            string AddZero(int value) => value<10? "0"+value:value.ToString();
            return AddZero((int)currentTime / 60) + ":" + AddZero((int)currentTime % 60);
        }
    }
}
