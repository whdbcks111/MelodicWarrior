using System;
using System.Collections.Generic;
using _02.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts.Level
{
    public class Player : MonoBehaviour
    {
        private readonly Queue<NoteObject> _triggeredNotes = new();

        private SpriteRenderer _renderer;
        private Animator _animator;
        
        private static readonly int AnimParry = Animator.StringToHash("Parry");
        private static readonly int AnimDefend = Animator.StringToHash("Defend");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Parry();   
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Defend();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(LevelManager.instance.isPlaying)
                    LevelManager.instance.Pause();
                else 
                    LevelManager.instance.Resume();
            }
        }

        public void Parry()
        {
            _animator.SetTrigger(AnimParry);
            if (_triggeredNotes.Count > 0 && _triggeredNotes.Peek().hitType == HitType.Parry)
            {
                _triggeredNotes.Dequeue().Hit();
            }
            else
            {
                
            }
        }

        public void Defend()
        {
            _animator.SetTrigger(AnimDefend);
            if (_triggeredNotes.Count > 0 && _triggeredNotes.Peek().hitType == HitType.Defend)
            {
                _triggeredNotes.Dequeue().Hit();
            }
            else
            { 
                
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out NoteObject noteObject)) return;
            
            _triggeredNotes.Enqueue(noteObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out NoteObject noteObject) && _triggeredNotes.Contains(noteObject))
            {
                _triggeredNotes.Dequeue();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}