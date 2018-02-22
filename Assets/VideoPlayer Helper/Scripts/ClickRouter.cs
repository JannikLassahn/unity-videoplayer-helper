using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Unity.VideoHelper
{
    public class ClickRouter : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnClick = new UnityEvent();
        public UnityEvent OnDoubleClick = new UnityEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
                OnDoubleClick.Invoke();
            else
                OnClick.Invoke();
        }
    }

}

