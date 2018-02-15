using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    [Serializable]
    public class FloatEvent : UnityEvent<float> {}

    /// <summary>
    /// 
    /// </summary>
    public class Timeline : Selectable, IDragHandler
    {
        #region Fields

        private Image progressImage;
        private Image previewImage;
        private RectTransform handleContainerRect;
        private Vector2 handleOffset;
        private DrivenRectTransformTracker tracker;

        [SerializeField]
        private RectTransform progressRect;

        [SerializeField]
        private RectTransform previewRect;

        [SerializeField]
        private RectTransform handleRect;

        [SerializeField]
        [Range(0, 1)]
        private float position;

        [SerializeField]
        private FloatEvent onPositionChanged = new FloatEvent();

        [SerializeField]
        private FloatEvent onSeeked = new FloatEvent();

        #endregion

        #region Properties

        public RectTransform ProgressRect
        {
            get { return progressRect; }
            set { progressRect = value; }
        }

        public RectTransform PreviewRect
        {
            get { return previewRect; }
            set { previewRect = value; }
        }

        public RectTransform HandleRect
        {
            get { return handleRect; }
            set { handleRect = value; }
        }

        public float Position
        {
            get { return position; }
            set { SetPosition(value); }
        }

        public FloatEvent OnPositionChanged
        {
            get { return onPositionChanged; }
        }

        public FloatEvent OnSeeked
        {
            get { return onSeeked; }
        }

        #endregion

        #region Unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateReferences();
            Position = position;
            UpdateVisuals();
        }

        #endregion


        private void SetPosition(float newPosition)
        {
            newPosition = Mathf.Clamp01(newPosition);

            if (newPosition == position)
                return;

            position = newPosition;

            UpdateVisuals();
            OnPositionChanged.Invoke(newPosition);
        }

        private void UpdateReferences()
        {
            if (progressRect != null)
                progressImage = progressRect.GetComponent<Image>();

            if (previewRect != null)
                previewImage = previewRect.GetComponent<Image>();

            if (handleRect != null)
                handleContainerRect = handleRect.parent.GetComponent<RectTransform>();
        }

        private void UpdateVisuals()
        {
            UpdateFillableVisuals(progressRect, progressImage, position);
            UpdateFillableVisuals(previewRect, previewImage, position);
            UpdateHandleVisuals();
        }

        private void UpdateHandleVisuals()
        {
            if (handleRect == null)
                return;

            tracker.Add(this, handleRect, DrivenTransformProperties.Anchors);

            var anchorMin = Vector2.zero;
            var anchorMax = Vector2.one;

            anchorMin[0] = anchorMax[0] = position;
            handleRect.anchorMin = anchorMin;
            handleRect.anchorMax = anchorMax;
        }

        private void UpdateFillableVisuals(RectTransform rect, Image image, float value)
        {
            if (rect == null)
                return;

            tracker.Add(this, rect, DrivenTransformProperties.Anchors);

            var anchorMax = Vector2.one;

            if (image != null && image.type == Image.Type.Filled)
                image.fillAmount = value;
            else
                anchorMax[0] = value;

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = anchorMax;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if(handleRect != null)
                handleRect.gameObject.SetActive(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if(handleRect != null)
                handleRect.gameObject.SetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            handleOffset = Vector2.zero;
            if (handleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(handleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(handleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    handleOffset = localMousePos;
            }
            else
            {
                UpdateProgress(eventData, eventData.pressEventCamera);
            }
        }

        private void UpdateProgress(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = handleContainerRect ?? progressRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                Position = Mathf.Clamp01((localCursor - handleOffset)[0] / clickRect.rect.size[0]);
                OnSeeked.Invoke(Position);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            UpdateProgress(eventData, eventData.pressEventCamera);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateVisuals();
        }
#endif
    }

}

