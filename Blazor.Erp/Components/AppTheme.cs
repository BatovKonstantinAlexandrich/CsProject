using MudBlazor;

namespace BlazorErp.Components;

public static class AppTheme
{
    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0f172a",
            Secondary = "#2563eb",
            Tertiary = "#7c3aed",
            Info = "#0284c7",
            Success = "#059669",
            Warning = "#d97706",
            Error = "#dc2626",
            AppbarBackground = "#0f172a",
            AppbarText = "#f8fafc",
            Background = "#f1f5f9",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "#334155",
            DrawerIcon = "#64748b",
            TextPrimary = "#0f172a",
            TextSecondary = "#64748b",
            ActionDefault = "#64748b",
            Divider = "#e2e8f0",
            TableLines = "#e2e8f0",
            LinesDefault = "#e2e8f0"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "260px",
            AppbarHeight = "64px"
        },
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = ["Segoe UI", "system-ui", "-apple-system", "sans-serif"]
            }
        }
    };
}
