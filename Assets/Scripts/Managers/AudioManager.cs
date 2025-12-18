using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle muteToggle;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Music")]
    [SerializeField] private AudioClip townMusic;
    [SerializeField] private AudioClip enemyAreaMusic;
    [SerializeField] private AudioClip menuMusic;
    
    [Header("Menu Sounds")]
    [SerializeField] private AudioClip removeItem;
    [SerializeField] private AudioClip equipItem;
    [SerializeField] private AudioClip useItem;
    [SerializeField] private AudioClip acceptQuest;
    [SerializeField] private AudioClip claimQuest;
    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private AudioClip buyItem;
    
    [Header("Player Sounds")]
    [SerializeField] private AudioClip playerDeath;
    [SerializeField] private AudioClip playerDamage;
    
    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip[] enemyDamage;
    
    private readonly string GAME_MUSIC = "GAME_MUSIC";
    private readonly string MUSIC_CLIP_VOLUME = "MUSIC_CLIP_VOLUME";
    private readonly string MASTER_VOLUME = "MASTER_VOLUME";
    private readonly string MUSIC_VOLUME = "MUSIC_VOLUME";
    private readonly string SFX_VOLUME = "SFX_VOLUME";
    private readonly string VOLUME_MUTED = "VOLUME_MUTED";

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        musicSource.ignoreListenerPause = true; 
        
        float masterVolume = SaveGame.Exists(MASTER_VOLUME) ? SaveGame.Load<float>(MASTER_VOLUME) : 1.0f;
        float musicVolume = SaveGame.Exists(MUSIC_VOLUME) ? SaveGame.Load<float>(MUSIC_VOLUME) : 1.0f;
        float sfxVolume = SaveGame.Exists(SFX_VOLUME) ? SaveGame.Load<float>(SFX_VOLUME) : 1.0f;
        bool isMuted = SaveGame.Exists(VOLUME_MUTED) ? SaveGame.Load<bool>(VOLUME_MUTED) : false;

        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;
        muteToggle.isOn = isMuted;
        
        SetMute(isMuted);
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        muteToggle.onValueChanged.AddListener(SetMute);
        
        musicSource.gameObject.SetActive(true);
    }
    
    public void SetMasterVolume(float volume)
    {    
        bool isMuted = SaveGame.Exists(VOLUME_MUTED) ? SaveGame.Load<bool>(VOLUME_MUTED) : false;
        if (volume > 0 && !isMuted)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); 
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", -80f); 
        }
        SaveGame.Save(MASTER_VOLUME, volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume > 0)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        SaveGame.Save(MUSIC_VOLUME, volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume > 0)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", -80f);
        }
        SaveGame.Save(SFX_VOLUME, volume);
    }

    public void SetMute(bool isMuted)
    {
        float volume = isMuted ? -80f : Mathf.Log10(masterVolumeSlider.value) * 20;
        audioMixer.SetFloat("MasterVolume", volume);
        SaveGame.Save(VOLUME_MUTED, isMuted);
    }
    

    public void SaveCurrentMusic()
    {
        string currentMusic = musicSource.clip.name;
        SaveGame.Save(GAME_MUSIC, currentMusic);
        SaveGame.Save(MUSIC_CLIP_VOLUME, musicSource.volume);
    }

    public void LoadCurrentMusic()
    {
        if (SaveGame.Exists(GAME_MUSIC))
        {
            string currentMusic = SaveGame.Load<string>(GAME_MUSIC);
            float volume = SaveGame.Load<float>(MUSIC_CLIP_VOLUME);
            AudioClip currentMusicClip = GetMusicClipByName(currentMusic);
            if (currentMusicClip != null)
            {
                PlayMusic(currentMusicClip, volume);
            }
        }
    }
    
    private AudioClip GetMusicClipByName(string clipName)
    {
        if (clipName == townMusic.name) return townMusic;
        if (clipName == enemyAreaMusic.name) return enemyAreaMusic;
        if (clipName == menuMusic.name) return menuMusic;

        return null; 
    }

    public void NewGameMusic()
    {
        PlayTownMusic();
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayMusic(AudioClip musicClip, float volume)
    {
        if (musicSource.clip == musicClip) return; 
        
        musicSource.volume = volume;
        musicSource.clip = musicClip;
        musicSource.Play();
        if (musicSource.clip != menuMusic)
        {
            SaveCurrentMusic();
        }
    }

    //Music
    public void PlayMenuMusic() => PlayMusic(menuMusic, 1f);
    public void PlayTownMusic() => PlayMusic(townMusic, 0.5f);
    public void PlayEnemyAreaMusic() => PlayMusic(enemyAreaMusic, 1f);

    // Menu Sounds
    public void PlayRemoveItemSound() => PlaySFX(removeItem, 0.7f);
    public void PlayEquipItemSound() => PlaySFX(equipItem);
    public void PlayUseItemSound() => PlaySFX(useItem);
    public void PlayAcceptQuestSound() => PlaySFX(acceptQuest, 0.7f);
    public void PlayClaimQuestSound() => PlaySFX(claimQuest);
    public void PlayButtonPressSound() => PlaySFX(buttonPress);
    public void PlayBuyItemSound() => PlaySFX(buyItem);

    // Player Sounds
    public void PlayPlayerDeathSound() => PlaySFX(playerDeath);
    public void PlayPlayerDamageSound() => PlaySFX(playerDamage);

    // Enemy Sounds
    public void PlayEnemyDeathSound() => PlaySFX(enemyDeath);

    public void PlayEnemyDamageSound()
    {
        int randomSound = Random.Range(0, enemyDamage.Length);
        PlaySFX(enemyDamage[randomSound]);
    }
}