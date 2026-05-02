using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private AudioSource musicSourceLoop;
    [SerializeField] private AudioSource musicSourceIntro;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Music")]
    [SerializeField] private AudioClip deadMusic;
    [SerializeField] private AudioClip menuMusicIntro;
    [SerializeField] private AudioClip menuMusicLoop;
    [SerializeField] private AudioClip villageMusicIntro;
    [SerializeField] private AudioClip villageMusicLoop;
    [SerializeField] private AudioClip forestMusicIntro;
    [SerializeField] private AudioClip forestMusicLoop;
    [SerializeField] private AudioClip forestMusicBattle;
    [SerializeField] private AudioClip hamsterMusicIntro;
    [SerializeField] private AudioClip hamsterMusicLoop;
    [SerializeField] private AudioClip templeMusicIntro;
    [SerializeField] private AudioClip templeMusicLoop;
    [SerializeField] private AudioClip templeMusicBattle;
    [SerializeField] private AudioClip[] combatMusic;
    
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
    [SerializeField] private AudioClip playerAttackMagic;
    [SerializeField] private AudioClip playerAttackMelee;
    
    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip[] enemyDamage;
    
    private readonly string GAME_MUSIC = "GAME_MUSIC";
    private readonly string MUSIC_CLIP_VOLUME = "MUSIC_CLIP_VOLUME";
    private readonly string MASTER_VOLUME = "MASTER_VOLUME";
    private readonly string MUSIC_VOLUME = "MUSIC_VOLUME";
    private readonly string SFX_VOLUME = "SFX_VOLUME";
    private readonly string VOLUME_MUTED = "VOLUME_MUTED";
    
    private AudioClip nextClip;
    private float nextClipVolume;
    
    private Dictionary<string, AudioClip> musicLookup;

    protected override void Awake()
    {
        base.Awake();
        PlayMenuMusic();
        
        musicLookup = new Dictionary<string, AudioClip>
        {
            { deadMusic.name, deadMusic },

            { menuMusicIntro.name, menuMusicIntro },
            { menuMusicLoop.name, menuMusicLoop },

            { villageMusicIntro.name, villageMusicIntro },
            { villageMusicLoop.name, villageMusicLoop },

            { forestMusicIntro.name, forestMusicIntro },
            { forestMusicLoop.name, forestMusicLoop },
            { forestMusicBattle.name, forestMusicBattle },

            { hamsterMusicIntro.name, hamsterMusicIntro },
            { hamsterMusicLoop.name, hamsterMusicLoop },

            { templeMusicIntro.name, templeMusicIntro },
            { templeMusicLoop.name, templeMusicLoop },
            { templeMusicBattle.name, templeMusicBattle }
        };
    }

    private void Start()
    {
        musicSourceLoop.ignoreListenerPause = true; 
        musicSourceIntro.ignoreListenerPause = true; 
        
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
        
        musicSourceLoop.gameObject.SetActive(true);
        musicSourceIntro.gameObject.SetActive(true);
    }
    
    void Update()
    {
        if(nextClip == null) return;
        if (musicSourceIntro.time >= musicSourceIntro.clip.length)
        {
            PlayMusic(nextClip, nextClipVolume);
        }
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

    public void SaveCurrentMusic(AudioClip clip)
    {
        string currentMusic = clip.name;
        
        SaveGame.Save(GAME_MUSIC, currentMusic);
        
        float volume;

        if (musicSourceIntro.clip == clip) volume = musicSourceIntro.volume;
        else if (musicSourceLoop.clip == clip) volume = musicSourceLoop.volume;
        else volume = musicSourceLoop.volume;
        
        SaveGame.Save(MUSIC_CLIP_VOLUME, volume);
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
                AudioClip loopClip = GetLoopForIntro(currentMusicClip);

                if (loopClip != null)
                {
                    PlayMusicWithIntro(currentMusicClip, loopClip, volume);
                }
                else
                {
                    PlayMusic(currentMusicClip, volume);
                    Debug.Log(currentMusic);
                }
            }
        }
    }
    
    private AudioClip GetLoopForIntro(AudioClip introClip)
    {
        if (introClip == menuMusicIntro) return menuMusicLoop;

        if (introClip == villageMusicIntro) return villageMusicLoop;

        if (introClip == forestMusicIntro) return forestMusicLoop;

        if (introClip == hamsterMusicIntro) return hamsterMusicLoop;

        if (introClip == templeMusicIntro) return templeMusicLoop;

        return null;
    }
    
    private AudioClip GetMusicClipByName(string clipName)
    {
        return musicLookup.TryGetValue(clipName, out var clip) ? clip : null;
    }

    public void NewGameMusic()
    {
        PlayVillageMusic();
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

    public void PlayMusicWithIntro(AudioClip introClip, AudioClip loopClip, float volume)
    {
        if (musicSourceIntro.clip == introClip && musicSourceIntro.isPlaying) return; 
        
        musicSourceLoop.Stop();
        
        musicSourceIntro.volume = volume;
        musicSourceIntro.clip = introClip;
        musicSourceIntro.Play();
        
        if (musicSourceIntro.clip != menuMusicIntro)
        {
            SaveCurrentMusic(introClip);
        }

        nextClip = loopClip;
        nextClipVolume = volume;
    }

    public void PlayMusic(AudioClip musicClip, float volume)
    {
        if (musicSourceLoop.clip == musicClip) return; 
        
        musicSourceIntro.Stop();
        
        musicSourceLoop.volume = volume;
        musicSourceLoop.clip = musicClip;
        musicSourceLoop.Play();
        
        nextClip = null;
    }
    
    private void PlayRandomSFX(AudioClip[] clips)
    {
        int randomSound = Random.Range(0, enemyDamage.Length);
        PlaySFX(clips[randomSound]);
    }

    private bool isCombatMusic(AudioClip clip)
    {
        if (combatMusic.Contains(clip))
        {
            return true;
        }
        return false;
    }

    //Music
    public void PlayMenuMusic() => PlayMusicWithIntro(menuMusicIntro, menuMusicLoop, 1f);
    public void PlayVillageMusic() => PlayMusicWithIntro(villageMusicIntro, villageMusicLoop, 1f);
    public void PlayDeadMusic() => PlayMusic(deadMusic, 1f);
    public void PlayCombatMusic(int index) => PlayMusic(combatMusic[index], 1f);

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
    public void PlayPlayerAttackMagicSound() => PlaySFX(playerAttackMagic);
    public void PlayPlayerAttackMeleeSound() => PlaySFX(playerAttackMelee);

    // Enemy Sounds
    public void PlayEnemyDeathSound() => PlaySFX(enemyDeath);

    public void PlayEnemyDamageSound() => PlayRandomSFX(enemyDamage);
}