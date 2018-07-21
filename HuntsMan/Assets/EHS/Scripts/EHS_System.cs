//////////////////////////////////////////////////////////
//                  Event Handle System                 //
//           by Mitchell Jenkins 10-Jun-18              //
//////////////////////////////////////////////////////////
#region namespace
using global::System.Collections.Generic;
using global::UnityEngine;
using global::System.Xml;
using global::System.Xml.Serialization;
using global::System.IO;
using global::UnityEngine.Console;
#endregion

#region Handles
public delegate void __eHandle<S, E> (S sender, E eventArgs);
public enum __eType { _NULL_, _INIT_, _CLOSE_, _ERROR_ }

public delegate void __OnEventInvoked();
#endregion

public struct RESULTS {
    public static readonly int FAILURE = -1;
}

// Event Args
public class __eArg<_T> {
    #region Constructor
    /// <summary>
    /// Arguments for the event
    /// </summary>
    /// <param name="sender">Object that sent the event</param>
    /// <param name="target">Object that is being targeted</param>
    /// <param name="value">Value that is being sent</param>
    /// <param name="type">Type of object that send the event</param>
    public __eArg (_T e, object target, object value, System.Type type) { this.arg = e; this.target = target; this.value = value; this.type = type; }
    #endregion

    #region Variables
    /// <summary>
    /// Object that is being targeted
    /// </summary>
    public object target { get; private set; }
    /// <summary>
    /// Value that is being sent
    /// </summary>
    public object value { get; private set; }
    /// <summary>
    /// Type of object that send the event
    /// </summary>
    public System.Type type { get; private set; }
    /// <summary>
    /// The event thats being sent
    /// </summary>
    public _T arg { get; private set; }
    #endregion
}

// Event System Options
public struct __eventSystemOptions {
    public enum SystemOptions {
        logEventsInEditor   = 1 << 0,
        logEventsInBuild    = 1 << 1,
        logToFile           = 1 << 2,
    }
    public static int DefaultOptions =
        (int)SystemOptions.logEventsInEditor |
        (int)SystemOptions.logEventsInBuild |
        (int)SystemOptions.logToFile;
}

// Event XML system
[XmlRoot("EventCollection")]
public class __eventXML {

    [XmlArray("ObjectList")]
    [XmlArrayItem("Object")]
    public List<__eventSystem.struct_events> xml_event = new List<__eventSystem.struct_events>();

    /// <summary>
    /// Logs all events to an XML file
    /// </summary>
    public void XMLSaveEvents() {
        xml_event = __eventSystem.g_events;

        XmlSerializer serializer = new XmlSerializer(typeof(__eventXML));
        StreamWriter writer = new StreamWriter("EventHandlingSystem.xml");
        serializer.Serialize(writer.BaseStream, this);
        writer.Close();
    }
}

// Event Logging System
[ExecuteInEditMode]
public static class __eventSystem {
    #region Variables
    /// <summary>
    /// Event
    /// </summary>
    public static event __OnEventInvoked HandleOnEventInvoked = null;

    /// <summary>
    /// Struct to hold the events
    /// </summary>
    public struct struct_events {
        [XmlAttribute("ObjectName")]
        public string eventName;
        [XmlArray("EventList")]
        [XmlArrayItem("Event")]
        public List<string> eventList;
    };


    public static List<struct_events> events = new List<struct_events>();
    public static List<struct_events> g_events = new List<struct_events>();

    public static int EventSystemOptions = __eventSystemOptions.DefaultOptions;
    public static __eventXML eventXML = new __eventXML();

    #endregion

