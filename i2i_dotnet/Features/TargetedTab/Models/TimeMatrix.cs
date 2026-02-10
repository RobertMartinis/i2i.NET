namespace i2i_dotnet.Features.TargetedTab.Models;

public sealed class TimeMatrix
{
    public List<TimeRow> Rows { get; set; } = new();

    public int RowCount => Rows.Count;

    public TimeRow this[int row] => Rows[row];
}

public sealed class TimeRow
{
    public double[] RetentionTimes { get; }

    public int Count => RetentionTimes.Length;

    public TimeRow(double[] retentionTimes)
    {
        RetentionTimes = retentionTimes ?? throw new ArgumentNullException(nameof(retentionTimes));
    }

    public double this[int col] => RetentionTimes[col];
}
