﻿//    Copyright (C) 2017 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System.Xml.Linq;
using System.Collections.Generic;
using TsGui.Validation.StringMatching;
using TsGui.Linking;

namespace TsGui.Queries
{
    public abstract class BaseQuery: IQuery
    {
        protected List<IStringMatchingRule> _ignorerules = new List<IStringMatchingRule>();
        protected bool _reprocess = false;
        protected bool _processed = false;
        protected ResultWrangler _processingwrangler = new ResultWrangler();
        protected ResultWrangler _returnwrangler;
        protected bool _ignoreempty = true;
        protected ILinkTarget _owner;

        public virtual ResultWrangler GetResultWrangler()
        {
            if ((this._reprocess == true) || (this._processed == false)) { return this.ProcessQuery(); }
            else { return this._returnwrangler; }
        }

        public abstract ResultWrangler ProcessQuery();

        public BaseQuery(ILinkTarget owner)
        {
            this._owner = owner;
        }

        protected void LoadXml(XElement InputXml)
        {
            this._reprocess = XmlHandler.GetBoolFromXAttribute(InputXml, "Reprocess", this._reprocess);
            this._ignoreempty = XmlHandler.GetBoolFromXAttribute(InputXml, "IgnoreEmpty", this._ignoreempty);
            foreach (XElement xignorerule in InputXml.Elements("Ignore"))
            {
                this._ignorerules.Add(MatchingRuleFactory.GetRuleObject(xignorerule, this._owner));
            }
        }

        protected bool ShouldIgnore(string input)
        {
            if ((this._ignoreempty == true) && (string.IsNullOrWhiteSpace(input) == true)) { return true; }

            foreach (IStringMatchingRule rule in this._ignorerules)
            {
                if (rule.DoesMatch(input) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
