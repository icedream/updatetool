using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;

namespace UpdateToolGui
{
    [XmlRoot("database")]
    [XmlInclude(typeof(DatabaseFileInfo))]
    [Serializable()]
    public class Database
    {
        public Database()
        {
            this.Files = new List<DatabaseFileInfo>();
            xsi = new XmlSerializer(typeof(Database));
        }

        public static bool CanDeserialize(XmlReader xmlReader)
        {
            return new XmlSerializer(typeof(Database)).CanDeserialize(xmlReader);
        }

        public static Database Deserialize(XmlReader xmlReader)
        {
            return (Database)new XmlSerializer(typeof(Database)).Deserialize(xmlReader);
        }

        public static Database Deserialize(TextReader textReader)
        {
            return (Database)new XmlSerializer(typeof(Database)).Deserialize(textReader);
        }

        public static Database Deserialize(Stream stream)
        {
            return (Database)new XmlSerializer(typeof(Database)).Deserialize(stream);
        }

        public void Serialize(Stream stream)
        {
            xsi.Serialize(stream, this);
        }

        public void Serialize(TextWriter textWriter)
        {
            xsi.Serialize(textWriter, this);
        }

        public void Serialize(XmlWriter xmlWriter)
        {
            xsi.Serialize(xmlWriter, this);
        }

        private XmlSerializer xsi { get; set; }

        [XmlArray("files")]
        [XmlArrayItem("file")]
        public List<DatabaseFileInfo> Files { get; set; }
    }

    public class DatabaseFileInfo
    {
        [XmlElement("folder")]
        public string Folder { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("size")]
        public long Size { get; set; }

        [XmlElement("modification")]
        public DateTime ModificationTime { get; set; }

        [XmlElement("hash")]
        public byte[] Hash { get; set; }
    }
}
