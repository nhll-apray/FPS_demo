using FpsDemo.Config;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Combat
{
    public class PlayerCombatFeedbackAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private AudioClip _hitSound;
        private AudioClip _headshotSound;
        private AudioClip _killSound;

        private void Awake()
        {

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            _hitSound = GameResources.LoadSfx(GameResourcePaths.Audio.Sfx.Damage);
            _headshotSound = GameResources.LoadSfx(GameResourcePaths.Audio.Sfx.Headshot);
            _killSound = GameResources.LoadSfx(GameResourcePaths.Audio.Sfx.Kill);
        }

        private void OnEnable()
        {
            EventManager.AddListener<DamageDealtEvent>(OnDamageDealt);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<DamageDealtEvent>(OnDamageDealt);
        }

        private void OnDamageDealt(DamageDealtEvent eventData)
        {
            DamageResult result = eventData.DamageResult;

            if (result.Attacker != gameObject)
                return;

            if (result.DamageApplied <= 0)
                return;

            PlayFeedback(result);
        }

        private void PlayFeedback(DamageResult result)
        {
            if (result.IsKill && _killSound != null)
            {
                audioSource.PlayOneShot(_killSound);
            }

            if (result.IsHeadshot && _headshotSound != null)
            {
                audioSource.PlayOneShot(_headshotSound);
            }
            else if (_hitSound != null)
            {
                audioSource.PlayOneShot(_hitSound);
            }
        }
    }
}
