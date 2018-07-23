using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class Spawner : MonoBehaviour {

    public int teamID;
    public enum spawn_type {
        melee,
        ranged,
        magic,
        rand,
    };
    public spawn_type spawnType;



    private Object[] objSpawn;

    private void Start() {
        switch (teamID) {
            case 1: {
                    GameObject temp;

                    switch (spawnType) {
                        case spawn_type.melee:
                            temp = Resources.Load(Resource.Team1 + "Character_Melee") as GameObject;
                            break;
                        case spawn_type.ranged:
                            temp = Resources.Load(Resource.Team1 + "Character_Ranged") as GameObject;
                            break;
                        case spawn_type.magic:
                            temp = Resources.Load(Resource.Team1 + "Character_Magic") as GameObject;
                            break;
                        case spawn_type.rand:
                            objSpawn = Resources.LoadAll(Resource.Team1) as Object[];
                            if (objSpawn == null || objSpawn.Length == 0) return;
                            temp = objSpawn[Random.Range(0, objSpawn.Length)] as GameObject;
                            break;
                        default:
                            objSpawn = Resources.LoadAll(Resource.Team1) as Object[];
                            if (objSpawn == null || objSpawn.Length == 0) return;
                            temp = objSpawn[Random.Range(0, objSpawn.Length)] as GameObject;
                            break;
                    }

                    GameObject character = Instantiate(temp, transform.position, transform.rotation);
                    character.GetComponent<Character>().SetTeam(teamID);

                    Manager.instance.AddToTeam(character, teamID);
                }
                break;
            case 2: {
                    objSpawn = Resources.LoadAll(Resource.Team2) as Object[];
                    if (objSpawn == null || objSpawn.Length == 0) return;

                    GameObject temp = objSpawn[Random.Range(0, objSpawn.Length)] as GameObject;
                    GameObject character = Instantiate(temp, transform.position, transform.rotation);
                    character.GetComponent<Character>().SetTeam(teamID);

                    Manager.instance.AddToTeam(character, teamID);
                }
                break;
        }
    }

    private void OnDrawGizmos() {
        switch (teamID) {
            case 1:
                Gizmos.color = Color.blue;
                break;

            case 2:
                Gizmos.color = Color.red;
                break;
        }
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
