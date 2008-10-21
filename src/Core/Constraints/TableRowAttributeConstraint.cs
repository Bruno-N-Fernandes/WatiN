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

using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Comparers;
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;

namespace WatiN.Core.Constraints
{
	/// <summary>
	/// Use this class to find a row which contains a particular value
	/// in a table cell contained in a table column.
	/// </summary>
	public class TableRowAttributeConstraint : AttributeConstraint
	{
		private readonly int columnIndex;
		private readonly ICompare containsText;

		/// <summary>
		/// Initializes a new instance of the <see cref="TableRowAttributeConstraint"/> class.
		/// </summary>
		/// <param name="findText">The text to find (exact match but case insensitive).</param>
		/// <param name="inColumn">The column index in which to look for the value.</param>
		public TableRowAttributeConstraint(string findText, int inColumn) : base(Find.textAttribute, new StringEqualsAndCaseInsensitiveComparer(findText))
		{
			columnIndex = inColumn;
			containsText = new StringContainsAndCaseInsensitiveComparer(findText);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TableRowAttributeConstraint"/> class.
		/// </summary>
		/// <param name="findTextRegex">The regular expression to match with.</param>
		/// <param name="inColumn">The column index in which to look for the value.</param>
		public TableRowAttributeConstraint(Regex findTextRegex, int inColumn) : base(Find.textAttribute, findTextRegex)
		{
			columnIndex = inColumn;
			containsText = new AlwaysTrueComparer();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TableRowAttributeConstraint"/> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <param name="inColumn">The column index in which to look for the value.</param>
		public TableRowAttributeConstraint(ICompare comparer, int inColumn) : base(Find.textAttribute, comparer)
		{
			columnIndex = inColumn;
			containsText = new AlwaysTrueComparer();
		}

		public override bool Compare(IAttributeBag attributeBag)
		{
		    var elementAttributeBag = (ElementAttributeBag) attributeBag;
		    var element = elementAttributeBag.Element;
            var nativeElement = (IHTMLElement)element.NativeElement;
			
			if (IsTextContainedIn(element.Text))
			{
				// Get all elements and filter this for TableCells
                var tableRowElement = (IHTMLTableRow)nativeElement;
				var tableCellElements = tableRowElement.cells;

				if (tableCellElements.length - 1 >= columnIndex)
				{
                    var tableCell = (IHTMLTableCell)tableCellElements.item(columnIndex, null);
				    var elementComparer = comparer as ICompareElement;
                    
                    if (elementComparer != null)
                    {
                        return elementComparer.Compare(new TableCell(elementAttributeBag.DomContainer, new IEElement(tableCell)));
                    }
				    return base.Compare(new ElementAttributeBag(elementAttributeBag.DomContainer, new IEElement(tableCell)));
				}
			}

			return false;
		}

		public bool IsTextContainedIn(string text)
		{
			return containsText.Compare(text);
		}
	}
}