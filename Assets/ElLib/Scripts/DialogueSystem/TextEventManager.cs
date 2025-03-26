using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ElLib.DialogueSystem
{
    public class TextEventManager : WindowDialogueController
    {
        public static TextEventManager Instance;

        TextEvent Current;

        [SerializeField] GameObject m_TextBox;
        private void Start()
        {
            Init();
        }
        public void Init()
        {
            base.Init();
            Instance = this;
        }
        public static void InitEvent()
        {

            Instance.Current.InitEvent();
        }

        public void SetActivePanel(bool panelState)
        {
            m_TextBox.SetActive(panelState);
        }
    }
}