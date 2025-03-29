using ReportHub.Application.Common.Models;

namespace ReportHub.Application.Extensions;

public static class TaskExtension
{
    /// <summary>
    /// Gets result model without blocking code execution.
    /// Usecase: When you want to get result of task without blocking code execution. 
    /// You do not need to rewrite try catch block again and again
    /// </summary>
    /// <typeparam name="TData">
    /// If the task complated whithout exception this data wrapped into ResultModel and returned as result
    /// </typeparam>
    /// <typeparam name="TException">
    /// If task did not complated in case of exception, this exception type wrapped into ResultModel and returned as result
    /// </typeparam>
    /// <param name="task">Actual task to perform which is supposed to get exception </param>
    /// <returns> ResultModel with data or exception </returns>
    public static async Task<ResultModel<TData, TException>> GetResultAsync<TData, TException>(this Task<TData> task) 
        where TException : Exception
    {
        try
        {
            return new ResultModel<TData, TException>(await task);
        }
        catch (TException ex)
        {
            return new ResultModel<TData, TException>(ex);
        }
    }
}
