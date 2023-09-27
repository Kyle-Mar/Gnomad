using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXPlayer : MonoBehaviour
{
    //public AudioMixer mixer;
    //public AudioMixerGroup mixerGroup;

    void Start()
    {
        //mixer = Resources.Load<AudioMixer>("Mixer");
        //mixerGroup = mixer.FindMatchingGroups("SoundEffects")[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySFX(AudioClip clip)
    {
        GameObject SFXObject = new GameObject("SoundEffect");
        Play(SFXObject, clip, 0f);
    }

    //spatial override
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        GameObject SFXObject = new GameObject("SoundEffect");
        SFXObject.transform.position = position;
        Play(SFXObject, clip, 1.0f);

    }

    private void Play(GameObject SFXObject, AudioClip clip, float spatialBlend)
    {
        AudioSource source = SFXObject.AddComponent<AudioSource>();
        //source.outputAudioMixerGroup = mixerGroup;
        source.clip = clip;
        source.spatialBlend = spatialBlend;
        source.volume = 0.33f;
        //source.pitch = Random.Range(0.75f, 1.0f);
        source.Play();
        //StartCoroutine(DestroyAfterClipLength(clip.length, SFXObject));
        Destroy(SFXObject, clip.length);

    }
}