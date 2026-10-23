public class UpdateWhenChangedAttribute: ViewAttribute
{
    private readonly bool _update;
    public UpdateWhenChangedAttribute( )
    {
        _update = true;
    }
    public UpdateWhenChangedAttribute(bool _update)
    {
        this._update = _update;
    }
    public UpdateWhenChangedAttribute(string _update)
    {
        this._update = true;
    }
}