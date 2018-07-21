using System.Collections.Generic;
using UnityEngine;

public abstract class EHSEditor : MonoBehaviour {
    [HideInInspector] public List<string> msg = new List<string>();
    [HideInInspector] public bool methodRasied = false;
    [HideInInspector] public int eventsInvoked = 0;

    public delegate void RepaintAction();
    public event RepaintAction WantRepaint;
    
    protected void Repaint() {
        if (WantRepaint != null) {
            WantRepaint();
        }
    }

    private void OnApplicationQuit() {
        msg.Clear();
        __eventSystem.Clear();
    }
}