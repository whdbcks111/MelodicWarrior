using System;
using _02.Scripts.Manager;
using UnityEngine;

namespace _02.Scripts.Level
{
    public class BeatLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer linePrefab;
        [SerializeField] private Color beatColor, measureColor;
        
        private LineRenderer[] _lines = null;
        
        private void Awake()
        {
        }

        private void Update()
        {
            var cam = Camera.main;
            if (cam == null) throw new Exception("Main Camera not found");
            if (LevelManager.instance == null || LevelManager.instance.currentLevel == null) return;

            var level = LevelManager.instance.currentLevel;
            var curBeat = LevelManager.instance.currentBeat;
            
            var leftBottom = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var rightTop = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

            var lineCount = (int)((rightTop.x - leftBottom.x) / level.baseScrollSpeed + 2);

            if (_lines == null)
            {
                _lines = new LineRenderer[lineCount];
                for (var i = 0; i < lineCount; i++)
                {
                    _lines[i] = Instantiate(linePrefab, transform);
                    _lines[i].name = $"Line {i + 1}";
                    _lines[i].positionCount = 2;
                }
            }
            
            for (var i = 0; i < lineCount; i++)
            {
                var leftLineCount = (int)(-leftBottom.x / level.baseScrollSpeed);
                var x = (-leftLineCount + i + -(curBeat % 1)) * level.baseScrollSpeed;
                
                var top = new Vector3(x, rightTop.y + 1);
                var bottom = new Vector2(x, leftBottom.y - 1);
                
                _lines[i].SetPosition(0, top);
                _lines[i].SetPosition(1, bottom);
                
                if ((level.beatsPerMeasure - (int)curBeat % level.beatsPerMeasure) % level.beatsPerMeasure ==
                    (i - leftLineCount + level.beatsPerMeasure) % level.beatsPerMeasure)
                {
                    _lines[i].startColor = _lines[i].endColor = measureColor;
                }
                else
                {
                    _lines[i].startColor = _lines[i].endColor = beatColor;
                }
            }
        }
    }
}