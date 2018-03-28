using UnityEngine;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    /// <summary>
    /// Factory for getting instances of <see cref="IDisplayController"/>.
    /// </summary>
    public static class DisplayController
    {
        private static readonly IDisplayController[] helper = new DisplayControllerInternal[Display.displays.Length];

        /// <summary>
        /// Gets the controller for display 0 (default).
        /// </summary>
        public static IDisplayController Default
        {
            get { return ForDisplay(0); }
        }

        /// <summary>
        /// Gets the controller for a display.
        /// </summary>
        /// <param name="display">The display index.</param>
        /// <returns></returns>
        public static IDisplayController ForDisplay(int display)
        {
            if (helper[display] == null)
                helper[display] = new DisplayControllerInternal(display);

            return helper[display];
        }
    }

    class DisplayControllerInternal : IDisplayController
    {
        internal DisplayControllerInternal(int display)
        {
            targetDisplay = display;
        }

        #region Fields

        private Vector2 anchorMin, anchorMax, offsetMin, offsetMax;
        private Vector3 scale;

        private GameObject fullscreenCanvas;
        private RectTransform target, targetParent;

        private bool isAlwaysFullscreen;
        private int targetDisplay;

        #endregion

        #region Properties

        public bool IsFullscreen
        {
            get { return fullscreenCanvas != null && fullscreenCanvas.activeSelf; }
        }

        #endregion

        #region Methods

        public void ToFullscreen(RectTransform rectTransform)
        {
            if (fullscreenCanvas == null)
                Setup();

            target = rectTransform;
            targetParent = target.parent as RectTransform;

            anchorMax = target.anchorMax;
            anchorMin = target.anchorMin;

            offsetMax = target.offsetMax;
            offsetMin = target.offsetMin;

            scale = target.localScale;

            fullscreenCanvas.SetActive(true);
            target.SetParent(fullscreenCanvas.transform);

            target.anchorMin = target.offsetMin = Vector2.zero;
            target.anchorMax = target.offsetMax = Vector2.one;
            target.localScale = Vector3.one;

            isAlwaysFullscreen = Screen.fullScreen;
            Screen.fullScreen = true;
        }

        public void ToNormal()
        {
            if (target == null)
                return;

            target.SetParent(targetParent);

            target.anchorMax = anchorMax;
            target.anchorMin = anchorMin;
            target.offsetMax = offsetMax;
            target.offsetMin = offsetMin;
            target.localScale = scale;

            fullscreenCanvas.SetActive(false);

            Screen.fullScreen = isAlwaysFullscreen;
        }

        #endregion

        #region Private methods

        private void Setup()
        {
            fullscreenCanvas = new GameObject("_DisplayController_ForDisplay_" + targetDisplay, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

            var canvas = fullscreenCanvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.targetDisplay = targetDisplay;
            
            var scaler = fullscreenCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        }

        #endregion

    }
}
