using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour {
    public int GridSize;
    public static int m_gridSize;
    public static GameObject grid;

    private void Awake() {
        grid = Resources.Load(Helper.Resource.Grid_Prefab) as GameObject;
        m_gridSize = GridSize;
    }

    public static bool CheckGrid(Vector3 Position) {
        if (Physics.OverlapBox(
            new Vector3(Position.x - 0.05f, 0.2f, Position.z - 0.05f),
            new Vector3(0.9f, 0.1f, 0.9f),
            Quaternion.identity,
            ~(1 << LayerMask.NameToLayer("Grid"))
            ).Length == 0) {
            
            return true;
        } return false;
    }

    public static GameObject CreateGrid(int actionPoints, Vector3 Position) {
        GameObject gridParant = new GameObject("Grid");

        Vector3 temp = new Vector3();
        for (int i = m_gridSize; i < actionPoints * m_gridSize; i+= m_gridSize) {
            if (CheckGrid(temp = new Vector3(i, 0.1f, 0) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i;
            if (CheckGrid(temp = new Vector3(-i, 0.1f, 0) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i;

            if (CheckGrid(temp = new Vector3(0, 0.1f, i) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i;
            if (CheckGrid(temp = new Vector3(0, 0.1f, -i) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i;

            for (int j = m_gridSize; j < (actionPoints * m_gridSize) - i; j+= m_gridSize) {
                if (CheckGrid(temp = new Vector3(i, 0.1f, j) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i + j;
                if (CheckGrid(temp = new Vector3(i, 0.1f, -j) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i + j;

                if (CheckGrid(temp = new Vector3(-i, 0.1f, j) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i + j;
                if (CheckGrid(temp = new Vector3(-i, 0.1f, -j) + Position)) Instantiate(grid, temp, Quaternion.identity, gridParant.transform).GetComponent<GridID>().ID = i + j;
            }
        }
        return gridParant;
    }
}
