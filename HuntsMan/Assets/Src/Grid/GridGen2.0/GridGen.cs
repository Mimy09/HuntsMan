using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridGen : MonoBehaviour {

    public static int scail;
    public static GameObject grid;

    private void Start() {
        grid = Resources.Load(Helper.Resource.Grid_Prefab) as GameObject;
        scail = 2;
    }

    public struct GridInfo {
        public List<List<Vector3>> gridPoints;
        public List<List<int>> gridIDs;
    }

    public static GridInfo GenPoints(int distance, int gridSize, Vector3 position, NavMeshAgent agent) {
        GridInfo info = new GridInfo();
        info.gridPoints = new List<List<Vector3>>();
        info.gridIDs = new List<List<int>>();

        if (info.gridPoints.Count > 0) info.gridPoints.Clear();

        for (int x = -(gridSize * scail); x < (gridSize * scail) + 1; x++) {
            info.gridPoints.Add(new List<Vector3>());
            info.gridIDs.Add(new List<int>());

            for (int y = -(gridSize * scail); y < (gridSize * scail) + 1; y++) {
                if (x == 0 && y == 0) continue;

                Vector3 pos = new Vector3(x, 0, y) * scail;

                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(position, position + pos, NavMesh.AllAreas, path);
                agent.path = path;

                if (agent.pathStatus == NavMeshPathStatus.PathInvalid) continue;
                else if (agent.pathStatus == NavMeshPathStatus.PathPartial) {
                    continue;
                }

                float pathDistance = GetPathLength(path);

                if (pathDistance < (distance * scail) + (scail)  && CheckGrid(position + pos)) {
                    for (int i = 0; i < path.corners.Length - 1; i++)
                        Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.green, 2);

                    info.gridPoints[info.gridPoints.Count - 1].Add(pos);
                    info.gridIDs[info.gridIDs.Count - 1].Add(Mathf.FloorToInt(pathDistance / scail));
                }

                agent.ResetPath();
            }
        }

        return info;
    }

    public static float GetPathLength(NavMeshPath path) {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1)) {
            for (int i = 1; i < path.corners.Length; ++i) {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }

    public static bool CheckGrid(Vector3 Position) {
        if (Physics.OverlapBox(
            new Vector3(Position.x - 0.05f, 0.2f, Position.z - 0.05f),
            new Vector3(0.9f, 0.1f, 0.9f),
            Quaternion.identity,
            ~(1 << LayerMask.NameToLayer("Grid"))
            ).Length == 0) {

            return true;
        }
        return false;
    }

    public static GameObject CreateGrid(GridInfo info, Vector3 position, int AP) {
        GameObject gridParant = new GameObject("Grid");

        for (int x = 0; x < info.gridPoints.Count; x++) {
            for (int y = 0; y < info.gridPoints[x].Count; y++) {
                GameObject gridTile = Instantiate(grid, position + info.gridPoints[x][y], Quaternion.identity, gridParant.transform);
                gridTile.GetComponent<GridID>().ID = info.gridIDs[x][y];

                Material mat = gridTile.GetComponent<MeshRenderer>().material;
                mat.color = new Color((float)info.gridIDs[x][y] / (float)AP, 1 - (float)info.gridIDs[x][y] / (float)AP, 0, 1);
                gridTile.GetComponent<MeshRenderer>().material = mat;
            }
        }

        return gridParant;
    }
}
