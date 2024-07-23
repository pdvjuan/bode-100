using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Timers;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Interfaces;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Interfaces.Measurements;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.Enumerations;
using OmicronLab.VectorNetworkAnalysis.AutomationInterface.DataTypes;
using Mages.Core.Tokens;

namespace BodeApp
{
    public partial class MainForm : Form
    {
        private BodeAutomationInterface auto;
        private BodeDevice bode;
        private AdapterMeasurement adapterMeasurement;
        private int measurementDuration; // in minutes
        private DateTime measurementEndTime;
        private List<double> resistanceAt1000HzList = new List<double>();
        private int measurementCount = 0;
        private CancellationTokenSource cts;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            auto = new BodeAutomation();
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
            resultsListBox.Items.Clear();
            durationTextBox.Text = string.Empty;
            measurementCount = 0;
            // Set the TextBox to the current date and time
            dateTimeTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            startMeasurementButton.BackColor = SystemColors.Control;
            stopMeasurementButton.Visible = false;
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

            startMeasurementButton.BackColor = Color.LightGreen;
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
                MessageBox.Show("Measurement process completed.");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Measurement process was canceled.");
                stopMeasurementButton.Visible = false;
                startMeasurementButton.BackColor = SystemColors.Control;
            }
            finally
            {
                stopMeasurementButton.Visible = false;
                startMeasurementButton.BackColor = SystemColors.Control;
            }

        }

        private void stopMeasurementButton_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }

        private void RunMeasurementLoop(int numberOfIntervals,CancellationToken token)
        {
            for (int i = 0; i < numberOfIntervals; i++)
            {
                token.ThrowIfCancellationRequested();
                ExecuteMeasurement();
                // 10000 = 10 seconds
                Thread.Sleep(10000);
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
            resistanceAt1000HzList.Add(resistanceAt1000Hz);

            this.Invoke((MethodInvoker)delegate
            {
                resultsListBox.Items.Add($"{measurementCount}. Resistance at index {index1000Hz}: {resistanceAt1000Hz} Ohms");
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



        private void SaveToCSV()
        {
            // Format the current date and time for use in the filename
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filePath = $"Report_{timestamp}.csv";

            var csvLines = new List<string>
            {
                "Date, Name, Test Name, Sample ID, Room Temp, Humidity, Sample Length, Test Length, Test Temp, 1000s Resistance",
            };

            foreach (double resistance in resistanceAt1000HzList)
            {
                csvLines.Add($"{dateTimeTextBox.Text}, {inputTextBox1.Text}, {inputTextBox2.Text}, {inputTextBox3.Text}, {inputTextBox4.Text}, {inputTextBox5.Text}, {inputTextBox6.Text},{measurementDuration}, {inputTextBox7.Text}, {resistance}");
            }

            File.WriteAllLines(filePath, csvLines);

            MessageBox.Show($"Data has been exported to {filePath}");
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bode?.ShutDown();
        }
    }
}
