using UnityEngine;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    public class VideoPresenter : MonoBehaviour
    {
        #region Fields

        [Header("Controls")]

        public Slider Timeline;
        public Toggle PlayPause;
        public Toggle MuteUnmute;
        public Toggle SmallFullscreen;

        [Header("Content")]

        public Sprite Play;
        public Sprite Pause;
        public Sprite Muted;
        public Sprite Unmuted;
        public Sprite Small;
        public Sprite Fullscreen;

        [Space(10)]

        public Sprite TimelineBackground;
        public Sprite TimelineForeground;
        public Sprite TimelineKnob;

        [Space(10)]

        public Sprite VolumeBackground;
        public Sprite VolumeForeground;
        public Sprite VolumeKnob;

        #endregion

        #region Unity methods

        private void Start()
        {
        }

        #endregion

    }

}

