using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    
    public bool muteBGM;
    public bool muteSFX;

    public BGM[] bgm;
    public SFX[] sfx;
	//hitting tree, metal, rock, etc
	public SFX[] hitTypeSFX;
	public enum hitSoundType
	{
		None,
		Wood,
		Stone,
		Flesh
	}

	[System.Serializable]
	public struct AnnouncementSFX
	{
		public SFX announcement;
		public float announcementTimer;
		public float cooldownTimer;
	}

	public AnnouncementSFX[] announcementSFX;
    
    public static AudioManager instance;


    private AudioSource bgmSource = null;
	private AudioSource announcementSource = null;

    void Awake() {
        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);


		bgmSource = transform.GetChild(0).gameObject.AddComponent<AudioSource>();
		announcementSource = transform.GetChild(0).gameObject.AddComponent<AudioSource>();

        foreach (SFX s in sfx) {
            s.source = transform.GetChild(1).gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

    }

	void Start () {
        //play bgm at start?
        PlayBGM("HumanX1");
	}
	
	// Update is called once per frame
	void Update () {


		UpdateAnnouncementTimers ();
	}

	public void UpdateAnnouncementTimers(){
		for (int i = 0; i < announcementSFX.Length; i++) {
			if (announcementSFX [i].cooldownTimer > 0.0f)
				announcementSFX [i].cooldownTimer -= Time.deltaTime;
		}
	}

	public void PlayAnnouncement(string name){
		for (int i = 0; i < announcementSFX.Length; i++) {
			if (announcementSFX [i].announcement.name == name && announcementSFX[i].cooldownTimer <= 0.0f) {
				SFX announce = announcementSFX [i].announcement;

				announcementSource.volume = announce.volume;
				announcementSource.pitch = announce.pitch;
				announcementSource.PlayOneShot (announce.clip);
				announcementSFX [i].cooldownTimer = announcementSFX [i].announcementTimer;
				
			}
		}
	}

    public void PlayBGM(string bgmName) {
        bgmSource.time = 0.0f;
        BGM music = Array.Find(bgm, bgm => bgm.name == bgmName);
        if (music == null)
            Debug.LogWarning("BGM: " + bgmName + " was not found");
        else {
            bgmSource.clip = music.clip;
            bgmSource.volume = music.volume;
            bgmSource.pitch = music.pitch;

            bgmSource.loop = true;
            bgmSource.Play();
        }

        if (muteBGM)
            bgmSource.Stop();
    }

    public void PlayBGM() {
        bgmSource.time = 0.0f;
        if (muteBGM) {
            bgmSource.Stop();
            return;
        }
        bgmSource.Play();
    }
    
    public void PauseBGM() {
        bgmSource.Pause();
    }

    public void StopBGM() {
        bgmSource.Stop();
    }

    public void PlaySFX(string soundName) {
        if (muteSFX)
            return;

        SFX s = Array.Find(sfx, sound => sound.name == soundName);
        if (s == null)
            Debug.LogWarning("Sound: " + soundName + " was not found");
        else {
            s.source.Play();
        }
    }

    public SFX RequestSFX(string soundClip) {
        SFX s = Array.Find(sfx, sound => sound.name == soundClip);
        if (s == null) {
            Debug.LogWarning("Sound: " + soundClip + " was not found");
        } else
            return s;
        return null;
    }

	public SFX RequestHitSFX(hitSoundType hitType){
		int numOfSounds = 0;
		string soundName = hitType.ToString();
		switch (hitType) {
		case hitSoundType.Wood: 
			numOfSounds = 4;
			break;
		case hitSoundType.Stone:
		case hitSoundType.Flesh:
			numOfSounds = 3;
			break;
		default:
			break;
		}

		numOfSounds = UnityEngine.Random.Range (1, numOfSounds);
		soundName += numOfSounds;

		SFX s = Array.Find(hitTypeSFX, sound => sound.name == soundName);
		if (s == null) {
			Debug.LogWarning("Sound: " + soundName + " was not found");
		} else
			return s;
		return null;

	}

    public void ToggleMuteBGM() {
        if (!muteBGM) {
            muteBGM = true;
            StopBGM();
        } else {
            muteBGM = false;
            PlayBGM();
        }
    }

    public void ToggleMuteSFX() {
        muteSFX = !muteSFX;
    }
}
