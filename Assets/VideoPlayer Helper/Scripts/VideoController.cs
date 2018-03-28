#if DEBUG
#define VIDEOPLAYER_DEBUG
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Unity.VideoHelper
{

    public class VideoController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private RawImage screen;

        [SerializeField]
        private bool startAfterPreparation = true;

        [Header("Optional")]

        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private AudioSource audioSource;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onPrepared = new UnityEvent();

        [SerializeField]
        private UnityEvent onStartedPlaying = new UnityEvent();

        [SerializeField]
        private UnityEvent onFinishedPlaying = new UnityEvent();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to automatically start playing the video after it is prepared.
        /// </summary>
        public bool StartAfterPreparation
        {
            get { return startAfterPreparation; }
            set { startAfterPreparation = value; }
        }

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
        public ulong Time
        {
            get { return (ulong)videoPlayer.time; }
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

        /// <summary>
        /// Gets or sets the volume of the audio source.
        /// </summary>
        public float Volume
        {
            get { return audioSource == null ? videoPlayer.GetDirectAudioVolume(0) : audioSource.volume; }
            set
            {
                if (audioSource == null)
                    videoPlayer.SetDirectAudioVolume(0, value);
                else
                    audioSource.volume = value;
            }
        }

        /// <summary>
        /// Gets or sets the image to show the video.
        /// </summary>
        public RawImage Screen
        {
            get { return screen; }
            set { screen = value; }
        }


        /// <summary>
        /// Fired when the video is prepared for playback.
        /// </summary>
        public UnityEvent OnPrepared
        {
            get { return onPrepared; }
        }

        /// <summary>
        /// Fired when the player started to play.
        /// </summary>
        public UnityEvent OnStartedPlaying
        {
            get { return onStartedPlaying; }
        }

        /// <summary>
        /// Fired when the video is finished.
        /// </summary>
        public UnityEvent OnFinishedPlaying
        {
            get { return onFinishedPlaying; }
        }

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
            if (videoPlayer == null)
            {
                videoPlayer = gameObject.GetOrAddComponent<VideoPlayer>();
                SubscribeToVideoPlayerEvents();
            }

            videoPlayer.playOnAwake = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepares the video player for the URL.
        /// </summary>
        /// <param name="url">The video URL.</param>
        public void PrepareForUrl(string url)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;
            videoPlayer.Prepare();
        }

        /// <summary>
        /// Prepares the player for a video clip.
        /// </summary>
        /// <param name="clip">The clip.</param>
        public void PrepareForClip(VideoClip clip)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = clip;
            videoPlayer.Prepare();
        }

        /// <summary>
        /// Plays a prepared video.
        /// </summary>
        public void Play()
        {
            if (!IsPrepared)
            {
                videoPlayer.Prepare();
                return;
            }

            videoPlayer.Play();
        }

        /// <summary>
        /// Pauses the player.
        /// </summary>
        public void Pause()
        {
            videoPlayer.Pause();
        }

        /// <summary>
        /// Plays or pauses the video.
        /// </summary>
        public void TogglePlayPause()
        {
            if (IsPlaying)
                Pause();
            else
                Play();
        }

        /// <summary>
        /// Sets the playback speed.
        /// </summary>
        /// <param name="speed">The speed, e.g. 0.5 for half the normal speed.</param>
        public void SetPlaybackSpeed(float speed)
        {
            videoPlayer.playbackSpeed = speed;
        }

        /// <summary>
        /// Jumps to the specified time in the video.
        /// </summary>
        /// <param name="time">The normalized time.</param>
        public void Seek(float time)
        {
            time = Mathf.Clamp(time, 0, 1);
            videoPlayer.time = time * Duration;
        }

        #endregion

        #region Private Methods

        private void OnStarted(VideoPlayer source)
        {
            onStartedPlaying.Invoke();
        }

        private void OnFinished(VideoPlayer source)
        {
            onFinishedPlaying.Invoke();
        }

        private void OnPrepareCompleted(VideoPlayer source)
        {
            onPrepared.Invoke();
            screen.texture = videoPlayer.texture;

#if VIDEOPLAYER_DEBUG
            Debug.LogWarning("[Video Controller] Depending on your Unity version you might not be able to hear audio in the Editor.");
#endif

            SetupAudio();
            SetupScreenAspectRatio();

            if (StartAfterPreparation)
                Play();
        }

        private void SetupScreenAspectRatio()
        {
            var fitter = screen.gameObject.GetOrAddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = (float)videoPlayer.texture.width / videoPlayer.texture.height;
        }

        private void SetupAudio()
        {
            if (videoPlayer.audioTrackCount <= 0)
                return;

            if(audioSource == null && videoPlayer.canSetDirectAudioVolume)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            }
            else
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                videoPlayer.SetTargetAudioSource(0, audioSource);
            }
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.EnableAudioTrack(0, true);
        }

        private void OnError(VideoPlayer source, string message)
        {
#if VIDEOPLAYER_DEBUG
            Debug.LogError("[Video Controller] " + message);
#endif
        }

        private void SubscribeToVideoPlayerEvents()
        {
            if (videoPlayer == null)
                return;

            videoPlayer.errorReceived += OnError;
            videoPlayer.prepareCompleted += OnPrepareCompleted;
            videoPlayer.started += OnStarted;
            videoPlayer.loopPointReached += OnFinished;
        }

        private void UnsubscribeFromVideoPlayerEvents()
        {
            if (videoPlayer == null)
                return;

            videoPlayer.errorReceived -= OnError;
            videoPlayer.prepareCompleted -= OnPrepareCompleted;
            videoPlayer.started -= OnStarted;
            videoPlayer.loopPointReached -= OnFinished;
        }

        #endregion

    }

}
