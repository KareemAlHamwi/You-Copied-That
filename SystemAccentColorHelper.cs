using System.Runtime.InteropServices;

public static class SystemAccentColorHelper {
    [DllImport("dwmapi.dll", EntryPoint = "DwmGetColorizationColor")]
    private static extern void DwmGetColorizationColor(out uint pcrColorization, out bool pfOpaqueBlend);

    public static Color GetAccentColor() {
        DwmGetColorizationColor(out uint colorization, out bool opaque);

        byte r = (byte)((colorization & 0x00FF0000) >> 16);
        byte g = (byte)((colorization & 0x0000FF00) >> 8);
        byte b = (byte)(colorization & 0x000000FF);

        //! Some systems may return partially transparent colors if "Transparency effects" are enabled.
        //? That's why a is replaeced with 255.
        return Color.FromArgb(255, r, g, b);
    }
}
