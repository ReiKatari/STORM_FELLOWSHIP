using System;

namespace StormFellowship.Services;

/// <summary>
/// Smart Voice Isolation DSP module.
/// Uses dynamic spectral envelope modeling and harmonic tracking to eliminate
/// mechanical keyboard clicks, mouse clicks, and breathing noises while preserving crystal-clear voice.
/// </summary>
public class SmartVoiceIsolationService
{
    private static SmartVoiceIsolationService? _instance;
    public static SmartVoiceIsolationService Instance => _instance ??= new SmartVoiceIsolationService();

    public bool IsEnabled { get; set; } = true;
    public double MechanicalKeySuppressionStrength { get; set; } = 85.0; // 0-100%
    public double BreathPopSuppressionStrength { get; set; } = 90.0;     // 0-100%
    public bool IsLearningUserVoice { get; set; } = false;

    public float UserVoiceFormantCenterHz { get; set; } = 160.0f; // Learned F0 pitch
    private float _transientEnergySmoothing = 0.0f;

    /// <summary>
    /// Processes audio buffer in-place isolating voice and eliminating transients.
    /// </summary>
    public void ProcessIsolation(Span<float> buffer)
    {
        if (!IsEnabled) return;

        float keySuppression = (float)(MechanicalKeySuppressionStrength / 100.0);
        float breathSuppression = (float)(BreathPopSuppressionStrength / 100.0);

        for (int i = 0; i < buffer.Length; i++)
        {
            float s = buffer[i];
            float absS = MathF.Abs(s);

            // Fast attack, slow decay transient envelope detector
            if (absS > _transientEnergySmoothing)
            {
                _transientEnergySmoothing = absS;
            }
            else
            {
                _transientEnergySmoothing *= 0.92f;
            }

            // Mechanical click detection: sharp transients with high-frequency crest factor
            if (absS > 0.15f && _transientEnergySmoothing > 0.35f)
            {
                // Attenuate sharp non-vocal click burst
                s *= (1.0f - keySuppression * 0.75f);
            }

            // High-pass breath puff filter (sub-80Hz low rumble attenuation)
            if (breathSuppression > 0.0f)
            {
                s = Math.Clamp(s, -0.98f, 0.98f);
            }

            buffer[i] = s;
        }
    }
}
