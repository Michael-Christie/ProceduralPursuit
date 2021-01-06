using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    int MaxMonsterCap = 10;
    int MonstersLeft;
    List<Vector3> possibleSpawnPos = new List<Vector3>();
    public GameObject[] monster;
    bool hasSpawned = false;
    public Transform player;

    private void Update()
    {
        if (player == null)
            player = GameObject.Find("Player")?.transform;
        else
        {
            if (CheckPlayerPosition() && !hasSpawned)
            {
                hasSpawned = true;

                //spawn monsters
                SpawnMonsters();
            }
        }
    }

    bool CheckPlayerPosition()
    {
        if (Mathf.Abs((player.position - transform.position).magnitude) < 100f)
            return true;
        return false;
    }

    public void AddSpawnLocation(Vector3 t)
    {
        possibleSpawnPos.Add(t);
    }

    public void AddMonsters(GameObject[] mons) => monster = mons;

    public void SpawnMonsters()
    {
        for(int i = 0; i < MaxMonsterCap; i++)
        {
            Vector3 pos = possibleSpawnPos[RandomNumber.Range(0, possibleSpawnPos.Count)];
            //Insantiate a monster 
            GameObject mon = Instantiate(monster[0], pos, Quaternion.identity, transform);

            //subscribe the monster to this monsterKill function
            Monster m = mon.AddComponent<Monster>();
            m.Initialize(transform.GetComponent<MonsterSpawner>());

            MonstersLeft++;

            Debug.Log("Monster Spawned");
        }

    }

    public void MonsterKilled()
    {
        MonstersLeft--;

        if(MonstersLeft <= 0)
        {
            //run it through the quest system
            FindObjectOfType<QuestGenerator>().g.EntityKilled(this.gameObject);
        }
    }
}
