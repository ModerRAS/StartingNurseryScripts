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
            var main = new MainLogic();
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
    }
}
