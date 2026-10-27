using UnityEngine;
using System.Collections;

public class AgentAudioHolder : MonoBehaviour
{
    public audioSourceClass sources = new audioSourceClass();
    public audioClipClass clips = new audioClipClass();
    [System.Serializable]
    public class audioSourceClass
    {
        public AudioSource A_hurt;
        public AudioSource A_death;
        [Space(10)]
        public AudioSource A_jump;
        public AudioSource A_jumpMidAir;
        public AudioSource A_land;
        public AudioSource A_stand;
        public AudioSource A_crouch;
        public AudioSource A_sprint;
        public AudioSource A_slide;
        [Space(10)]
        public AudioSource A_step;
        public AudioSource A_sprintStep;
        public AudioSource A_crouchStep;
        [Space(10)]
        public AudioSource A_pickupSmall;
        public AudioSource A_pickup;
        public AudioSource A_throww;
    }
    [System.Serializable]
    public class audioClipClass
    {
        public AudioClip[] hurt = new AudioClip[0];
        public AudioClip[] death = new AudioClip[0];
        [Space(10)]
        public AudioClip[] jump = new AudioClip[0];
        public AudioClip[] jumpMidAir = new AudioClip[0];
        public AudioClip[] land = new AudioClip[0];
        public AudioClip[] stand = new AudioClip[0];
        public AudioClip[] crouch = new AudioClip[0];
        public AudioClip[] sprint = new AudioClip[0];
        public AudioClip[] slide = new AudioClip[0];
        [Space(10)]
        public AudioClip[] step = new AudioClip[0];
        public AudioClip[] sprintStep = new AudioClip[0];
        public AudioClip[] crouchStep = new AudioClip[0];
        [Space(10)]
        public AudioClip[] pickupSmall = new AudioClip[0];
        public AudioClip[] pickup = new AudioClip[0];
        public AudioClip[] throww = new AudioClip[0];
    }

    public enum type
    {
        hurt,
        death,

        jump,
        jumpMidAir,
        land,
        stand,
        crouch,
        sprint,
        slide,

        step,
        sprintStep,
        crouchStep,

        pickupSmall,
        pickup,
        throww,
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play(type _type)
    {
        switch (_type)
        {
            case type.hurt: 
                Play(sources.A_hurt, clips.hurt); break;
            case type.death: 
                Play(sources.A_death, clips.death); break;

            case type.jump: 
                Play(sources.A_jump, clips.jump); break;
            case type.jumpMidAir: 
                Play(sources.A_jumpMidAir, clips.jumpMidAir); break;
            case type.land:
                Play(sources.A_land, clips.land); break;
            case type.stand:
                Play(sources.A_stand, clips.stand); break;
            case type.crouch:
                Play(sources.A_crouch, clips.crouch); break;
            case type.sprint:
                if (!sources.A_sprint.isPlaying)
                    StartCoroutine(FadeIn(sources.A_sprint, clips.sprint)); break;
            case type.slide:
                if (!sources.A_slide.isPlaying)
                    StartCoroutine(FadeIn(sources.A_slide, clips.slide)); break;

            case type.step: 
                Play(sources.A_step, clips.step); break;
            case type.sprintStep:
                Play(sources.A_sprintStep, clips.sprintStep); break;
            case type.crouchStep:
                Play(sources.A_crouchStep, clips.crouchStep); break;

            case type.pickupSmall:
                Play(sources.A_pickupSmall, clips.pickupSmall); break;
            case type.pickup:
                Play(sources.A_pickup, clips.pickup); break;
            case type.throww:
                Play(sources.A_throww, clips.throww); break;
            default:
                break;
        }
    }
    public void Stop(type _type)
    {
        switch (_type)
        {
            case type.sprint:
                if (sources.A_sprint.isPlaying)
                    StartCoroutine(FadeOut(sources.A_sprint)); 
                break;
            case type.slide:
                if (sources.A_slide.isPlaying)
                    StartCoroutine(FadeOut(sources.A_slide)); 
                break;
            default:
                break;
        }
    }

    void Play(AudioSource _source, AudioClip[] _clips)
    {
        if (_clips.Length > 0)
        {
            int i = Random.Range(0, _clips.Length);
            _source.clip = _clips[i];
            _source.pitch = 1 + Random.Range(-0.1f, 0.1f);
            _source.Play();
        }
    }

    IEnumerator FadeIn(AudioSource _source, AudioClip[] _clips, float _speed = 0.5f, bool _unscaled = false)
    {
        _source.volume = 0;
        Play(_source, _clips);
        float timer = 0;
        while (timer < 1)
        {
            _source.volume = Mathf.Lerp(0, 1, timer);
            yield return new WaitForEndOfFrame();
            if (_unscaled)
                timer += Time.unscaledDeltaTime / _speed;
            else
                timer += Time.deltaTime / _speed;
        }
        _source.volume = 1;
    }
    IEnumerator FadeOut(AudioSource _source, float _speed = 0.5f, bool _unscaled = false)
    {
        float _volume = _source.volume;
        float timer = 0;
        while (timer < 1)
        {
            _source.volume = Mathf.Lerp(_volume, 0, timer);
            yield return new WaitForEndOfFrame();
            if (_unscaled)
                timer += Time.unscaledDeltaTime / _speed;
            else
                timer += Time.deltaTime / _speed;
        }
        _source.Stop();
        _source.volume = _volume;
    }
}