using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingNurseryScript.Common {
    public class MapRect : ICloneable  {
        public int SourceX { get; set; }
        public int SourceY { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }

        public object Clone() {
            return new MapRect() {
                SourceX = this.SourceX,
                SourceY = this.SourceY,
                TargetX = this.TargetX,
                TargetY = this.TargetY
            };
        }
    }
    public class Calculate {
        public static int[,] GenerateMap(int[] source) {
            var map = new int[16, 10];
            for (var y = 0; y < 16; y++) {
                for (var x = 0; x < 10; x++) {
                    map[y, x] = source[y * 10 + x];
                }
            }
            return map;

        }
        public static int CalculateRectSum(MapRect rect, int[,] map) {
            var sum = 0;
            for (var x = rect.SourceX; x <= rect.TargetX; x++) {
                for (var y = rect.SourceY; y <= rect.TargetY; y++) {
                    sum += map[y, x];
                }
            }
            return sum;
        }
        public static bool IsValidRect(MapRect rect, int[,] map) {
            try {
                return CalculateRectSum(rect, map) == 10;
            } catch (System.IndexOutOfRangeException) {
                return false;
            }
        }
        static int[,] Copy2DArray(int[,] originalArray) {
            int[,] copiedArray = new int[originalArray.GetLength(0), originalArray.GetLength(1)];
            Array.Copy(originalArray, copiedArray, originalArray.Length);
            return copiedArray;
        }

        public static int[,] SetValidRect(MapRect rect, int[,] map) {
            //if (!IsValidRect(rect, map)) {
            //    throw new Exception($"Not Valid Rect{rect.SourceX},{rect.SourceY},{rect.TargetX},{rect.TargetY}");
            //}
            var tmp = Copy2DArray(map);
            for (var x = rect.SourceX; x <= rect.TargetX; x++) {
                for (var y = rect.SourceY; y <= rect.TargetY; y++) {
                    tmp[y, x] = 0;
                }
            }
            return tmp;
        }
        public static int[,] SetValidRectRaw(MapRect rect, int[,] map) {
            for (var x = rect.SourceX; x <= rect.TargetX; x++) {
                for (var y = rect.SourceY; y <= rect.TargetY; y++) {
                    map[y, x] = 0;
                }
            }
            return map;
        }
        public static int CalculateScore(int[,] map) {
            var score = 0;
            foreach(var num in map) {
                if (num == 0) {
                    score++;
                }
            }
            return score;
        }
        public static List<MapRect> GetOrCopyMapRect(List<MapRect> rects) => rects != null ? new List<MapRect>(rects.Select(item => (MapRect)item.Clone())) : new List<MapRect>();
        public static (List<MapRect>, int[,]) CalculateBestScoreDFS(int[,] Map, List<MapRect> rects = null) {
            var Scores = new List<(List<MapRect>, int[,])>();
            Parallel.For(0, Map.GetLength(0), StartY => {
                for (var StartX = 0; StartX < Map.GetLength(1); StartX++) {
                    for (var EndX = StartX; EndX < Map.GetLength(1); EndX++) {
                        for (var EndY = StartY; EndY < Map.GetLength(0); EndY++) {
                            var rect = new MapRect { SourceX = StartX, SourceY = StartY, TargetX = EndX, TargetY = EndY };
                            if (IsValidRect(rect, Map)) {
                                var NewMap = SetValidRect(rect, Map);
                                List<MapRect> rectsCopy = GetOrCopyMapRect(rects);
                                Scores.Add(CalculateBestScoreDFS(NewMap, rectsCopy));
                            }
                        }
                    }
                }
            });
            //for (var StartX = 0; StartX < TmpMap.GetLength(1); StartX++) {
            //    for (var StartY = 0; StartY < TmpMap.GetLength(0); StartY++) {
            //        for (var EndX = StartX; EndX < TmpMap.GetLength(1); EndX++) {
            //            for (var EndY = StartY; EndY < TmpMap.GetLength(0); EndY++) {
            //                var rect = new MapRect { SourceX = StartX, SourceY = StartY, TargetX = EndX, TargetY = EndY };
            //                if (IsValidRect(rect, TmpMap)) {
            //                    var NewMap = SetValidRect(rect, TmpMap);
            //                    List<MapRect> rectsCopy = GetOrCopyMapRect(rects);
            //                    Scores.Add(CalculateBestScoreDFS(NewMap, rectsCopy));
            //                }
            //            }
            //        }
            //    }
            //}
            var MaxScores = (rects, Map);
            foreach (var e in Scores) {
                if (CalculateScore(e.Item2) > CalculateScore(MaxScores.Item2)) {
                    MaxScores = e;
                }
            }
            return MaxScores;
        }

        public static (List<MapRect>, int[,]) CalculateBestScoreGreedyAlgorithm(int[,] Map) {
            var TmpMap = Copy2DArray(Map);
            var Score = new List<MapRect>();
            var ScoreLength = Score.Count;
            var MaxX = TmpMap.GetLength(1);
            var MaxY = TmpMap.GetLength(0);
            while (true) {
                for (var MaxOffset = 0; MaxOffset < (MaxX > MaxY ? MaxX : MaxY); MaxOffset++) {
                    var MaxOffsetX = (MaxX < MaxOffset ? MaxX : MaxOffset);
                    var MaxOffsetY = (MaxY < MaxOffset ? MaxY : MaxOffset);
                    for (var StartNumber = 9; StartNumber > 0; StartNumber--) {
                        for (var CenterX = 0; CenterX < MaxX; CenterX++) {
                            for (var CenterY = 0; CenterY < MaxY; CenterY++) {
                                if (TmpMap[CenterY, CenterX] != StartNumber) {
                                    continue;
                                }
                                
                                for (var OffsetX = -MaxOffsetY; OffsetX < MaxOffsetX; OffsetX++) {
                                    if (CenterX + OffsetX < 0 || CenterX + OffsetX >= MaxX) {
                                        break;
                                    }

                                    for (var OffsetY = -MaxOffsetY; OffsetY < MaxOffsetY; OffsetY++) {
                                        if (CenterY + OffsetY < 0 || CenterY + OffsetY >= MaxY) {
                                            break;
                                        }
                                        var rect = new MapRect {
                                            SourceX = OffsetX < 0 ? CenterX + OffsetX : CenterX,
                                            SourceY = OffsetY < 0 ? CenterY + OffsetY : CenterY,
                                            TargetX = OffsetX > 0 ? CenterX + OffsetX : CenterX,
                                            TargetY = OffsetY > 0 ? CenterY + OffsetY : CenterY,
                                        };
                                        if (rect.SourceX == rect.TargetX && rect.SourceY == rect.TargetY) {
                                            break;
                                        }
                                        if (IsValidRect(rect, TmpMap)) {
                                            TmpMap = SetValidRectRaw(rect, TmpMap);
                                            Score.Add(rect);
                                            goto NextPoint;
                                        }
                                    }
                                }
                            NextPoint:
                                continue;
                            }
                        }
                    NextSearch:
                        continue;
                    }
                }

                if (Score.Count == ScoreLength) {
                    break;
                } else {
                    ScoreLength = Score.Count;
                }
            }
            var OutScore = CalculateScore(TmpMap);
            Console.WriteLine(OutScore);
            return (Score, TmpMap);
        }
    }
}
