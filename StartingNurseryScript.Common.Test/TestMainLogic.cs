using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common.Test {
    [TestClass]
    public class TestMainLogic {
        public static readonly int[] TestData = {
7,4,1,2,6,1,8,3,7,1,
2,9,9,2,6,3,3,5,5,3,
5,3,5,5,4,9,5,1,9,4,
8,6,7,8,8,7,2,7,7,2,
8,3,8,2,7,1,7,8,3,7,
6,1,1,1,3,3,9,7,2,4,
8,5,4,1,9,6,8,1,8,1,
2,3,8,5,7,5,3,8,1,1,
2,1,2,7,3,7,9,3,2,5,
4,9,1,7,1,2,1,6,9,7,
1,8,5,5,7,3,5,6,8,4,
2,6,2,1,1,3,3,4,6,8,
4,5,2,4,8,4,7,9,2,8,
8,7,9,5,9,9,7,6,6,3,
1,3,8,8,5,1,5,4,5,6,
9,4,6,9,6,6,3,2,6,7,
        };
        [TestMethod]
        public void TestMainLogic_1() {

            string adbPath = @"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe";
            string deviceSerial = "192.168.121.223:39581";
            var main = new MainLogic(adbPath, deviceSerial);
            main.ExecuteAsync().Wait();
        }

        [TestMethod]
        public void TestCalculate() {
            var map = Calculate.GenerateMap(TestData);
            //var (bestStep, bestMap) = Calculate.CalculateBestScoreDFS(map);
            //Console.WriteLine(bestStep.Count);
        }
        [TestMethod]
        public void TestCalculateWithGA() {
            var map = Calculate.GenerateMap(TestData);
            var (bestStep, bestMap) = Calculate.CalculateBestScoreGreedyAlgorithm(map);
            Console.WriteLine(bestStep.Count);
        }
        [TestMethod]
        public void ConstructMap_Test() {
            // 创建一些 NumberPhoto 对象
            var numberPhotos = new List<NumberPhoto> {
                new NumberPhoto { Points = new[] { new Point(0, 0), new Point(10, 0), new Point(10, 10), new Point(0, 10) }, Number = 1 },
                new NumberPhoto { Points = new[] { new Point(15, 0), new Point(25, 0), new Point(25, 10), new Point(15, 10) }, Number = 2 },
                new NumberPhoto { Points = new[] { new Point(30, 0), new Point(40, 0), new Point(40, 10), new Point(30, 10) }, Number = 3 },
                new NumberPhoto { Points = new[] { new Point(0, 15), new Point(10, 15), new Point(10, 25), new Point(0, 25) }, Number = 4 },
                new NumberPhoto { Points = new[] { new Point(15, 15), new Point(25, 15), new Point(25, 25), new Point(15, 25) }, Number = 5 },
                new NumberPhoto { Points = new[] { new Point(30, 15), new Point(40, 15), new Point(40, 25), new Point(30, 25) }, Number = 6 },
                new NumberPhoto { Points = new[] { new Point(0, 30), new Point(10, 30), new Point(10, 40), new Point(0, 40) }, Number = 7 },
                new NumberPhoto { Points = new[] { new Point(15, 30), new Point(25, 30), new Point(25, 40), new Point(15, 40) }, Number = 8 },
                new NumberPhoto { Points = new[] { new Point(30, 30), new Point(40, 30), new Point(40, 40), new Point(30, 40) }, Number = 9 }
            };

            int[,] expectedMap =
            {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

            int[,] map = MainLogic.ConstructMap(ref numberPhotos);

            // 调试输出实际生成的二维数组的大小
            Console.WriteLine("Actual map dimensions: {0}x{1}", map.GetLength(0), map.GetLength(1));

            // 调试输出期望的二维数组的大小
            Console.WriteLine("Expected map dimensions: {0}x{1}", expectedMap.GetLength(0), expectedMap.GetLength(1));


            // 断言两个二维数组相等
            CollectionAssert.AreEqual(expectedMap, map);
        }

        [TestMethod]
        public void ConstructMap_Test_2() {
            // 创建一些 NumberPhoto 对象
            var numberPhotos = new List<NumberPhoto> {
                new NumberPhoto { Points = new[] { new Point(0, 0), new Point(10, 0), new Point(10, 10), new Point(0, 10) }, Number = 1 },
                new NumberPhoto { Points = new[] { new Point(15, 0), new Point(25, 0), new Point(25, 10), new Point(15, 10) }, Number = 2 },
                new NumberPhoto { Points = new[] { new Point(0, 15), new Point(10, 15), new Point(10, 25), new Point(0, 25) }, Number = 4 },
                new NumberPhoto { Points = new[] { new Point(15, 15), new Point(25, 15), new Point(25, 25), new Point(15, 25) }, Number = 5 },
                new NumberPhoto { Points = new[] { new Point(0, 30), new Point(10, 30), new Point(10, 40), new Point(0, 40) }, Number = 7 },
                new NumberPhoto { Points = new[] { new Point(15, 30), new Point(25, 30), new Point(25, 40), new Point(15, 40) }, Number = 8 }
            };

            int[,] expectedMap =
            {
            { 1, 2 },
            { 4, 5 },
            { 7, 8 }
        };

            int[,] map = MainLogic.ConstructMap(ref numberPhotos);

            // 调试输出实际生成的二维数组的大小
            Console.WriteLine("Actual map dimensions: {0}x{1}", map.GetLength(0), map.GetLength(1));

            // 调试输出期望的二维数组的大小
            Console.WriteLine("Expected map dimensions: {0}x{1}", expectedMap.GetLength(0), expectedMap.GetLength(1));


            // 断言两个二维数组相等
            CollectionAssert.AreEqual(expectedMap, map);
        }

        [TestMethod]
        public void TestOpenCVMatchTemplate_1() {
            var photoByte = File.ReadAllBytes("TestData/Numbers/718.jpg");
            var photo = new NumberPhoto() {
                Photo = photoByte,
            };
            var number = OpenCVHelper.Detect(photo);
            Assert.AreEqual(7, number.Number);
        }
        [TestMethod]
        public void TestOpenCVMatchTemplate_2() {
            var photoByte = File.ReadAllBytes("TestData/Numbers/719.jpg");
            var photo = new NumberPhoto() {
                Photo = photoByte,
            };
            var number = OpenCVHelper.Detect(photo);
            Assert.AreEqual(2, number.Number);
        }
    }
}
