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

using NUnit.Framework;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
	[TestFixture]
	public class AlertDialogHandlerTests : WatiNTest
	{
		[Test]
		public void AlertDialogHandler()
		{
			using (IE ie = new IE(TestEventsURI))
			{
				Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

				AlertDialogHandler alertDialogHandler = new AlertDialogHandler();
				using (new UseDialogOnce(ie.DialogWatcher, alertDialogHandler))
				{
					ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

					alertDialogHandler.WaitUntilExists();

					string message = alertDialogHandler.Message;
					alertDialogHandler.OKButton.Click();

					ie.WaitForComplete();

					Assert.AreEqual("This is an alert!", message, "Unexpected message");
					Assert.IsFalse(alertDialogHandler.Exists(), "Alert Dialog should be closed.");
				}
			}
		}
		[Test]
		public void AlertDialogHandler2()
		{
using (IE ie = new IE("www.watinexamples.com"))
{
	AlertDialogHandler alertDialogHandler = new AlertDialogHandler();
	ie.DialogWatcher.Add(alertDialogHandler);
	
	ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

	alertDialogHandler.WaitUntilExists();
	alertDialogHandler.OKButton.Click();

	ie.WaitForComplete();
}
		}

		[Test]
		public void AlertDialogHandlerWithoutAutoCloseDialogs()
		{
			using (IE ie = new IE(TestEventsURI))
			{
				Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

				ie.DialogWatcher.CloseUnhandledDialogs = false;

				ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

				AlertDialogHandler alertDialogHandler = new AlertDialogHandler();

				using (new UseDialogOnce(ie.DialogWatcher, alertDialogHandler))
				{
					alertDialogHandler.WaitUntilExists();

					string message = alertDialogHandler.Message;
					alertDialogHandler.OKButton.Click();

					ie.WaitForComplete();

					Assert.AreEqual("This is an alert!", message, "Unexpected message");
					Assert.IsFalse(alertDialogHandler.Exists(), "Alert Dialog should be closed.");
				}
			}
		}
	}
}