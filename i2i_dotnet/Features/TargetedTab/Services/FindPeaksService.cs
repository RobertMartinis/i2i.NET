using System;
using System.Collections.Generic;
using System.Linq;
using i2i_dotnet.Features.TargetedTab.Models;
using i2i_dotnet.Shared.Stores;

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

        return new FindPeaksResult(
            MassList: masslist,
            AllMassDiff: allMassDiff,
            AllClosestMass: allClosestMass,
            AllIntensities: allIntensities,
            FsMassDiff: fsMassDiff,
            FsClosestMass: fsClosestMass,
            SignalInten: signalInten,
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

    private static int FindClosestIndexLinear(double[] mz, double target)
    {
         int bestIdx = 0;
         double best = Math.Abs(mz[0] - target);
         for (int i = 1; i < mz.Length; i++)
         {
             double d = Math.Abs(mz[i] - target);
             if (d < best) { best = d; bestIdx = i; }
         }
         return bestIdx;
     }
}

public sealed record FindPeaksResult(
    double[] MassList,
    List<double[,]> AllMassDiff,
    List<double[,]> AllClosestMass,
    List<double[,]> AllIntensities,
    List<double[,]> FsMassDiff,
    List<double[,]> FsClosestMass,
    List<double[,]> SignalInten,
    List<double[,]> AnalyteMatrix
);
