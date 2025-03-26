using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ElLib
{
    public class FpsCalculator : MonoBehaviour
    {
        [SerializeField] TMP_Text fpsText;

        Timer fpsTimer;

        int countOfFrames; 

        void Start()
        {
            fpsTimer = new Timer(.5F);
            fpsTimer.OnTimesUp.AddListener(RestartCalculator);
            countOfFrames = 0;
        }

        public void RestartCalculator()
        {
            fpsTimer.Reset();
            fpsText.text = (countOfFrames * 2).ToString();

            countOfFrames = 0;

        }

        // Update is called once per frame
        void Update()
        {
            countOfFrames++;

            fpsTimer?.Update(false);
        }
    }

}