using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElLib.Pool;
using ElLib.Sound;
using ElLib.DialogueSystem;

public class DemoEntryPoint : MonoBehaviour
{
    [SerializeField] TextEventManager m_TextEventManager;
    [SerializeField] List<PoolObject> m_Spheres;

    void Awake()
    {
        m_TextEventManager.Init();
    }

    private void SpawnSphere()
    {
        var newObj = PoolManager.GetObject("Sphere");
        m_Spheres.Add(newObj.GetComponent<PoolObject>());
        newObj.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        SoundManager.Instance.PlayEffect(SoundType.Addictive, "spawn",newObj.transform.position);
    }

    private void RemoveSphere()
    {
        if(m_Spheres.Count > 0)
        {
            int id = Random.Range(0, m_Spheres.Count);
            SoundManager.Instance.PlayEffect(SoundType.Addictive, "delete", m_Spheres[id].transform.position);
            m_Spheres[id].ReturnToPool();
            m_Spheres.RemoveAt(id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) SpawnSphere();
        if(Input.GetMouseButtonDown(1)) RemoveSphere();

        if(Input.mouseScrollDelta.y>0) for(int i = 0; i< 5; i++) SpawnSphere();
        if(Input.mouseScrollDelta.y<0) for (int i = 0; i < 5; i++) RemoveSphere();

    }
}
