using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LoopStartMarkerReceiver : MonoBehaviour, INotificationReceiver
{

    public void OnNotify(Playable origin, INotification notification, object context) 
    {
        var element = notification as LoopStartMarker;
        if (element == null) return;

    }
}
public class LoopStartMarker : Marker, INotification
{
    public PropertyName id { get; }

}
