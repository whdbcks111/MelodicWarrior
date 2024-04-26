using System;
using _02.Scripts.Manager;
using UnityEngine;

namespace _02.Scripts.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class NoteObject : MonoBehaviour
    {
        public string noteType;
        public HitType hitType;
        public AudioClip hitSound;
        public SpriteRenderer spriteRenderer;
        
        [NonSerialized] public Note note;

        protected bool isHitted = false;
        protected bool canPlayHitSound = true;

        private void Awake()
        {
            spriteRenderer.enabled = false;
        }

        private void Start()
        {
            UpdatePosition();
        }

        protected virtual void UpdatePosition()
        {
            var beatPos = note.appearBeat - LevelManager.instance.currentBeat;
            transform.position = new Vector3(
                beatPos * LevelManager.instance.currentLevel.baseScrollSpeed, 
                LevelManager.instance.currentBoss.shootPoint.position.y
                );
        }

        private void Update()
        {
            UpdatePosition();
            CheckNoteAppear();
            CheckHitSound();
        }

        protected virtual void CheckNoteAppear()
        {
            if (transform.position.x <= LevelManager.instance.currentBoss.shootPoint.position.x)
            {
                if (isHitted || spriteRenderer.enabled) return;
                
                spriteRenderer.enabled = true;
                Appear();
            }
            else if (spriteRenderer.enabled)
            {
                spriteRenderer.enabled = false;
            }
        }

        protected virtual void Appear()
        {
            
        }

        public virtual void Hit()
        {
            isHitted = true;
            spriteRenderer.enabled = false;
            switch (hitType)
            {
                case HitType.Parry:
                    ParticleManager.instance.SpawnParryParticle(this);
                    LevelManager.instance.currentBoss.Hit();
                    break;
                case HitType.Defend:
                    ParticleManager.instance.SpawnSplitParticle(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckHitSound()
        {
            var beatOffset = (note.appearBeat - LevelManager.instance.currentBeat) /
                LevelManager.instance.currentLevel.defaultBpm * 60f - LevelManager.instance.offset;
            if (beatOffset < LevelManager.instance.offset + 0.2f)
            {
                if (!canPlayHitSound) return;
                SoundManager.instance.PlaySfxScheduled(hitSound, AudioSettings.dspTime + beatOffset);
                canPlayHitSound = false;
            }
            else
            {
                canPlayHitSound = true;
            }
        }
    }

    public enum HitType
    {
        Parry, Defend
    }
}