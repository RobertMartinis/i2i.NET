using System;
using System.Collections.Generic;
using System.Linq;
using Numerics.NET;
using i2i_dotnet.Features.TargetedTab.Models;
using i2i_dotnet.Shared.Stores;
using MathNet.Numerics;
using Numerics.NET.Curves;
using OxyPlot;

namespace i2i_dotnet.Features.TargetedTab.Services;

public class FindPeaksService : IFindPeaksService
{
    private readonly IExperimentStore _store;
    
    public FindPeaksService(IExperimentStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }


    public FindPeaksResult FindPeaks(double treshold, string scanFilter)
    {
        var linescans = _store.MSExperiment.GetLineScans().ToList();
        var analytelist = _store.Analytes.ToList();

 
        double[] masslist = analytelist.Select(a => a.Mz).ToArray();

        var allMassDiff = new List<double[,]>(linescans.Count);
        var allClosestMass = new List<double[,]>(linescans.Count);
        var allIntensities = new List<double[,]>(linescans.Count);

        foreach (LineScan line in linescans)
        {
            var byFilter = line.GetSpectrasByScanFilter(StringComparer.Ordinal);
            if (byFilter.Count == 0)
                continue;


            var spectras = byFilter[scanFilter]; // Spectrum[]
            int S = spectras.Length;
            int A = masslist.Length;                    // analytes

            var massdiff = new double[A, S];
            var closestmass = new double[A, S];
            var intensities = new double[A, S];

            for (int j = 0; j < S; j++)
            {
                Spectrum spectra = spectras[j];

            
                double[] mz = spectra.MZList.ToArray();
                double[] it = spectra.Intensities.ToArray();

                if (mz == null || it == null || mz.Length != it.Length || mz.Length == 0)
                    continue;

           
                for (int a = 0; a < A; a++)
                {
                    double target = masslist[a];
                    int idx = FindClosestIndexSorted(mz, target);

                    double foundMz = mz[idx];
                    double foundIt = it[idx];

                    double ppm = Math.Abs((foundMz - target) / target) * 1e6;

                    massdiff[a, j] = ppm;
                    closestmass[a, j] = foundMz;
                    intensities[a, j] = foundIt;
                }
            }

            allMassDiff.Add(massdiff);
            allClosestMass.Add(closestmass);
            allIntensities.Add(intensities);
        }


        var (fsMassDiff, fsClosestMass, signalInten) =
            FilterByPpmCutoff(allMassDiff, allClosestMass, allIntensities, treshold);


        var analyteMatrix = BuildAnalyteMatrix(signalInten);
        TimeMatrix timeMatrix = CreateTimeMatrix(_store.MSExperiment.GetLineScans().ToArray(), scanFilter);
        analyteMatrix = AlignPeaks(analyteMatrix, timeMatrix);

        return new FindPeaksResult(
          
            AnalyteMatrix: analyteMatrix
        );
    }

    private static (List<double[,]> FsMassDiff,
                    List<double[,]> FsClosestMass,
                    List<double[,]> SignalInten)
        FilterByPpmCutoff(
            List<double[,]> allMassDiff,
            List<double[,]> allClosestMass,
            List<double[,]> allIntensities,
            double cutoffPpm)
    {
        int L = allMassDiff.Count;

        var fsMassDiff = new List<double[,]>(L);
        var fsClosestMass = new List<double[,]>(L);
        var signalInten = new List<double[,]>(L);

        for (int u = 0; u < L; u++)
        {
            var diff = allMassDiff[u];
            var close = allClosestMass[u];
            var inten = allIntensities[u];

            int A = diff.GetLength(0);
            int S = diff.GetLength(1);

            var fDiff = (double[,])diff.Clone();
            var fClose = (double[,])close.Clone();
            var fInten = (double[,])inten.Clone();

            for (int a = 0; a < A; a++)
            {
                for (int s = 0; s < S; s++)
                {
                    if (fDiff[a, s] > cutoffPpm)
                    {
                        fDiff[a, s] = 0;
                        fClose[a, s] = 0;
                        fInten[a, s] = 0;
                    }
                }
            }

            fsMassDiff.Add(fDiff);
            fsClosestMass.Add(fClose);
            signalInten.Add(fInten);
        }

        return (fsMassDiff, fsClosestMass, signalInten);
    }


    private static List<double[,]> BuildAnalyteMatrix(List<double[,]> signalInten)
    {
        int numLines = signalInten.Count;
        if (numLines == 0) return new List<double[,]>();

        int A = signalInten[0].GetLength(0); // analytes
        int maxScans = signalInten.Max(m => m.GetLength(1));

        var analyteMatrix = new List<double[,]>(A);

        for (int i = 0; i < A; i++)
        {
            var tmp = new double[numLines, maxScans];

            for (int j = 0; j < numLines; j++)
            {
                var lineInten = signalInten[j];
                int scansInThisLine = lineInten.GetLength(1);

                for (int s = 0; s < scansInThisLine; s++)
                    tmp[j, s] = lineInten[i, s];
            }

            analyteMatrix.Add(tmp);
        }

        return analyteMatrix;
    }

