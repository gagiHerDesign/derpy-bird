# derpy-bird

# Voice Flappy Bird — MVP Setup

Minimum playable version: make a sound into the mic → the bird flies up; stay quiet → the bird falls. Obstacles scroll in from the right.

## Files From the Professor's Repo We Use

Source: https://github.com/mnolan4/AudioScripts (all under `Assets/AudioFeatureLab/Runtime`)

| File | What it does for us |
| --- | --- |
| `MicrophoneCapture.cs` | Opens the microphone and provides `ReadLatest()` to read the newest audio samples (handles ring-buffer wraparound). This is the game's input entry point. |
| `MicrophonePeakMeter.cs` | Calculates the mic's peak and RMS level, converts them to dB, and smooths them with attack/release. The MVP reads its `rmsDecibels` value directly. |
| `AudioUnits.cs` | dB conversion and the `Ballistic` smoothing function. Required by the two scripts above and reusable for our own processing later. |
| `FrequencyBandAnalyzer.cs` (later) | FFT analysis with custom frequency bands (how much energy is in low vs. high frequencies). Not needed for the MVP. |
| `03_InputOutput_Meters` scene (testing) | Use it to check that the mic works and see what dB range it produces. |

What the repo does **not** include: pitch detection (FFT bands can't tell which note is being sung; that needs an algorithm such as autocorrelation or YIN), a noise gate, or clap detection. The noise gate is implemented in `BirdVoiceController.cs`.

### Things to Watch Out For

1. The repo uses **Unity 6000.4.1f1**. The team should use the same version or a newer Unity 6.
2. The repo uses the old **Input Manager** (`UnityEngine.Input`). If the project defaults to the new Input System, the scripts will throw errors. Go to Project Settings → Player → **Active Input Handling** and set it to `Input Manager` or `Both`.

## Our Scripts

- **`BirdVoiceController.cs`**: reads the mic level → noise gate with hysteresis → applies upward force while voiced. The bird brightens and grows while voiced; hitting anything stops the game, and R restarts. Holding Space also makes the bird fly (for testing). The top-left corner shows the live dB level for tuning.
- **`PipeSpawner.cs`**: spawns obstacles on the right at a fixed interval and random height.
- **`ScrollLeft.cs`**: moves obstacles left and deletes them once off-screen.

## Unity Setup Steps

1. Copy the professor's `Assets/AudioFeatureLab/Runtime` folder, together with `AudioFeatureLab.asmdef`, into our project's Assets. Put our three scripts into Assets as well.
2. Create a 2D scene. Create a "Bird" object and add: SpriteRenderer (a square is fine for now), Rigidbody2D (Gravity Scale about 1–1.5), CircleCollider2D, AudioSource, `MicrophoneCapture`, `MicrophonePeakMeter`, and `BirdVoiceController`.
3. Make an obstacle prefab: an empty object with a top pipe and a bottom pipe as children (Sprite + BoxCollider2D each), leaving a gap in the middle. Add `ScrollLeft` to the parent.
4. Place an empty object on the right side of the screen, add `PipeSpawner`, and drag the prefab into `pipePrefab`.
5. Place a long collider along the top and bottom edges as a ceiling and floor.
6. Press Play. **Stay quiet for a few seconds and note the dB value in the top-left** (the room's noise floor), then speak and see how high it goes. Set `gateOpenDb` between the two, and set `gateCloseDb` about 5 dB lower.

## Tuning the Feel

| Parameter | Effect |
| --- | --- |
| `thrust` | How strongly the bird rises while voiced |
| `maxFallSpeed` | How fast the bird falls; slower gives players more time to breathe |
| `PipeSpawner.interval` | Spacing between obstacles; voice reacts slower than tapping, so keep it generous |
