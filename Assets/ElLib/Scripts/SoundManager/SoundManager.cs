using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace ElLib.Sound
{
    public enum SoundType { Weapon, UI, Addictive, Enemies }

    [Serializable]
    public class SoundEffectData
    {
        public string name;
        public AudioClip clip;
    }
    [System.Serializable]
    public struct EffectPool
    {
        public SoundType type;
        public List<SoundEffectData> effects;
    }

    public class SoundManager : MonoBehaviour
    {
        [System.Serializable]
        public enum MusicState { Menu, GamePlay, ZombiRush }
        [SerializeField] GameObject m_SoundEffect;

        [SerializeField] List<EffectPool> m_EffectPools;

        List<GameObject> m_EffectObjects;

        [SerializeField] List<AudioClip> m_Music;

        public static SoundManager Instance { get; private set; }

        [SerializeField] private int m_MaxEffectCount;

        private void Awake()
        {
            Init();
        }
        public void Init()
        {
            if (Instance == null)
            {
                m_EffectObjects = new List<GameObject>();
                Instance = this;
                DontDestroyOnLoad(gameObject);
                for (int i = 0; i < m_MaxEffectCount; i++)
                {
                    m_EffectObjects.Add(Instantiate(m_SoundEffect, transform).gameObject);
                    m_EffectObjects[i].SetActive(false);
                    DontDestroyOnLoad(m_EffectObjects[i]);
                }
                InitMusic();
                PlayerPrefs.SetFloat("Music", 0);
                PlayerPrefs.SetFloat("Sounds", 0);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        void InitMusic()
        {
            GetComponent<AudioSource>().clip = m_Music[0];
            GetComponent<AudioSource>().Play();
            m_Music.Add(m_Music[0]);
            m_Music.RemoveAt(0);
        }
        public void OnValidate()
        {
            foreach (var pool in m_EffectPools)
                foreach (var effect in pool.effects)
                    if (effect.clip != null)
                        effect.name = effect.clip.name;
        }
        public void PlayEffect(SoundType usingPool, string effectName, Vector3 position, bool isUI = false)
        {
            var pool = m_EffectPools.Find(x => x.type == usingPool);

            if (pool.effects.Exists(x => x.name == effectName))
            {
                if (!m_EffectObjects.Exists(x => x.activeInHierarchy == false))
                {
                    m_EffectObjects[0].SetActive(false);
                    m_EffectObjects.Add(m_EffectObjects[0]);
                    m_EffectObjects.RemoveAt(0);
                }
                var newEffect = m_EffectObjects.Find(x => x.activeInHierarchy == false);

                newEffect.GetComponent<SoundEffect>().Initialize(pool.effects.Find(x => x.name == effectName).clip, isUI);
                newEffect.transform.position = position;
                //Debug.Log("Sound <" + effectName + "> from the pool <" + usingPool + "> is playing!");
            }
            else
            {
                Debug.LogError("Dont find sound <" + effectName + "> in the pool <" + usingPool + ">");
            }
        }
        public AudioClip GetEffect(SoundType usingPool, string effectName)
        {
            var pool = m_EffectPools.Find(x => x.type == usingPool);
            if (pool.effects.Exists(x => x.name == effectName))
            {
                return pool.effects.Find(x => x.name == effectName).clip;
            }
            return null;
        }
        private void Update()
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                InitMusic();
            }
        }
    }
}