# Scripts
## VideoController.cs
Helps to control Unitys `VideoPlayer` component through wrapper methods and easy access of events in the Editor.

![VideoControllerEditor]

### Editor

| Name						| Description
| -----						| -----------
| Screen					| The `RectTransform` to host the output of the video.
| Start After Preparation	| If set true, starts playing the video immediatly after its preparation. 
| Video Player				| (Optional) Specifies the player. Defaults to a new video player created by the *VideoController* during runtime.
| Audio Source				| (Optional) Specifies the audio source for the player to use. Defaults to `Direct` mode, if available.

### Methods

| Name							    | Description
| ----							    | -----------
| `PrepareForUrl(string url)`	    | Prepares the video to play the video specified in the URL.
| `Play()`						    | Plays the video or tries to prepare it.
| `Pause()`						    | Pauses the video.
| `TogglePlayPause()`			    | Toggles between play and pause.
| `Seek(float time)`		        | Jumps to the specified time in the video. The parameter must be between 0 and 1, where 0 is the beginning and 1 the end of the video.
| `SetPlaybackSpeed(float speed)`   | Sets the speed at which the video is supposed to play. Common values are 0.25, 0.75, 1.0, 1.25 and 1.5.

### Events

| Name				| Description 
| ----				| -----------
| OnPrepared		| Called when the video is prepared.
| OnStartedPlaying	| Called when the video started to play.
| OnFinishedPlaying | Called when the video finished playing.

## VideoPresenter.cs
Helps to control common visual elements of a video player. 

![VideoPresenterEditor]

### Editor

| Name              | Description
| ----              | -----------
| Screen            | The `RectTransform` that hosts the video output.
| Controls Panel    | The `RectTransform` that hosts all control for the video player.
| Loading Indicator | The `RectTransform` that hosts the loading indicator. This GameObject is set active whenever the video player prepares playback.
| Thumbnail         | The `RectTransform` that hosts the thumbnail. This GameObject is set inactive when the video starts playing.
| Timeline          | The `Timeline` responsible for showing the current position in the video and seeking through it. 
| Volume            | The `Slider` responsible for adjusting the volume.
| Play Pause        | The `Image` that hosts the sprite for the typical play and pause icons.
| Mute Unmute       | The `Image` that hosts the sprite for the typical mute and unmute icons. The sprites are driven by the Volumes array.
| Normal Fullscreen | The `Image` that hosts the sprite for the typical normal and fullscreen icons.
| Current           | The `Text` that hosts the text for the current position in the video.
| Duration          | The `Text` that hosts the text for the duration of the video.
| ==================
| Fullscreen Key    | The key to press for going to a fullscreen view. Set to *None* to disable.
| Normal Key        | The key to press for going back to a normal view. Set to *None* to disable.
| Play Pause Key    | The key to toggle between playing and pausing the video. Set to *None* to disable.
| Toggle Screen On Double Click | If active, toggles between normal and fullscreen view when the user double clicks the screen.
| Toggle Play Pause On Click    | If active, toggles between play and pause when the user clicks the screen.
| ==================
| Play              | The sprite that indicates to play the video.
| Pause             | The sprite that indicates to pause the video.
| Normal            | The sprite that indicates to show a normal sized view of the video.
| Fullscreen        | The sprite that indicates to show a full sized view of the video.
| ==================
| Volumes           | The volume states give you great control over representation for all stages of volume.
| - Minimum         | The minimum volume for the sprite to be shown. Range 0.0 to 1.0, where 0 is no volume and 1 the loudest volume.
| - Sprite          | The sprite to show for the volume range.

[VideoControllerEditor]: Images/VideoControllerEditor.jpg "VideoController Editor"
[VideoPresenterEditor]: Images/VideoPresenterEditor.jpg "VideoPresenter Editor"
