using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _02.Scripts.Level;
using _02.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _02.Scripts.Manager
{
    [RequireComponent(typeof(AudioSource))]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance { get; private set; }

        public bool isPlaying { get; private set; } = false;
        public float offset => 0.25f;
        public float currentPlayTime => (float)(_accDspTime + (
            isPlaying ? 
                AudioSettings.dspTime - _startDspTime : 
                0.0
            ));

        public float currentBeat => currentPlayTime * currentLevel.defaultBpm / 60f; 

        [NonSerialized] public Level currentLevel;
        [NonSerialized] public Boss currentBoss;
        [NonSerialized] public bool isLoaded = false;

        private double _startDspTime, _pauseDspTime, _accDspTime = 0.0;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            instance = this;

            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            LoadLevel().Forget();
        }

        private async UniTask LoadLevel()
        {
            currentLevel = JsonUtility.FromJson<Level>(await File.ReadAllTextAsync(
                Path.Combine(Application.dataPath, "NewChallenger.level.json")
                ));
            
            var bossObj = await Addressables.InstantiateAsync($"Boss/{currentLevel.bossId}");
            if (!bossObj || !bossObj.TryGetComponent(out currentBoss))
            {
                throw new Exception($"Boss Load Failed (Id: {currentLevel.bossId})");
            }

            _audioSource.clip = await Addressables.LoadAssetAsync<AudioClip>($"Music/{currentLevel.musicId}");
            if (!_audioSource.clip)
            {
                throw new Exception($"Music Load Failed (Id: {currentLevel.musicId})");
            }
            
            var parryCount = 0;
            
            foreach (var note in currentLevel.pattern)
            {
                var prefab = currentBoss.noteMap[note.noteType];
                var noteObj = Instantiate(prefab);
                noteObj.note = note;

                if (noteObj.hitType == HitType.Parry) parryCount++;
            }
            
            currentBoss.InitHp(parryCount);
            
            _startDspTime = AudioSettings.dspTime + 2;
            _pauseDspTime = _startDspTime - 1;
            
            _audioSource.PlayScheduled(_startDspTime - offset);
            
            isPlaying = true;
            isLoaded = true;
        }

        public void Pause()
        {
            _pauseDspTime = AudioSettings.dspTime;
            _accDspTime += _pauseDspTime - _startDspTime;
            if(_accDspTime >= 0f) _audioSource.Pause();
            else _audioSource.Stop();

            isPlaying = false;
        }

        public void Resume()
        {
            _startDspTime = AudioSettings.dspTime;
            if (_accDspTime < 0f)
            {
                _audioSource.PlayScheduled(AudioSettings.dspTime - _accDspTime - offset);
            }
            else _audioSource.UnPause();
            
            isPlaying = true;
        }
    }

    [Serializable]
    public class Level
    {
        public int defaultBpm;
        public float baseScrollSpeed;
        public string bossId;
        public string musicId;
        
        public List<Note> pattern;
    }

    [Serializable]
    public class Note
    {
        public float appearBeat;
        public string noteType;
    }
}