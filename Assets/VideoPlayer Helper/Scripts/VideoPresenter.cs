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

        #endregion

        #region Public Fields

        [Header("Controls")]

        public Timeline Timeline;
        public Slider Volume;
        public Image PlayPause;
        public Image MuteUnmute;
        public Image SmallFullscreen;
        public Text Current;
        public Text Duration;

        [Header("Input")]

        public KeyCode FullscreenKey = KeyCode.F;
        public KeyCode WindowedKey = KeyCode.Escape;
        public KeyCode TogglePlayKey = KeyCode.Space;

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

            Volume.onValueChanged
                .AddListener(OnVolumeChanged);

            PlayPause.gameObject.AddComponent<ClickRouter>()
                .OnClick.AddListener(ToggleIsPlaying);

            SmallFullscreen.gameObject.AddComponent<ClickRouter>()
                .OnClick.AddListener(ToggleFullscreen);

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

        private void Update()
        {
            CheckKeys();            
        }

        #endregion

        #region Private methods

        private void CheckKeys()
        {
            if (Input.GetKeyDown(FullscreenKey))
            {
                ScreenSizeHelper.GoFullscreen(gameObject.transform as RectTransform);
            }
            if (Input.GetKeyDown(WindowedKey))
            {
                ScreenSizeHelper.GoWindowed();
            }
            if (Input.GetKeyDown(TogglePlayKey))
            {
                ToggleIsPlaying();
            }
        }

        private void ToggleIsPlaying()
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
        }

        private void OnStartedPlaying()
        {
            Duration.text = PrettyTimeFormat(TimeSpan.FromSeconds(controller.Duration));
        }

        private void OnTimeLinePositionChanged(float position)
        {
            Timeline.Position = position;
            Current.text = PrettyTimeFormat(TimeSpan.FromSeconds(controller.Time));
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

        private void ToggleFullscreen()
        {
            if (ScreenSizeHelper.IsFullscreen)
                ScreenSizeHelper.GoWindowed();
            else
                ScreenSizeHelper.GoFullscreen(gameObject.transform as RectTransform);
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

