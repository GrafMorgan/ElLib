using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ElLib.DialogueSystem
{
    [Serializable]
    public class SequencePart
    {
        public UnityEvent Action = new UnityEvent();
        public UnityEvent OnPartEnd = new UnityEvent();
        public float DelayAfterPart;
    }
    [Serializable]
    public class DialoguePart : SequencePart
    {
        public UnityEvent ActionAfterLastSymbol = new UnityEvent();
        public List<Remark> Remarks;
        public float DelayAfterSymbol;
        public float DelayAfterSentance;
    }
    public class DialogueSequence
    {

        public List<SequencePart> Parts;

        public DialogueSequence()
        {
            Parts = new List<SequencePart>();
        }
        public void AppendDialogue(List<Remark> remarks, float delayAfterDialogue = 1, float delayAfterSymbol = .05f, float delayAfterSentance = 1)
        {
            var n = new DialoguePart();
            n.Remarks = new List<Remark>();
            n.Remarks.AddRange(remarks);
            n.DelayAfterPart = delayAfterDialogue;
            n.DelayAfterSymbol = delayAfterSymbol;
            n.DelayAfterSentance = delayAfterSentance;
            Parts.Add(n);
        }
        public void AppendDialogue(List<Remark> remarks, UnityAction actionAfterLastSymbol, float delayAfterDialogue = 1, float delayAfterSymbol = .1f, float delayAfterSentance = 1)
        {
            AppendDialogue(remarks, delayAfterDialogue, delayAfterSymbol, delayAfterSentance);
            (Parts[Parts.Count - 1] as DialoguePart).ActionAfterLastSymbol.AddListener(actionAfterLastSymbol);
        }
        public void AppendAction(UnityAction action, float delay)
        {
            var n = new SequencePart();
            n.DelayAfterPart = delay;
            n.Action.AddListener(action);
            Parts.Add(n);
        }
        public void AppendDialogue(List<Remark> remarks, UnityEvent actionAfterLastSymbol, float delayAfterDialogue = 1, float delayAfterSymbol = .1f, float delayAfterSentance = 1)
        {
            AppendDialogue(remarks, delayAfterDialogue, delayAfterSymbol, delayAfterSentance);
            (Parts[Parts.Count - 1] as DialoguePart).ActionAfterLastSymbol = (actionAfterLastSymbol);
        }
        public void AppendAction(UnityEvent action, float delay)
        {
            var n = new SequencePart();
            n.DelayAfterPart = delay;
            n.Action = (action);
            Parts.Add(n);
        }
        public void AppendInterval(float delay)
        {
            var n = new SequencePart();
            n.DelayAfterPart = delay;
            Parts.Add(n);
        }
    }
}