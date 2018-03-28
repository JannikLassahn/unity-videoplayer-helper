using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    [Serializable]
    public class VolumeInfo
    {
        public float Minimum;
        public Sprite Sprite;
    }

    /// <summary>
    /// Handles UI state for the video player.
    /// </summary>
    [RequireComponent(typeof(VideoController))]
    public class VideoPresenter : MonoBehaviour, ITimelineProvider
    {
        #region Consts

        private const string MinutesFormat = "{0:00}:{1:00}";
        private const string HoursFormat = "{0:00}:{1:00}:{2:00}";

        #endregion

        #region Private Fields

        private IDisplayController display;
        private VideoController controller;
        private float previousVolume;

        #endregion

        #region Public Fields

        [Header("Controls")]
        public Transform Screen;
        public Transform ControlsPanel;
        public Transform LoadingIndicator;
        public Transform Thumbnail;
        public Timeline Timeline;
        public Slider Volume;
        public Image PlayPause;
        public Image MuteUnmute;
        public Image NormalFullscreen;
        public Text Current;
        public Text Duration;

        [SerializeField]
        private int targetDisplay = 0;

        [Header("Input")]

        public KeyCode FullscreenKey = KeyCode.F;
        public KeyCode WindowedKey = KeyCode.Escape;
        public KeyCode TogglePlayKey = KeyCode.Space;

        [Space(10)]
        public bool ToggleScreenOnDoubleClick = true;
        public bool TogglePlayPauseOnClick = true;

        [Header("Content")]

        public Sprite Play;
        public Sprite Pause;

        public Sprite Normal;
        public Sprite Fullscreen;
        
        public VolumeInfo[] Volumes = new VolumeInfo[0];

        #endregion

        #region Properties

        public int TargetDisplay
        {
            get { return targetDisplay; }
            set
            {
                targetDisplay = value;
                display = DisplayController.ForDisplay(targetDisplay);
            }
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            controller = GetComponent<VideoController>();

            if (controller == null)
            {
                Debug.Log("There is no video controller attached.");
                DestroyImmediate(this);
                return;
            }

            controller.OnStartedPlaying.AddListener(OnStartedPlaying);

            Volume.onValueChanged.AddListener(OnVolumeChanged);

            Thumbnail.OnClick(Prepare);
            MuteUnmute.OnClick(ToggleMute);

            PlayPause.OnClick(ToggleIsPlaying);
            NormalFullscreen.OnClick(ToggleFullscreen);


#if UNITY_WEBGL
            // FIX for not being able to go fullscreen in WebGL. See DirectClickRouter for details.
            Screen.gameObject.AddComponent<DirectClickRouter>();
#endif

            Screen.OnDoubleClick(ToggleFullscreen);
            Screen.OnClick(ToggleIsPlaying);

            ControlsPanel.SetGameObjectActive(false);
            LoadingIndicator.SetGameObjectActive(false);
            Thumbnail.SetGameObjectActive(true);

            TargetDisplay = targetDisplay;

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

            if (controller.IsPlaying)
                Timeline.Position = controller.NormalizedTime;
        }

#endregion

        public string GetFormattedPosition(float time)
        {
            return PrettyTimeFormat(TimeSpan.FromSeconds(time * controller.Duration));
        }

#region Private methods

        private void ToggleMute()
        {
            if (Volume.value == 0)
                Volume.value = previousVolume;
            else
            {
                previousVolume = Volume.value;
                Volume.value = 0f;
            }
        }

        private void Prepare()
        {
            Thumbnail.SetGameObjectActive(false);
            LoadingIndicator.SetGameObjectActive(true);
        }

        private void CheckKeys()
        {
            if (Input.GetKeyDown(FullscreenKey))
            {
                display.ToFullscreen(gameObject.transform as RectTransform);
            }
            if (Input.GetKeyDown(WindowedKey))
            {
                display.ToNormal();
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
            Screen.SetGameObjectActive(true);
            ControlsPanel.SetGameObjectActive(true);
            LoadingIndicator.SetGameObjectActive(false);

            if(Duration != null)
                Duration.text = PrettyTimeFormat(TimeSpan.FromSeconds(controller.Duration));

            StartCoroutine(SetCurrentPosition());

            Volume.value = controller.Volume;
            NormalFullscreen.sprite = Fullscreen;
            PlayPause.sprite = Pause;
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
            if (display.IsFullscreen)
            {
                display.ToNormal();
                NormalFullscreen.sprite = Fullscreen;
            }
            else
            {
                display.ToFullscreen(gameObject.transform as RectTransform);
                NormalFullscreen.sprite = Normal;
            }
        }

        private IEnumerator SetCurrentPosition()
        {
            while (controller.IsPlaying)
            {
                if (Current != null)
                    Current.text = PrettyTimeFormat(TimeSpan.FromSeconds(controller.Time));

                yield return new WaitForSeconds(1);
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

