using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    /// <summary>
    /// Helps controlling canvas-based overlays.
    /// </summary>
    public class OverlayController : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;
        private GameObject blocker;

        public GameObject Target
        {
            get { return target; }
            set
            {
                if(value != null && target != value)
                {
                    target = value;
                    SetupTarget();
                }
            }
        }

        private void Start()
        {
            SetupTarget();
        }

        public void ToggleHideOrShow()
        {
            if (blocker == null)
                Show();
            else
                Hide();
        }

        public void Show()
        {
            if (blocker != null)
                return;

            target.SetActive(true);

            var list = new List<Canvas>();
            GetComponentsInParent(false, list);
            if (list.Count == 0)
                return;

            blocker = CreateBlocker(list[0]);
        }

        public void Hide()
        {

            if (blocker == null)
                return;

            Destroy(blocker);
            target.SetActive(false);
            blocker = null;
        }

        private void SetupTarget()
        {
            var targetCanvas = target.GetOrAddComponent<Canvas>();
            targetCanvas.overrideSorting = true;
            targetCanvas.sortingOrder = 1000;

            target.AddComponent<GraphicRaycaster>();
            target.SetActive(false);
        }

        private GameObject CreateBlocker(Canvas root)
        {
            var blocker = new GameObject("Blocker", 
                                typeof(RectTransform), 
                                typeof(Canvas), 
                                typeof(GraphicRaycaster), 
                                typeof(Image),
                                typeof(Button));

            var blockerRect = blocker.GetComponent<RectTransform>();
            blockerRect.SetParent(root.transform, false);
            blockerRect.anchorMin = Vector2.zero;
            blockerRect.anchorMax = Vector2.one;
            blockerRect.sizeDelta = Vector2.zero;

            var overlayCanvas = target.GetComponent<Canvas>();
            var blockerCanvas = blocker.GetComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingLayerID = overlayCanvas.sortingLayerID;
            blockerCanvas.sortingOrder = overlayCanvas.sortingOrder - 1;

            blocker.GetComponent<Image>().color = Color.clear;
            blocker.GetComponent<Button>().onClick.AddListener(Hide);

            return blocker;
        }
    }
}
