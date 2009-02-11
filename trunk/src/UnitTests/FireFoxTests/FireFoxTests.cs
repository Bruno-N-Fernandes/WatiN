﻿using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.FireFoxTests
{
    [TestFixture]
    public class FireFoxTests : BaseWatiNTest
    {
        [Test]
        public void CheckFireFoxIsInheritingProperTypes()
        {
			using (var firefox = new FireFox())
			{
				Assert.IsInstanceOfType(typeof (FireFox), firefox, "Should be a FireFox instance");
				Assert.IsInstanceOfType(typeof (Browser), firefox, "Should be a Browser instance");
				Assert.IsInstanceOfType(typeof (DomContainer), firefox, "Should be a DomContainer instance");
			}
        }

        [Test, Category("InternetConnectionNeeded")]
        public void GoogleSearchWithEncodedQueryStringInConstructor()
        {
            var url = string.Format("http://www.google.com/search?q={0}", HttpUtility.UrlEncode("a+b"));

            using (var firefox = new FireFox(url))
            {
                Assert.That(firefox.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
            }
        }

        [Test, Category("InternetConnectionNeeded")]
        public void AddProtocolToUrlIfOmmitted()
        {
            using (var firefox = new FireFox("www.google.com"))
            {
                Assert.That(firefox.Url, Text.StartsWith("http://"));
            }
        }

        [Test]
        public void NewFireFoxInstanceShouldTake_Settings_MakeNewBrowserInstanceVisible_IntoAccount()
        {
            // TODO: make this work for firefox if possible. Do rename the setting from ...IeInstance... to ...BrowserInstance...
        }
    }
}