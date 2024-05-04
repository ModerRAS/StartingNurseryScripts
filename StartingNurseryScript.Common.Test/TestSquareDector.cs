using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common.Test {
    [TestClass]
    public class TestSquareDector {
        [TestMethod]
        public void TestSquareDector_1() {
            string imagePath = "C:\\Users\\ModerRAS\\Pictures\\IMG_5357.MP4_20240503_131110.422.jpg";
            string outputPath = "C:\\Users\\ModerRAS\\Pictures\\output.jpg";

            SquareDetector squareDetector = new SquareDetector(imagePath);
            var boxes = squareDetector.DetectAndDrawSquare(outputPath);
            squareDetector.GetSmallImagesAsByteArrays(boxes);

            Console.WriteLine("正方形检测完成，结果已保存至：" + outputPath);
        }
    }
}
