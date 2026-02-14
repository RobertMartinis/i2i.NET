namespace i2i_dotnet.Features.TargetedTab.Models;

public class Experiment
{
    private readonly List<LineScan> _lineScans = new();

    public IReadOnlyList<LineScan> LineScans => _lineScans;
    
    public TimeMatrix TimeMatrix { get; set; } = new ();


    public int LineCount => _lineScans.Count;

    public Experiment()
    {
    }

    public void AddLineScan(LineScan lineScan)
    {
        _lineScans.Add(lineScan);
    }

    public void AddLineScans(IEnumerable<LineScan> lineScans)
    {
        _lineScans.AddRange(lineScans);
    }

    public IReadOnlyList<LineScan> GetLineScans()
    {
        return _lineScans;
    }

    public LineScan GetLineScan(int index)
    {
        return _lineScans[index];
    }
     
}