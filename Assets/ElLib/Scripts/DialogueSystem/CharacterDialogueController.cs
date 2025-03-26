using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ElLib.DialogueSystem
{
    public class CharacterDialogueController : MonoBehaviour
    {
        ProfileData m_Data;
        public string Id => m_Data.Id;

        Transform m_Point;

        public void Initialize(ProfileData data, Transform point)
        {
            m_Point = point;
            m_Data = data;
            GetComponent<Image>().sprite = data.Sprite;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Point != null)
                transform.position = Vector3.Lerp(transform.position, m_Point.position, .03f);
        }
        public void DestroyChar()
        {
            GetComponent<Image>().DOFade(0, .5f).SetUpdate(true).onComplete = () =>
            {
                Destroy(m_Point.gameObject);
                Destroy(gameObject);
            };
        }
    }
}