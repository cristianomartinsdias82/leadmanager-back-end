using System.Text.Json.Serialization;

namespace Shared.Results;

public sealed class ApplicationResponse<T>
{
    public bool Success { get => !(inconsistencies?.Any() ?? false) && Exception is null; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OperationCodes? OperationCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
    public T Data { get; set; } = default!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExceptionData? Exception { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Inconsistency>? Inconsistencies => inconsistencies;

    private IList<Inconsistency>? inconsistencies;

    public void AddInconsistency(string fieldOrLabel, string description)
        => AddInconsistency(new Inconsistency(fieldOrLabel, description));

    public void AddInconsistency(Inconsistency inconsistency)
    {
        inconsistencies ??= new List<Inconsistency>();
        inconsistencies.Add(inconsistency);
    }

    public void ClearErrors()
    {
        inconsistencies?.Clear();
        Exception = default;
    }

    public static ApplicationResponse<T> Create(
        T data,
        string? message = default,
        OperationCodes? operationCode = default,
        ExceptionData? exception = default,
        params Inconsistency[] inconsistencies)
    {
        var response = new ApplicationResponse<T>
        {
            Data = data,
            Message = message,
            OperationCode = operationCode ?? (exception is not null ? OperationCodes.Error : OperationCodes.Successful),
            Exception = exception
        };

        inconsistencies?.ToList().ForEach(it => response.AddInconsistency(it));

        return response;
    }
}