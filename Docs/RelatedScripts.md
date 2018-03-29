# Related scripts

## Timline.cs

A custom `Selectable` that is basically a slider component, but with some adjustments to make it behave and look like a proper timeline, e.g a seeking preview and a tooltip that shows the current position.

## OverlayController.cs

A component that helps wih overlays, like the settings panel in the YoutTube sample. It takes a `Target` GameObject that is set active with a call to `Show` and inactive with a call to `Hide` or if the user clicks outside of the target panel. The behaviour closely resembles the built in `Dropdown` component.

## DisplayController.cs

Controls where content is placed in fullscreen mode. This script stores anchors and offsets of its target before going into fullscreen and restores them once it goes back to normal. This also works if the application itself is fullscreen.

## AnimationCurveAnimator

A base class for simple in and out animations using `AnimationCurve`s and coroutines.

## CanvasGroupAnimator

Subclass of `AnimationCurveAnimator` that animates the alpha value of a `CanvasGroup`.

## ScaleTransformAnimator

Subclass of `AnimationCurveAnimator` that animates the scale value of a `Transform`.