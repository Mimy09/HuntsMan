﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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

        navMeshAgent.SetDestination(position);
        yield return new WaitUntil(() => navMeshAgent.path != null);

        path = navMeshAgent.path;
    }

    public override void Update() {
        base.Update();

        

        if (navMeshAgent.destination == this.transform.position) {
            if (selected && createGrid == true) {
                ClearGrid();
                grid = GenerateGrid.CreateGrid(actionPoints, this.transform.position);
                createGrid = false;
            }
            path = null;
            navMeshAgent.ResetPath();
            lineRenderer.positionCount = 0;
            canMove = true;
            Realign();
        }

        if (isDead()) {
            Destroy(this.gameObject);
        }

        if (path == null) return;
        lineRenderer.positionCount = path.corners.Length;

        for (int i = 0; i < path.corners.Length; i++) {
            lineRenderer.SetPosition(i, path.corners[i]);
        }

        
    }

    public override void Start() {
        base.Start();

        if (weapon != "")
            equiptedWeapon = ItemDataBase.FindWeapon(weapon);
        else
            equiptedWeapon = ItemDataBase.FindWeapon("Weapon_test_1");

        if (ability.Count != 0) {
            for (int i = 0; i < ability.Count; i++) {
                abilities.Add(ItemDataBase.FindAbility(ability[i]));
            }
        } else {
            abilities.Add(ItemDataBase.FindAbility("Ability_test_1"));
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, equiptedWeapon.range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, abilities[0].range);

    }
}
