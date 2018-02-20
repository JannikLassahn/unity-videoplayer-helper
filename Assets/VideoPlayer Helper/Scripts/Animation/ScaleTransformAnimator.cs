using UnityEngine;

namespace Unity.VideoHelper.Animation
{
    /// <summary>
    /// Animates <see cref="Transform.localScale"/>.
    /// </summary>
    public class ScaleTransformAnimator : AnimationCurveAnimator, IActivate, IDeactivate
    {
        public Transform Target;

        public void Activate()
        {
            Animate(In, InDuration, x => Target.localScale = new Vector3(x, x, x));
        }

        public void Deactivate()
        {
            Animate(Out, OutDuration, x => Target.localScale = new Vector3(x, x, x));
        }
    }
}
