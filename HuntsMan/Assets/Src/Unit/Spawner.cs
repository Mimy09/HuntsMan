using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class Spawner : MonoBehaviour {

    public int teamID;

    private Object[] objSpawn;

    private void Start() {
        switch (teamID) {
            case 1: {
                    objSpawn = Resources.LoadAll(Resource.Team1) as Object[];
                    if (objSpawn == null || objSpawn.Length == 0) return;

                    GameObject temp = objSpawn[Random.Range(0, objSpawn.Length)] as GameObject;
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
