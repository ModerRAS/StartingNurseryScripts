using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common {
    public class MainLogic {
        public AdbWrapper adbWrapper { get; set; }
        public MainLogic(string adbPath, string deviceSerial) {
            adbWrapper = new AdbWrapper(adbPath, deviceSerial);
        }
        public MainLogic(string deviceSerial) {
            var adbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb", "adb.exe");
            adbWrapper = new AdbWrapper(adbPath, deviceSerial);
        }
        public static int[,] ConstructMap(ref List<NumberPhoto> numberPhotos) {
            // 根据 Center 点的 Y 坐标分组，Y 坐标差在 10 以内的为同一行
            var rows = new List<List<NumberPhoto>>();
            foreach (var np in numberPhotos) {
                bool added = false;
                foreach (var row in rows) {
                    if (row.Any() && Math.Abs(row.First().Center.Y - np.Center.Y) <= 10) {
                        // 将 NumberPhoto 对象按照 X 坐标排序插入到行中
                        int index = row.FindIndex(p => p.Center.X > np.Center.X);
                        if (index == -1) {
                            row.Add(np);
                        } else {
                            row.Insert(index, np);
                        }
                        added = true;
                        break;
                    }
                }
                if (!added) {
                    rows.Add(new List<NumberPhoto> { np });
                }
            }

            // 获取行数和每行的元素数量，以确定二维数组的大小
            int rowCount = rows.Count;
            int maxElementsInRow = rows.Any() ? rows.Max(row => row.Count) : 0;

            // 构建二维数组
            int[,] map = new int[rowCount, maxElementsInRow];

            // 填充二维数组
            for (int i = 0; i < rowCount; i++) {
                var row = rows[i];
                for (int j = 0; j < row.Count; j++) {
                    map[i, j] = row[j].Number;
                    row[j].Y = i;
                    row[j].X = j;
                }
            }

            return map;
        }

        public async Task StartGame() {
            var detect = new DetectNumber();
            while (true) {
                await Task.Delay(5000);
                adbWrapper.CaptureScreenshot("tmp.jpg");
                var jpg = File.ReadAllBytes("tmp.jpg");
                var p = detect.GetStartGame(jpg);
                if (!(p.X == 0 && p.Y == 0)) {
                    adbWrapper.Swipe(p.X, p.Y, p.X + 1, p.Y + 1);
                    break;
                }
                p = detect.GetStartNext(jpg);
                if (!(p.X == 0 && p.Y == 0)) {
                    adbWrapper.Swipe(p.X, p.Y, p.X + 1, p.Y + 1);
                    break;
                }
                p = detect.GetOk(jpg);
                if (!(p.X == 0 && p.Y == 0)) {
                    adbWrapper.Swipe(p.X, p.Y, p.X + 1, p.Y + 1);
                }
                p = detect.GetTip(jpg);
                if (!(p.X == 0 && p.Y == 0)) {
                    adbWrapper.Swipe(p.X, p.Y, p.X + 1, p.Y + 1);
                }
            }

            await Task.Delay(5000);
        }


        public async Task ExecuteAsync() {

            await StartGame();
            var Name = $"{DateTime.Now.Millisecond}";
            var imagePath = $"{Name}.png";
            adbWrapper.CaptureScreenshot(imagePath);
            string outputPath = "output.jpg";
            var square = new SquareDetector(imagePath);
            var squares = square.DetectAndDrawSquare(outputPath);
            var numberPhotos = square.GetSmallImagesAsByteArrays(squares);
            var outPhotos = new List<NumberPhoto>();
            foreach (var e in numberPhotos) {
                outPhotos.Add(OpenCVHelper.Detect(e));
            }
            int count = 0;
            foreach (var e in outPhotos) {
                if (e.Number < 0) {
                    Console.WriteLine(e.Number);
                    count++;
                }
            }
            StringBuilder mapstr = new StringBuilder();
            var map = ConstructMap(ref outPhotos);
            for (var y = 0; y < 16; y++) {
                for (var x = 0; x < 10; x++) {
                    Console.Write(map[y, x]);
                    Console.Write(" ");
                    mapstr.Append(map[y, x]);
                    mapstr.Append(" ");
                }
                Console.WriteLine();
                mapstr.Append('\n');
            }
            File.WriteAllText($"{Name}.txt", mapstr.ToString());
            Console.WriteLine($"Count is: {count}");
            Console.WriteLine($"OutPhotos Count: {outPhotos.Count}");


            var (bestStep, bestMap) = Calculate.CalculateBestScoreACO(map);

            Console.WriteLine(bestStep.Count);
            Console.WriteLine(Calculate.CalculateScore(bestMap));

            File.WriteAllText($"{Name}.json", JsonConvert.SerializeObject(bestStep));
            foreach (var e in bestStep) {
                var sourceX = 0;
                var sourceY = 0;
                var targetX = 0;
                var targetY = 0;
                foreach (var x in outPhotos) {
                    if (x.X == e.SourceX) {
                        sourceX = x.Center.X;
                    }
                    if (x.Y == e.SourceY) { 
                        sourceY = x.Center.Y;
                    }
                    if (x.X == e.TargetX) {
                        targetX = x.Center.X;
                    }
                    if (x.Y == e.TargetY) { 
                        targetY = x.Center.Y;
                    }
                }
                adbWrapper.Swipe(sourceX, sourceY, targetX, targetY);
                await Task.Delay(500);
            }
            await Task.Delay(30000);

            
            
            
        }


    }
}
