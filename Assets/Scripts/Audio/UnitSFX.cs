using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UnitSFX : MonoBehaviour {

	public string walkSoundName = "Walk";
	public string swingSoundName = "Swing1";
	public string damagedSoundName = "GetHit";
	public string deathSoundName = "Death";
	public string dropItemSoundName = "DropItem";

    private AudioSource sourceWalk;
	private AudioSource sourceSwing;
	private AudioSource sourceAttack;
    private AudioSource sourceDamaged;
	private AudioSource sourceDeath;
	private AudioSource sourceDropItem;

    [HideInInspector] public bool sfxIsInitialized = false;

    void Awake() {
        sourceWalk = gameObject.AddComponent<AudioSource>();
		sourceSwing = gameObject.AddComponent<AudioSource>();
		sourceAttack = gameObject.AddComponent<AudioSource> ();
		sourceDamaged = gameObject.AddComponent<AudioSource>();
		sourceDeath = gameObject.AddComponent<AudioSource>();
		sourceDropItem = gameObject.AddComponent<AudioSource>();

		SetUpSounds();
    }

	void Update(){
		if (!sfxIsInitialized)
			SetUpSounds ();
	}

	public void SetUpSounds() {
		if (!AudioManager.instance)
			return;
		SetUpWalkClip();
		SetUpSwingClip();
		SetUpAttackClip();
		SetUpDamagedClip();
		SetUpDeathClip();
        sfxIsInitialized = true;
    }

	public void SetUpWalkClip() {
		SFX walkSFX = AudioManager.instance.RequestSFX (walkSoundName);
		if (walkSFX != null) {
			sourceWalk.clip = walkSFX.clip;
			sourceWalk.volume = walkSFX.volume;
			sourceWalk.pitch = walkSFX.pitch;
            sourceWalk.spatialBlend = 1.0f;
        }

    }

	public void SetUpSwingClip() {
		SFX swingSFX = AudioManager.instance.RequestSFX (swingSoundName);
		if (swingSFX != null) {
			sourceSwing.clip = swingSFX.clip;
			sourceSwing.volume = swingSFX.volume;
			sourceSwing.pitch = swingSFX.pitch;
			sourceSwing.spatialBlend = 1.0f;
        }

    }

	public void SetUpDamagedClip() {
		SFX damagedSFX = AudioManager.instance.RequestSFX (damagedSoundName);
		if (damagedSFX != null) {
			sourceDamaged.clip = damagedSFX.clip;
			sourceDamaged.volume = damagedSFX.volume;
			sourceDamaged.pitch = damagedSFX.pitch;
            sourceDamaged.spatialBlend = 1.0f;
        }
    }

	public void SetUpDeathClip() {
		SFX deathSFX = AudioManager.instance.RequestSFX (deathSoundName);
		if (deathSFX != null) {
			sourceDeath.clip = deathSFX.clip;
			sourceDeath.volume = deathSFX.volume;
			sourceDeath.pitch = deathSFX.pitch;
			sourceDeath.spatialBlend = 1.0f;
		}
	}

	public void SetUpAttackClip () {
		SFX attackSFX = AudioManager.instance.RequestHitSFX (AudioManager.hitSoundType.Wood);
		if (attackSFX != null) {
			sourceAttack.clip = attackSFX.clip;
			sourceAttack.volume = attackSFX.volume;
			sourceAttack.pitch = attackSFX.pitch;
			sourceAttack.spatialBlend = 1.0f;
		}
	}

	public void SetUpDropItemClip(){
		SFX dropItemSFX = AudioManager.instance.RequestSFX (dropItemSoundName);
		if (dropItemSFX != null) {
			sourceDropItem.clip = dropItemSFX.clip;
			sourceDropItem.volume = dropItemSFX.volume;
			sourceDropItem.pitch = dropItemSFX.pitch;
			sourceDropItem.spatialBlend = 1.0f;
		}
	}

    public void PlayWalkSFX() {
        if (!AudioManager.instance.muteSFX)
			sourceWalk.PlayOneShot(sourceWalk.clip);
    }

	/// <summary>
	/// Played when unit starts to swing the weapon
	/// </summary>
    public void PlaySwingSFX() {
        if (!AudioManager.instance.muteSFX)
			sourceSwing.PlayOneShot(sourceSwing.clip);
    }

    public void PlayDamagedSFX() {
        if (!AudioManager.instance.muteSFX)
			sourceDamaged.PlayOneShot(sourceDamaged.clip);
    }

	/// <summary>
	/// Played when unit STRIKES the target
	/// </summary>
	public void PlayAttackSFX() {
		if (!AudioManager.instance.muteSFX)
			sourceAttack.PlayOneShot(sourceAttack.clip);
	}

	public void PlayDeathSFX(){
		if (!AudioManager.instance.muteSFX) 
			sourceDeath.Play ();
	}

	public void PlayDropItemSFX(){
		if (!AudioManager.instance.muteSFX)
			sourceDropItem.Play ();	
	}

	/// <summary>
	/// called by owning character and passes along their target
	/// </summary>
	public void UpdateHitSFX(GameObject target){
		if (target.tag == "Resource") {
			if (target.GetComponent<Resource> ().resourceType == Resource.ResourceType.Wood) {
				SwitchHitSFX (AudioManager.hitSoundType.Wood);
			} else {
				SwitchHitSFX (AudioManager.hitSoundType.Stone);
			}
		} else if (target.tag == "Player" || target.tag == "Ally" || target.tag == "Enemy") {
			if (GetComponent<AI_Character> () || GetComponent<PlayerCharacter> ()) {
				if (target.tag != "Enemy")
					SwitchHitSFX (AudioManager.hitSoundType.None);
				else 
					SwitchHitSFX (AudioManager.hitSoundType.Flesh);
			} else if (GetComponent<EnemyCharacter> () && target.tag != "Enemy") {
				SwitchHitSFX (AudioManager.hitSoundType.Flesh);
			}
		} else {
			SwitchHitSFX (AudioManager.hitSoundType.None);
		}


	}

	public void SwitchHitSFX(AudioManager.hitSoundType hitType){
		if (AudioManager.instance.muteSFX)
			return;
		if (hitType == AudioManager.hitSoundType.None) {
			sourceAttack.volume = 0.0f;
			return;
		}


		SFX attackSFX = AudioManager.instance.RequestHitSFX (hitType);
		if (attackSFX != null) {
			sourceAttack.clip = attackSFX.clip;
			sourceAttack.volume = attackSFX.volume;
			sourceAttack.pitch = attackSFX.pitch;
			sourceAttack.spatialBlend = 1.0f;
		}
	}






    
}