    private static int FindClosestIndexSorted(double[] sortedMz, double target)
    {
        int n = sortedMz.Length;
        if (n == 0) throw new ArgumentException("mz array empty.");

        if (target <= sortedMz[0]) return 0;
        if (target >= sortedMz[n - 1]) return n - 1;

        int lo = 0, hi = n - 1;
        while (hi - lo > 1)
        {
            int mid = (lo + hi) >> 1;
            if (sortedMz[mid] < target) lo = mid;
            else hi = mid;
        }

        return (target - sortedMz[lo] <= sortedMz[hi] - target) ? lo : hi;
    }

    
    private TimeMatrix CreateTimeMatrix(
        LineScan[] lineScans,
        string scanFilterKey,
        StringComparer? comparer = null)
    {
        comparer ??= StringComparer.Ordinal;

        if (string.IsNullOrWhiteSpace(scanFilterKey))
            return new TimeMatrix { Rows = new List<TimeRow>(0) };

        // Compute max size for *that* filter only
        int maxSize = lineScans
            .Where(ls => ls != null)
            .Select(ls =>
            {
                var dict = ls!.GetSpectrasByScanFilter(comparer);
                return dict.TryGetValue(scanFilterKey, out var arr) ? (arr?.Length ?? 0) : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        var matrix = new TimeMatrix { Rows = new List<TimeRow>(lineScans.Length) };

        foreach (var ls in lineScans)
        {
            var padded = new double[maxSize];

            if (ls != null)
            {
                var dict = ls.GetSpectrasByScanFilter(comparer);

                if (dict.TryGetValue(scanFilterKey, out var spectra) && spectra != null)
                {
                    // CHANGE THIS to your actual time property
                    var rt = spectra.Select(s => s.RetentionTime).ToArray();

                    Array.Copy(rt, 0, padded, 0, Math.Min(rt.Length, maxSize));
                }
            }

            matrix.Rows.Add(new TimeRow(padded));
        }

        return matrix;
    }
    
   private List<double[,] > 
    AlignPeaks(List<double[,]> analyteMatrix, TimeMatrix time_out)
{
    // max_time = max(time_out,[],'all');
    double maxTime = MaxTime(time_out);
    Console.WriteLine($"maxTime={maxTime}, nSim={(int)Math.Round(maxTime * 20.0 * 60.0)}");

    // time_simulated = linspace(0,max_time,max_time*20*60);
    int nSim = (int)Math.Round(maxTime * 20.0 * 60.0);
    if (nSim < 2) nSim = 2;

    double[] timeSimulated = Generate.LinearSpaced(nSim, 0.0, maxTime);

    var aligned = new List<double[,]>(analyteMatrix.Count);

    for (int i = 0; i < analyteMatrix.Count; i++)
    {
        double[,] ion = analyteMatrix[i];

        int rows = ion.GetLength(0);
        int cols = ion.GetLength(1);

        var outIon = new double[rows, nSim];

        for (int j = 0; j < rows; j++)
        {
            double[] time_axis_all = time_out.Rows[j].RetentionTimes;

            // ind = time_axis > 0
            var xList = new List<double>(cols);
            var yList = new List<double>(cols);

            int take = Math.Min(cols, time_axis_all.Length);

            for (int c = 0; c < take; c++)
            {
                double t = time_axis_all[c];
                if (t > 0)
                {
                    xList.Add(t);
                    yList.Add(ion[j, c]);
                }
            }

            if (xList.Count == 0)
                continue;

            double[] xValues = xList.ToArray();
            double[] yValues = yList.ToArray();

            // IMPORTANT: Numerics.NET requires strictly increasing xValues.
            // If your retention times are already increasing per row, you can remove this.
            // Otherwise keep it:
           Array.Sort(xValues, yValues);

            for (int q = 0; q < nSim; q++)
            {
                double x = timeSimulated[q];

                outIon[j, q] = Interpolation.Nearest(
                    xValues, yValues, x, outOfRangeMode: ExtrapolationMode.Clamp
                );
            }
        }

        aligned.Add(outIon);
    }

    return (aligned);
}

private static double MaxTime(TimeMatrix tm)
{
    double max = double.NegativeInfinity;
    foreach (var row in tm.Rows)
        foreach (var t in row.RetentionTimes)
            if (t > max) max = t;
    return max;
}
}

public sealed record FindPeaksResult(
   
    List<double[,]> AnalyteMatrix
);
