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
        private Coroutine currentCoroutine;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the duration of the <see cref="In"/> animation.
        /// </summary>
        public float InDuration { get; private set; }

        /// <summary>
        /// Gets the duration of the <see cref="Out"/> animation.
        /// </summary>
        public float OutDuration { get; private set; }

        /// <summary>
        /// Gets or sets the curve for animating a target in.
        /// </summary>
        public AnimationCurve In
        {
            get { return inAnimation; }
            set
            {
                inAnimation = value;
                InDuration = inAnimation.keys[inAnimation.keys.Length - 1].time;
            }
        }

        /// <summary>
        /// Gets or sets the curve for animating a target out.
        /// </summary>
        public AnimationCurve Out
        {
            get { return outAnimation; }
            set
            {
                outAnimation = value;
                OutDuration = outAnimation.keys[outAnimation.keys.Length - 1].time;
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
            if(currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            if (smooth)
                time = Mathf.Clamp(duration - time, 0, duration);
            else
                time = 0f;

            currentCoroutine = StartCoroutine(AnimateInternal(curve, duration, action));
        }

        protected virtual IEnumerator AnimateInternal(AnimationCurve curve, float duration, Action<float> action)
        {
            CallbackStarting(curve);

            while (time < duration)
            {
                action(curve.Evaluate(time));
                time += Time.deltaTime;
                yield return null;
            }

            CallbackFinished(curve);
        }

        private void CallbackFinished(AnimationCurve curve)
        {
            if (curve == In)
                InFinished();
            else if (curve == Out)
                OutFinished();
        }

        private void CallbackStarting(AnimationCurve curve)
        {
            if (curve == In)
                InStarting();
            else if (curve == Out)
                OutStarting();
        }

        protected virtual void InStarting() { }
        protected virtual void OutStarting() { }
        protected virtual void InFinished() { }
        protected virtual void OutFinished() { }

        #endregion
    }
}
