using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BackgroundNpcManager : MonoBehaviour
{
    public int maxNpcs = 30;
    private int previousMaxNpcs;
    public List<GameObject> npcPrefabs;
    private List<GameObject> backgroundNpcs = new List<GameObject>();

    private List<GameObject> ToBeDeleted = new List<GameObject>();
    public Transform spawnRegion;

    public Transform fairRegion;


    private void DestroyNpc(GameObject npc, bool fade)
    {

        ToBeDeleted.Add(npc);

    }

    // Update is called once per frame
    void Update()
    {
        //remove all in to be deleted then clear the list
        for (int i = 0; i < ToBeDeleted.Count; i++)
        {
            GameObject npc = ToBeDeleted[i];
            if (npc != null)
            {
                Destroy(npc);
                backgroundNpcs.Remove(npc);
            }
        }
        
        //update npcs on change of max
        if (previousMaxNpcs != maxNpcs)
        {
            SpawnNpcs();
            previousMaxNpcs = maxNpcs;
        }
    }

    private void changeAllNpcRegion(Transform Region)
    {
        foreach (GameObject go in backgroundNpcs)
        {
            BackgroundNpc bgnpc = go.GetComponent<BackgroundNpc>();
            bgnpc.wanderRegion = Region;
        }
    }
    private void SpawnNpcs()
    {
        ToBeDeleted.Clear();
        foreach (GameObject gameObject in backgroundNpcs)
        {
            DestroyNpc(gameObject, true);
        }
        for (int i = 0; i < (int)(maxNpcs); i++)
        {
            backgroundNpcs.Add(createNPC());
        } 
    }
    private GameObject createNPC()
    {
        //using the scale of the image here to determine spawning range
        //enable mesh renderer to see the range
        float radius = transform.localScale.x;
        Vector3 randomPos = RandomNavSphere(spawnRegion.position, spawnRegion.lossyScale.x, -1);
        //for some reason its infitity sometimes
        if (randomPos.Equals(Vector3.positiveInfinity))
        {
            randomPos = Vector3.zero;
        }
        int rand = Random.Range(0, npcPrefabs.Count);
        GameObject npc = Instantiate(npcPrefabs[rand], randomPos, npcPrefabs[rand].transform.rotation, transform);
        return npc;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

}
