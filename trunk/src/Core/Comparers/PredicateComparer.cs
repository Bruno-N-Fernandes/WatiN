#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

#if !NET11
using System;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Comparers
{

	public class PredicateComparer : BaseComparer, ICompareElement
	{
		private readonly Predicate<string> _compareString;
		private readonly Predicate<Element> _compareElement;
	
		public PredicateComparer(Predicate<string> predicate)
		{
			_compareString = predicate;	
		}
	
		public PredicateComparer(Predicate<Element> predicate)
		{
			_compareElement = predicate;	
		}

	    public override bool Compare(string value)
		{
			return _compareString.Invoke(value);
		}

		public virtual bool Compare(Element element)
		{
			return _compareElement.Invoke(element);
		}
	}

	public class PredicateComparer<E> : BaseComparer, ICompareElement where E : Element
	{
        private readonly Predicate<E> _compareElement;

        public PredicateComparer(Predicate<E> predicate)
		{
			_compareElement = predicate;	
		}

		public virtual bool Compare(Element element)
		{
		    try
		    {
		        return _compareElement.Invoke((E)element);
		    }
		    catch (Exception e)
		    {
		        throw new WatiNException("Exception during execution of predicate for " + element.OuterHtml, e);
		    }
		}
	}
}
#endif
