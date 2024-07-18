using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collider : MonoBehaviour
{
    public bool isTalking = false;
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        //print("Ʈ���ſ� ����");
        //if(other.gameObject.GetComponent<Collider>().isTalking == true)
        //{
        //    return;
        //}

        if (other.gameObject.CompareTag("Npc") && other.gameObject.name != gameObject.name && other.gameObject.GetComponent<Collider>().isTalking == false && isTalking == false)
        {
            // ���� ���� ���̸�?
            // ���� ��ȭ���� ���·� �����ϰ�
            // �ȱ⸦ ���߸�, ���θ� �ٶ󺻴�.
            isTalking = true;
            other.gameObject.GetComponent<Collider>().isTalking = true;

            gameObject.GetComponent<NPC>().isWalking = false;
            other.gameObject.GetComponent<NPC>().isWalking = false;

            gameObject.transform.LookAt(other.transform.position);
            other.transform.LookAt(gameObject.transform.position);


            GameObject go = other.gameObject;

            // �׸��� 10�� �� �ٽ� �����δ�.
            StartCoroutine("CoWaitASec", go);
        }


    }

    IEnumerator CoWaitASec(GameObject go)
    {
        yield return new WaitForSeconds(10f);

        gameObject.GetComponent<NPC>().isWalking = true;
        isTalking = false;

        yield return new WaitForSeconds(3f);

        go.GetComponent<Collider>().isTalking = false;
        go.GetComponent<NPC>().isWalking = true;


    }
}