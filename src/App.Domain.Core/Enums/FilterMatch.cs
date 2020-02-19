namespace App.Domain.Core.Enums
{
    /// <summary>
    /// What likelihood does this filter represent
    /// </summary>
    public enum FilterMatch
    {
        Like, // for string values only
        StartsWith, // for string values only
        EndsWith, // for string values only
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual
    }
}
