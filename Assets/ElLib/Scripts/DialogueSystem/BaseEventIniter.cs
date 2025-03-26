using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ElLib.DialogueSystem
{
    public class BaseEventIniter : TextEvent
    {
        [SerializeField] bool m_IsAutoStart = true;

        [SerializeField] List<Remark> m_Remarks;
        private void Start()
        {
            if (m_IsAutoStart)
                InitEvent();
        }
        public override void InitEvent()
        {
            Time.timeScale = 0;
            base.InitEvent();

            string name = System.Environment.UserName;

            DialogueSequence sequence = new DialogueSequence();

            sequence.AppendDialogue(m_Remarks);
            sequence.AppendAction(() =>
            {
                EndEvent();
                TextEventManager.Instance.gameObject.SetActive(false);
            }, 0);

            TextEventManager.Instance.PlaySequence(sequence, .9f, false, false, false);
        }
    }
}