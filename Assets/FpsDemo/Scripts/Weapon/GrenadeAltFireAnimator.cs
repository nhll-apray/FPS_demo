using UnityEngine;

namespace FpsDemo.Weapon
{
    public class GrenadeAltFireAnimator : MonoBehaviour
    {
        private static readonly int ThrowGrenade = Animator.StringToHash("ThrowGrenade");
        private static readonly int IsAltFiring = Animator.StringToHash("IsAltFiring");

        [SerializeField] private Animator animator;
        [SerializeField] private GrenadeAltFire grenadeAltFire;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            if (grenadeAltFire == null)
                grenadeAltFire = GetComponent<GrenadeAltFire>();
        }

        private void OnEnable()
        {
            grenadeAltFire.OnStarted += OnStarted;
            grenadeAltFire.OnFinished += OnFinished;
        }

        private void OnDisable()
        {
            grenadeAltFire.OnStarted -= OnStarted;
            grenadeAltFire.OnFinished -= OnFinished;
        }

        private void OnStarted()
        {
            animator.SetTrigger(ThrowGrenade);
            animator.SetBool(IsAltFiring, true);
        }

        private void OnFinished()
        {
            animator.SetBool(IsAltFiring, false);
        }
    }
}