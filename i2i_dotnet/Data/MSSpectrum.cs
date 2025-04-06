using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i2i_learn.Data
{   /// <summary>
    /// A MS Spectrum. Contains m/z, intensities, and retention time. 
    /// </summary>
    public class MSSpectrum
    {
        /// <summary>
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
        public readonly List<string> ScanFilters;

        /// <summary>
        /// This Spectrums total ion count.
        /// </summary>
        public readonly double TotalIonCount;

        /// <summary>
        /// Constructor that sets the scannumber.
        /// </summary>
        /// <param name="scanNumber">This spectrums index.</param>
        public MSSpectrum(int scanNumber)
        {
            ScanNumber = scanNumber;
            MZList = new List<double>();
            Intensities = new List<double>();
            ScanFilters = new List<string>();
        }
        /// <summary>
        /// Constructor for MSSpectrum that sets the scan number, m/z list, intensities and scanfilters.
        /// </summary>
        /// <param name="scanNumber">This spectrums index.</param>
        /// <param name="mzlist">A list of all m/z in this spectrum.</param>
        /// <param name="intensities">A list of all intensities in this spectrum.</param>
        /// <param name="scanfilters">A list of all scanfilters in this spectrum.</param>
        public MSSpectrum(int scanNumber, IList<double> mzlist, IList<double> intensities, IList<string> scanfilters)
            : this(scanNumber)
        {
            MZList.Capacity = mzlist.Count;
            Intensities.Capacity = intensities.Count;
            ScanFilters.Capacity = scanfilters.Count;

            for (int i = 0; i < scanfilters.Count; i++)
            {
                ScanFilters.Add(scanfilters[i]);
            }

            for (int i = 0; i <mzlist.Count; i++)
            {  MZList.Add(mzlist[i]);
               Intensities.Add(intensities[i]);
            }

        }


    }
}
