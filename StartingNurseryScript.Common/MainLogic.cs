using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common {
    public class MainLogic {
        public async Task ExecuteAsync() {
            string imagePath = "C:\\Users\\ModerRAS\\Pictures\\IMG_5357.MP4_20240503_131110.422.jpg";
            string outputPath = "C:\\Users\\ModerRAS\\Pictures\\output.jpg";
            var detector = new DetectNumber();
            var square = new SquareDetector(imagePath);
            var squares = square.DetectAndDrawSquare(outputPath);
            var numberPhotos = square.GetSmallImagesAsByteArrays(squares);
            var outPhotos = new List<NumberPhoto>();
            foreach (var e in numberPhotos) {
                outPhotos.Add(TesseractHelper.Detect(e));
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
