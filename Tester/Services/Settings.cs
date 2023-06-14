using System.Diagnostics;
using System.Xml;

namespace Tester.Services
{
    class Settings
    {
        string maindir;

        public Settings()
        {
            maindir = FileSystem.Current.AppDataDirectory;

            Debug.WriteLine(maindir);
        }


        public List<string> getSettings()
        {
            XmlDocument doc = new XmlDocument();

            if (!File.Exists(maindir + "/Settings.xml"))
            {
                Debug.WriteLine("settings file not existing " + maindir + "/Settings.xml");
                return null;
            }
            doc.Load(maindir + "/Settings.xml");

            List<string> settings = new List<string>();

            Debug.WriteLine(doc.DocumentElement.ChildNodes);

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                string text = node.InnerText;

                settings.Add(text);
            }
            return settings;

        }

        public void setSettings(List<string> settings)
        {
            string maindir = FileSystem.Current.AppDataDirectory;
            using (XmlWriter writer = XmlWriter.Create(maindir + "/Settings.xml"))
            {
                writer.WriteStartElement("Settings");
                writer.WriteElementString("name", settings[0]);
                writer.WriteElementString("server", settings[1]);
                writer.WriteElementString("dbName", settings[2]);
                writer.WriteElementString("username", settings[3]);
                writer.WriteElementString("password", settings[4]);
                writer.WriteEndElement();
                writer.Flush();
            }
        }





    }
}
