using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common.Test {
    [TestClass]
    public class TestADB {
        [TestMethod]
        public void TestADB_1() {
            string adbPath = @"C:\path\to\adb.exe";
            string deviceSerial = "your_device_serial";

            AdbWrapper adbWrapper = new AdbWrapper(adbPath, deviceSerial);

            string screenshotFilePath = Path.Combine(Environment.CurrentDirectory, "screenshot.png");
            adbWrapper.CaptureScreenshot(screenshotFilePath);
            Console.WriteLine($"截图已保存至: {screenshotFilePath}");

            int startX = 100;
            int startY = 500;
            int endX = 500;
            int endY = 100;
            adbWrapper.Swipe(startX, startY, endX, endY);
            Console.WriteLine($"从点({startX}, {startY})滑动到点({endX}, {endY})");
        }
    }
}
