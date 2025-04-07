namespace ReportHub.Application.Common.Models;

public class ResultModel<TData, TException>
{
    public TData? Data { get; }

    public TException? Exception { get; }

    public bool IsSuccess => Exception is null;

    public ResultModel(TData data) => Data = data;

    public ResultModel(TException exception) => Exception = exception;  

}
