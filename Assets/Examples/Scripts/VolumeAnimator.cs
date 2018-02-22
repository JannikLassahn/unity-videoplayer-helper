using System;
using Unity.VideoHelper.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeAnimator : AnimationCurveAnimator, IPointerExitHandler
{
    public LayoutElement Target;
    public RectTransform Transform;
    public Selectable Selectable;

    private bool isEntered;

    private void Start()
    {
        var router = Transform.gameObject.AddComponent<PointerRouter>();
        router.OnEnter += () =>
        {
            if(!isEntered)
                Animate(In, InDuration, x => Target.preferredWidth = x);
        };
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animate(Out, OutDuration, x => Target.preferredWidth = x);
        isEntered = false;
    }

    protected override void InStarting()
    {
        Selectable.interactable = true;
    }

    protected override void InFinished()
    {
        isEntered = true;
    }

    protected override void OutFinished()
    {
        Selectable.interactable = false;
    }
}

public class PointerRouter : MonoBehaviour, IPointerEnterHandler
{
    public event Action OnEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnter != null)
            OnEnter.Invoke();
    }
}
