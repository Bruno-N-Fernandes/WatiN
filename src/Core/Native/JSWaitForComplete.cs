﻿#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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

namespace WatiN.Core.Native
{
    using Mozilla;

    public class JSWaitForComplete : WaitForCompleteBase
    {
        private readonly JSBrowserBase nativeBrowser;

        public JSWaitForComplete(JSBrowserBase nativeBrowser, int waitForCompleteTimeOut) : base(waitForCompleteTimeOut)
        {
            this.nativeBrowser = nativeBrowser;
        }

        protected override void InitialSleep()
        {
            // Seems like this is not needed
        }

        protected override void WaitForCompleteOrTimeout()
        {
            this.WaitWhileDocumentNotAvailable();
        }

        protected virtual void WaitWhileDocumentNotAvailable()
        {
            this.WaitUntil(() => !this.nativeBrowser.IsLoading(),
                      () => "waiting for main document becoming available");

            this.nativeBrowser.ClientPort.InitializeDocument();
        }
    }
}