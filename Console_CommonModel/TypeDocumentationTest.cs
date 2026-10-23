namespace Console_CommonModel
{
    [Label("Проверка наличия атрибов описывающих типы")]
    public class TypeDocumentationTest : TestingElement
    {
        public TypeDocumentationTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            foreach(var kv in TypeDescription.ValidateTypes())
            {
                foreach(var kvv in kv.Value)
                {
                    this.Info($"Тип {kv.Key.GetTypeName()}: {kvv.Key}");
                    this.Info(kvv.Value);
                }
            }            
        }
    }
}