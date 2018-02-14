using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    [Serializable]
    public class VolumeStore
    {
        public float Minimum;
        public Sprite Sprite;
    }

    /// <summary>
    /// Handles UI state for the video player.
    /// </summary>
    public class VideoPresenter : MonoBehaviour
    {
        #region Consts

        private const string MinutesFormat = "{0:00}:{1:00}";
        private const string HoursFormat = "{0:00}:{1:00}:{2:00}";

        #endregion

        #region Private Fields

        private VideoController controller;
        private bool isFullscreen;

        #endregion

        #region Public Fields

        [Header("Controls")]

        public Slider Timeline;
        public Slider Volume;
        public Image PlayPause;
        public Image MuteUnmute;
        public Image SmallFullscreen;
        public Text Current;
        public Text Duration;

        [Header("Content")]

        public Sprite Play;
        public Sprite Pause;
        public Sprite Normal;
        public Sprite Fullscreen;

        [Space(10)]

        public VolumeStore[] Volumes = new VolumeStore[0];

        [Space(10)]

        public Sprite TimelineBackground;
        public Sprite TimelineForeground;
        public Sprite TimelineKnob;

        [Space(10)]

        public Sprite VolumeBackground;
        public Sprite VolumeForeground;
        public Sprite VolumeKnob;

        [Header("Animation")]
        public AnimationCurve FadeIn = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve FadeOut = AnimationCurve.Linear(0, 1, 1, 0);

        #endregion

        #region Unity methods

        private void Start()
        {
            controller = GetComponent<VideoController>();
            controller.TimelinePositionChanged.AddListener(OnTimeLinePositionChanged);
            controller.StartedPlaying.AddListener(OnStartedPlaying);

            Volume.onValueChanged.AddListener(OnVolumeChanged);

            PlayPause.gameObject.AddComponent<ClickRouter>().OnClick.AddListener(() =>
            {
                if (controller.IsPlaying)
                {
                    controller.Pause();
                    PlayPause.sprite = Play;
                }
                else
                {
                    controller.Play();
                    PlayPause.sprite = Pause;
                }
            });

            SmallFullscreen.gameObject.AddComponent<ClickRouter>().OnClick.AddListener(() =>
            {
                if(isFullscreen)
                {
                    SmallFullscreen.sprite = Normal;
                }
                else
                {
                    SmallFullscreen.sprite = Fullscreen;
                }

                isFullscreen = !isFullscreen;
                Screen.fullScreen = isFullscreen;
            });

            Array.Sort(Volumes, (v1, v2) =>
            {
                if (v1.Minimum > v2.Minimum)
                    return 1;
                else if (v1.Minimum == v2.Minimum)
                    return 0;
                else
                    return -1;
            });
        }

        private void OnStartedPlaying()
        {
            Duration.text = PrettyTimeFormat(TimeSpan.FromSeconds(controller.Duration));
        }

        private void OnTimeLinePositionChanged(float position)
        {
            Timeline.value = position;
            Current.text = PrettyTimeFormat(TimeSpan.FromSeconds(controller.Time));
        }

        private void OnTimelineValueChanged(float time)
        {
            controller.Seek(time);
        }

        private void OnVolumeChanged(float volume)
        {
            controller.Volume = volume;

            for (int i = 0; i < Volumes.Length; i++)
            {
                var current = Volumes[i];
                var next = Volumes.Length - 1 >= i + 1 ? Volumes[i + 1].Minimum : 2;

                if (current.Minimum <= volume && next > volume)
                {
                    MuteUnmute.sprite = current.Sprite;
                }
            }
        }


        private string PrettyTimeFormat(TimeSpan time)
        {
            if (time.TotalHours <= 1)
                return string.Format(MinutesFormat, time.Minutes, time.Seconds);
            else
                return string.Format(HoursFormat, time.Hours, time.Minutes, time.Seconds);
        }

        #endregion

    }

}

