using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common {
    public class DetectNumber {
        public PaddleOcrAll all { get; set; }
        public DetectNumber() {
            FullOcrModel model = LocalFullModels.ChineseV3;

            all = new PaddleOcrAll(model,
                PaddleDevice.Mkldnn()
                ) {
                AllowRotateDetection = true, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            };
        }

        public PaddleOcrResult GetOcrResult(byte[] image) {
            using (Mat src = Cv2.ImDecode(image, ImreadModes.Color)) {
                PaddleOcrResult result = all.Run(src);
                return result;
            }
        }
        public int ConvertToResults(PaddleOcrResult paddleOcrResult) {
            foreach (var region in paddleOcrResult.Regions) {
                if (int.TryParse(region.Text, out var result)) {
                    return result;
                }
            }
            return (int)ErrorNumber.CannotDetectNumber;
        }
        public NumberPhoto Detect(NumberPhoto image) {
            var number = ConvertToResults(GetOcrResult(image.Photo));
            return new NumberPhoto() {
                Number = number,
                Photo = image.Photo,
                Points = image.Points
            };
        }
    }
}
