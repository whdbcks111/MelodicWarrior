using System;
using _02.Scripts.Manager;
using TMPro;
using UnityEngine;

namespace _02.Scripts.Utils
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DebugTextUI : MonoBehaviour
    {

        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if(!LevelManager.instance.isLoaded) return;
            _text.SetText($"Beat {LevelManager.instance.currentBeat}\n" +
                          $"Time {LevelManager.instance.currentPlayTime}");
        }
    }
}