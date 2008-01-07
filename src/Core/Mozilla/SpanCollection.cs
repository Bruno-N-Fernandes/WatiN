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

using System.Collections.Generic;
using WatiN.Core;
using WatiN.Core.Interfaces;
using WatiN.Core.Constraints;

namespace WatiN.Core.Mozilla
{
    /// <summary>
    /// FireFox implementation of <see cref="ISpanCollection"/>.
    /// </summary>
    public class SpanCollection : BaseElementCollection, ISpanCollection
    {
        public SpanCollection(List<Element> elements, FireFoxClientPort clientPort) : base(elements, clientPort)
        {
        }

        public SpanCollection(FireFoxClientPort clientPort, ElementFinder elementFinder)
            : base(clientPort, elementFinder)
        {
        }

        protected override List<Element> FindElements()
        {
            List<Element> spans = new List<Element>();
            foreach (string spanVariable in this.ElementFinder.FindAll())
            {
                spans.Add(new Span(spanVariable, this.ClientPort));
            }

            return spans;
        }

        /// <summary>
        /// Gets the <see cref="ISpan"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ISpan this[int index]
        {
            get { return (ISpan)this.Elements[index]; }
        }

        public ISpanCollection Filter(BaseConstraint findBy)
        {
            List<Element> filteredElements = new List<Element>();

            foreach (Element element in this.Elements)
            {
                FireFoxElementAttributeBag attributeBag = new FireFoxElementAttributeBag(element.ElementVariable, this.ClientPort);
                if (findBy.Compare(attributeBag))
                {
                    filteredElements.Add(element);
                }
            }

            return new SpanCollection(filteredElements, this.ClientPort);
        }
    }
}