    #region Static Functions
    /// <summary>
    /// Logs the an event
    /// </summary>
    /// <param name="eventName">The object that the event is coming from</param>
    /// <param name="eventArg">The event being logged</param>
    public static void LogEvent(string eventName, string eventArg) {
        #region Options
#if UNITY_EDITOR
        if ((EventSystemOptions & (int)__eventSystemOptions.SystemOptions.logEventsInEditor)
            != (int)__eventSystemOptions.SystemOptions.logEventsInEditor) {
            return;
        }
#endif  
#if !UNITY_EDITOR
        if ((EventSystemOptions & (int)__eventSystemOptions.SystemOptions.logEventsInBuild)
            != (int)__eventSystemOptions.SystemOptions.logEventsInBuild) {
            return;
        }
#endif
        #endregion

        // Loops through all the events and will add the event if the
        // object has already been logged
        for (int i = 0; i < events.Count; i++) {
            if (events[i].eventName == eventName) {
                events[i].eventList.Add(eventArg);
                g_events[i].eventList.Add(eventArg + " :: Time=" + Time.time);

                
                if (HandleOnEventInvoked != null) {
                    HandleOnEventInvoked();
                }
                return;
            }
        }

        // If the object has not been logged I will create a new one
        // and fill it in
        struct_events newEvent = new struct_events();
        newEvent.eventName = eventName;
        List<string> newEventList = new List<string>();
        newEventList.Add(eventArg);
        newEvent.eventList = newEventList;
        events.Add(newEvent);

        newEventList = new List<string>();
        newEventList.Add(eventArg + " :: Time=" + Time.time);
        newEvent.eventList = newEventList;

        g_events.Add(newEvent);
        if (HandleOnEventInvoked != null) {
            HandleOnEventInvoked();
        }
    }

    /// <summary>
    /// Gets the oldest event and then deletes it
    /// </summary>
    /// <param name="eventName">The object you are getting the events from</param>
    /// <returns>Returns the oldest event</returns>
    public static string[] GetEvent(string eventName) {
        for (int i = 0; i < events.Count; i++) {
            if (events[i].eventName == eventName) {
                if (events[i].eventList.Count <= 0) {
                    return new string[0];
                }
                string[] tempEvent = new string[2];
                tempEvent[0] = events[i].eventName;
                tempEvent[1] = events[i].eventList[0];
                events[i].eventList.RemoveAt(0);

                return tempEvent;
            }
        }
        return new string[0];
    }

    public static void RaiseOnEventInvoked(__OnEventInvoked f) {
        if (HandleOnEventInvoked == null)
            HandleOnEventInvoked += f;
    }
    public static void ConsumeOnEventInvoked(__OnEventInvoked f) {
        HandleOnEventInvoked -= f;
    }

    /// <summary>
    /// Clears the events
    /// </summary>
    public static void Clear() {
        if ((EventSystemOptions & (int)__eventSystemOptions.SystemOptions.logToFile)
            == (int)__eventSystemOptions.SystemOptions.logToFile) {
            eventXML.XMLSaveEvents();
        }
        events.Clear();
        g_events.Clear();
    }
    #endregion
}

// Events
public class __event<_T> {
    #region Variables
    /// <summary>
    /// The variable that stores all the events
    /// </summary>
    public static event __eHandle<object, __eArg<_T>> HandleEvent = null;
    /// <summary>
    /// Stores all the Subscribed functions to this event system
    /// </summary>
    public static List<__eHandle<object, __eArg<_T>>> Subscribers = new List<__eHandle<object, __eArg<_T>>>();

    
    #endregion

    #region Static Functions
    /// <summary>
    /// Invokes an event using the arguments that where passed
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">arguments for the event</param>
    public static void InvokeEvent (object sender, __eArg<_T> e) {
        if (e.value != null) {
            __eventSystem.LogEvent(sender.ToString(), "INVOKE :: ARG=" + e.arg.ToString() + " :: VALUE="+e.value);
        } else {
            __eventSystem.LogEvent(sender.ToString(), "INVOKE :: ARG=" + e.arg.ToString());
        }
        
        UnityConsole.Instance.LogConsole("EHS - OBJ=" + sender.ToString() + "\n\t :: INVOKE :: ARG=" + e.arg.ToString() + " :: Time=" + Time.time + "\n");

        if (HandleEvent != null) HandleEvent(sender, e);
    }
    /// <summary>
    /// Invokes an event using the arguments that where passed
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">arguments for the event</param>
    public static void InvokeEvent (object sender, _T e) { InvokeEvent(sender, new __eArg<_T>(e, null, null, null)); }
    /// <summary>
    /// Invokes an event using the arguments that where passed
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">arguments for the event</param>
    public static void InvokeEvent (object sender, _T e, object value) { InvokeEvent(sender, new __eArg<_T>(e, null, value, null)); }

