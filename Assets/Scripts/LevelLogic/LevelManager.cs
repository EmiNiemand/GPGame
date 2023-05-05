using System;
using UnityEngine;

namespace LevelLogic
{
    public class LevelManager : MonoBehaviour
    {
        private GameUI gameUI;
        private Timer timer;
    
        // Start is called before the first frame update
        void Start()
        {
            gameUI = FindObjectOfType<GameUI>();
            timer = new Timer();
            timer.Activate();
        }

        private void FixedUpdate()
        {
            if(timer.UpdateTime(Time.fixedDeltaTime))
                gameUI.UpdateTimer(timer.ToString());
        }
    }
}
