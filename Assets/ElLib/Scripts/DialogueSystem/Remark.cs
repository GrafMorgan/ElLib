
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ElLib.DialogueSystem
{
    [System.Serializable]
    public class Remark
    {
        public string ProfileId;
        public string English;
        public string Russian;

        public List<string> ShowBeforeSentance;
        public List<string> HideAfterSentance;

        public string GetRemark()
        {
            return Russian;
        }
    }
}
