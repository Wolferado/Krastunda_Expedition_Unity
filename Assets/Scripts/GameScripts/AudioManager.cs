using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameScripts
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Source Clips")]
        public Sound[] musicClips;
        public Sound[] soundClips;

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource timerSoundSource;
        public AudioSource soundSource;

        private EventManager eventManagerScript;
        private bool isGamePaused = false;
        private float oneSecondInterval;

        void Start()
        {

        }

        void Update()
        {
            if (oneSecondInterval >= 0 && !isGamePaused)
                oneSecondInterval -= Time.deltaTime;
            else if (oneSecondInterval < 0 && !isGamePaused)
                oneSecondInterval = 1.0f;
        }

        void Awake()
        {
            ResumeAllSoundSources();

            // Communication with EventManager
            eventManagerScript = GameObject.Find("EventSystem").GetComponent<EventManager>();

            if (eventManagerScript != null)
            {
                eventManagerScript.gamePause.AddListener(PauseAllSoundSources);
                eventManagerScript.gameResume.AddListener(ResumeAllSoundSources);
                eventManagerScript.onTimerTimeDecrement.AddListener(delegate(float timeLeft)
                {
                    if (oneSecondInterval < 0.0f && timeLeft >= 60)
                        PlayTimerSound("TimerBeepNormal");
                    else if (oneSecondInterval < 0.0f && timeLeft < 60)
                        PlayTimerSound("TimerBeepCritical");
                });

                eventManagerScript.itemInspectionStart.AddListener(delegate { PlaySound("ZoomIn"); });
                eventManagerScript.itemInspectionEnd.AddListener(delegate { PlaySound("ZoomOut"); });
                eventManagerScript.itemPickUp.AddListener(delegate { PlaySound("CollectSound"); });
                eventManagerScript.itemCollect.AddListener(delegate { PlaySound("CollectSound"); });
                eventManagerScript.itemInteractionStart.AddListener(delegate { PlaySound("UseSound"); });
            }

            // Communication with SettingsScript (to update volumes, if changed)
            SettingsScript settingsScript = GameObject.Find("SettingsWindow").GetComponent<SettingsScript>();

            if (settingsScript.gameObject.activeInHierarchy && SceneManager.GetActiveScene().name == "MainMenuScene") // To hide only if in main menu, otherwise cause an error with null exception, because deletes it earlier than other script
                GameObject.Find("SettingsWindow").SetActive(false); // Dog-nail to hide SettingsWindow (active to get its reference)

            if(settingsScript != null)
            {
                settingsScript.onMusicVolumeChanged.AddListener(SetMusicVolume);
                settingsScript.onMasterVolumeChanged.AddListener(SetSoundVolume);
                settingsScript.onMasterVolumeChanged.AddListener(SetAudioListenerVolume);
            }

            // Set default values
            if (musicSource != null)
                musicSource.volume = PlayerPrefs.GetInt("MusicVolume") / 100;

            if (soundSource != null)
                soundSource.volume = PlayerPrefs.GetInt("SoundVolume") / 100;
        }

        public void PlayMusic(string musicName)
        {
            Sound musicToPlay = Array.Find(musicClips, musicClip => musicClip.name == musicName);

            if(musicToPlay == null)
                return;
            else
            {
                musicSource.clip = musicToPlay.clip;
                musicSource.Play();
            }
        }

        public void PlayTimerSound(string soundName)
        {
            Sound soundToPlay = Array.Find(soundClips, soundClip => soundClip.name == soundName);

            if (soundToPlay == null)
                return;
            else
            {
                timerSoundSource.clip = soundToPlay.clip;
                timerSoundSource.Play();
            }
        }

        public void PlaySound(string soundName)
        {
            Sound soundToPlay = Array.Find(soundClips, soundClip => soundClip.name == soundName);

            if (soundToPlay == null)
                return;
            else
            {
                soundSource.clip = soundToPlay.clip;
                soundSource.Play();
            }
        }

        public void SetMusicVolume(float newVolume)
        {
            musicSource.volume = newVolume;
        }

        public void SetSoundVolume(float newVolume)
        {
            soundSource.volume = newVolume;
        }

        public void PauseMusic()
        {
            if (musicSource != null)
                musicSource.Pause();
        }

        public void ResumeMusic()
        {
            if (musicSource != null)
                musicSource.Play();
        }

        public void PauseSound()
        {
            if (soundSource != null)
                soundSource.Pause();
        }

        public void ResumeSound()
        {
            if(soundSource != null) 
                soundSource.Play();
        }

        public void PauseTimerSound()
        {
            if (timerSoundSource != null)
                timerSoundSource.Stop();
        }

        public void ResumeTimerSound()
        {
            if (timerSoundSource != null)
                timerSoundSource.Play();
        }

        public void PauseAllSoundSources()
        {
            isGamePaused = true;
            PauseMusic();
            PauseSound();
            PauseTimerSound();
            AudioListener.pause = true;
        }

        public void ResumeAllSoundSources()
        {
            isGamePaused = false;
            ResumeMusic();
            ResumeSound();
            ResumeTimerSound();
            AudioListener.pause = false;
        }

        private void SetAudioListenerVolume(float newVolume)
        {
            AudioListener.volume = newVolume;
        }
    }
}
