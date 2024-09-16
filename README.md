# bode-100
Bode 100 AI app

This is a Windows Forms application developed in C# using Visual Studio 2022.

## Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [.NET Framework](https://dotnet.microsoft.com/download/dotnet-framework)

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/your-username/MyWindowsFormsApp.git
    ```

2. Open the solution file (`.sln`) in Visual Studio.

3. Restore the NuGet packages:
    ```sh
    dotnet restore
    ```
    
4. Install the MCC DAQ Software:
   ```sh
   Download and install the MCC DAQ Software from the Measurement Computing website. This installation includes InstaCal (for hardware configuration) and the MCC DAQ API.
   ```
   ```sh
   Once installed, run InstaCal to configure and test your DAQ hardware. Ensure the device is properly configured before running the project.
   ```

5. Add MCC DAQ Reference to the Project:
   ```sh
   After installing the MCC DAQ software, add the required MccDaq.dll reference to your project:
   ```
       
       In Visual Studio, right-click on References in the Solution Explorer and select Add Reference.
       
       In the dialog, click Browse and navigate to the MCC DAQ installation folder (typically C:\Program Files (x86)\Measurement Computing\DAQ\ or a similar path).
       
       Select MccDaq.dll and add it to your project.
      

7. Build the project:
    ```sh
    dotnet build
    ```
8. Run the project to verify communication with the DAQ hardware.

## Running the Application

1. In Visual Studio, set the `MainForm` as the startup form.
2. Make sure to change the customPath in MainForm.cs and in CSV Functions uncomment out the sections of code stated in the function
3. Press `F5` or click the "Start" button in the toolbar to run the application.

## Configuration

- Ensure that the correct versions of the NuGet packages are installed.
- Update the connection settings if necessary.

## Usage

- Follow the on-screen instructions to use the application.
- Connect to the Bode 100 device as described.
