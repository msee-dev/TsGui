﻿using System;
using System.Collections.Generic;
using Core.Logging;
using Core.Diagnostics;
using System.Xml.Linq;

namespace TsGui.Scripts
{
    public static class ScriptLibrary
    {
        private static Dictionary<string, IScript> _scripts = new Dictionary<string, IScript>();

        public static void LoadXml(XElement InputXml)
        {
            foreach (XElement x in InputXml.Elements("Scrtip"))
            {
                AddScript(ScriptFactory.GetScript(x));
            }
        }

        public static IScript GetScript(string name)
        {
            IScript outscript;
            if (_scripts.TryGetValue(name, out outscript)==false)
            {
                Log.Warn("Unable to find script in library: " + name);
                return null;
            }
            else
            {
                return outscript;
            }
        }

        public static void AddScript(IScript script)
        {
            if (_scripts.ContainsKey(script.Name))
            {
                throw new KnownException("Script with that name already exists in configuration: " + script.Name, String.Empty);
            }
            else
            {
                _scripts.Add(script.Name, script);
            }
        }
    }
}
