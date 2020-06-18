using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SectionMarkerReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context) 
    {

    }
}

public class SectionMarker : Marker, INotification 
{
    public PropertyName id { get; }
}
