using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UnitSFX : MonoBehaviour {

    public string walkSound;
    public string attackSound;
    public string damagedSound;

    private AudioSource sourceWalk;
    private AudioSource sourceAttack;
    private AudioSource sourceDamaged;
    [HideInInspector] public bool sfxIsInitialized = false;

    void Awake() {
        sourceWalk = gameObject.AddComponent<AudioSource>();
        sourceAttack = gameObject.AddComponent<AudioSource>();
        sourceDamaged = gameObject.AddComponent<AudioSource>();

        SetUpClip(walkSound, attackSound, damagedSound);
    }

    public void SetUpClip(string walkSoundName, string attackSoundName, string damagedSoundName) {
        SetUpWalkClip(walkSoundName);
        SetUpAttackClip(attackSoundName);
        SetUpDamagedSound(damagedSoundName);
        sfxIsInitialized = true;
    }

    public void SetUpWalkClip(string walkSoundName) {
        SFX walkSound = AudioManager.instance.RequestSFX(walkSoundName);
        if (walkSound != null) {
            sourceWalk.clip = walkSound.clip;
            sourceWalk.volume = walkSound.volume;
            sourceWalk.pitch = walkSound.pitch;
            sourceWalk.spatialBlend = 1.0f;
        }

    }

    public void SetUpAttackClip(string attackSoundName) {
        SFX attackSound = AudioManager.instance.RequestSFX(attackSoundName);

        if (attackSound != null) {
            sourceAttack.clip = attackSound.clip;
            sourceAttack.volume = attackSound.volume;
            sourceAttack.pitch = attackSound.pitch;
            sourceAttack.spatialBlend = 1.0f;
        }

    }

    public void SetUpDamagedSound(string damagedSoundName) {
        SFX damagedSound = AudioManager.instance.RequestSFX(damagedSoundName);
        if (damagedSound != null) {
            sourceDamaged.clip = damagedSound.clip;
            sourceDamaged.volume = damagedSound.volume;
            sourceDamaged.pitch = damagedSound.pitch;
            sourceDamaged.spatialBlend = 1.0f;
        }
    }

    public void PlayWalkSFX() {
        if (!AudioManager.instance.muteSFX)
            sourceWalk.Play();
    }

    public void PlayAttackSFX() {
        if (!AudioManager.instance.muteSFX)
            sourceAttack.Play();
    }

    public void PlayDamagedSFX() {
        if (!AudioManager.instance.muteSFX)
            sourceDamaged.Play();
    }


    
}
