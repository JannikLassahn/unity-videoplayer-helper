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

    /// <summary>
    /// Controls state of a transform for a display.
    /// </summary>
    public interface IDisplayController
    {
        /// <summary>
        /// Gets whether the content is presented fullscreen.
        /// </summary>
        bool IsFullscreen { get; }

        /// <summary>
        /// Makes the target fill the whole display.
        /// </summary>
        /// <param name="target">The tranform to make fullscreen.</param>
        void ToFullscreen(RectTransform target);

        /// <summary>
        /// Resets a target to its original state before <see cref="ToFullscreen(RectTransform)"/>.
        /// </summary>
        void ToNormal();
    }

}
