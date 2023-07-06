namespace Shared.Results;

public struct Inconsistency
{
    public string FieldOrLabel { get; set; }
    public string Description { get; set; }

    public Inconsistency(string fieldOrLabel, string description)
    {
        FieldOrLabel = fieldOrLabel;
        Description = description;
    }
}
