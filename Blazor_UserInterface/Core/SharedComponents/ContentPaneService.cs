using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Определяет методы позиционирования блока исходя из собственных свойств
/// </summary>
public interface IContentPaneService
{
    public void MoveToRightBorder(Pane pane);
    public void MoveToTopBorder(Pane pane);
    public void MoveToLeftBorder(Pane pane);
    public void MoveToBottomBorder(Pane pane);
    public void SetAbsolutelyWidth(Pane pane, int px);
    public void SetAbsolutelyHeight(Pane pane, int px);
    public void SetRelativeWidth(Pane pane, int px);
    public void SetRelativeHeight(Pane pane, int px);
}

public class ContentPaneService : IContentPaneService
{
    private const string PositionClassPrefix = "pane-pos-";

    private static void SetPosition(Pane pane, string position)
    {
        if (pane == null)
            return;

        pane.ClassList.RemoveAll(c => c.StartsWith(PositionClassPrefix, StringComparison.Ordinal));
        pane.ClassList.Add(PositionClassPrefix + position);
        pane.Changed = true;
    }

    public void MoveToBottomBorder(Pane pane) => SetPosition(pane, "bottom");

    public void MoveToLeftBorder(Pane pane) => SetPosition(pane, "left");

    public void MoveToRightBorder(Pane pane) => SetPosition(pane, "right");

    public void MoveToTopBorder(Pane pane) => SetPosition(pane, "top");

    public void SetAbsolutelyHeight(Pane pane, int px)
    {
        if (pane == null)
            return;
        pane.Height = Math.Max(1, px);
        pane.Changed = true;
    }

    public void SetAbsolutelyWidth(Pane pane, int px)
    {
        if (pane == null)
            return;
        pane.Width = Math.Max(1, px);
        pane.Changed = true;
    }

    public void SetRelativeHeight(Pane pane, int percent)
    {
        if (pane == null)
            return;
        pane.Height = Math.Clamp(percent, 1, 100);
        pane.Changed = true;
    }

    public void SetRelativeWidth(Pane pane, int percent)
    {
        if (pane == null)
            return;
        pane.Width = Math.Clamp(percent, 1, 100);
        pane.Changed = true;
    }
}

public static class ContentPaneExtension
{
    public static IServiceCollection AddContentPane(this IServiceCollection services)
    {
        services.AddScoped<IContentPaneService, ContentPaneService>();
        return services;
    }
}
