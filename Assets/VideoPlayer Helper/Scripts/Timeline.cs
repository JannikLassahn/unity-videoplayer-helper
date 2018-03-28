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
    /// A slider-like component specialized in media playback.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class Timeline : Selectable, IDragHandler, ICanvasElement, IInitializePotentialDragHandler
    {
        #region Fields

        private Image positionImage, previewImage;
        private Text tooltipText;
        private RectTransform positionContainerRect, handleContainerRect, tooltipContainerRect;
        private Vector2 handleOffset;
        private Camera cam;
        private DrivenRectTransformTracker tracker;
        private float previewPosition;
        private float stepSize = 0.05f;
        private bool isInControl;
        private ITimelineProvider provider;

        [SerializeField]
        private RectTransform positionRect, previewRect, handleRect, tooltipRect;

        [SerializeField]
        [Range(0, 1)]
        private float position;

        [SerializeField]
        private FloatEvent onSeeked = new FloatEvent();

        #endregion

        #region Properties

        public RectTransform PositionRect
        {
            get { return positionRect; }
            set { positionRect = value; }
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

        public RectTransform TooltipRect
        {
            get { return tooltipRect; }
            set { tooltipRect = value; }
        }

        public float Position
        {
            get { return position; }
            set { SetPosition(value); }
        }

        public UnityEvent<float> OnSeeked
        {
            get { return onSeeked; }
        }

        #endregion

        #region Unity methods

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateReferences();
            SetPosition(position, false);
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            tracker.Clear();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if(IsActive())
                UpdateVisuals();
        }

        private void Update()
        {
            if (!isInControl)
                return;

            var newPreviewPosition = GetPreviewPoint();
            if (newPreviewPosition == previewPosition)
                return;
            else
                previewPosition = newPreviewPosition;

            UpdateFillableVisuals(previewRect, previewImage, previewPosition);
            UpdateAnchorBasedVisuals(tooltipRect, previewPosition);

            tooltipText.text = provider.GetFormattedPosition(previewPosition);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (IsActive())
            {
                UpdateReferences();
                SetPosition(position, false);
                UpdateVisuals();
            }

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif

        #endregion

        #region Methods

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            Func<bool> isAutomatic = () => navigation.mode == Navigation.Mode.Automatic;

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (isAutomatic())
                        Move(position - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (isAutomatic())
                        Move(position + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                case MoveDirection.Up:
                    base.OnMove(eventData);
                    break;
            }
        }

        private void Move(float value)
        {
            SetPosition(value);
            onSeeked.Invoke(value);
        }

        private void UpdateReferences()
        {
            if (positionRect)
            {
                positionImage = positionRect.GetComponent<Image>();
                positionContainerRect = positionRect.parent.GetComponent<RectTransform>();
            }
            else
            {
                positionRect = null;
                positionImage = null;
                positionContainerRect = null;
            }

            if (previewRect)
            {
                previewImage = previewRect.GetComponent<Image>();
            }
            else
            {
                previewRect = null;
                previewImage = null;
            }

            if (handleRect)
            {
                handleContainerRect = handleRect.parent.GetComponent<RectTransform>();
            }
            else
            {
                handleRect = null;
                handleContainerRect = null;
            }

            if (tooltipRect)
            {
                tooltipContainerRect = tooltipRect.parent.GetComponent<RectTransform>();
                tooltipText = tooltipRect.GetComponentInChildren<Text>();
            }
            else
            {
                tooltipRect = null;
                tooltipContainerRect = null;
            }

            cam = Camera.main;
            provider = GetComponentInParent<VideoPresenter>();
        }

        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateReferences();
#endif

            tracker.Clear();

            if (positionContainerRect)
                UpdateFillableVisuals(positionRect, positionImage, position);

            if(!isInControl)
                UpdateFillableVisuals(previewRect, previewImage, position);

            if (handleContainerRect)
                UpdateAnchorBasedVisuals(handleRect, position);
        }

        protected void UpdateAnchorBasedVisuals(RectTransform rect, float position)
        {
            if (rect == null)
                return;

            tracker.Add(this, rect, DrivenTransformProperties.Anchors);

            var anchorMin = Vector2.zero;
            var anchorMax = Vector2.one;

            anchorMin[0] = anchorMax[0] = position;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }

        protected void UpdateFillableVisuals(RectTransform rect, Image image, float value)
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

        private bool CanDrag()
        {
            return IsActive() && IsInteractable();
        }

        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = handleContainerRect ?? positionContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                Position = Mathf.Clamp01((localCursor - handleOffset)[0] / clickRect.rect.size[0]);
                onSeeked.Invoke(Position);
            }
        }

        private float GetPreviewPoint()
        {
            Vector2 screenMousePosition = RectTransformUtility.WorldToScreenPoint(cam, Input.mousePosition);
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipContainerRect, screenMousePosition, cam, out localMousePosition);
            localMousePosition -= tooltipContainerRect.rect.position;

            return Mathf.Clamp01((localMousePosition[0]) / tooltipContainerRect.rect.size[0]);
        }

        private void SetPosition(float newPosition, bool sendCallback = true)
        {
            newPosition = Mathf.Clamp01(newPosition);

            if (position == newPosition)
                return;

            position = newPosition;

            UpdateVisuals();
        }

        #endregion

        #region IPointerEnter, IPointerExit, IPointerDown, IDragHandler members

        public override void OnPointerEnter(PointerEventData eventData)
        {
            isInControl = true;

            if (handleRect != null)
                SetActive(handleRect.gameObject, true);

            if (tooltipRect != null)
                tooltipRect.gameObject.SetActive(true);
        }

        private void SetActive(GameObject gameObject, bool value)
        {
            if (gameObject == null)
                return;

            if (value)
            {
                var activate = gameObject.GetComponent<IActivate>();
                if (activate != null)
                {
                    activate.Activate();
                    return;
                }
            }
            else
            {
                var deactivate = gameObject.GetComponent<IDeactivate>();
                if (deactivate != null)
                {
                    deactivate.Deactivate();
                    return;
                }
            }
           
            gameObject.SetActive(value);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            isInControl = false;

            if (handleRect != null)
                SetActive(handleRect.gameObject, false);

            if (tooltipRect != null)
                tooltipRect.gameObject.SetActive(false);

            UpdateFillableVisuals(previewRect, previewImage, 0);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDrag())
                return;

            base.OnPointerDown(eventData);

            handleOffset = Vector2.zero;
            if (handleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(handleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(handleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    handleOffset = localMousePos;
                handleOffset.y = -handleOffset.y;
            }
            else
            {
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag())
                return;

            isInControl = true;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        #endregion

        #region ICanvasElement members

        public void Rebuild(CanvasUpdate executing)
        {
        }

        public void LayoutComplete()
        {
        }

        public void GraphicUpdateComplete()
        {
        }

        #endregion

        #region IInitializePotentialDragHandler members

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        #endregion

    }

}

