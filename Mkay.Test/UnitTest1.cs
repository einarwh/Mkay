using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkay.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.B)
                .Obeys("(and (> A) (< C))");
        }

        [TestMethod]
        public void TestMethod2()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.B)
                .Obeys("(or (< A) (< C))");
        }

        [TestMethod]
        public void TestMethod3()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.B)
                .Violates("(and (< A) (< C))");
        }

        [TestMethod]
        public void TestMethod4()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.A)
                .Violates("(or (> B) (> C))");
        }

        [TestMethod]
        public void TestMethod5()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.C)
                .Obeys("(== (+ A B))");
            
        }

        [TestMethod]
        public void TestMethod6()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.C)
                .Obeys("(>= (+ A B))");
        }

        [TestMethod]
        public void TestMethod7()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.C)
                .Violates("(> (+ A B))");
        }

        [TestMethod]
        public void TestMethod8()
        {
            new { A = 1, B = 2, C = 3 }
                .Property(x => x.C)
                .Violates("(> (+ A B))");
        }

        [TestMethod]
        public void TestMethod9()
        {
            new { Name = "Averell" }
                .Property(x => x.Name)
                .Violates("(< (len .) 5)");
        }

        [TestMethod]
        public void TestMethod10()
        {
            new { Name = "Joe" }
                .Property(x => x.Name)
                .Obeys("(< (len .) 5)");
        }

        [TestMethod]
        public void TestMethod11()
        {
            new { Maybe = true }
                .Property(x => x.Maybe)
                .Obeys("(and .)");
        }

        [TestMethod]
        public void TestMethod12()
        {
            new { Maybe = true }
                .Property(x => x.Maybe)
                .Obeys("(== true)");
        }

        [TestMethod]
        public void TestMethod13()
        {
            new { Maybe = true }
                .Property(x => x.Maybe)
                .Violates("(== false)");
        }

        [TestMethod]
        public void TestMethod14()
        {
            new { Maybe = false }
                .Property(x => x.Maybe)
                .Obeys("(== false)");
        }

        [TestMethod]
        public void TestMethod15()
        {
            new { Maybe = false }
                .Property(x => x.Maybe)
                .Violates("(== true)");
        }


        [TestMethod]
        public void MaxOfDoubleValues()
        {
            new { A = 20.5 }
                .Property(x => x.A)
                .Obeys("> (max 11.4 2.1)");
        }

        [TestMethod]
        public void MaxOfIntegerValues()
        {
            new { A = 20 }
                .Property(x => x.A)
                .Obeys("> (max 19 4)");
        }

        [TestMethod]
        public void MaxOfIntegerAndDouble()
        {
            new { A = 20 }
                .Property(x => x.A)
                .Obeys("> (max 19 4.1)");
        }

        [TestMethod]
        public void MinOfDoubleValues()
        {
            new { A = 5.4 }
                .Property(x => x.A)
                .Obeys("> (min 11.4 2.1)");
        }

        [TestMethod]
        public void MinOfIntegerValues()
        {
            new { A = 5 }
                .Property(x => x.A)
                .Obeys("> (min 19 4)");
        }

        [TestMethod]
        public void MinOfIntegerAndDouble()
        {
            new { A = 6.3 }
                .Property(x => x.A)
                .Obeys("> (min 19 4.1)");
        }

        [TestMethod]
        public void SimpleRegexTest()
        {
            new { Name = "Bartholomew" }
                .Property(x => x.Name)
                .Obeys("~ \"holo\"");
        }

        [TestMethod]
        public void SimpleRegexTest2()
        {
            new { Name = "Bartholomew" }
                .Property(x => x.Name)
                .Obeys("~ \"^Bart\"");
        }

        [TestMethod]
        public void SimpleRegexTest3()
        {
            new { Name = "Bartholomew" }
                .Property(x => x.Name)
                .Violates("~ \"Bart$\"");
        }

        [TestMethod]
        public void SimpleRegexTest4()
        {
            new { A = "nomatter" }
                .Property(x => x.A)
                .Obeys("~ \"Bartholomew\" \"^Bart\"");
        }

        [TestMethod]
        public void SimpleRegexTest5()
        {
            new { Name = "Bartholomew", Rule = "holo" }
                .Property(x => x.Name)
                .Obeys("~ Rule");
        }

        [TestMethod]
        public void SimpleRegexTest6()
        {
            new { Name = "Bartholomew", Rule = "holo$" }
                .Property(x => x.Name)
                .Violates("~ Rule");
        }
    }
}
