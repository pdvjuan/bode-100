using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Timers;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Interfaces;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Interfaces.Measurements;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Enumerations;
using MccDaq;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.DataTypes;
using Mages.Core.Tokens;
using Mages.Core.Runtime.Converters;

namespace BodeApp
{
    public partial class MainForm : Form
    {
        private BodeAutomationInterface auto;
        private BodeDevice bode;
        private AdapterMeasurement adapterMeasurement;
        private int measurementDuration; // in minutes
        private DateTime measurementEndTime;
        private List<double> resistanceAt1000HzListA = new List<double>();
        private List<double> resistanceAt1000HzListB = new List<double>();
        private List<double> resistanceAt1000HzListC = new List<double>();
        private List<double> resistanceAt1000HzListD = new List<double>();
        private int measurementCount = 0;
        private List<string> lengthOfSampleList = new List<string>();
        private string lengthOfSample = "";
        private List<string> testTempList = new List<string>();
        private List<string> timeList = new List<string>();
        private string testTemp = "";
        private string timeOfMeasurement = "";
        private int currChannel = 0;
        private int sampleCount = 4;
        private CancellationTokenSource cts;
        static MccBoard daqBoard = new MccDaq.MccBoard(0); // DAQ ID 0
        //Enter file path below
        private string customPath = @"C:\Users\juanp\Documents";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            auto = new BodeAutomation();
            daqBoard.DConfigPort(DigitalPortType.AuxPort, DigitalPortDirection.DigitalOut);
            // Set the TextBox to the current date and time
            dateTimeTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            bode = auto.Connect();
            if (bode != null)
            {
                MessageBox.Show("Connected to Bode 100");
                connectButton.BackColor = System.Drawing.Color.LightGreen;
                connectButton.Enabled = false;
                disconnectButton.Visible = true;
                disconnectButton.BackColor = Color.Red;
                openCalibrationButton.Enabled = true;
            }
        }

        private void openCalibrationButton_Click(object sender, EventArgs e)
        {
            adapterMeasurement = bode.Impedance.CreateAdapterMeasurement();
            ExecutionState state = adapterMeasurement.Calibration.FullRange.ExecuteOpen();
            if (state == ExecutionState.Ok)
            {
                openCalibrationButton.BackColor = Color.LightGreen;
                shortCalibrationButton.Enabled = true;
                openCalibrationButton.Enabled = false;
            }
            else
            {
                MessageBox.Show("Open calibration failed");
                openCalibrationButton.BackColor = SystemColors.Control;
            }
        }

        private void shortCalibrationButton_Click(object sender, EventArgs e)
        {
            ExecutionState state = adapterMeasurement.Calibration.FullRange.ExecuteShort();
            if (state == ExecutionState.Ok)
            {
                shortCalibrationButton.BackColor = Color.LightGreen;
                loadCalibrationButton.Enabled = true;
                shortCalibrationButton.Enabled = false;
            }
            else
            {
                MessageBox.Show("Short calibration failed");
                shortCalibrationButton.BackColor = SystemColors.Control;
            }
        }

