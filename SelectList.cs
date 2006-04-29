using System.Collections.Specialized;

using mshtml;

using WatiN.Exceptions;
using WatiN.Logging;

namespace WatiN
{
  public class SelectList : Element
  {
    /// <summary>
    /// Returns an initialized instance of a SelectList object.
    /// Mainly used by the collectionclass SelectLists.
    /// </summary>
    /// <param name="ie"></param>
    /// <param name="htmlSelectElement"></param>
    public SelectList(DomContainer ie, IHTMLElement htmlSelectElement) : base(ie, htmlSelectElement)
    {}

    /// <summary>
    /// This method clears the selected items in the select box
    /// </summary>
    public void ClearList()
    {
      Logger.LogAction("Clearing selection(s) in " + GetType().Name + " '" + Id + "'");

      bool wait = false;
      int numberOfOptions = selectElement.length;

      for (int index = 0; index < numberOfOptions; index++)
      {
        IHTMLOptionElement option = GetOptionElement(index);

        if (option.selected)
        {
          option.selected = false;
          wait = true;
        }
      }

      if (wait)
      {
        WaitForComplete();
      }
    }

    public bool Multiple
    {
      get { return selectElement.multiple; }
    }

    /// <summary>
    /// This method selects an item by text.
    /// Raises NoValueFoundException if the specified value is not found.
    /// </summary>
    /// <param name="text"></param>
    public void Select(string text)
    {
      Logger.LogAction("Selecting '" + text + "' in " + GetType().Name + " '" + Id + "'");

      SelectByTextOrValue(text, true);
    }

    /// <summary>
    /// Selects an item in a select box, by value.
    /// Raises NoValueFoundException if the specified value is not found.
    /// </summary>
    /// <param name="value"></param>
    public void SelectByValue(string value)
    {
      Logger.LogAction("Selecting item with value '" + value + "' in " + GetType().Name + " '" + Id + "'");

      SelectByTextOrValue(value, false);
    }

    private void SelectByTextOrValue(string textOrValue, bool selectByText)
    {
      bool optionFound = false;
      int numberOfOptions = selectElement.length;

      for (int index = 0; (optionFound == false) && (index < numberOfOptions) ; index++)
      {
        IHTMLOptionElement option = GetOptionElement(index);

        string compareValueOrText;

        if (selectByText) 
        { compareValueOrText = option.text; }
        else 
        { compareValueOrText = option.value; }

        if (textOrValue.Equals(compareValueOrText))
        {
          if (option.selected)
          { optionFound = true; }
          else
          {
            option.selected = true;
            this.FireEvent("onchange");
            optionFound = true;
          }
        }
      }

      if (!optionFound)
      {
        throw new SelectListItemNotFoundException(textOrValue);
      }

    }

    private HTMLSelectElementClass selectElement
    {
      get { return ((HTMLSelectElementClass) element); }
    }

    /// <summary>
    /// Returns all the items in the select list as an array.
    /// An empty array is returned if the select box has no contents.
    /// </summary>
    public StringCollection AllContents
    {
      get
      {
        StringCollection items = new StringCollection();
        int numberOfOptions = selectElement.length;

        for (int index = 0; index < numberOfOptions; index++)
        {
          IHTMLOptionElement option = GetOptionElement(index);

          items.Add(option.text);
        }

        return items;
      }
    }

    private IHTMLOptionElement GetOptionElement(int index)
    {
      // Despite the fact that item(object name, object index) is the defenition,
      // item(null,index) always returns the fist item in the array.
      // item(index,null) seems to return the expected item at the given index.
      return ((IHTMLOptionElement) selectElement.item(index, null));
    }

    /// <summary>
    /// Returns the selected item(s) as an array.
    /// </summary>
    public StringCollection SelectedItems
    {
      get
      {
        StringCollection items = new StringCollection();

        for (int index = 0; index < selectElement.length; index++)
        {
          IHTMLOptionElement option = GetOptionElement(index);

          if (option.selected)
          {
            items.Add(option.text);
          }
        }

        return items;
      }
    }    
    
    /// <summary>
    /// Returns the first selected item in the selectlist. There might by more.
    /// Use SelectedItems to get a StringCollection of all selected items.
    /// When there's no item selected, the return value will be null.
    /// </summary>
    public string SelectedItem
    {
      get
      {
        for (int index = 0; index < selectElement.length; index++)
        {
          IHTMLOptionElement option = GetOptionElement(index);

          if (option.selected)
          {
            return option.text;
          }
        }

        return null;
      }
    }

    public bool HasSelectedItems
    {
      get { return (SelectedItem != null); }
    }
  }
}