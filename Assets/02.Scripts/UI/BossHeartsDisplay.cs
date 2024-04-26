using System;
using _02.Scripts.Manager;
using UnityEngine;

namespace _02.Scripts.UI
{
    public class BossHeartsDisplay : MonoBehaviour
    {
        [SerializeField] private Sprite fullHeart, halfHeart, emptyHeart;
        [SerializeField] private SpriteRenderer[] heartSprites;

        private void Awake()
        {
            UpdateHearts(1);
        }

        private void Update()
        {
            UpdateHearts((float)LevelManager.instance.currentBoss.hp / LevelManager.instance.currentBoss.maxHp);
        }

        private void UpdateHearts(float hpRate)
        {
            var scaledHp = hpRate * heartSprites.Length;
            
            for (var i = 0; i < heartSprites.Length; i++)
            {
                var heart = heartSprites[i];
                if (i <= scaledHp - 1f)
                {
                    heart.sprite = fullHeart;
                }
                else if (i < scaledHp - 0.33f || (i == 0 && i < scaledHp))
                {
                    heart.sprite = halfHeart;
                }
                else
                {
                    heart.sprite = emptyHeart;
                }
            }
        }
    }
}