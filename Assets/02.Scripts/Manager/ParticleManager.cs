using System.Collections.Generic;
using _02.Scripts.Level;
using _02.Scripts.Level.Note;
using UnityEngine;
using UnityEngine.Serialization;

namespace _02.Scripts.Manager
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager instance { get; private set; }

        [SerializeField] private ParticleSystem splitParticle;
        [SerializeField] private ParticleSystem hitParticle;
        
        public ParticleSystem defendParticle;
        public ParticleSystem attackParticle;

        private readonly Dictionary<string, (ParticleSystem, ParticleSystem)> _noteSplitParticleMap = new();
        private readonly Dictionary<string, ParticleSystem> _noteParryParticleMap = new();
        
        private void Awake()
        {
            instance = this;
        }

        public void PlayParticle(ParticleSystem origin, Vector3 pos)
        {
            var sprite = origin.textureSheetAnimation.GetSprite(0);
            if (sprite)
            {
                var main = origin.main;
                main.startSize = new ParticleSystem.MinMaxCurve(sprite.rect.width / sprite.pixelsPerUnit);
            }
            
            origin.transform.position = pos;
            origin.Play();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SpawnSplitParticle(NoteObject noteObj)
        {
            (ParticleSystem, ParticleSystem) particles;
            if(!_noteSplitParticleMap.ContainsKey(noteObj.note.noteType))
            {
                particles = _noteSplitParticleMap[noteObj.note.noteType] = (Instantiate(splitParticle, transform), Instantiate(splitParticle, transform));

                particles.Item1.transform.eulerAngles = new Vector3(90, 0, 0);
                particles.Item2.transform.eulerAngles = new Vector3(-90, 0, 0);
                
                var main1 = particles.Item1.main;
                main1.startSize = new ParticleSystem.MinMaxCurve(noteObj.spriteRenderer.sprite.rect.width / noteObj.spriteRenderer.sprite.pixelsPerUnit);
                
                var main2 = particles.Item2.main;
                main2.startSize = new ParticleSystem.MinMaxCurve(noteObj.spriteRenderer.sprite.rect.width / noteObj.spriteRenderer.sprite.pixelsPerUnit);
                
                particles.Item1.textureSheetAnimation.SetSprite(0, noteObj.spriteRenderer.sprite);
                particles.Item2.textureSheetAnimation.SetSprite(0, noteObj.spriteRenderer.sprite);
            
                particles.Item1.GetComponent<Renderer>().material = noteObj.spriteRenderer.material;
                particles.Item2.GetComponent<Renderer>().material = noteObj.spriteRenderer.material;
            }
            particles = _noteSplitParticleMap[noteObj.note.noteType];
            
            particles.Item1.transform.position = particles.Item2.transform.position = noteObj.transform.position;
            particles.Item1.Play();
            particles.Item2.Play();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void SpawnHitParticle(NoteObject noteObj)
        {
            ParticleSystem particle;
            if(!_noteParryParticleMap.ContainsKey(noteObj.note.noteType))
            {
                particle = _noteParryParticleMap[noteObj.note.noteType] = Instantiate(hitParticle, transform);

                var main = particle.main;
                main.startSize = new ParticleSystem.MinMaxCurve(noteObj.spriteRenderer.sprite.rect.width / noteObj.spriteRenderer.sprite.pixelsPerUnit);
                particle.transform.eulerAngles = new Vector3(0, 90, 0);
                particle.textureSheetAnimation.SetSprite(0, noteObj.spriteRenderer.sprite);
                particle.GetComponent<Renderer>().material = noteObj.spriteRenderer.material;
            }
            particle = _noteParryParticleMap[noteObj.note.noteType];
            
            particle.transform.position = noteObj.transform.position;
            particle.Play();
        }
    }
}