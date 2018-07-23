using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : Unit {
    public override void Move(Vector3 position, int actionPoints) {
        base.Move(position, actionPoints);

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = true;

        navMeshAgent.SetDestination(position);

        StartCoroutine(ReControll(position));

        ClearGrid();
    }

    IEnumerator ReControll(Vector3 position) {
        
        yield return new WaitForSeconds(1);

        navMeshAgent.Warp(this.transform.position);
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;

        animator.SetBool("Moving", true);

        navMeshAgent.SetDestination(position);
        yield return new WaitUntil(() => navMeshAgent.path != null);

        path = navMeshAgent.path;
    }

    public override void Update() {
        base.Update();

        if (navMeshAgent.destination == this.transform.position) {
            if (selected && createGrid == true) {
                ClearGrid();

                gridInfo = GridGen.GenPoints(actionPoints, actionPoints, this.transform.position, navMeshAgent);

                if (gridInfo.gridPoints != null || gridInfo.gridPoints.Count > 0)
                    grid = GridGen.CreateGrid(gridInfo, this.transform.position, actionPoints);

                createGrid = false;
            }
            path = null;
            navMeshAgent.ResetPath();
            lineRenderer.positionCount = 0;
            canMove = true;
            Realign();

            animator.SetBool("Moving", false);
        }

        if (path == null) return;
        lineRenderer.positionCount = path.corners.Length;

        for (int i = 0; i < path.corners.Length; i++) {
            lineRenderer.SetPosition(i, path.corners[i]);
        }

        
    }

    public override void Start() {
        base.Start();

        if (weapon != "") equiptedWeapon = ItemDataBase.FindWeapon(weapon);

        if (ability.Count != 0) {
            for (int i = 0; i < ability.Count; i++) {
                abilities.Add(ItemDataBase.FindAbility(ability[i]));
                abilityCooldown.Add(abilities[abilities.Count - 1].cooldown);
            }
        }
    }

    //void OnDrawGizmosSelected() {
    //    if (Application.isPlaying) {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position, equiptedWeapon.range);

    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(transform.position, abilities[0].range);
    //    }
    //}
}
