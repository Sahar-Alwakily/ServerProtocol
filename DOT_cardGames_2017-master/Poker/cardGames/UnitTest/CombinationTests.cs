using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoincheServer;

namespace UnitTest
{
    [TestClass]
    public class CombinationTests
    {
        [TestMethod]
        public void CombinationTestWhenThreeOfKind()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('H', '2', 1));
            test.Add(new Card('C', '2', 1));
            test.Add(new Card('D', '8', 7));
            test.Add(new Card('C', 'X', 9));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('S', 'J', 10));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckThreeOfAKind(test, test2);
            Assert.AreNotEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenNotThreeOfKind()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('H', '3', 1));
            test.Add(new Card('C', '2', 1));
            test.Add(new Card('D', '8', 7));
            test.Add(new Card('C', 'X', 9));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('S', 'J', 10));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckThreeOfAKind(test, test2);
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenFourOfKind()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('H', '2', 1));
            test.Add(new Card('C', '2', 1));
            test.Add(new Card('H', '2', 7));
            test.Add(new Card('C', 'X', 9));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('S', 'J', 10));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckFourOfAKind(test, test2);
            Assert.AreNotEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenFull()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('H', '2', 1));
            test.Add(new Card('C', '2', 1));
            test.Add(new Card('D', '8', 7));
            test.Add(new Card('C', 'X', 9));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('S', '8', 7));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckFull(test, test2);
            Assert.AreNotEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenFlush()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('S', '1', 13));
            test.Add(new Card('S', '7', 6));
            test.Add(new Card('S', '8', 7));
            test.Add(new Card('H', 'X', 9));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('H', '8', 7));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckFlush(test, test2);
            Assert.AreNotEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenDoublePair()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('H', '2', 1));
            test.Add(new Card('C', '7', 6));
            test.Add(new Card('D', '8', 7));
            test.Add(new Card('H', 'K', 12));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('H', '8', 7));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckDoublePair(test, test2);
            Assert.AreNotEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenStraight()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('S', '2', 1));
            test.Add(new Card('H', '3', 2));
            test.Add(new Card('C', '4', 3));
            test.Add(new Card('D', '5', 4));
            test.Add(new Card('H', '6', 5));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('H', '8', 7));
            test2.Add(new Card('S', 'K', 12));

            int res = comb.CheckStraight(test, test2);
            Assert.AreNotEqual(0, res);
        }

        [TestMethod]
        public void CombinationTestWhenRoyalFLush()
        {
            Combination comb = new Combination();
            List<Card> test = new List<Card>();
            test.Add(new Card('H', 'X', 9));
            test.Add(new Card('H', 'J', 10));
            test.Add(new Card('H', 'Q', 11));
            test.Add(new Card('H', 'K', 12));
            test.Add(new Card('H', '1', 13));

            List<Card> test2 = new List<Card>();
            test2.Add(new Card('H', '8', 7));
            test2.Add(new Card('S', '2', 1));

            int res = comb.CheckRoyalFlush(test, test2);
            Assert.AreNotEqual(0, res);
        }
    }
}
