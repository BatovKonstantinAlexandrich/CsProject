using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.DataComponents;

namespace Console_CommonModel.CommonLib.LibData
{

    /// <summary>
    /// Сервис предоставляет функции создания элементов управления данными на основе анализа 
    /// отношений между сущностям.
    /// Для отношения много-ко-многим, действуют следующие правила:
    /// -связи храняться в отдельной таблице
    /// -для одной связи повторения не возможны    
    /// </summary>
    public class BaseEntityControlsService : ServiceComponentBase,
        IControlService<BaseEntity>
    {
        public BaseEntity Control { get; set; }

        public void SetControl(BaseEntity item)
        {
            Control = item;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Action> GetActionsByRelations()
        {
            Dictionary<string, Action> keyValuePairs = new Dictionary<string, Action>();
            //foreach (var relation in programDbContext.GetType().GetOneToOneRelations())
            //{
            //    var relativeType = relation.RelativeEntityType.ToType();
            //    keyValuePairs[relativeType.GetLabel()] = () =>
            //    {
            //        var controls = new ControlsFactory();
            //        var reference = programDbContext.GetReferenceByType(relativeType);

            //        var panel = new InputPanel(reference);
            //        panel.ToSubmit().ShowDialog();
            //    };
            //}
            return keyValuePairs;
        }
    }
}
