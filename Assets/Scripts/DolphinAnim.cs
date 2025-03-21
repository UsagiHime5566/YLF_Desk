using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Threading.Tasks;

public class DolphinAnim : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    public InteractMessage interactMessage;
    public AudioSource audioSource;
    public int delayAnim = 300;
    public bool isPlayOnStart = false;
    
    void Start()
    {
        interactMessage.OnShootingButtonPressed += PlayTimeline;
        
        if(isPlayOnStart)
        {
            PlayTimeline();
        }
    }

    public async void PlayTimeline()
    {
        await Task.Delay(delayAnim);
        timelineDirector.Play();
        audioSource.Play();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayTimeline();
        }
    }
}
