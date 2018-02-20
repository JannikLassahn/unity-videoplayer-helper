using System;
using System.Collections;
using UnityEngine;

namespace Unity.VideoHelper.Animation
{
    /// <summary>
    /// Base class for simple in /out animations using <see cref="AnimationCurve"/>.
    /// </summary>
    public abstract class AnimationCurveAnimator : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private AnimationCurve inAnimation = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField]
        private AnimationCurve outAnimation = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [SerializeField]
        private bool smooth = true;

        private float time;

        private float inDuration;
        private float outDuration;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the duration of the <see cref="In"/> animation.
        /// </summary>
        public float InDuration
        {
            get { return inDuration; }
        }

        /// <summary>
        /// Gets the duration of the <see cref="Out"/> animation.
        /// </summary>
        public float OutDuration
        {
            get { return outDuration; }
        }

        public AnimationCurve In
        {
            get { return inAnimation; }
            set
            {
                inAnimation = value;
                inDuration = inAnimation.keys[inAnimation.keys.Length - 1].time;
            }
        }

        public AnimationCurve Out
        {
            get { return outAnimation; }
            set
            {
                outAnimation = value;
                outDuration = outAnimation.keys[outAnimation.keys.Length - 1].time;
            }
        }

        /// <summary>
        /// /Gets or sets whether to smooth transitions if there is already running an animation.
        /// </summary>
        public bool Smooth
        {
            get { return smooth; }
            set { smooth = value; }
        }

        #endregion

        #region Unity methods

        protected virtual void OnEnable()
        {
            In = inAnimation;
            Out = outAnimation;
        }

        #endregion

        #region Methods

        protected void Animate(AnimationCurve curve, float duration, Action<float> action)
        {
            StopAllCoroutines();

            if (smooth)
                time = Mathf.Clamp(duration - time, 0, duration);
            else
                time = 0f;

            StartCoroutine(AnimateInternal(curve, duration, action));
        }

        private IEnumerator AnimateInternal(AnimationCurve curve, float duration, Action<float> action)
        {
            while (time < duration)
            {
                action(curve.Evaluate(time));
                time += Time.deltaTime;
                yield return null;
            }
        }

        #endregion
    }

    /// <summary>
    /// Experimental generic animator.
    /// </summary>
    /// <example>
    /// 
    /// This example shows how to set up control over the alpha value of the CanvasGroup.
    /// 
    ///     var myAnimator = new AnimationCurveAnimator<CanvasGroup>();
    ///     myAnimator.SetProperty(group => { return y => group.alpha = y; });
    /// 
    /// To start the animations:
    ///     myAnimator.AnimateIn();
    ///     myAnimator.AnimateOut();
    /// 
    /// </example>
    /// <typeparam name="TTarget">The target.</typeparam>
    public class AnimationCurveAnimator<TTarget> : AnimationCurveAnimator
    {
        private Func<TTarget, Action<float>> targetProperty;

        public TTarget Target;

        public void SetProperty(Func<TTarget, Action<float>> func)
        {
            targetProperty = func;
        }

        public void AnimateIn()
        {
            Animate(In, InDuration, targetProperty(Target));
        }

        public void AnimateOut()
        {
            Animate(Out, OutDuration, targetProperty(Target));
        }
    }
}
