using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

using System;
using System.IO;
using Tesseract;

namespace StartingNurseryScript.Common;
class TesseractHelper {
    public static byte[] ConvertJpegToTiff(byte[] jpegBytes) {
        using (var jpegStream = new MemoryStream(jpegBytes))
        using (var image = System.Drawing.Image.FromStream(jpegStream))
        using (var tiffStream = new MemoryStream()) {
            image.Save(tiffStream, System.Drawing.Imaging.ImageFormat.Tiff);
            return tiffStream.ToArray();
        }
    }
    public static int RecognizeDigits(byte[] imageBytes) {
        using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
            using (var img = Pix.LoadTiffFromMemory(ConvertJpegToTiff(imageBytes))) {
                using (var page = engine.Process(img)) {
                    if (int.TryParse(page.GetText().Trim(), out var result)) {
                        return result;
                    }
                    return (int)ErrorNumber.CannotDetectNumber;
                }
            }
        }
    }
    public static NumberPhoto Detect(NumberPhoto image) {
        var number = RecognizeDigits(image.Photo);
        return new NumberPhoto() {
            Number = number,
            Photo = image.Photo,
            Points = image.Points
        };
    }
}
