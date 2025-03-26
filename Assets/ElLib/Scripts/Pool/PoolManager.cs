using System.Collections.Generic;
using UnityEngine;


namespace ElLib.Pool
{
    [System.Serializable]
    class PoolPart
    {
        public string Name;
        public PoolObject Prefab;
        public PoolBehaviour<PoolObject> Pool;
        public int Count;

        public void Validate()
        {
            if (Prefab != null)
                Name = Prefab.name;
        }
        public void InitPool(Transform parent)
        {
            GameObject Parent = new GameObject(Name + "Pool");
            Parent.transform.parent = parent;
            Pool = new PoolBehaviour<PoolObject>();
            Pool.CreateParent(Parent.transform);
            Pool.Pooling(Prefab, Count);
        }
    }

    public class PoolManager : MonoBehaviour
    {
        [SerializeField] List<PoolPart> m_Pools;

        static List<PoolPart> Pools;

        private void Awake()
        {
            Init();
        }
        public void Init()
        {
            GameObject gParent = new GameObject("Pools");
            Pools = m_Pools;
            foreach (var p in Pools) p.InitPool(gParent.transform);
        }
        public static GameObject GetObject(string name)
        {
            foreach (var p in Pools)
            {
                if (p.Name == name)
                    return p.Pool.Take().gameObject;
            }
            Debug.LogError("Doesn't Find object with name" + name + " in the pool!");
            return null;
        }
        private void OnValidate()
        {
            foreach (var p in m_Pools) { p.Validate(); }
        }
    }
}