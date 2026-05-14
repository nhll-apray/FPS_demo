using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Combat
{
    public class PlayerCombatFeedbackAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip headshotSound;
        [SerializeField] private AudioClip killSound;

        private void Awake()
        {

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
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
            DamageResult result = eventData.damageResult;
            
            Debug.Log(result.attacker);
            Debug.Log(result.damageApplied);

            if (result.attacker != gameObject)
                return;

            if (result.damageApplied <= 0)
                return;

            PlayFeedback(result);
        }

        private void PlayFeedback(DamageResult result)
        {
            if (result.isKill && killSound != null)
            {
                audioSource.PlayOneShot(killSound);
            }

            if (result.isHeadshot && headshotSound != null)
            {
                audioSource.PlayOneShot(headshotSound);
            }
            else if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }
}