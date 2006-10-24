#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a HTML table element.
  /// </summary>
  public class Table : ElementsContainer
  {
    public Table(DomContainer ie, HTMLTable htmlTable) : base(ie, (IHTMLElement) htmlTable)
    {}

    public override TableRowCollection TableRows
    {
      get 
      {
        IHTMLElementCollection bodyElements = GetBodyElements();
        return ElementsSupport.TableRows(DomContainer, bodyElements); 
      }
    }

    private IHTMLElementCollection GetBodyElements()
    {
      return (IHTMLElementCollection)(GetFirstTBody().all);
    }

    private IHTMLElement GetFirstTBody()
    {
      return (IHTMLElement)((IHTMLTable)DomElement).tBodies.item(0,null);
    }

    /// <summary>
    /// Finds te first row that matches findText in inColumn. If no match is found, null is returned.
    /// </summary>
    /// <param name="findText">The text to find.</param>
    /// <param name="inColumn">Index of the column to find the text in.</param>
    /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
    public TableRow FindRow(string findText, int inColumn)
    {
      Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      TableRowFinder finder = new TableRowFinder(findText, inColumn);
      
      return findRow(finder);
    }

    /// <summary>
    /// Finds te first row that matches findTextRegex in inColumn. If no match is found, null is returned.
    /// </summary>
    /// <param name="findTextRegex">The regular expression the cell text must match.</param>
    /// <param name="inColumn">Index of the column to find the text in.</param>
    /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
    public TableRow FindRow(Regex findTextRegex, int inColumn)
    {
      Logger.LogAction("Matching regular expression'" + findTextRegex + "' with text in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      TableRowFinder finder = new TableRowFinder(findTextRegex, inColumn);

      return FindRow(finder);
    }

    private TableRow findRow(TableRowFinder finder)
    {
      string innertext = GetFirstTBody().innerText;
      
      if (innertext != null && finder.IsTextContainedIn(innertext))
      {
        return FindRow(finder);
      }
      
      return null;
    }

    public override string ToString()
    {
      return Id;
    }
    
    public TableRow FindRow(TableRowFinder findBy)
    {
      IHTMLElementCollection bodyElements = GetBodyElements();
      IHTMLElement element = ElementsSupport.FindFirstElement(ElementsSupport.TableRowTagName, ElementsSupport.InputNullType, findBy, bodyElements, false);
      
      if (element != null)
      {
        return new TableRow(DomContainer,(HTMLTableRow)element);
      }

      return null;
    }
           
    /// <summary>
    /// Use this class to find a row which contains a particular value
    /// in a table cell contained in a table column.
    /// </summary>
    public class TableRowFinder : Text
    {
      private int columnIndex;
      private ICompare containsText;
      
      /// <summary>
      /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
      /// </summary>
      /// <param name="findText">The text to find (exact match but case insensitive).</param>
      /// <param name="inColumn">The column index in which to look for the value.</param>
      public TableRowFinder(string findText, int inColumn): base(findText)
      {
        columnIndex = inColumn;
        comparer = new StringEqualsAndCaseInsensitiveComparer(findText);
        containsText = new StringContainsAndCaseInsensitiveComparer(findText);
      }
      
      /// <summary>
      /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
      /// </summary>
      /// <param name="findTextRegex">The regular expression to match with.</param>
      /// <param name="inColumn">The column index in which to look for the value.</param>
      public TableRowFinder(Regex findTextRegex, int inColumn): base(findTextRegex)
      {
        columnIndex = inColumn;
        containsText = new AlwaysTrueComparer();
      }
      
      public override bool Compare(object ihtmlelement)
      {
        IHTMLElement element = GetIHTMLElement(ihtmlelement);

        if (IsTextContainedIn(element.innerText))
        {
          // Get all elements and filter this for TableCells
          IHTMLElementCollection allElements = (IHTMLElementCollection)element.all;
          IHTMLElementCollection tableCellElements = ElementsSupport.getElementCollection(allElements, ElementsSupport.TableCellTagName);
        
          return base.Compare(tableCellElements.item(columnIndex, null));
        }
        
        return false;
      }
      
      private class AlwaysTrueComparer : ICompare
      {
        public bool Compare(string value)
        {
          return true;
        }
      }

      public bool IsTextContainedIn(string text)
      {
        return containsText.Compare(text);
      }
    }
  }
}