using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Unity.VideoHelper
{
    public class FloatEvent : UnityEvent<float> { }

    public class VideoController : MonoBehaviour
    {

        #region Fields

        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private RawImage image;

        private UnityEvent<float> timelinePositionChanged = new FloatEvent();
        private UnityEvent startedPlaying = new UnityEvent();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value between 0 and 1 that represents the video position.
        /// </summary>
        public float NormalizedTime
        {
            get { return (float)(videoPlayer.time / Duration); }
        }

        /// <summary>
        /// Gets the duration of the video in seconds.
        /// </summary>
        public ulong Duration
        {
            get { return videoPlayer.frameCount / (ulong)videoPlayer.frameRate; }
        }

        /// <summary>
        /// Gets the current time in seconds.
        /// </summary>
        public double Time
        {
            get { return videoPlayer.time; }
        }

        /// <summary>
        /// Gets whether the player prepared buffer for smooth playback.
        /// </summary>
        public bool IsPrepared
        {
            get { return videoPlayer.isPrepared; }
        }

        /// <summary>
        /// Gets whether the video is playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return videoPlayer.isPlaying; }
        }

        public float Volume
        {
            get { return audioSource.volume; }
            set { audioSource.volume = value; }
        }


        public UnityEvent<float> TimelinePositionChanged { get { return timelinePositionChanged; } }

        public UnityEvent StartedPlaying { get { return startedPlaying; } }

        #endregion

        #region Unity methods

        private void OnEnable()
        {
            SubscribeToVideoPlayerEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromVideoPlayerEvents();
        }

        private void Start()
        {
            if (image == null)
                image = gameObject.AddComponent<RawImage>();

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            if (videoPlayer == null)
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
                SubscribeToVideoPlayerEvents();
            }

            videoPlayer.playOnAwake = false;
            audioSource.playOnAwake = false;

            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        private void Update()
        {
            if (!IsPrepared || !IsPlaying)
                return;

            TimelinePositionChanged.Invoke(NormalizedTime);
        }

        #endregion

        #region Methods

        public void PrepareForUrl(string url)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;
            videoPlayer.Prepare();
        }

        public void PrepareForFile(string path)
        {
            videoPlayer.source = VideoSource.VideoClip;
            throw new NotImplementedException();
        }

        public void Play()
        {
            if (!IsPrepared)
                return;

            videoPlayer.Play();
            audioSource.Play();
        }

        public void Pause()
        {
            videoPlayer.Pause();
        }

        public void Seek(float time)
        {
            time = Mathf.Clamp(time, 0, 1);
            videoPlayer.time = time * Duration;
        }

        #endregion

        #region Private Methods

        private void OnStarted(VideoPlayer source)
        {
            Debug.Log("Started video");
            startedPlaying.Invoke();
        }

        private void OnSeekCompleted(VideoPlayer source)
        {
            Debug.Log("Seek completed");
        }

        private void OnPrepareCompleted(VideoPlayer source)
        {
            Debug.Log("Prepare completed");

            image.texture = videoPlayer.texture;
            Play();
        }

        private void OnError(VideoPlayer source, string message)
        {
            Debug.LogError("Error in video player");
        }

        private void SubscribeToVideoPlayerEvents()
        {
            videoPlayer.errorReceived += OnError;
            videoPlayer.prepareCompleted += OnPrepareCompleted;
            videoPlayer.seekCompleted += OnSeekCompleted;
            videoPlayer.started += OnStarted;
        }

        private void UnsubscribeFromVideoPlayerEvents()
        {
            videoPlayer.errorReceived -= OnError;
            videoPlayer.prepareCompleted -= OnPrepareCompleted;
            videoPlayer.seekCompleted -= OnSeekCompleted;
            videoPlayer.started -= OnStarted;
        }

        #endregion
    }

}
