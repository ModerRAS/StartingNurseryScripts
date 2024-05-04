using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common {
    public class MainLogic {
        public static int[,] ConstructMap(List<NumberPhoto> numberPhotos) {
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
                }
            }

            return map;
        }


        public async Task ExecuteAsync() {
            string imagePath = "C:\\Users\\ModerRAS\\Pictures\\IMG_5357.MP4_20240503_131110.422.jpg";
            string outputPath = "C:\\Users\\ModerRAS\\Pictures\\output.jpg";
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
            Console.WriteLine(count);
            Console.WriteLine(outPhotos);
        }


    }
}
