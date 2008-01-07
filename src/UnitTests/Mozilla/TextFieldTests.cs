#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.Mozilla;

namespace WatiN.Core.UnitTests.Mozilla
{
    [TestFixture]
    public class TextFieldTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Logger.LogWriter = new DebugLogWriter();
        }

        [Test]
        public void GetAttributeValuesSpecificToTextFields()
        {
            using (FireFox ff = new FireFox())
            {
                ff.GoTo(BaseElementsTests.MainURI.ToString());
                Assert.AreEqual(BaseElementsTests.MainURI, ff.Url);

                ITextField readonlyTextField = ff.TextField("readonlytext");
                Assert.AreEqual("readonly", readonlyTextField.Value);
            }
        }

        [Test]
        public void SetAttributeValuesSpecificToTextFields()
        {
            using (FireFox ff = new FireFox())
            {
                ff.GoTo(BaseElementsTests.MainURI.ToString());
                Assert.AreEqual(BaseElementsTests.MainURI, ff.Url);

                ITextField nameTextField = ff.TextField("name");
                nameTextField.Value = "Steve Jobs";
                Assert.AreEqual("Steve Jobs", nameTextField.Value);
            }
        }
    }
}