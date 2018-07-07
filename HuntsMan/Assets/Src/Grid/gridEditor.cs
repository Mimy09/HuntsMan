using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridEditor : MonoBehaviour {
    public bool active;
    public int size;
    public int gridSize;

    private void OnDrawGizmos() {
        if (!active) return;
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        for (int x = -size * gridSize; x < size * gridSize; x += gridSize) {
            Gizmos.DrawLine(new Vector3(x, 0, -size * gridSize), new Vector3(x, 0, size * gridSize));
        }

        for (int y = -size * gridSize; y < size * gridSize; y += gridSize) {
            Gizmos.DrawLine(new Vector3(-size * gridSize, 0, y), new Vector3(size * gridSize, 0, y));
        }
    }
}
