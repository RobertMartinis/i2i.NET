namespace i2i_learn
{
    using Emgu.CV;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Windows.Forms;
    using System.Threading.Tasks;
    using System.IO.MemoryMappedFiles;
    using ThermoFisher.CommonCore.Data;
    using ThermoFisher.CommonCore.Data.Business;
    using ThermoFisher.CommonCore.Data.Interfaces;
    using ThermoFisher.CommonCore.BackgroundSubtraction;
    using Emgu.CV.Flann;
    using System.Formats.Tar;
    using System.Runtime.CompilerServices;
    using OxyPlot.WindowsForms;
    using OxyPlot.Annotations;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using System.ComponentModel;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    public partial class Form1 : Form
    {
        // Declare class-level variables
        private ArrayList mzBag = new ArrayList();
        private ArrayList intensityBag = new ArrayList();
        private List<double[]> timeBag = new List<double[]>();


        private double tmax;
        public Form1()
        {
            InitializeComponent();

        }
        string filePath;




        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "e:\\";
            openFileDialog.Filter = "(*.raw)|*.raw";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)

                filePath = openFileDialog.FileName;
            var rawFile = RawFileReaderFactory.CreateThreadManager(filePath);
            IRawDataPlus myThreadDataReader = rawFile.CreateThreadAccessor();
            myThreadDataReader.SelectInstrument(Device.MS, 1);
            var options = myThreadDataReader.DefaultMassOptions();
            options.ToleranceUnits = ToleranceUnits.ppm;
            var scanFilters = myThreadDataReader.GetAutoFilters();
            dropdownFilter.Items.AddRange(scanFilters);


            int instrumentString = myThreadDataReader.GetInstrumentCountOfType(Device.MS);
            int firstScanNumber = myThreadDataReader.RunHeaderEx.FirstSpectrum;
            int lastScanNumber = myThreadDataReader.RunHeaderEx.LastSpectrum;

            ChromatogramTraceSettings settings = new ChromatogramTraceSettings(TraceType.BasePeak);
            settings.MassRangeCount = 1;
            settings.Filter = scanFilters[0];

            settings.SetMassRange(0, new ThermoFisher.CommonCore.Data.Business.Range(0, myThreadDataReader.RunHeader.HighMass));

            var data = myThreadDataReader.GetChromatogramData(new IChromatogramSettings[] { settings }, firstScanNumber, lastScanNumber);
            var trace = ChromatogramSignal.FromChromatogramData(data);
            myThreadDataReader.Dispose();
            double[] time2;
            double[] int2;


            var selectedFilter = dropdownFilter.SelectedItem.ToString();

            (int2, time2) = ThermoWrappers.LoadFileSingle(filePath, selectedFilter);
            double[] time = (double[])trace[0].Times;
            double[] intensities = (double[])trace[0].Intensities;

            var line1 = new OxyPlot.Series.LineSeries()
            {
                Title = $"Series 1",
                Color = OxyPlot.OxyColors.Blue,
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = OxyPlot.MarkerType.None
            };
            for (int i = 0; i < trace[0].Length; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(time2[i], int2[i]));
            };
            var linePlot = new OxyPlot.PlotModel
            {
                Title = "tic2"
            };
            linePlot.Series.Add(line1);
            plotWindow1.Model = linePlot;


        }
        string folderPath;
        string[] filesInFolder;
        private void dropdownFilter_ItemsAdded(object sender, EventArgs e)
        {
            dropdownFilter.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            dropdownFilter.Items.Clear();
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowser.SelectedPath;
                textFilePath.Text = folderPath;
                filesInFolder = Directory.GetFiles(folderPath, "*.raw");
                textNumFiles.Value = filesInFolder.Length;
            }

            var rawFile = RawFileReaderFactory.CreateThreadManager(filesInFolder[0]);
            IRawDataPlus myThreadDataReader = rawFile.CreateThreadAccessor();
            myThreadDataReader.SelectInstrument(Device.MS, 1);

            var scanFilters = myThreadDataReader.GetAutoFilters();

            dropdownFilter.Items.AddRange(scanFilters);

            textBoxAnalyzer.Text = myThreadDataReader.GetAllInstrumentFriendlyNamesFromInstrumentMethod()[0];
            myThreadDataReader.Dispose();
            progressBarFileLoad.Maximum = filesInFolder.Length;
            dropdownFilter.SelectedIndex = 0;
            listBoxTolUnit.SelectedIndex = 2;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void clickLoadAll(object sender, MouseEventArgs e)
        {

            ArrayList timeMatrix = new ArrayList();
            ArrayList intensityMatrix = new ArrayList();

            var selectedFilter = dropdownFilter.SelectedItem.ToString();

            var list = ThermoWrappers.LoadAndStoreFile(filePath, selectedFilter);

            var intensityBag = new ConcurrentBag<double[]>();
            var indexBag = new ConcurrentBag<int>();
            var timeBag = new ConcurrentBag<double[]>();


            if (selectedFilter != null)
            {
                List<int> indices = Enumerable.Range(0, filesInFolder.Count()).ToList();
                Parallel.ForEach(indices, index =>
                {
                    string fileName = filesInFolder[index];
                    (double[] intensities, double[] time) = ThermoWrappers.LoadFileSingle(fileName, selectedFilter);
                    indexBag.Add(index);
                    intensityBag.Add(intensities);
                    timeBag.Add(time);

                });
            }




            // Hitta maxv�rden i alla arrays i timeBag
            var timeBagMax = new ConcurrentBag<double>(timeBag.Select(array => array.Max()));
            var tmax = timeBagMax.Max();
            var t0 = 0;


            var timeVector = MathNet.Numerics.Generate.LinearSpaced(1200, t0, tmax);
            var indexList = indexBag.ToList();
            ArrayList timeList = new ArrayList(timeBag.ToArray());
            ArrayList intensityList = new ArrayList(intensityBag.ToArray());

            double[,] matris = new double[indexList.Count, timeVector.Length];

            for (int i = 0; i < indexList.Count; i++)
            {
                double[] currentTimeList = (double[])timeList[i];
                double[] currentIntensityList = (double[])intensityList[i];

                for (int j = 0; j < timeVector.Length; j++)
                {
                    var result = currentTimeList.Select(x => Math.Abs(x - timeVector[j])).ToArray();
                    int minIndex = Array.IndexOf(result, result.Min());
                    matris[indexList[i], j] = currentIntensityList[minIndex];
                }

            }





            var model = new PlotModel { Title = "Heatmap" };

            // Color axis (the X and Y axes are generated automatically)
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Viridis(256)

            });



            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = matris.GetLength(1),
                Y0 = 0,
                Y1 = matris.GetLength(0),
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = matris


            };

            model.Series.Add(heatMapSeries);

            plotWindow1.Model = model;

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listBoxScanFilters_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var selectedFilter = dropdownFilter.SelectedItem.ToString();
            // Clear bags before loading new data
            mzBag.Clear();
            intensityBag.Clear();
            timeBag.Clear();

            for (int i = 0; i < filesInFolder.Count(); i++)
            {
                string fileName = filesInFolder[i];

                mzBag.Add(ThermoWrappers.LoadAndStoreFile_fullfile(fileName));
                progressBarFileLoad.Value = i + 1;
                progressBarFileLoad.Update();
            }


        }

        public void button2_Click(object sender, EventArgs e)
        {
            var selectedFilter = dropdownFilter.SelectedItem.ToString();
            //var selectedFilter = listBoxScanFilters.GetItemText(listBoxScanFilters.SelectedItem);
            // Clear bags before loading new data
            mzBag.Clear();
            intensityBag.Clear();
            timeBag.Clear();

            if (selectedFilter != null)
            {
                for (int i = 0; i < filesInFolder.Count(); i++)
                {
                    string fileName = filesInFolder[i];
                    (ArrayList mz, ArrayList intensity, double[] timeArray) = ThermoWrappers.LoadAndStoreFile(fileName, selectedFilter);
                    mzBag.Add(mz);
                    intensityBag.Add(intensity);
                    timeBag.Add(timeArray);
                    progressBarFileLoad.Value = i + 1;
                    progressBarFileLoad.Update();
                }

            }


            // Skapa en lista f�r att lagra de st�rsta v�rdena
            List<double> maxValues = new List<double>();
            // Loopa igenom varje double[] i timeBag
            foreach (double[] array in timeBag)
            {
                // H�mta det sista elementet i arrayn
                double lastValue = array[array.Length - 1];

                // L�gg till det sista v�rdet i listan av maxv�rden
                maxValues.Add(lastValue);
            }

            // Hitta det st�rsta v�rdet bland alla maxv�rden
            tmax = maxValues.Max();
            double t0 = 0;
            // Get mz data
            ArrayList extractedDat = wrappers.extractMassToCharge(mzBag, intensityBag, (double)numericMzValue.Value);
            double[,] matris_eic = wrappers.timeAlignment_single(timeBag, extractedDat, tmax);
            PlotModel model = wrappers.makeHeatMapFig(matris_eic);
            var x_dim = (double)numericVelocity.Value * tmax * 60;
            var y_dim = (double)numericSpacingBetweenLines.Value * timeBag.Count;
            var aspectRatio = x_dim / y_dim;
            double numPixels = 300;
            plotWindow1.Model = model;
            plotWindow1.Width = (int)Math.Round(numPixels);
            plotWindow1.Height = (int)Math.Round(numPixels / aspectRatio);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Get mz data

            ArrayList extractedDat = wrappers.extractMassToCharge(mzBag, intensityBag, (double)numericMzValue.Value);
            double[,] matris_eic = wrappers.timeAlignment_parallel(timeBag, extractedDat, tmax);
            PlotModel model = wrappers.makeHeatMapFig(matris_eic);
            var x_dim = (double)numericVelocity.Value * tmax * 60;
            var y_dim = (double)numericSpacingBetweenLines.Value * timeBag.Count;
            var aspectRatio = x_dim / y_dim;
            double numPixels = 300;
            plotWindow1.Model = model;
            plotWindow1.Width = (int)Math.Round(numPixels);
            plotWindow1.Height = (int)Math.Round(numPixels / aspectRatio);

            plotWindow1.Update();

        }

        private void textFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLoadAll_Click(object sender, EventArgs e)
        {


        }

        private void button4_Click(object sender, EventArgs e)
        {

            ConcurrentStack<double[]> intensityBag = new ConcurrentStack<double[]>();
            ConcurrentStack<double[]> timeBag = new ConcurrentStack<double[]>();

            var selectedFilter = dropdownFilter.SelectedItem.ToString();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 12
            };
            string selectedUnit = listBoxTolUnit.SelectedItem.ToString();

            progressBarFileLoad.Value = 0;

       
   
                var result = Enumerable.Range(0, mzBag.Count).AsParallel().AsOrdered()
                    .WithDegreeOfParallelism(parallelOptions.MaxDegreeOfParallelism)
                    .Select(index =>
                    {
                        IRawDataPlus file = (IRawDataPlus)mzBag[index];
                        
                        return ThermoWrappers.GetEIC((double)numericMzValue.Value, file, selectedFilter, (double)numPPM.Value, selectedUnit);
                        
                        
                    })
                    .ToArray();

            progressBarFileLoad.Value = mzBag.Count;




            foreach ((double[] intensity, double[]time) in result)
            {
                timeBag.Push(time);
                intensityBag.Push(intensity);
            }

            List<double> maxValues = new List<double>();


            foreach (double[] array in timeBag)
            {
                // H�mta det sista elementet i arrayn
                
                double lastValue = array[array.Length - 1];

                // L�gg till det sista v�rdet i listan av maxv�rden
                maxValues.Add(lastValue);
            }
           

 
            tmax = maxValues.Max();

            double[,] matris_eic = wrappers.timeAlignment_parallel_concurrent(timeBag, intensityBag, tmax);
            PlotModel model = wrappers.makeHeatMapFig(matris_eic);
            var x_dim = (double)numericVelocity.Value * tmax * 60;
            var y_dim = (double)numericSpacingBetweenLines.Value * timeBag.Count;
            var aspectRatio = x_dim / y_dim;
            double numPixels = 300;
            plotWindow1.Model = model;
            plotWindow1.Width = (int)Math.Round(numPixels);
            plotWindow1.Height = (int)Math.Round(numPixels / aspectRatio);
            

        }

        private void averageButton_Click(object sender, EventArgs e)
        {
            List<Scan> allavg = new List<Scan>(mzBag.Count);


            SpectrumAverager spectrumAverager = new SpectrumAverager();
            FtAverageOptions ftAverageOptions = new FtAverageOptions();
            ftAverageOptions.MergeInParallel = true;
            ftAverageOptions.MergeTaskBatching = 12;
            ftAverageOptions.MaxChargeDeterminations = 1;
            var selectedFilter = dropdownFilter.SelectedItem.ToString();
            string selectedUnit = listBoxTolUnit.SelectedItem.ToString();
            IRawDataPlus file = (IRawDataPlus)mzBag[0];

            file.SelectInstrument(Device.MS, 1);
            var options = file.DefaultMassOptions();
            options.Tolerance = (double)numPPM.Value;

            switch (selectedUnit)
            {
                case "ppm":
                    options.ToleranceUnits = ToleranceUnits.ppm;
                    break;
                case "mmu":
                    options.ToleranceUnits = ToleranceUnits.mmu;
                    break;
                case "amu":
                    options.ToleranceUnits = ToleranceUnits.amu;
                    break;
            }

            int start1 = file.GetFilteredScanEnumerator(selectedFilter).ElementAt(1);
            int stop1 = file.GetFilteredScanEnumerator(selectedFilter).Last();
            List<int> listScans = file.GetFilteredScansListByScanRange(selectedFilter, start1, stop1);
            var hej = file.GetScans(listScans);
            Scan avgFirstFile = file.AverageScansInScanRange(start1, stop1, selectedFilter, options);
            avgFirstFile = Scan.ToCentroid(avgFirstFile);
            // var tester = file.GetSegmentedScanFromScanNumber(start1);
            // var tester2 = file.GetSegmentedScanFromScanNumber(stop1);



            for (int i = 1; i < mzBag.Count; i++)
            {
                IRawDataPlus file2 = (IRawDataPlus)mzBag[i];
                file2.SelectInstrument(Device.MS, 1);
                int start = file2.GetFilteredScanEnumerator(selectedFilter).First();
                int stop = file2.GetFilteredScanEnumerator(selectedFilter).Last();


                Scan avgOneFile = file2.AverageScansInScanRange(start, stop, selectedFilter, options);
                avgOneFile = Scan.ToCentroid(avgOneFile);
                avgOneFile.AlwaysMergeSegments = true;

                avgFirstFile = spectrumAverager.Add(avgOneFile, avgFirstFile);
                avgFirstFile.AlwaysMergeSegments = true;

                List<int> listScans2 = file.GetFilteredScansListByScanRange(selectedFilter, start, stop);
                var hej2 = file.GetScans(listScans2);
                hej = hej.Concat(hej2);





                file2.Dispose();

                progressBarFileLoad.Value = i + 1;
                progressBarFileLoad.Update();
            }
            file.Dispose();

        }

        private void buttonROI_Click(object sender, EventArgs e)
        {

            TrackerManipulator roi = new TrackerManipulator(plotWindow1);
            

        }
        public void EnableDrawing()
        {
            
            this.MouseDown += new MouseEventHandler(testROI);
        }
 
        List<ScreenPoint> area = new List<ScreenPoint>();
        PolylineAnnotation roi = new PolylineAnnotation();
        private void testROI(object sender, MouseEventArgs e)
        {
            roi.LineStyle = LineStyle.Solid;
            roi.StrokeThickness = 50;
            roi.Color = OxyColors.Red;
            

            plotWindow1.Model.Annotations.Remove(roi);
           
                var mousePosition = new ScreenPoint(e.X, e.Y);
                area.Add(mousePosition);
            DataPoint v = OxyPlot.Axes.Axis.InverseTransform(mousePosition, plotWindow1.Model.Axes[0], plotWindow1.Model.Axes[1]);
            roi.Points.Add(v);



            // Uppdatera plot
            
            plotWindow1.Visible = true;
            plotWindow1.InvalidatePlot(false);
            
            plotWindow1.Model.Annotations.Add(roi);
            plotWindow1.Update();
        }
       
    }
}

