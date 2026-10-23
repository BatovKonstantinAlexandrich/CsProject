using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata
{
    public class KeyMetadata : BaseEntity
    {

        [NotNullNotEmpty]
        [InputEngWord]
        public string SourceTable { get; set; }

        [NotNullNotEmpty]
        [InputEngWord()]
        public string SourceColumn { get; set; }

        [NotNullNotEmpty]
        [InputEngWord]
        public string TargetTable { get; set; }

        [NotNullNotEmpty]
        [InputEngWord]
        public string TargetColumn { get; set; }

        public string ToString()
        {

            this.EnsureIsValide();
            var name = SourceTable.Split(".").ToList().Last();
            return $"FK_{name.ReplaceAll("[", "").ReplaceAll("]", "")}_{SourceColumn}";
        }
    }
}
