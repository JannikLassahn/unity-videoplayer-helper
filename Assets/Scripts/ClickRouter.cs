using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Unity.VideoHelper
{
    public class ClickRouter : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
        }
    }

}

