using UnityEngine;
using UnityEngine.SceneManagement;
using AudioFeatureLab;

/// <summary>
/// Sound → the bird flies up; silence → gravity pulls the bird down.
/// Depends on MicrophoneCapture + MicrophonePeakMeter from the professor's repo (on the same GameObject).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BirdVoiceController : MonoBehaviour
{
    [Header("Audio (from AudioFeatureLab)")]
    public MicrophoneCapture mic;
    public MicrophonePeakMeter meter;

    [Header("Noise Gate (dB) — tune using the readout in the top-left corner")]
    public float gateOpenDb = -35f;   // above this = voice detected
    public float gateCloseDb = -40f;  // below this = silence (slightly lower than open to prevent flicker)
    public bool keyboardFallback = true; // hold Space to fly too (for testing / mic-shy players)

    [Header("Flight")]
    public float thrust = 30f;        // upward force while voice is detected
    public float maxUpSpeed = 5f;     // max rise speed, so a sudden shout doesn't launch the bird off-screen
    public float maxFallSpeed = 3.5f; // max fall speed; keep it slow so players have time to breathe

    [Header("Feedback")]
    public SpriteRenderer sprite;
    public Color idleColor = Color.white;
    public Color voicedColor = new Color(1f, 0.85f, 0.2f);
    public float voicedScale = 1.15f;

    [Header("State (read only)")]
    public bool isVoiced;
    public bool isDead;

    Rigidbody2D rb;
    Vector3 baseScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (mic == null) mic = GetComponent<MicrophoneCapture>();
        if (meter == null) meter = GetComponent<MicrophonePeakMeter>();

        if (mic != null)
        {
            mic.keyboardControl = false; // prevent the M/N/K keys from accidentally toggling the mic
            mic.monitorVolume = 0f;      // don't play the mic through the speakers (causes feedback)
            if (!mic.isRunning) mic.StartCapture(null);
        }
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        // Noise gate (with hysteresis)
        float db = meter != null ? meter.rmsDecibels : AudioUnits.SilenceDecibels;
        if (!isVoiced && db > gateOpenDb) isVoiced = true;
        else if (isVoiced && db < gateCloseDb) isVoiced = false;

        if (keyboardFallback && Input.GetKey(KeyCode.Space)) isVoiced = true;

        // Visual feedback: brighten and grow while voiced
        Vector3 targetScale = isVoiced ? baseScale * voicedScale : baseScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 15f * Time.deltaTime);
        if (sprite != null)
            sprite.color = Color.Lerp(sprite.color, isVoiced ? voicedColor : idleColor, 15f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (isVoiced) rb.AddForce(Vector2.up * thrust);

        Vector2 v = rb.linearVelocity;
        v.y = Mathf.Clamp(v.y, -maxFallSpeed, maxUpSpeed);
        rb.linearVelocity = v;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        isDead = true;
        Time.timeScale = 0f;
    }

    // Debug: show the current level to help tune gateOpenDb / gateCloseDb
    void OnGUI()
    {
        float db = meter != null ? meter.rmsDecibels : AudioUnits.SilenceDecibels;
        GUI.Label(new Rect(10, 10, 400, 22), $"Mic RMS: {db:F1} dB   gate open {gateOpenDb} / close {gateCloseDb}");
        GUI.Label(new Rect(10, 32, 400, 22), isVoiced ? "VOICE: ON" : "VOICE: off");
        if (isDead) GUI.Label(new Rect(10, 54, 400, 22), "Hit! Press R to restart");
    }
}
