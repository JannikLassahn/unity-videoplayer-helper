using UnityEngine;

namespace Unity.VideoHelper
{
    /// <summary>
    /// Provides information for a timeline.
    /// </summary>
    public interface ITimelineProvider
    {
        string GetFormattedPosition(float normalizedTime);
    }

    /// <summary>
    /// Provides custom activation logic around <see cref="GameObject.SetActive(bool)"/>.
    /// </summary>
    public interface IActivate
    {
        void Activate();
    }

    /// <summary>
    /// Provides custom deactivation logic around <see cref="GameObject.SetActive(bool)"/>.
    /// </summary>
    public interface IDeactivate
    {
        void Deactivate();
    }

}
