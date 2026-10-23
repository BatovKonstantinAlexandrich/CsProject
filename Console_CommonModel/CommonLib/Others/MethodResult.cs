public class MethodResult : MethodResult<object>
{
}
 
public class MethodResult<TResult> where TResult : class
{
    public bool Succeeded { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public TResult Result { get; set; } = null;
    public string Exception { get; set; } = null;
    public Dictionary<string, object> Args { get; set; }
    public IDictionary<string, IEnumerable<string>> Validations { get; set; }
    public DateTime Started { get; set; }
    public DateTime Completed { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public static MethodResult<TResult> OnError(Exception ex)
    {
        return new MethodResult<TResult>()
        {
            Succeeded = false,
            IsCompleted = true,
            Exception = ex.Message
        };
    }
    public static MethodResult<TResult> OnError(Dictionary<string, object> args, Exception ex)
    {
        return new MethodResult<TResult>()
        {
            Args = args,
            Completed = DateTime.Now,
            Succeeded = false,
            IsCompleted = true,
            Exception = ex.Message
        };
    }

    public static MethodResult<TResult> OnError(string ex)
    {
        return new MethodResult<TResult>()
        {
            Succeeded = false,
            IsCompleted = true,
            Exception = ex
        };
    }
    public static MethodResult<TResult> OnFailed(Exception ex, DateTime started)
    {
        return OnError(ex);
    }

    public static MethodResult<TResult> OnValidationError(IDictionary<string, IEnumerable<string>> description)
    {
        var res = OnError(new Exception(description.ToJsonOnScreen()));
        return res;
    }

    public static MethodResult<TResult> OnValidationError(
        System.Collections.Generic.Dictionary<string, object> parameters,
        IDictionary<string, IEnumerable<string>> description)
    {
        return new MethodResult<TResult>()
        {
            Args = parameters,
            Completed = DateTime.Now,
            Result = null,
            Succeeded = false,
            IsCompleted = false,
        };
    }

    public static MethodResult<TResult> OnValidationError(DateTime started,
        System.Collections.Generic.Dictionary<string, object> parameters,
        IDictionary<string, IEnumerable<string>> description)
    {
        return new MethodResult<TResult>()
        {
            Args = parameters,
            Started = started,
            Validations = description,
            Completed = DateTime.Now,
            Result = null,
            Succeeded = false,
            IsCompleted = false,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public static MethodResult<TResult> OnResult(TResult result)
    {
        return new MethodResult<TResult>()
        {
            Result = result,
            Succeeded = true,
            IsCompleted = true,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public static MethodResult<TResult> OnComplete(TResult result, System.Collections.Generic.Dictionary<string, object> parameters, DateTime started)
    {
        return new MethodResult<TResult>()
        {
            Args = parameters,
            Started = started,
            Completed = DateTime.Now,
            Result = result,
            Succeeded = true,
            IsCompleted = true,
        };
    }


    /// <summary>
    /// 
    /// </summary>
    public static MethodResult<TResult> OnAsyncResult(IAsyncResult todo)
    {
        return new MethodResult<TResult>()
        {
            Succeeded = false,
            IsCompleted = true,

        };
    }


}