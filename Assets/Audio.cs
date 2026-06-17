using UnityEngine;

public class Audio : StateMachineBehaviour
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool playOnEnter = true;
    public bool playOnExit = false;
    public bool stopOnExit = false;
    public bool useOneShot = true;

    private AudioSource GetOrAddAudioSource(Animator animator)
    {
        var source = animator.GetComponent<AudioSource>() ?? animator.GetComponentInParent<AudioSource>();
        if (source == null)
        {
            source = animator.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        return source;
    }

    // Called when the state is entered
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!playOnEnter || clip == null)
            return;

        var src = GetOrAddAudioSource(animator);
        if (useOneShot)
            src.PlayOneShot(clip, volume);
        else
        {
            src.clip = clip;
            src.volume = volume;
            src.Play();
        }
    }

    // Optional: Called when the state is exited
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var src = GetOrAddAudioSource(animator);
        if (stopOnExit)
        {
            
            if (src != null)
                src.Stop();
        }

        if (!playOnExit || clip == null)
            return;

        
        if (useOneShot)
            src.PlayOneShot(clip, volume);
        else
            src.Play();
    }
}