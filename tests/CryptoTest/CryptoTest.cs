using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Crypto;

namespace CryptoTest
{
    [TestClass]
    public class CryptoTest
    {
        private String password1 = "The first of many passwords";
        private String password2 = "The second one";

        [TestMethod]
        public void CorrectPasswordTest()
        {
            var hashedPassword = PasswordUtils.HashPassword(password1);
            Assert.IsTrue(PasswordUtils.ComparePassword(password1, hashedPassword));
        }

        [TestMethod]
        public void IncorrectPasswordTest()
        {
            var hashedPassword = PasswordUtils.HashPassword(password1);
            Assert.IsFalse(PasswordUtils.ComparePassword(password2, hashedPassword));
        }
    }
}
