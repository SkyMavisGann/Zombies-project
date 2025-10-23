using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//detect if an npc is nearby another, then trigger talking,
//rest is handled in BackgroundNPC
public class NpcTalkingCollider : MonoBehaviour
{
    public string tagName;
    public BackgroundNpc bnpc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagName && other != bnpc.gameObject.GetComponent<Collider>())
        {
            bnpc.BeginChasing(other);
        }
        if (other.tag == "Stairs")
        {
            bnpc.IsOnStairs = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Stairs")
        {
            bnpc.IsOnStairs = false;
        }
    }
}
