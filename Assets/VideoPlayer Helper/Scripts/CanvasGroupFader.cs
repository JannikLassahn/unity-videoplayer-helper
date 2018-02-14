using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.VideoHelper
{
    /// <summary>
    /// Animates the alpha value of a <see cref="CanvasGroup"/>.
    /// </summary>
    public class CanvasGroupFader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields

        [SerializeField]
        private AnimationCurve fadeIn = AnimationCurve.Linear(0, 0, .3f, 1);

        [SerializeField]
        private AnimationCurve fadeOut = AnimationCurve.Linear(0, 1, .3f, 0);

        [SerializeField]
        private bool hideOnStart;

        [SerializeField]
        private CanvasGroup group;

        private float time = 0;

        private float fadeInDuration;
        private float fadeOutDuration;

        private bool isFadingIn = false;
        private bool isFadingOut = false;

        #endregion

        #region Properties

        public bool HideOnStart
        {
            get { return hideOnStart; }
            set { hideOnStart = value; }
        }

        public AnimationCurve FadeIn
        {
            get { return fadeIn; }
            set
            {
                if (value == null)
                    return;

                fadeIn = value;
                fadeInDuration = fadeIn.keys[fadeIn.keys.Length - 1].time;
            }
        }

        public AnimationCurve FadeOut
        {
            get { return fadeOut; }
            set
            {
                if (value == null)
                    return;

                fadeOut = value;
                fadeOutDuration = fadeOut.keys[fadeOut.keys.Length - 1].time;
            }
        }

        public CanvasGroup Group
        {
            get { return group; }
            set { group = value; }
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            fadeInDuration = fadeIn.keys[fadeIn.keys.Length - 1].time;
            fadeOutDuration = fadeOut.keys[fadeOut.keys.Length - 1].time;

            if (Group == null)
                Group = GetComponent<CanvasGroup>();

            if (HideOnStart)
                group.alpha = 0;
        }

        private void Update()
        {
            if (isFadingIn)
            {
                if (time > fadeInDuration)
                {
                    isFadingIn = false;
                    return;
                }

                group.alpha = FadeIn.Evaluate(time);
                time += Time.deltaTime;
                return;
            }

            if (isFadingOut)
            {
                if (time > fadeOutDuration)
                {
                    isFadingOut = false;
                    return;
                }

                group.alpha = FadeOut.Evaluate(time);
                time += Time.deltaTime;
            }
        }

        #endregion

        #region IPointerEnter and IPointerExit members

        public void OnPointerEnter(PointerEventData eventData)
        {
            isFadingIn = true;
            time = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isFadingOut = true;
            isFadingIn = false;
            time = 0;
        }

        #endregion

    }
}

