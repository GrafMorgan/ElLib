using UnityEngine;

namespace ElLib.Pool
{
    public class PoolObject : MonoBehaviour
    {
        Transform m_Parent;
        public void Init()
        {
            m_Parent = transform.parent;
        }
        public void ReturnToPool()
        {
            transform.parent = m_Parent;
            gameObject.SetActive(false);
        }
    }
}