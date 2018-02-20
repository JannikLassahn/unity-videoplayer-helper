using UnityEngine;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    internal static class ScreenSizeHelper
    {
        #region Fields

        private static Vector2 anchorMin, anchorMax, offsetMin, offsetMax;
        private static Vector3 scale;

        private static GameObject fullscreenCanvas;
        private static RectTransform target, targetParent;

        private static bool IsAlwaysFullscreen;

        #endregion

        #region Properties

        public static bool IsFullscreen
        {
            get { return fullscreenCanvas != null && fullscreenCanvas.activeSelf; }
        }

        #endregion

        #region Methods

        public static void GoFullscreen(RectTransform rectTransform)
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

            IsAlwaysFullscreen = Screen.fullScreen;
            Screen.fullScreen = true;
        }

        public static void GoWindowed()
        {
            target.SetParent(targetParent);

            target.anchorMax = anchorMax;
            target.anchorMin = anchorMin;
            target.offsetMax = offsetMax;
            target.offsetMin = offsetMin;
            target.localScale = scale;

            fullscreenCanvas.SetActive(false);

            Screen.fullScreen = IsAlwaysFullscreen;
        }

        #endregion  

        #region Private methods

        private static void Setup()
        {
            fullscreenCanvas = new GameObject("_VideoPresenter_Fullscreen", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

            var canvas = fullscreenCanvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = fullscreenCanvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        }

        #endregion

    }
}
