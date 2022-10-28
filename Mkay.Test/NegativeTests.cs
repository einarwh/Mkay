using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkay.Test
{
    [TestClass]
    public class NegativeTests
    {
        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void MissingEndParens()
        {
            new { A = 1, B = 2 }
                .Property(x => x.B)
                .Obeys("(> B A");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void MissingStartParens()
        {
            new { A = 1, B = 2 }
                .Property(x => x.B)
                .Obeys("> B A)");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void MissingStartParens2()
        {
            new { A = 2 }
                .Property(x => x.A)
                .Obeys("> 1.5)");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void ExtraEndParens()
        {
            new { A = 2 }
                .Property(x => x.A)
                .Obeys("(> 1.5))");
        }

        [TestMethod]
        public void HandlesDateTimeStringWithSpace()
        {
            new { A = new DateTime(2013, 02, 01) }
                .Property(x => x.A)
                .Obeys("(> \"2013.01.31 14:59:59\")");
        }

        [TestMethod]
        public void HandlesDateTimeString()
        {
            new { A = new DateTime(2013, 02, 01) }
                .Property(x => x.A)
                .Violates("(> \"2013.02.01\")");
        }


        [TestMethod]
        public void HandlesAdditionWithDoublesAndComparisonWithInteger()
        {
            new { A = 5 }
                .Property(x => x.A)
                .Obeys("> (+ 1.5 1.5)");
        }

        [TestMethod]
        public void HandlesAdditionWithIntAndDouble()
        {
            new { A = 5, B = 1 }
                .Property(x => x.A)
                .Obeys("> (+ B 1.5)");
        }

        [TestMethod]
        public void CanCompareDatePropertyToString()
        {
            new { A = DateTime.Now }
                .Property(x => x.A)
                .Obeys("> \"31.01.2012\"");
        }

        [TestMethod]
        public void HandlesComparisonBetweenLongAndInt()
        {
            new { A = 2L }
                .Property(x => x.A)
                .Obeys("> 1");
        }

        [TestMethod]
        public void HandlesComparisonBetweenShortAndInt()
        {
            new { A = (short)2 }
                .Property(x => x.A)
                .Obeys("> 1");
        }

        [TestMethod]
        public void ThereIsANowMethod()
        {
            new { A = DateTime.Now }
                .Property(x => x.A)
                .Obeys("<= (now)");
        }

        [TestMethod]
        public void ThereIsANowMethod2()
        {
            new { A = DateTime.Now }
                .Property(x => x.A)
                .Violates("> (now)");
        }


    }
}
