
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using WatiN.Core;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Mozilla
{
	public class ElementFinder
	{
		private readonly string tagName;
		private readonly string type;
		private readonly AttributeConstraint constraint;
		private readonly FireFoxClientPort clientPort;
	    private Element parentElement;

        /// <summary>
        /// Creates a new instance of the <see cref="ElementFinder"/> class.
        /// </summary>
        /// <param name="parentElement">The element to start searching from.</param>
        /// <param name="tagname"></param>
        /// <param name="constraint"></param>
        /// <param name="clientPort"></param>
	    public ElementFinder(Element parentElement, String tagname, AttributeConstraint constraint, FireFoxClientPort clientPort) : this (tagname, constraint, clientPort)
	    {
	        this.parentElement = parentElement;
	    }

		public ElementFinder(string tagname, AttributeConstraint constraint, FireFoxClientPort clientPort) : this (tagname, null, constraint, clientPort)
		{}
		
		public ElementFinder(string tagname, string type, AttributeConstraint constraint, FireFoxClientPort clientPort)
		{
			this.type = type;
			this.tagName = tagname;
            if (UtilityClass.IsNullOrEmpty(this.tagName))
			{
                this.tagName = "*";
			}
			this.constraint = constraint;
			this.clientPort = clientPort;
		}

		/// <summary>
		/// Finds the first element that matches the given constaints
		/// </summary>
		/// <returns>A javascript variable name with a reference to the matching element, or null of no match is found.</returns>
		public string FindFirst()
		{
			string elementArrayName = FireFoxClientPort.CreateVariableName();

			string command = string.Format("{0} = {1}.getElementsByTagName(\"{2}\"); ", elementArrayName, FireFoxClientPort.DocumentVariableName, this.tagName);

			// TODO: Can't get this to work, but if it does then the TypeIsOk check 
			// Can be removed.
//            if (this.type != null)
//            {
//            	command = command + FilterInputTypes(elementArrayName);
//            }
            command = command + string.Format("{0}.length;", elementArrayName);
            this.clientPort.Write(command);

            int numberOfElements = int.Parse(this.clientPort.LastResponse);
			
			for (int index = 0; index < numberOfElements; index++)
			{	            
				string indexedElementVariableName = string.Format("{0}[{1}]", elementArrayName, index);
                FireFoxElementAttributeBag attributebag = new FireFoxElementAttributeBag(indexedElementVariableName, this.clientPort);
                                
                if (TypeIsOK(attributebag) && this.constraint.Compare(attributebag))
	            {
	            	string elementVariableName = FireFoxClientPort.CreateVariableName();
					command = string.Format("{0}={1};", elementVariableName, indexedElementVariableName);
                    this.clientPort.Write(command);
		        	
					return elementVariableName;
	            }
			}
			
			return null;
		}

        /// <summary>
        /// Finds all the elements that match the given constraint
        /// </summary>
        /// <returns>A list of element references that match the given constraint</returns>
        public List<string> FindAll()
        {
            string elementArrayName = FireFoxClientPort.CreateVariableName();
            string elementToSearchFrom = FireFoxClientPort.DocumentVariableName;
                
            if (this.parentElement != null && this.parentElement.Exists)
            {
                elementToSearchFrom = this.parentElement.ElementVariable;
            }

            string command = string.Format("{0} = {1}.getElementsByTagName(\"{2}\"); ", elementArrayName, elementToSearchFrom, this.tagName);
            command = command + string.Format("{0}.length;", elementArrayName);
            this.clientPort.Write(command);

            int numberOfElements = int.Parse(this.clientPort.LastResponse);
            List<string> elementReferences = new List<string>();

            for (int index = 0; index < numberOfElements; index++)
            {
                string indexedElementVariableName = string.Format("{0}[{1}]", elementArrayName, index);
                FireFoxElementAttributeBag attributebag = new FireFoxElementAttributeBag(indexedElementVariableName, this.clientPort);
                if (TypeIsOK(attributebag) && (this.constraint == null || this.constraint.Compare(attributebag)))
                {
                    string elementVariableName = FireFoxClientPort.CreateVariableName();
                    command = string.Format("{0}={1};", elementVariableName, indexedElementVariableName);
                    this.clientPort.Write(command);

                    elementReferences.Add(elementVariableName);
                }
            }

            return elementReferences;
        }

        private bool TypeIsOK(FireFoxElementAttributeBag attributebag)
        {
            if (this.type != null)
            {
            	string elementtype = attributebag.GetValue("type");
            	if (elementtype == null)
            	{
            		elementtype = "text";
            	}
            	return this.type.ToLowerInvariant().Contains(elementtype.ToLowerInvariant());
            }

            return true;
        }
        
        // TODO: Can't get this to work, but if it does then the TypeIsOk check 
		// Can be removed.
        private string FilterInputTypes(string elementArrayName)
        {
          	string typeArrayName = FireFoxClientPort.CreateVariableName();
        	string types = FireFoxClientPort.CreateVariableName();
        	string elementtype = FireFoxClientPort.CreateVariableName();
        	
			StringBuilder command = new StringBuilder(string.Format("{0} = {1}.getElementsByTagName(\"{2}\"); ", elementArrayName, FireFoxClientPort.DocumentVariableName, this.tagName));

        	command.Append(string.Format("{0} = new Array();", typeArrayName));
        	command.Append(string.Format("for(i=0;i<{0}.length;i++)", elementArrayName));
        	command.Append("{");
        	command.Append(string.Format("{0}={1}[i].type;", elementtype, elementArrayName));
        	command.Append(string.Format("if ({0}== null)", elementtype));
        	command.Append("{");
        	command.Append(string.Format("{0}=\"text\";", elementtype));
        	command.Append("}");
        	command.Append(string.Format("if(\"{0}\".indexOf({1}.toLowerCase()) > 0)", this.type.ToLower(), elementtype));
        	command.Append("{");
        	command.Append(string.Format("{0}.push({1}[i]);", typeArrayName, elementArrayName));
        	command.Append("}}");
        	command.Append(string.Format("{0} = {1};", elementArrayName, typeArrayName));
        	command.Append(string.Format("{0} = null;", typeArrayName));
        	
        	return command.ToString();
        }
	}
}

