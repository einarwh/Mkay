using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkay.Test
{
    [TestClass]
    public class SugarTests
    {
        [TestMethod]
        public void NoSugar()
        {
            new { A = 1, B = 2, C = 3 }
               .Property(x => x.B)
                .Obeys("(> B A)");
        }

        [TestMethod]
        public void DotAliasForPropertyName()
        {
            new { A = 1, B = 2, C = 3 }
               .Property(x => x.B)
                .Obeys("(> . A)");
        }

        [TestMethod]
        public void ImplicitDotForComparison()
        {
            new { A = 1, B = 2, C = 3 }
               .Property(x => x.B)
                .Obeys("(> A)");
        }

        [TestMethod]
        public void OmitOuterParens()
        {
            new { A = 1, B = 2, C = 3 }
               .Property(x => x.B)
                .Obeys("> A");
        }
    }
}
