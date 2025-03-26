using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElLib.Sound
{
    public class SoundEffect : MonoBehaviour
    {
        AudioSource source;
        bool m_IsStopable;
        [SerializeField] bool m_IsInterface = false;
        public bool IsPlayinf => source.isPlaying;
        public void Initialize(AudioClip clip, bool isUI = false, bool isStopable = true)
        {
            m_IsInterface = isUI;
            gameObject.SetActive(true);
            source = GetComponent<AudioSource>();
            source.clip = clip;
            source.Play();

            source.volume = .4f;

            m_IsStopable = isStopable;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_IsStopable)
            {
                if (!source.isPlaying)
                {
                    gameObject.SetActive(false);
                }
            }

        }
    }
}