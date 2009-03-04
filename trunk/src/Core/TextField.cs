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

using WatiN.Core.Actions;
using WatiN.Core.Logging;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML input element of type 
	/// text password textarea hidden and for a HTML textarea element.
	/// </summary>
    [ElementTag("input", InputType = "text", Index = 0)]
    [ElementTag("input", InputType = "password", Index = 1)]
    [ElementTag("input", InputType = "textarea", Index = 2)]
    [ElementTag("input", InputType = "hidden", Index = 3)]
    [ElementTag("textarea", Index = 4)]
    public class TextField : Element<TextField>
	{
	    private TypeTextAction _typeTextAction;

	    public TextField(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

        public TextField(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

		public int MaxLength
		{
			get
			{
                var tagForTextArea = new ElementTag("textarea");
                if (tagForTextArea.IsMatch(NativeElement))
                    return 0;

                var value = GetAttributeValue("maxLength");
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
			}
		}

		public bool ReadOnly
		{
			get
			{
                var value = GetAttributeValue("readOnly");
                return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
            }
		}

		public void TypeText(string value)
		{
			Logger.LogAction("Typing '" + value + "' into " + GetType().Name + " '" + ToString() + "'");

            TypeTextAction.TypeText(value);
		}

		public void AppendText(string value)
		{
			Logger.LogAction("Appending '" + value + "' to " + GetType().Name + " '" + ToString() + "'");

            TypeTextAction.AppendText(value);
		}

		public void Clear()
		{
			Logger.LogAction("Clearing " + GetType().Name + " '" + ToString() + "'");

            TypeTextAction.Clear();
		}

	    public TypeTextAction TypeTextAction
	    {
	        get
	        {
	            if (_typeTextAction == null)
	            {
	                _typeTextAction = new TypeTextAction(this);
	            }
	            return _typeTextAction;
	        }
            set { _typeTextAction = value;}
	    }

		public string Value
		{
			get
			{
			    var value = GetAttributeValue("value");
			    return string.IsNullOrEmpty(value) ? null : value;
			}
		    // Don't use this set property internally (in this class). 
			set
			{
				Logger.LogAction("Setting " + GetType().Name + " '" + ToString() + "' to '" + value + "'");

                SetAttributeValue("value", value);
			}
		}

		/// <summary>
		/// Returns the same as the Value property
		/// </summary>
		public override string Text
		{
			get { return Value; }
		}

		public void Select()
		{
            NativeElement.Select();
		}

		public override string ToString()
		{
		    var title = Title;
		    if (UtilityClass.IsNotNullOrEmpty(title))
			{
				return title;
			}

		    var id = Id;
		    if (UtilityClass.IsNotNullOrEmpty(id))
			{
				return id;
			}

			return UtilityClass.IsNotNullOrEmpty(Name) ? Name : base.ToString();
		}

		public string Name
		{
            get { return GetAttributeValue("name"); }
		}
	}
}
