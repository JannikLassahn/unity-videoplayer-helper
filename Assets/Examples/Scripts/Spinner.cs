using UnityEngine;

public class Spinner : MonoBehaviour
{
    public RectTransform Progress;
    public float RotationSpeed = 100f;

    private void Update()
    {
        Progress.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }
}
