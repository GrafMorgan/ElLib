using DG.Tweening;
using ElLib.Sound;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ElLib;

namespace ElLib.DialogueSystem
{
    public class WindowDialogueController : MonoBehaviour
    {
        protected List<ProfileData> m_Characters;

        [SerializeField] GameObject m_CharacterHolder;
        [SerializeField] GameObject m_PointsHolder;

        [SerializeField] Image m_ImagePrefab;
        List<Image> m_Images = new List<Image>();
        [SerializeField] RectTransform m_PointPrefab;
        List<RectTransform> m_Points = new List<RectTransform>();

        public UnityEvent<Remark> OnStartSentance = new UnityEvent<Remark>();
        public UnityEvent OnEndSentance = new UnityEvent();

        [SerializeField] GameObject m_Panel;
        [SerializeField] private Image m_Image;
        [SerializeField] private TMP_Text m_DialogueText;
        [SerializeField] private TMP_Text m_NameText;
        [SerializeField] private TMP_Text m_ContinueText;

        public static System.Action<DialogueSequence, float, bool, bool, bool> PlayDialogue;

        private Dialogue m_Dialogue;
        private Timer m_Timer;

        int m_SentanceId;
        string m_CurrentSentance;

        private DialogueSequence m_CurrentSequence;

        private float m_SymbolDelay;
        private float m_SentanceDelay;

        Timer m_TimerOf_;
        bool m_IsNeed_ = true;
        private bool m_IsWriting;

        SequencePart m_CurrentPart;

