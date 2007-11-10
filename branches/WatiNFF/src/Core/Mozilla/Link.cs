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

using WatiN.Core.Interfaces;

namespace WatiN.Core.Mozilla
{
    /// <summary>
    /// Represents the behavior and attributes of an HTML link element implemented for the FireFox browser
    /// </summary>
    public class Link : Element, ILink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="outerHtml">The outer HTML that defines the element.</param>
        /// <param name="clientPort">The client port.</param>
        public Link(string outerHtml, FireFoxClientPort clientPort) : base(outerHtml, clientPort)
        {
        }

        /// <summary>
        /// Gets the URL of this link element.
        /// </summary>
        /// <value>The URL of this link element.</value>
        public string Url
        {
            get { return GetAttributeValue("href"); }
        }
    }
}