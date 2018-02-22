# VideoPlayer Helper for Unity

## Samples

| YouTube							| Vimeo							|
| ---								| ---							|
|![YouTube Player Image][YouTube]	|![Vimeo Player Image][Vimeo]	|

Each sample demonstrates a different approach in using the helper classes. 

## Scripts
### VideoController.cs
Helps to control Unitys `VideoPlayer` component through wrapper methods and easy access of events in the Editor.
#### Editor

| Name						| Description
| -----						| -----------
| Screen					| The `RectTransform` to host the output of the video.
| Start After Preparation	| If set true, starts playing the video immediatly after its preparation. 
| Video Player				| (Optional) Specifies the player. Defaults to a new video player created by the *VideoController* during runtime.
| Audio Source				| (Optional) Specifies the audio source for the player to use. Defaults to `Direct` mode, if available.

#### Methods

| Name							| Description
| ----							| -----------
| `PrepareForUrl(string url)`	| Prepares the video to play the video specified in the URL.
| `Play()`						| Plays the video or tries to prepare it.
| `Pause()`						| Pauses the video.
| `TogglePlayPause()`			| Toggles between play and pause.
| `Seek(float time)`			| Jumps to the specified time in the video. The parameter must be between 0 and 1, where 0 is the beginning and 1 the end of the video.

#### Events

| Name				| Description 
| ----				| -----------
| OnPrepared		| Called when the video is prepared.
| OnStartedPlaying	| Called when the started to play.
| OnFinishedPlaying | Called when the video finished.

### VideoPresenter.cs
Helps to control common visual elements of a video player. 

TODO

[YouTube]: Docs/YouTubePlayer.jpg "YouTube Sample"
[Vimeo]: Docs/VimeoPlayer.jpg "Vimeo Sample"