using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LoopMarkerReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context) 
    {
        LoopEndMarker loopEndMarker = notification as LoopEndMarker;
        if (loopEndMarker == null) return;
        LoopStartMarker loopStartMarker = loopEndMarker.LoopStartMarker;
        if (loopStartMarker == null) return;

        Playable playable = origin.GetGraph().GetRootPlayable(0);
        playable.SetTime(loopStartMarker.time);
    }
}

public class LoopStartMarker : Marker 
{

}
public class LoopEndMarker : Marker, INotification 
{
    public PropertyName id { get; }
    public LoopStartMarker LoopStartMarker;
}
