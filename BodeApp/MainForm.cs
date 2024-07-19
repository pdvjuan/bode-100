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
                startMeasurementButton.Enabled = true;
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
            }
        }


        private void startMeasurementButton_Click(object sender, EventArgs e)
        {
            AdapterMeasurement adapterMeasurement = bode.Impedance.CreateAdapterMeasurement();

            // Perform Open Calibration
            ExecutionState state = adapterMeasurement.Calibration.FullRange.ExecuteOpen();
            if (state != ExecutionState.Ok)
            {
                MessageBox.Show("Open calibration failed");
                bode.ShutDown();
                return;
            }

            // Perform Short Calibration
            state = adapterMeasurement.Calibration.FullRange.ExecuteShort();
            if (state != ExecutionState.Ok)
            {
                MessageBox.Show("Short calibration failed");
                bode.ShutDown();
                return;
            }

            // Perform Load Calibration
            state = adapterMeasurement.Calibration.FullRange.ExecuteLoad();
            if (state != ExecutionState.Ok)
            {
                MessageBox.Show("Load calibration failed");
                bode.ShutDown();
                return;
            }

            // Configure the measurement criteria HERE
            //TODO - find out if anything else is needed here for set up
            adapterMeasurement.ConfigureSweep(10, 400, 201, SweepMode.Logarithmic);



            // Start the measurement
            state = adapterMeasurement.ExecuteMeasurement();
            if (state != ExecutionState.Ok)
            {
                MessageBox.Show("Measurement failed");
                bode.ShutDown();
                return;
            }

            // TODO - find out how to get the right results - TODO START

            // Results HERE - Version 1
            double[] inductivity = adapterMeasurement.Results.Rs();

            resultsListBox.Items.Clear();

            resultsListBox.Items.Add("Inductivity (Ohm)");

            for (int i = 0; i < inductivity.Length; i++)
            {
                resultsListBox.Items.Add($"{inductivity[i]}");
            }
            // Results Ends - Version 1


            // Results HERE - Version 2
            //double[] frequencies = adapterMeasurement.Results.MeasurementFrequencies;
            //double[] inductivity = adapterMeasurement.Results.Rs(); 
            //
            //resultsListBox.Items.Clear();
            //
            //resultsListBox.Items.Add("Frequency (Hz)  |  Inductivity (Ohm)");
            //for (int i = 0; i < frequencies.Length; i++)
            //{
            //    resultsListBox.Items.Add($"{frequencies[i]}  |  {inductivity[i]}");
            //}
            // Results Ends - Version 2


            // Results HERE - Version 3
            //double[] frequencies = adapterMeasurement.Results.MeasurementFrequencies;
            //double[] inductivity = adapterMeasurement.Results.Rs();
            //double[] phase = adapterMeasurement.Results.Phase(AngleUnit.Degree);
            //
            //resultsListBox.Items.Clear();
            //
            //resultsListBox.Items.Add("Frequency (Hz)  |  Magnitude (dB)  |  Phase (Degrees)");
            //for (int i = 0; i < frequencies.Length; i++)
            //{
            //    resultsListBox.Items.Add($"{frequencies[i]}  |  {inductivity[i]}  |  {phase[i]}");
            //}
            // Results End - Version 3

            // TODO - find out how to get the right results - TODO START

            exportButton.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bode?.ShutDown();
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            bool allInputsFilled = ValidateInputs();

            connectButton.Enabled = allInputsFilled;
            exportButton.Enabled = allInputsFilled;
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

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                SaveToCSV();
            }
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
    }
}
