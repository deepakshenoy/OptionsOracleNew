using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace OptionsOracle
{
    public class Lang
    {
        public static string SysDefaultLanguage
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.Name;
            }
        }

        public static string AppDefaultLanguage
        {
            get
            {
                return "en-US";
            }
        }

        public static string AppPreferredLanguage
        {
            get
            {
                string app_default_lang = Properties.Resources.AppDefaultLanguage.ToString();

                if (app_default_lang == "") return SysDefaultLanguage;
                else return app_default_lang;
            }
        }

        public static Stream GetResourceFileStream(string filename)
        {
            try
            {
                Stream stream;
                Assembly assembly = Assembly.GetExecutingAssembly();

                // get stream based on preferred language
                stream = assembly.GetManifestResourceStream("OptionsOracle.Resources." + AppPreferredLanguage.Replace('-', '_') + "." + filename);

                // if failed get stream based on default language
                if (stream == null)
                    stream = assembly.GetManifestResourceStream("OptionsOracle.Resources." + AppDefaultLanguage.Replace('-', '_') + "." + filename);

                return stream;
            }
            catch { }

            return null;
        }

        public static string ReadResourceFile(string filename)
        {
            try
            {
                Stream stream = GetResourceFileStream(filename);

                // create stream reader and extract content
                StreamReader stream_reader = new StreamReader(stream);
                return stream_reader.ReadToEnd();
            }
            catch { }

            return null;
        }
    }
}
