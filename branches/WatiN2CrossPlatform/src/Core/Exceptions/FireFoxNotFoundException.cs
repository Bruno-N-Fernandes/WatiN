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

using System;
using System.Runtime.Serialization;

namespace WatiN.Core.Exceptions
{
    /// <summary>
    /// Thrown if the searched for Firefox can't be found.
    /// </summary>
    [Serializable]
    public class FireFoxNotFoundException : WatiNException
    {
        public FireFoxNotFoundException(string constraint, int waitTimeInSeconds) :
            base("Could not find a Firefox window matching constraint: " + constraint + ". Search expired after '" + waitTimeInSeconds.ToString() + "' seconds.") { }

        public FireFoxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
