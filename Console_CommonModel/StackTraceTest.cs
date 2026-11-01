public class StackTraceTest: TestingElement
{
    public override void OnTest()
    {
        new A().ToDo_A();
    }

    public class A
    {
        public void ToDo_A(int x = 11)
        {
            this.Commit(() => {
                var b = new B();
                b.ToDo_B();
                return new object();
            }, new
            {
                x
            });
        }
    }

    public class B
    {
        public void ToDo_B()
        {
            var stack = this.GetInvokeInfo();
            this.Info(stack);
            //var actionInfo = this.GetActionInfo();
            //this.Info(actionInfo);
        }
    }
}
