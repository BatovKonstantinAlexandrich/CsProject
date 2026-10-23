using static WinFormsApp.API.DesignerResizeTool;

public interface IStyledItem
{
    bool AnchorBottom { get; set; }
    bool AnchorLeft { get; set; }
    bool AnchorRight { get; set; }
    bool AnchorTop { get; set; }
    bool AutoSize { get; set; }
    string BackgroundColor { get; set; }
    string BorderColor { get; set; }
    float BorderWidth { get; set; }
    string Color { get; set; }
    WinFormsApp.API.DesignerResizeTool.Cursors Cursor { get; set; }
    string Dock { get; set; }
    float FontSize { get; set; }
    int Height { get; set; }
    int Margin { get; set; }
    int MinHeight { get; set; }
    int MinWidth { get; set; }
    int Padding { get; set; }
    string TextAlign { get; set; }
    int TextIndent { get; set; }
    int Width { get; set; }
    int X { get; set; }
    int Y { get; set; }

    string GetCssText();
 
    void SetBaseTextStyle();
}