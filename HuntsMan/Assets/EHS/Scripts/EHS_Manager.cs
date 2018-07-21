using System.Collections.Generic;
using UnityEngine;

public class EHS_Manager : MonoBehaviour {
    public static int MESSAGE_COUNT = 20;

    private static string[] m_messages = new string[MESSAGE_COUNT];
    public static string[] Messages {
        get { return m_messages; }
    }

    private static EHS_Manager s_instance;
    private static EHS_Manager Instance {
        get {
            if (s_instance == null) {
                return new GameObject("[EHS] Manager").AddComponent<EHS_Manager>();
            } else { return s_instance; }
        }
    }

    private static bool s_everInialized;
    public static bool EverInialized {
        get { return s_everInialized; }
    }

    private static bool m_initialized;
    public static bool Initialized {
        get { return m_initialized; }
    }

    private void Awake () {
        if (s_instance != null) {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        if (s_everInialized) throw new System.Exception("Tried to initialize twice in one session");
        DontDestroyOnLoad(gameObject);
        
        int handle = __event<__eType>.Raise(this, HandleEvents);
        if (handle == RESULTS.FAILURE) {
            LogMessage("[ERROR] Could no initialize manager!");
            throw new System.Exception("Failed to create a raise a message loop");
        }

        m_initialized = true;
        s_everInialized = true;

#if UNITY_EDITOR
        LogMessage("[INITALIZING]");
#endif
    }

    private void Start () {
        __event<__eType>.InvokeEvent(this, __eType._INIT_, true);
    }

    private void OnApplicationQuit () {
        __event<__eType>.InvokeEvent(this, __eType._CLOSE_);
    }
    
    public delegate void RepaintAction ();
    public static event RepaintAction WantRepaint;

    private static void Repaint () {
        if (WantRepaint != null) {
            WantRepaint();
        }
    }

#if UNITY_EDITOR
    public static void LogMessage (string message) {
        int index = 0;
        for (; index < MESSAGE_COUNT; index++) {
            if (m_messages[index] == null) {
                m_messages[index] = message;
                break;
            }
        }
        if (index == MESSAGE_COUNT) {
            for (int i = 0; i < MESSAGE_COUNT - 1; i++) {
                m_messages[i] = m_messages[i + 1];
            }
            m_messages[MESSAGE_COUNT - 1] = message;
        }
        Repaint();
    }
    public static void ClearMessages () {
        m_messages = new string[MESSAGE_COUNT];
        Repaint();
    }
#else
    public static void LogMessage (string message) { }
    public static void ClearMessages () { }
#endif

    private void HandleEvents(object s, __eArg<__eType> e) {
        switch (e.arg) {
        case __eType._CLOSE_:
#if UNITY_EDITOR
            LogMessage("[CLOSING]");
#endif
            __event<__eType>.UnsubscribeAll();
            __event<__eType>.ConsumeAll();
            break;
        case __eType._ERROR_:
#if UNITY_EDITOR
            LogMessage("[ERROR MSG='"+e.value+"', FROM=" + (s as MonoBehaviour).name+"]");
#endif
            __event<__eType>.UnsubscribeAll();
            __event<__eType>.ConsumeAll();
            break;
        }
    }
}