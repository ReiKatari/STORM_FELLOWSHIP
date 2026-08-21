using StormFellowship.Models;

namespace StormFellowship.Services;

public class SpatialAudioService
{
    private static SpatialAudioService? _instance;
    public static SpatialAudioService Instance => _instance ??= new SpatialAudioService();

    public (double pan, double volumeAttenuation) Calculate3DPanAndAttenuation(double sourceX, double sourceY, double sourceZ)
    {
        // sourceX: -100 (Hard Left) to +100 (Hard Right)
        // sourceY: -100 (Behind) to +100 (In Front)
        double pan = Math.Clamp(sourceX / 100.0, -1.0, 1.0);

        // Distance from listener (0,0,0)
        double distance = Math.Sqrt(sourceX * sourceX + sourceY * sourceY + sourceZ * sourceZ);
        double maxDist = 150.0;
        double attenuation = Math.Clamp(1.0 - (distance / maxDist) * 0.7, 0.3, 1.0);

        return (pan, attenuation);
    }
}
