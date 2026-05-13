using UnityEngine;

namespace FpsDemo.Combat
{
    public interface IAimProvider
    {
        public Ray GetAimRay();
    }
}