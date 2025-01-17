using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class NpcFaceMove : MonoBehaviour
{
    [SerializeField] Transform target;
    public float speed = 1f;
    public bool talking = false;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (ChatManager.instance.talk == true && talking == true)
        {
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0;

            Quaternion rot = Quaternion.LookRotation(dir.normalized);

            transform.rotation = rot;
        }
    }
}