        private void loadCalibrationButton_Click(object sender, EventArgs e)
        {
            ExecutionState state = adapterMeasurement.Calibration.FullRange.ExecuteLoad();
            if (state == ExecutionState.Ok)
            {
                loadCalibrationButton.BackColor = Color.LightGreen;
                startMeasurementButton.Enabled = true;
                loadCalibrationButton.Enabled = false;
            }
            else
            {
                MessageBox.Show("Load calibration failed");
                loadCalibrationButton.BackColor = SystemColors.Control;
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (bode != null)
            {
                bode.ShutDown();
                MessageBox.Show("Disconnected from Bode 100");
                connectButton.BackColor = SystemColors.Control;
                connectButton.Enabled = true;
                disconnectButton.Visible = false;
                startMeasurementButton.Enabled = false;
                ResetCalibrationButtons();
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                SaveToCSV();
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            resistanceAt1000HzListA.Clear();
            resistanceAt1000HzListB.Clear();
            resistanceAt1000HzListC.Clear();
            resistanceAt1000HzListD.Clear();

            resultsListBox.Items.Clear();
            lengthOfSampleList.Clear();
            testTempList.Clear();
            timeList.Clear();

            measurementCount = 0;
        }

        // IMPORTANT CODE SECTION FOR MEASUREMENTS - START
        private async void startMeasurementButton_Click(object sender, EventArgs e)
        {
            // Get the duration in minutes from the user
            if (!int.TryParse(durationTextBox.Text, out measurementDuration))
            {
                MessageBox.Show("Please enter a valid number of minutes.");
                return;
            }

            // Check if SelectedItem is null
            if (inputComboBox1.SelectedItem == null)
            {
                MessageBox.Show("No item selected in the ComboBox.");
                return;
            }

            // Try parsing the selected item to an integer
            bool isValid = int.TryParse(inputComboBox1.SelectedItem.ToString(), out sampleCount);

            if (!isValid)
            {
                MessageBox.Show("Invalid number selected. Please select a valid number.");
                return;
            }

            resultsListBox.Items.Add($"Number of Samples: {sampleCount}");

            startMeasurementButton.BackColor = Color.LightGreen;
            resetButton.Enabled = false;
            stopMeasurementButton.Visible = true;
            stopMeasurementButton.Enabled = true;

            int seconds = measurementDuration * 60;
            int numberOfIntervals = seconds / 10;

            // Initialize the cancellation token source
            cts = new CancellationTokenSource();

            try
            {
                // Start the measurement process on a separate thread
                await Task.Run(() => RunMeasurementLoop(numberOfIntervals, cts.Token));
                if (ValidateInputs())
                {
                    SaveToCSVAuto();
                }
                MessageBox.Show("Measurement process completed and exported.");
                resetButton.Enabled = true;
            }
            catch (OperationCanceledException)
            {
                SaveToCSVAuto();
                MessageBox.Show("Measurement process was canceled and exported.");
                stopMeasurementButton.Visible = false;
                startMeasurementButton.BackColor = SystemColors.Control;
                resetButton.Enabled = true;
                
            }
            finally
            {
                stopMeasurementButton.Visible = false;
                startMeasurementButton.BackColor = SystemColors.Control;
                resetButton.Enabled = true;
            }

        }

        private void stopMeasurementButton_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }

        private void RunMeasurementLoop(int numberOfIntervals, CancellationToken token)
        {
            for (int i = 0; i < numberOfIntervals; i++)
            {
                token.ThrowIfCancellationRequested();
                
                timeOfMeasurement = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                timeList.Add(timeOfMeasurement);

                lengthOfSample = inputTextBox6.Text;
                lengthOfSampleList.Add(lengthOfSample);

                testTemp = inputTextBox7.Text;
                testTempList.Add(testTemp);

                // Ex. 10000 = 10 seconds
                int sleepTime = 10000 - (sampleCount * 2000);

                if (sampleCount > 1)
                {
                    for (int channel = 0; channel < sampleCount; channel++)
                    {
                        SwitchChannel(channel);
                    }
                }
                else
                {
                    ExecuteMeasurement();
                }


                Thread.Sleep(sleepTime);
            }
        }
        private void ExecuteMeasurement()
        {
            measurementCount++;
            // Configure sweep
            adapterMeasurement.ConfigureSweep(900, 1100, 201, SweepMode.Logarithmic);

            // Start the measurement
            ExecutionState state = adapterMeasurement.ExecuteMeasurement();
            if (state != ExecutionState.Ok)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(state.ToString());
                    bode.ShutDown();
                });
                return;
            }

            // Retrieve the measurement frequencies and resistance values
            double[] frequencies = adapterMeasurement.Results.MeasurementFrequencies;
            double[] resistances = adapterMeasurement.Results.Rs();

            // Find index of the frequencies closest to 1000 Hz
            int index1000Hz = FindClosestIndex(frequencies, 1000);

            // Retrieve resistance values at the closest indices
            double resistanceAt1000Hz = resistances[index1000Hz];

            // Add the resistance value to the list
            if (currChannel == 0)
            {
                resistanceAt1000HzListA.Add(resistanceAt1000Hz);
            }
            else if (currChannel == 1)
            {
                resistanceAt1000HzListB.Add(resistanceAt1000Hz);
            }
            else if (currChannel == 2)
            {
                resistanceAt1000HzListC.Add(resistanceAt1000Hz);
            }
            else if (currChannel == 3)
            {
                resistanceAt1000HzListD.Add(resistanceAt1000Hz);
            }

            this.Invoke((MethodInvoker)delegate
            {
                resultsListBox.Items.Add($"{measurementCount}. Channel: {currChannel}. Sample: {lengthOfSample}. Temp {testTemp}. Res at index {index1000Hz}: {resistanceAt1000Hz} Ohms. Time: {timeOfMeasurement}");
            });

            exportButton.Enabled = true;
        }
        static int FindClosestIndex(double[] array, double target)
        {
            int closestIndex = -1;
            double smallestDifference = double.MaxValue;

            for (int i = 0; i < array.Length; i++)
            {
                double difference = Math.Abs(array[i] - target);
                if (difference < smallestDifference)
                {
                    smallestDifference = difference;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        //Code for MccDaq
        private void SwitchChannel(int channel)
        {
            daqBoard.DBitOut(DigitalPortType.AuxPort, 0, (channel & 1) > 0 ? DigitalLogicState.High : DigitalLogicState.Low);
            daqBoard.DBitOut(DigitalPortType.AuxPort, 1, (channel & 2) > 0 ? DigitalLogicState.High : DigitalLogicState.Low);
            daqBoard.DBitOut(DigitalPortType.AuxPort, 2, (channel & 4) > 0 ? DigitalLogicState.High : DigitalLogicState.Low);
            daqBoard.DBitOut(DigitalPortType.AuxPort, 3, (channel & 8) > 0 ? DigitalLogicState.High : DigitalLogicState.Low);

            currChannel = channel;

            ExecuteMeasurement();

            Console.WriteLine($"Switched to channel I{channel}");
        }

        // IMPORTANT CODE SECTION FOR MEASUREMENTS - END

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            bool allInputsFilled = ValidateInputs();

            connectButton.Enabled = allInputsFilled;
            openCalibrationButton.Enabled = allInputsFilled;
        }

        private void ResetCalibrationButtons()
        {
            openCalibrationButton.Enabled = true;
            shortCalibrationButton.Enabled = false;
            loadCalibrationButton.Enabled = false;

            openCalibrationButton.BackColor = SystemColors.Control;
            shortCalibrationButton.BackColor = SystemColors.Control;
            loadCalibrationButton.BackColor = SystemColors.Control;
            startMeasurementButton.BackColor = SystemColors.Control;
        }

        private bool ValidateInputs()
        {
            return !string.IsNullOrWhiteSpace(inputTextBox1.Text) &&
                   !string.IsNullOrWhiteSpace(inputTextBox2.Text) &&
                   !string.IsNullOrWhiteSpace(inputTextBox3.Text) &&
                   !string.IsNullOrWhiteSpace(inputTextBox4.Text) &&
                   !string.IsNullOrWhiteSpace(inputTextBox5.Text) &&
                   !string.IsNullOrWhiteSpace(inputTextBox6.Text) &&
                   !string.IsNullOrWhiteSpace(inputTextBox7.Text);
        }

        private void SaveToCSVAuto()
        {
            // Format the current date and time for use in the filename
            string timestamp = DateTime.Now.ToString("yy-MM-dd HH_mm_ss");
            string fileName = $"Report_{inputTextBox2.Text}_{timestamp}.csv";

            string[] sampleIds = { inputTextBox3.Text, inputTextBox9.Text, inputTextBox10.Text, inputTextBox11.Text };

            for (int i = sampleCount; i < sampleIds.Length; i++)
            {
                sampleIds[i] = ""; // Set to empty string for unused samples
            }

            string userName = inputTextBox1.Text;
            string testName = inputTextBox2.Text;
            string roomTemp = inputTextBox4.Text;
            string humidity = inputTextBox5.Text;

            var header = $"Date, Name, Test Name, Room Temp, Humidity, Sample Length, Test Length, Test Temp";
            for (int i = 0; i < sampleCount; i++)
            {
                header += $", {sampleIds[i]}";
            }

            var csvLines = new List<string> { header };

            List<double>[] resistanceLists = {
                resistanceAt1000HzListA,
                resistanceAt1000HzListB,
                resistanceAt1000HzListC,
                resistanceAt1000HzListD
            };

            // Ensure all lists have the same length
            int maxLength = resistanceLists.Max(list => list.Count);
            maxLength = Math.Max(maxLength, lengthOfSampleList.Count);
            maxLength = Math.Max(maxLength, testTempList.Count);
            maxLength = Math.Max(maxLength, timeList.Count);

            // Pad all resistance lists to the maximum length
            for (int i = 0; i < sampleCount; i++)
            {
                while (resistanceLists[i].Count < maxLength)
                {
                    resistanceLists[i].Add(0);
                }
            }

            // Pad the other lists (sampleSize, testTemperature, and timePoint) to match the maximum length
            while (lengthOfSampleList.Count < maxLength)
            {
                lengthOfSampleList.Add("0");
            }

            while (testTempList.Count < maxLength)
            {
                testTempList.Add("0");
            }

            while (timeList.Count < maxLength)
            {
                timeList.Add("00:00:00");
            }

            for (int i = 0; i < resistanceAt1000HzListA.Count; i++)
            {
                string sampleSize = lengthOfSampleList[i];
                string testTemperature = testTempList[i];
                string timePoint = timeList[i];

                string line = $"{timePoint}, {userName}, {testName}, {roomTemp}, {humidity}, {sampleSize}, {measurementDuration}, {testTemperature}";

                for (int j = 0; j < sampleCount; j++)
                {
                    line += $", {resistanceLists[j][i]}";
                }

                csvLines.Add(line);
            }

            //Code Below can be enable to give the program a custom path to save csv (BECAREFUL if using onedrive due to conectivity issues)
            string filePath = Path.Combine(customPath, fileName);

            //Update fileName to filePath if setting up custom path
            File.WriteAllLines(filePath, csvLines);
        }

        private void SaveToCSV()
        {
            // Format the current date and time for use in the filename
            string timestamp = DateTime.Now.ToString("yy-MM-dd HH_mm_ss");
            string fileName = $"Report_{inputTextBox2.Text}_{timestamp}.csv";

            string[] sampleIds = { inputTextBox3.Text, inputTextBox9.Text, inputTextBox10.Text, inputTextBox11.Text };

            for (int i = sampleCount; i < sampleIds.Length; i++)
            {
                sampleIds[i] = ""; // Set to empty string for unused samples
            }

            string userName = inputTextBox1.Text;
            string testName = inputTextBox2.Text;
            string roomTemp = inputTextBox4.Text;
            string humidity = inputTextBox5.Text;

            var header = $"Date, Name, Test Name, Room Temp, Humidity, Sample Length, Test Length, Test Temp";
            for (int i = 0; i < sampleCount; i++)
            {
                header += $", {sampleIds[i]}";
            }

            var csvLines = new List<string> { header };

            List<double>[] resistanceLists = {
                resistanceAt1000HzListA,
                resistanceAt1000HzListB,
                resistanceAt1000HzListC,
                resistanceAt1000HzListD
            };

            // Ensure all lists have the same length
            int maxLength = resistanceLists.Max(list => list.Count);
            maxLength = Math.Max(maxLength, lengthOfSampleList.Count);
            maxLength = Math.Max(maxLength, testTempList.Count);
            maxLength = Math.Max(maxLength, timeList.Count);

            // Pad all resistance lists to the maximum length
            for (int i = 0; i < sampleCount; i++)
            {
                while (resistanceLists[i].Count < maxLength)
                {
                    resistanceLists[i].Add(0);
                }
            }

            // Pad the other lists (sampleSize, testTemperature, and timePoint) to match the maximum length
            while (lengthOfSampleList.Count < maxLength)
            {
                lengthOfSampleList.Add("0");
            }

            while (testTempList.Count < maxLength)
            {
                testTempList.Add("0");
            }

            while (timeList.Count < maxLength)
            {
                timeList.Add("00:00:00");
            }

            for (int i = 0; i < resistanceAt1000HzListA.Count; i++)
            {
                string sampleSize = lengthOfSampleList[i];
                string testTemperature = testTempList[i];
                string timePoint = timeList[i];

                string line = $"{timePoint}, {userName}, {testName}, {roomTemp}, {humidity}, {sampleSize}, {measurementDuration}, {testTemperature}";

                for (int j = 0; j < sampleCount; j++)
                {
                    line += $", {resistanceLists[j][i]}";
                }

                csvLines.Add(line);
            }

            //Code Below can be enable to give the program a custom path to save csv (BECAREFUL if using onedrive due to conectivity issues)
            string filePath = Path.Combine(customPath, fileName);

            //Update fileName to filePath if setting up custom path
            File.WriteAllLines(filePath, csvLines);

            //Update fileName to filePath if setting up custom path
            MessageBox.Show($"Data has been exported to {filePath}");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bode?.ShutDown();
        }
    }
}
