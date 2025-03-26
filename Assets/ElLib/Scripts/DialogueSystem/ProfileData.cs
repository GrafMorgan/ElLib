using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ElLib.DialogueSystem
{
    [CreateAssetMenu(fileName = "new profile", menuName = "profile", order = 51)]
    public class ProfileData : ScriptableObject
    {
        [SerializeField] string m_Id;
        [SerializeField] string m_CharName;
        [SerializeField] Sprite m_Sprite;
        [SerializeField] Color m_NameColor;

        public string Id => m_Id;
        public string Name => m_CharName;
        public Sprite Sprite => m_Sprite;
        public Color NameColor => m_NameColor;

    }
}