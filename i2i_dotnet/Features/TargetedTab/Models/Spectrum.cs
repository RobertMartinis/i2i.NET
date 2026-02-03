namespace i2i_dotnet.Features.TargetedTab.Models;

public class Spectrum
{
    /// <summary>V
    /// The scan number of this MS Spectrum.
    /// </summary>
    public readonly int ScanNumber;

    /// <summary>
    /// List of all m/z values in this spectrum.
    /// </summary>
    public readonly List<double> MZList;

    /// <summary>
    /// List of all intensities in this spectrum. 
    /// </summary>
    public readonly List<double> Intensities;

    /// <summary>
    /// Total count of m/z values in this spectrum.
    /// </summary>
    public int MzCount => MZList.Count;

    /// <summary>
    /// List of scanfilters contained in this spectrum.
    /// </summary>
    public readonly string ScanFilter;

    /// <summary>
    /// This Spectrums total ion count.
    /// </summary>
    public readonly double TotalIonCount;

    /// <summary>
    /// The retention time of this spectrum.
    /// </summary>
    public readonly double RetentionTime;
    
    /// <summary>
    /// Constructor that sets the scannumber.
    /// </summary>
    /// <param name="scanNumber">This spectrums index.</param>
    public Spectrum(int scanNumber)
    {
        ScanNumber = scanNumber;
        MZList = new List<double>();
        Intensities = new List<double>();
        ScanFilter = "";
    }

    /// <summary>
    /// Constructor for Experiment that sets the scan number, m/z list, intensities and scanfilters.
    /// </summary>
    /// <param name="scanNumber">This spectrums index.</param>
    /// <param name="mzlist">A list of all m/z in this spectrum.</param>
    /// <param name="intensities">A list of all intensities in this spectrum.</param>
    /// <param name="scanfilters">A list of all scanfilters in this spectrum.</param>
    public Spectrum(int scanNumber, IList<double> mzlist, IList<double> intensities, string scanfilter, double
        retentionTime)
        : this(scanNumber)
    {
        MZList.Capacity = mzlist.Count;
        Intensities.Capacity = intensities.Count;
        ScanFilter = scanfilter;
        TotalIonCount = intensities.Sum();
        RetentionTime = retentionTime;

        for (int i = 0; i < mzlist.Count; i++)
        {
            MZList.Add(mzlist[i]);
            Intensities.Add(intensities[i]);
        }

    }
}