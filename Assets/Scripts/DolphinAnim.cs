using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DolphinAnim : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    public InteractMessage interactMessage;
    
    void Start()
    {
        interactMessage.OnShootingButtonPressed += PlayTimeline;
    }

    public void PlayTimeline()
    {
        timelineDirector.Play();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayTimeline();
        }
    }
}