        DG.Tweening.Sequence m_SaySeq;
        public void Init()
        {
            PlayDialogue += PlaySequence;
            m_Characters = new List<ProfileData>();
            m_Characters.AddRange(Resources.LoadAll<ProfileData>(""));
            Debug.Log(m_Characters.Count);
        }
        public void WriteDialogue(Dialogue d)
        {
            m_Dialogue = d;
            m_SentanceId = 0;

            WriteSentance();
        }
        public void WriteSentance()
        {
            if (m_SentanceId > 0) if (m_Dialogue.Remarks[m_SentanceId - 1].HideAfterSentance.Count > 0)
                {
                    Debug.Log("c " + m_Dialogue.Remarks[m_SentanceId - 1].HideAfterSentance.Count);

                    foreach (var s in m_Dialogue.Remarks[m_SentanceId - 1].HideAfterSentance)
                        DestroyCharacter(s);
                }
            if (m_Dialogue.Remarks[m_SentanceId].ShowBeforeSentance.Count > 0)
            {
                m_Timer = null;
                foreach (var id in m_Dialogue.Remarks[m_SentanceId].ShowBeforeSentance)
                    InitNewCharacter(id);
                DG.Tweening.Sequence s = DOTween.Sequence();
                s.InsertCallback(.5F, () =>
                {
                    CharacterSaid(m_Dialogue.Remarks[m_SentanceId].ProfileId);
                    m_IsWriting = true;
                    m_CurrentSentance = m_Dialogue.Remarks[m_SentanceId].GetRemark();

                    OnStartSentance?.Invoke(m_Dialogue.Remarks[m_SentanceId]);

                    if (m_NameText != null)
                    {
                        m_NameText.transform.parent.gameObject.SetActive(!(m_Dialogue.Remarks[m_SentanceId].ProfileId == "none" || m_Dialogue.Remarks[m_SentanceId].ProfileId == ""));
                        m_NameText.text = m_Characters.Find(x => x.Id == m_Dialogue.Remarks[m_SentanceId].ProfileId).Name;
                        m_NameText.color = m_Characters.Find(x => x.Id == m_Dialogue.Remarks[m_SentanceId].ProfileId).NameColor;
                    }
                    m_SentanceId++;


                    m_DialogueText.text = "";

                    WriteSymbol();
                });
                s.SetUpdate(true);
            }
            else
            {
                CharacterSaid(m_Dialogue.Remarks[m_SentanceId].ProfileId);
                m_IsWriting = true;
                m_CurrentSentance = m_Dialogue.Remarks[m_SentanceId].GetRemark();

                OnStartSentance?.Invoke(m_Dialogue.Remarks[m_SentanceId]);

                if (m_NameText != null)
                {
                    m_NameText.transform.parent.gameObject.SetActive(!(m_Dialogue.Remarks[m_SentanceId].ProfileId == "none" || m_Dialogue.Remarks[m_SentanceId].ProfileId == ""));
                    m_NameText.text = m_Characters.Find(x => x.Id == m_Dialogue.Remarks[m_SentanceId].ProfileId).Name;
                    m_NameText.color = m_Characters.Find(x => x.Id == m_Dialogue.Remarks[m_SentanceId].ProfileId).NameColor;
                }
                m_SentanceId++;


                m_DialogueText.text = "";

                WriteSymbol();
            }
        }
        public void WriteSymbol(bool isSound = true)
        {
            m_Timer = null;

            m_DialogueText.text += m_CurrentSentance[0];
            m_CurrentSentance = m_CurrentSentance.Remove(0, 1);

            if (isSound)
                SoundManager.Instance.PlayEffect(SoundType.UI, "textScrollFast", Camera.main.transform.position);
            if (m_CurrentSentance.Length > 0)
            {
                m_Timer = new Timer((m_CurrentPart as DialoguePart).DelayAfterSymbol);
                m_Timer.OnTimesUp.AddListener(() => { WriteSymbol(); });
            }
            else
            {
                m_SaySeq?.Kill();
                OnEndSentance?.Invoke();
                m_IsWriting = false;

                if (m_SentanceId >= m_Dialogue.Remarks.Count)
                {
                    (m_CurrentPart as DialoguePart).ActionAfterLastSymbol.Invoke();
                }
            }
        }
        public void WriteAll()
        {
            while (m_IsWriting)
            {
                WriteSymbol(false);
            }
        }
        public void BakeSequence(DialogueSequence sequence, bool isUnFreeze = true)
        {
            foreach (var i in m_Images)
                Destroy(i.gameObject);
            foreach (var p in m_Points)
                Destroy(p.gameObject);
            foreach (Transform p in m_PointsHolder.transform)
                Destroy(p.gameObject);
            m_Images.Clear();
            m_Points.Clear();

            m_DialogueText.text = "";
            m_NameText.text = "";
            foreach (var s in sequence.Parts)
            {
                if (s.GetType() == typeof(DialoguePart))
                {
                    (s as DialoguePart).Action.AddListener(() =>
                    {
                        EnableDialogPanel();
                        m_CurrentPart = s;
                        SetDialogue((s as DialoguePart).Remarks);
                    });

                }
                else
                {
                    Debug.Log("Setting action");
                    s.Action.AddListener(() =>
                    {
                        Debug.Log("Doing     action");
                        m_Timer = new Timer(s.DelayAfterPart);
                        m_Timer.OnTimesUp.AddListener(() =>
                        {
                            Debug.Log("End action");
                            m_Timer = null;
                            s.OnPartEnd?.Invoke();
                        });
                    });
                }
            }
            m_CurrentPart = sequence.Parts[0];

            for (int i = 0; i < sequence.Parts.Count - 1; i++)
            {

                sequence.Parts[i].OnPartEnd.AddListener(() =>
                {
                    int nextId = m_CurrentSequence.Parts.FindIndex(x => x == m_CurrentPart) + 1;
                    //Debug.Log(i + " => TryToNext: " + (i+1));
                    m_SentanceId = 0;
                    m_CurrentPart = m_CurrentSequence.Parts[nextId];
                    m_CurrentSequence.Parts[nextId].Action.Invoke();
                });
            }

            sequence.Parts[sequence.Parts.Count - 1].OnPartEnd.AddListener(() => { EndSequence(isUnFreeze); });

        }
        void EndSequence(bool isUnFreeze)
        {
            DisableDialogPanel();
            Time.timeScale = 1;
            m_CurrentSequence.Parts.Clear();
            m_Timer = null;
        }
        public void PlaySequence(DialogueSequence sequence, float backAlpha = 0, bool isTimeStop = true, bool isControlled = false, bool isUnFreeze = true)
        {
            GetComponent<Image>().DOFade(backAlpha, .5f).SetUpdate(true);
            m_SentanceId = 0;
            EnableDialogPanel();
            //Debug.Log("Play Sequence");

            m_CurrentSequence = sequence;

            BakeSequence(m_CurrentSequence, isUnFreeze);

            m_CurrentPart.Action.Invoke();

            if (isTimeStop) Time.timeScale = 0;

        }
        void EnableDialogPanel()
        {
            if (m_Panel != null)
                m_Panel?.SetActive(true);
        }
        void DisableDialogPanel()
        {
            if (m_Panel != null)
                m_Panel?.SetActive(false);
        }

