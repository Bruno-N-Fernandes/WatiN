#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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
    using System;
    using Mozilla;
using WatiN.Core.Native.Windows;

    /// <summary>
    /// Defines behaviour common to most javascript controlled browsers.
    /// </summary>
    public abstract class JSBrowserBase : INativeBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSBrowserBase"/> class.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        public JSBrowserBase(ClientPortBase clientPort)
        {
            this.ClientPort = clientPort;
        }

        /// <summary>
        /// Gets the client port used to communicate with the instance of FireFox.
        /// </summary>
        /// <value>The client port.</value>
        public ClientPortBase ClientPort { get; private set; }

        /// <summary>
        /// Gets the name of the browser variable.
        /// </summary>
        /// <value>The name of the browser variable.</value>
        public string BrowserVariableName
        {
            get { return this.ClientPort.BrowserVariableName;  }
        }

        #region INativeBrowser Members
        /// <inheritdoc />
        public bool GoBack()
        {
            return this.Navigate("goBack");
        }

        /// <inheritdoc />
        public bool GoForward()
        {
            return this.Navigate("goForward");
        }

        /// <inheritdoc />
        public void Reopen()
        {
            this.Reopen(null);
        }


        /// <inheritdoc />
        public void NavigateTo(Uri url)
        {
            this.LoadUri(url, true);
        }

        /// <inheritdoc />
        public void NavigateToNoWait(Uri url)
        {
            this.LoadUri(url, false);
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        /// <param name="forceGet">When it is <c>true</c>, causes the page to always be reloaded from the server. 
        /// If it is <c>false</c>, the browser may reload the page from its cache.</param>
        public void Reload(bool forceGet)
        {
            this.ClientPort.Write("{0}.location.reload({1});", FireFoxClientPort.WindowVariableName, forceGet.ToString().ToLower());
        }

        /// <inheritdoc />
        public void Refresh()
        {
            this.Reload(false);
        }

        /// <summary>
        /// Closes the browser.
        /// </summary>
        public void Close()
        {
            this.ClientPort.Write("{0}.close()", FireFoxClientPort.WindowVariableName);
        }

        public bool IsLoading()
        {
            bool loading;
            switch (this.ClientPort.JavaScriptEngine)
            {
                case JavaScriptEngineType.WebKit:
                    loading = this.ClientPort.WriteAndReadAsBool("{0}.readyState != 'complete';", this.ClientPort.DocumentVariableName);
                    this.ClientPort.WriteAndRead("{0}.readyState;", this.ClientPort.DocumentVariableName);
                    this.ClientPort.WriteAndRead("window.location.href");
                    break;
                case JavaScriptEngineType.Mozilla:
                    loading = this.ClientPort.WriteAndReadAsBool("{0}.webProgress.isLoadingDocument;", this.BrowserVariableName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return loading;
        }

        protected void Reopen(Uri url)
        {
            this.ClientPort.Dispose();
            this.ClientPort.Connect(url == null ? string.Empty : url.ToString());
        }

        /// <summary>
        /// Load a URL into the document. see: http://developer.mozilla.org/en/docs/XUL:browser#m-loadURI
        /// </summary>
        /// <param name="url">The URL to laod.</param>
        /// <param name="waitForComplete">If false, makes to execution of LoadUri asynchronous.</param>
        protected abstract void LoadUri(Uri url, bool waitForComplete);

        public int WindowCount
        {
            get { return this.ClientPort.WriteAndReadAsInt("getWindows().length"); }
        }

        private bool Navigate(string action)
        {
            var ticks = DateTime.Now.Ticks;
            this.ClientPort.Write("window.document.WatiNGoBackCheck={0};", ticks);
            this.ClientPort.Write("{0}.{1}();", this.BrowserVariableName, action);
            return this.ClientPort.WriteAndReadAsBool("window.document.WatiNGoBackCheck!={0};", ticks);
        }

        public abstract INativeDocument NativeDocument { get; }

        public abstract WatiN.Core.Native.Windows.Window HostWindow { get; }

        public abstract INativeDialogManager NativeDialogManager { get; }

        #endregion

        #region IDisposable Members

        public abstract void Dispose();
        #endregion
    }
}