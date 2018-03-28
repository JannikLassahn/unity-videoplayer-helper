using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.VideoHelper
{
    /// <summary>
    /// Forwards clicks when the pointer is pressed. 
    /// Use <see cref="ClickRouter"/> for forwarding upon release of the pointer.
    /// </summary>
    /// <remarks>
    /// Use this component on GameObjects that initiate fullscreen in WebGL builds.
    /// There is a browser limitation that ensures that you can only go fullscreen when a user clicks the page.
    /// Not using <see cref="OnPointerDown(PointerEventData)"/> requires another click for Unity to go into fullscreen.
    /// </remarks>
    public class DirectClickRouter : ClickRouter
    {
        private float lastClickTime = 0f;
        private const float clickInterval = 0.3f;

        public override void OnPointerClick(PointerEventData eventData)
        {
            // do nothing
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (lastClickTime + clickInterval > Time.time)
                OnDoubleClick.Invoke();
            else
                OnClick.Invoke();

            lastClickTime = Time.time;
        }
    }
}