        private void SetDialogue(List<Remark> remarks)
        {

            m_Dialogue = new Dialogue(remarks);
            WriteSentance();
        }
        public void StringPartSwitch(string from, string to, bool isNeedAfterSwitch = true)
        {
            var d = new Dialogue(m_Dialogue.Remarks);
            d.SetData(m_Dialogue);
            var q = m_Dialogue;
            m_Dialogue = d;
            foreach (Remark s in m_Dialogue.Remarks)
            {
                s.Russian = s.Russian.Replace(from, to);
            }

            Debug.Log("from " + from + " to " + to);

            if (isNeedAfterSwitch)
                m_CurrentSequence.Parts[m_CurrentSequence.Parts.Count - 1].OnPartEnd.AddListener(() => { StringPartSwitch(to, from, false); });

        }

        private void Update()
        {
            m_Timer?.Update(false);

            if (m_CurrentPart != null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (m_CurrentPart.GetType() == typeof(DialoguePart))
                    {
                        if (m_IsWriting)
                        {
                            WriteAll();
                        }
                        else
                        {
                            if (m_SentanceId >= m_Dialogue.Remarks.Count)
                            {
                                m_CurrentPart.OnPartEnd?.Invoke();
                            }
                            else
                            {
                                WriteSentance();
                            }
                        }
                    }
                }
            }
        }

        public void InitNewCharacter(string id)
        {
            if (!m_Images.Exists(x => x.GetComponent<CharacterDialogueController>().Id == id))
            {
                var p = Instantiate(m_PointPrefab.gameObject, m_PointsHolder.transform).transform;
                var i = Instantiate(m_ImagePrefab.gameObject, m_CharacterHolder.transform).GetComponent<Image>();

                m_Images.Add(i);
                i.GetComponent<CharacterDialogueController>().Initialize(m_Characters.Find(x => x.Id == id), p);
            }

        }
        public void CharacterSaid(string id)
        {
            foreach (var c in m_Images)
            {
                c.transform.localScale = Vector3.one;
            }
            m_SaySeq?.Kill();
            m_SaySeq = DOTween.Sequence();
            if (m_Images.Exists(x => x.GetComponent<CharacterDialogueController>().Id == id))
            {
                m_SaySeq.Append(m_Images.Find(x => x.GetComponent<CharacterDialogueController>().Id == id).
                    transform.DOPunchScale(new Vector3(.05f, .05f, .05f), .1f, 25));
                m_SaySeq.SetLoops(-1).SetUpdate(true);
            }
        }
        public void DestroyCharacter(string id)
        {
            if (m_Images.Exists(x => x.GetComponent<CharacterDialogueController>().Id == id))
            {
                m_Images.Find(x => x.GetComponent<CharacterDialogueController>().Id == id).
                    GetComponent<CharacterDialogueController>().DestroyChar();

                m_Images.Remove(m_Images.Find(x => x.GetComponent<CharacterDialogueController>().Id == id));
            }
        }
    }
}