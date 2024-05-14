using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdcb.PaddleOCR.Models.Online;

namespace StartingNurseryScript.Common {
    public class DetectNumber {
        public PaddleOcrAll all { get; set; }
        public DetectNumber() {
            InitModel().Wait();
        }
        public async Task InitModel() {
            Sdcb.PaddleOCR.Models.Online.Settings.GlobalModelDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var model = await OnlineFullModels.ChineseV3.DownloadAsync();
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
        public Point GetStringPoint(byte[] image, string str) {
            var result = GetOcrResult(image);
            foreach (var region in result.Regions) {
                if (region.Text.Equals(str)) {
                    return new Point() {
                        X = (int)region.Rect.Center.X,
                        Y = (int)region.Rect.Center.Y
                    };
                }
            }
            return new Point() { X = 0, Y = 0 };
        }
        public Point GetStartNext(byte[] image) {
            return GetStringPoint(image, "再次挑战");
        }
        public Point GetOk(byte[] image) {
            return GetStringPoint(image, "确定");
        }
        public Point GetStartGame(byte[] image) {
            return GetStringPoint(image, "开始游戏");
        }
        public Point GetTip(byte[] image) {
            return GetStringPoint(image, "领取");
        }
        public Point GetIKnow(byte[] image) {
            return GetStringPoint(image, "知道了");
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
