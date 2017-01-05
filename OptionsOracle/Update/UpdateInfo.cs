/*
 * OptionsOracle
 * Copyright 2006-2012 SamoaSky (Shlomo Shachar & Oren Moshe)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace OptionsOracle.Update
{
    public class UpdateInfo
    {
        private string module;      // module name
        private string filename;    // module file
        private string current;     // current module version
        private string latest;      // latest module version

        public UpdateInfo(string module, string current, string latest)
        { this.module = module; this.current = current; this.latest = latest; }

        public UpdateInfo(string module, string current, string latest, string filename)
        { this.module = module; this.current = current; this.latest = latest; this.filename = filename; }

        public string Module { get { return module; } set { module = value; } }
        public string CurrentVersion { get { return current; } set { current = value; } }
        public string LatestVersion { get { return latest; } set { latest = value; } }
        public string FileName { get { return filename; } set { filename = value; } }
    }
}
