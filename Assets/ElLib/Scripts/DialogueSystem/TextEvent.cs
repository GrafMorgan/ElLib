using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ElLib.DialogueSystem
{
    public class TextEvent : MonoBehaviour
    {
        protected List<Sequence> sequences;
        protected Transform talkingMob;


        protected void LookAt(Transform obj, Vector3 position, float time, Action actionOnEnd = null)
        {
            Vector3 startrot = obj.eulerAngles;
            obj.LookAt(position);
            Vector3 targetrot = obj.eulerAngles;
            obj.eulerAngles = startrot;


            Sequence s = DOTween.Sequence();
            s.Append(obj.DORotate(targetrot, time));
            if (actionOnEnd != null)
                s.AppendCallback(() => { actionOnEnd(); });
        }

        protected void MoveAt(Transform obj, Vector3 position, float time, Vector3 positionLookAtEnd, Action actionOnEnd = null)
        {
            LookAt(obj, position, time / 15f,
            () =>
            {
                Sequence s = DOTween.Sequence();
                s.Append(obj.DOMove(position, (time / 15f) * 13f));
                s.AppendCallback(() => { LookAt(obj, positionLookAtEnd, time / 15f, actionOnEnd); });
            });
        }

        protected void StopAll()
        {
            foreach (var s in sequences)
            {
                s.Kill();
            }

            sequences = new List<Sequence>();

            talkingMob = null;
        }
        protected void InitTalkAnim()
        {
            if (talkingMob != null)
            {
                Sequence S = DOTween.Sequence();
                S.Append(talkingMob.transform.DOScaleY(1.4f, .14f));
                S.Append(talkingMob.transform.DOScaleY(1, .06f));
                S.SetLoops(-1);
                sequences.Add(S);
            }
        }

        protected virtual void OnNewSentance(Remark remark)
        {

        }
        public virtual bool CheckEventCondition()
        {
            return true;
        }
        public virtual void InitEvent()
        {

            TextEventManager.Instance.gameObject.SetActive(true);

            sequences = new List<Sequence>();

            TextEventManager.Instance.OnStartSentance = new UnityEngine.Events.UnityEvent<Remark>();
            TextEventManager.Instance.OnStartSentance.AddListener(OnNewSentance);

            TextEventManager.Instance.OnEndSentance = new UnityEngine.Events.UnityEvent();
            TextEventManager.Instance.OnEndSentance.AddListener(StopAll);

        }

        public virtual void EndEvent(bool isNeedToSpawnNextArena = true)
        {
            TextEventManager.Instance.GetComponent<Image>().DOFade(0, .5f).SetUpdate(true);
            Destroy(gameObject);
            Time.timeScale = 1;
        }
    }
}