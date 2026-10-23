using WinFormsApp.AppComponents.AppControls;

[Label("Контрол коллекции")]
public class CollectionControl<TEntity> : ControlBase<CollectionView<TEntity>> where TEntity : BaseEntity
{
    public override void Bind(CollectionView<TEntity> view)
    {
        view.AddPropertyChangeListener(OnViewOptionsChanged);
    }

    private void OnViewOptionsChanged(PropertyChangedMessage message)
    {
        switch(message.Property)
        {
            case nameof(ViewOptions.Dropable): break;
            case nameof(ViewOptions.Draggable): break;
            case nameof(ViewOptions.Selectable): break;
            case nameof(ViewOptions.Focusable): break;
            case nameof(ViewOptions.Searchable): break;
            case nameof(ViewOptions.Editable): break;
            case nameof(ViewOptions.Selected): break;
            case nameof(ViewOptions.Edited): break;
            case nameof(ViewOptions.Expanded): break;
            case nameof(ViewOptions.Checked): break;
            case nameof(ViewOptions.Visible): break;
        }
    }
}
