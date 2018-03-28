using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    public class ClickRouter : Selectable, IPointerClickHandler
    {
        public UnityEvent OnClick = new UnityEvent();
        public UnityEvent OnDoubleClick = new UnityEvent();

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
                OnDoubleClick.Invoke();
            else
                OnClick.Invoke();
        }
    }

}