    /// <summary>
    /// Gets the Subscriber using the index of it
    /// </summary>
    /// <param name="i">index of Subscriber</param>
    /// <returns></returns>
    public static __eHandle<object, __eArg<_T>> GetSubscriber (int i) { return Subscribers[i]; }
    /// <summary>
    /// Creates a new Subscriber and adds it to storage
    /// </summary>
    /// <param name="f">function to subscribe</param>
    /// <param name="MSA">Max Subscriber amount</param>
    /// <returns></returns>
    public static int CreateSubscriber (object sender, __eHandle<object, __eArg<_T>> f) {
        if (f == null) {
            __eventSystem.LogEvent(sender.ToString(), "SUBSCRIBING FAILED :: ID=" + (Subscribers.Count - 1) + 1 + " :: NAME=" + f.Method.Name);
            UnityConsole.Instance.LogConsole("EHS - OBJ=" + sender.ToString() + "\n\t :: SUBSCRIBING FAILED :: ID=" + (Subscribers.Count - 1) + " :: NAME=" + f.Method.Name + " :: Time=" + Time.time + "\n");
            return -1;
        }
        Subscribers.Add(f);
        __eventSystem.LogEvent(sender.ToString(), "SUBSCRIBING :: ID=" + (Subscribers.Count - 1) + " :: NAME=" + f.Method.Name);
        UnityConsole.Instance.LogConsole("EHS - OBJ=" + sender.ToString() + "\n\t :: SUBSCRIBING :: ID=" + (Subscribers.Count - 1) + " :: NAME=" + f.Method.Name + " :: Time=" + Time.time + "\n");
        return Subscribers.Count - 1;
    }
    /// <summary>
    /// Finds the index of the subscriber
    /// </summary>
    /// <param name="f">Function to find</param>
    /// <returns></returns>
    public static int FindSubscriber (__eHandle<object, __eArg<_T>> f) {
        for (int i = 0; i < Subscribers.Count; i++) {
            if (f == Subscribers[i]) return i;
        }
        return -1;
    }
    /// <summary>
    /// Raises a new function
    /// </summary>
    /// <param name="i">Index of the Subscribed function to raise</param>
    /// <returns></returns>
    public static int Raise (int i) {
        if (i == -1) { return -1; }
        HandleEvent += Subscribers[i]; return i;
    }
    /// <summary>
    /// Raises a new function
    /// </summary>
    /// <param name="f">function to raise</param>
    public static int Raise (object sender, __eHandle<object, __eArg<_T>> f) {
        int i = Raise(CreateSubscriber(sender, f));
        if (i == -1) {
            __eventSystem.LogEvent(sender.ToString(), "RAISE FAILED :: NAME=" + f.Method.Name + " :: FROM=" + f.Target.ToString());
            UnityConsole.Instance.LogConsole("EHS - OBJ=" + sender.ToString() + "\n\t :: RAISE FAILED :: NAME=" + f.Method.Name + " :: FROM=" + f.Target.ToString() + " :: Time=" + Time.time + "\n");
            return i;
        }
        __eventSystem.LogEvent(sender.ToString(), "RAISE :: NAME=" + f.Method.Name + " :: FROM=" + f.Target.ToString());
        UnityConsole.Instance.LogConsole("EHS - OBJ=" + sender.ToString() + "\n\t :: RAISE :: NAME=" + f.Method.Name + " :: FROM=" + f.Target.ToString() + " :: Time=" + Time.time + "\n");
        return i;
    }
    /// <summary>
    /// Consumes a subscribed function
    /// </summary>
    /// <param name="i">Index of subscribed function to consume</param>
    /// <returns></returns>
    public static int Consume (int i) {
        if (i == -1 || i >= Subscribers.Count) {
            return -1;
        }
        HandleEvent -= Subscribers[i];
        Subscribers.RemoveAt(i);
        return i;
    }
    /// <summary>
    /// Consumes a subscribed function
    /// </summary>
    /// <param name="f">function to consume</param>
    public static int Consume (__eHandle<object, __eArg<_T>> f) {
        return Consume(FindSubscriber(f));
    }
    /// <summary>
    /// Consumes all subscribed functions
    /// </summary>
    public static void ConsumeAll () {
        HandleEvent = null;
    }
    /// <summary>
    /// Unsubscribes all functions
    /// </summary>
    public static void UnsubscribeAll () {
        Subscribers = new List<__eHandle<object, __eArg<_T>>>();
    }

    
    #endregion
}