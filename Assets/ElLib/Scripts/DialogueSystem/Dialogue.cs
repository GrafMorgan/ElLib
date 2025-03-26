
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ElLib.DialogueSystem
{
    public class Dialogue
    {
        public string Id { get; set; }
        public bool IsReaden { get; set; }
        public bool IsManyTimesReaden { get; set; }
        public bool IsGameplayLocked { get; set; }
        public List<Remark> Remarks { get; set; }

        public Dialogue(List<Remark> remarks)
        {
            Remarks = new List<Remark>();
            Remarks.AddRange(remarks);
        }
        public void SetData(Dialogue refer)
        {
            Id = refer.Id;
            IsReaden = refer.IsReaden;
            IsManyTimesReaden = refer.IsManyTimesReaden;
            IsGameplayLocked = refer.IsGameplayLocked;

            Remarks = new List<Remark>();

            Remarks.AddRange(refer.Remarks);
        }
    }
}