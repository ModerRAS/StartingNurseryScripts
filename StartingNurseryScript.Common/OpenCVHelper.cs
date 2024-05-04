using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
namespace StartingNurseryScript.Common;
public static class OpenCVHelper {
    static OpenCVHelper() {
        for (int i = 1; i < 10; i++) {
            templateImages.Add(i, Cv2.ImRead($"template/{i}.jpg"));
        }
    }
    public static Dictionary<int, Mat> templateImages { get; set; } = new ();
    public static int MatchToBestTemplate(Mat sourceImage) {
        // 创建 SURF 特征检测器和描述子提取器
        var surf = SURF.Create(300, 4, 2, true);

        // 提取所有模板图像的特征点和描述子
        List<KeyPoint> keyPoints = new List<KeyPoint>();
        List<Mat> descriptors = new List<Mat>();
        foreach (var kvp in templateImages) {
            KeyPoint[] kp;
            Mat desc = new Mat();
            surf.DetectAndCompute(kvp.Value, null, out kp, desc);
            keyPoints.AddRange(kp);
            descriptors.Add(desc);
        }

        // 提取源图像的特征点和描述子
        KeyPoint[] keyPointsSource;
        Mat descriptorsSource = new Mat();
        surf.DetectAndCompute(sourceImage, null, out keyPointsSource, descriptorsSource);

        // 使用暴力匹配器进行特征点匹配
        var matcher = new BFMatcher(NormTypes.L2, false);
        List<DMatch[]> matchesList = new List<DMatch[]>();
        foreach (var descriptor in descriptors) {
            DMatch[] matches = matcher.Match(descriptorsSource, descriptor);
            matchesList.Add(matches);
        }

        // 计算匹配得分
        double[] scores = new double[templateImages.Count];
        for (int i = 0; i < templateImages.Count; i++) {
            scores[i] = matchesList[i].Average(m => m.Distance);
        }

        // 找到得分最低的模板图索引
        int bestMatchIndex = Array.IndexOf(scores, scores.Min());

        // 返回对应的数字
        return templateImages.ElementAt(bestMatchIndex).Key;
    }
    public static NumberPhoto Detect(NumberPhoto image) {
        var number = MatchToBestTemplate(Mat.FromImageData(image.Photo));
        return new NumberPhoto() {
            Number = number,
            Photo = image.Photo,
            Points = image.Points
        };
    }
}
