using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LoopEndMarkerReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context) 
    {

    }
}

public class LoopEndMarker : Marker, INotification 
{
    public PropertyName id { get; }
}
