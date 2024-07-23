using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Interfaces;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Interfaces.Measurements;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Enumerations;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.DataTypes;

namespace BodeApp
{
    public partial class MainForm : Form
    {
        private BodeAutomationInterface auto;
        private BodeDevice bode;
        private AdapterMeasurement adapterMeasurement;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            auto = new BodeAutomation();
            dateTimeTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Set the TextBox to the current date and time
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            bode = auto.Connect();
            if (bode != null)
            {
                MessageBox.Show("Connected to Bode 100");
                connectButton.BackColor = System.Drawing.Color.Green;
                disconnectButton.Visible = true;
                openCalibrationButton.Enabled = true;
                //startMeasurementButton.Enabled = true;
            }
        }

        private void openCalibrationButton_Click(object sender, EventArgs e)
        {
            adapterMeasurement = bode.Impedance.CreateAdapterMeasurement();
            ExecutionState state = adapterMeasurement.Calibration.FullRange.ExecuteOpen();
            if (state == ExecutionState.Ok)
            {
                openCalibrationButton.BackColor = Color.Green;
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
                shortCalibrationButton.BackColor = Color.Green;
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
                loadCalibrationButton.BackColor = Color.Green;
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
                connectButton.BackColor = SystemColors.Control; // Reset to default color
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


        // IMPORTANT CODE SECTION FOR MEASUREMENTS - START

        private void startMeasurementButton_Click(object sender, EventArgs e)
        {
            // Configure the measurement criteria HERE
            //TODO - find out if anything else is needed here for set up - NEED TO CHECK MEASUREMENTS AT 1KHZ AND 10KHZ
            // 200 Points to measure
            adapterMeasurement.ConfigureSweep(900, 1100, 201, SweepMode.Logarithmic); 

            // Start the measurement
            ExecutionState state = adapterMeasurement.ExecuteMeasurement();
            if (state != ExecutionState.Ok)
            {
                //MessageBox.Show("Measurement failed");
                MessageBox.Show(state.ToString());
                bode.ShutDown();
                return;
            }

            // TODO - find out how to get the right results - TODO START

            double[] frequenciesAt1000 = adapterMeasurement.Results.MeasurementFrequencies;
            double[] resistanceAt1000 = adapterMeasurement.Results.Rs();

            resultsListBox.Items.Clear();
            
            resultsListBox.Items.Add("Frequency (Hz)  |  Res" );
            for (int i = 0; i < frequenciesAt1000.Length; i++)
            {
                resultsListBox.Items.Add($"{frequenciesAt1000[i]}  |  {resistanceAt1000[i]}");
            }

            adapterMeasurement.ConfigureSweep(9900, 10100, 201, SweepMode.Logarithmic);

            state = adapterMeasurement.ExecuteMeasurement();
            if (state != ExecutionState.Ok)
            {
                //MessageBox.Show("Measurement failed");
                MessageBox.Show(state.ToString());
                bode.ShutDown();
                return;
            }

            // TODO - find out how to get the right results - TODO START

            double[] frequenciesAt10000 = adapterMeasurement.Results.MeasurementFrequencies;
            double[] resistanceAt10000 = adapterMeasurement.Results.Rs();

            resultsListBox.Items.Add("Frequency (Hz)  |  Res");
            for (int i = 0; i < frequenciesAt10000.Length; i++)
            {
                resultsListBox.Items.Add($"{frequenciesAt10000[i]}  |  {resistanceAt10000[i]}");
            }

            int index1000Hz = FindClosestIndex(frequenciesAt1000, 1000);
            int index10000Hz = FindClosestIndex(frequenciesAt10000, 10000);

            resultsListBox.Items.Add($"Index closest to 1000 Hz: {index1000Hz}");
            resultsListBox.Items.Add($"Frequency at index {index1000Hz}: {frequenciesAt1000[index1000Hz]} Hz");
            resultsListBox.Items.Add($"Resistance at index {index1000Hz}: {resistanceAt1000[index1000Hz]} Hz");

            resultsListBox.Items.Add($"Index closest to 10000 Hz: {index10000Hz}");
            resultsListBox.Items.Add($"Frequency at index {index10000Hz}: {frequenciesAt10000[index10000Hz]} Hz");
            resultsListBox.Items.Add($"Resistance at index {index10000Hz}: {resistanceAt10000[index10000Hz]} Hz");

            // TODO - find out how to get the right results - TODO START

            exportButton.Enabled = true;
        }

        // IMPORTANT CODE SECTION FOR MEASUREMENTS - END
        
        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            bool allInputsFilled = ValidateInputs();

            connectButton.Enabled = allInputsFilled;
            openCalibrationButton.Enabled = allInputsFilled;
            exportButton.Enabled = allInputsFilled;
        }

        private void ResetCalibrationButtons()
        {
            openCalibrationButton.Enabled = true;
            shortCalibrationButton.Enabled = false;
            loadCalibrationButton.Enabled = false;

            openCalibrationButton.BackColor = SystemColors.Control;
            shortCalibrationButton.BackColor = SystemColors.Control;
            loadCalibrationButton.BackColor = SystemColors.Control;
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

        private void SaveToCSV()
        {
            // Format the current date and time for use in the filename
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filePath = $"Report_{timestamp}.csv";

            var csvLines = new List<string>
            {
                "Date, Input 1, Input 2, Input 3, Input 4, Input 5, Input 6, Input 7",
                $"{dateTimeTextBox.Text}, {inputTextBox1.Text}, {inputTextBox2.Text}, {inputTextBox3.Text}, {inputTextBox4.Text}, {inputTextBox5.Text}, {inputTextBox6.Text}, {inputTextBox7.Text}"
            };

            File.WriteAllLines(filePath, csvLines);

            MessageBox.Show($"Data has been exported to {filePath}");
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bode?.ShutDown();
        }
    }
}
