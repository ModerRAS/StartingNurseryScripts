using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StartingNurseryScript.Common {
    public class AdbWrapper {
        private string adbPath;
        private string deviceSerial;

        public AdbWrapper(string adbPath) {
            this.adbPath = adbPath;
            this.deviceSerial = GetConnectedDevices().FirstOrDefault();
        }

        public AdbWrapper(string adbPath, string deviceSerial) {
            this.adbPath = adbPath;
            this.deviceSerial = deviceSerial;
        }

        public void SetDeviceSerial(string deviceSerial) {
            this.deviceSerial = deviceSerial;
        }

        public void CaptureScreenshot(string savePath) {
            ExecuteAdbCommand($"shell screencap -p > /sdcard/screenshot.png");
            ExecuteAdbCommand($"pull /sdcard/screenshot.png {savePath}");
        }

        public void Swipe(int startX, int startY, int endX, int endY) {
            ExecuteAdbCommand($"shell input swipe {startX} {startY} {endX} {endY}");
        }

        private void ExecuteAdbCommand(string command, string savePath = "") {
            string args = $"{(deviceSerial != null ? "-s " + deviceSerial : "")} {command}";

            Process process = new Process();
            process.StartInfo.FileName = adbPath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            if (!string.IsNullOrEmpty(savePath)) {
                MemoryStream memoryStream = new MemoryStream();
                process.StandardOutput.BaseStream.CopyTo(memoryStream);
                File.WriteAllBytes(savePath, memoryStream.ToArray());
            }

            process.WaitForExit();
        }

        private string[] GetConnectedDevices() {
            Process process = new Process();
            process.StartInfo.FileName = adbPath;
            process.StartInfo.Arguments = "devices";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(line => line.Split('\t').First())
                .ToArray();
        }
    }
}
