using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace YouTube
{
    class SubscriptionManager
    {
        internal static Xml.opmlBodyOutlineOutline[] LoadFromFile(string v)
        {
            using (var file = File.OpenRead(v))
            {
                var opml = (Xml.opml)new XmlSerializer(typeof(Xml.opml)).Deserialize(file);
                return opml.body.outline.outline;
            }
        }
    }

    namespace Xml
    {

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        [XmlRoot(Namespace = "", IsNullable = false)]
        public partial class opml
        {
            public opmlBody body { get; set; }
            [XmlAttribute()]
            public decimal version { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        public partial class opmlBody
        {
            public opmlBodyOutline outline { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        public partial class opmlBodyOutline
        {
            [XmlElement("outline")]
            public opmlBodyOutlineOutline[] outline { get; set; }

            [XmlAttribute()]
            public string text { get; set; }

            [XmlAttribute()]
            public string title { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        public partial class opmlBodyOutlineOutline
        {
            [XmlAttribute()]
            public string text { get; set; }

            [XmlAttribute()]
            public string title { get; set; }

            [XmlAttribute()]
            public string type { get; set; }

            [XmlAttribute()]
            public string xmlUrl { get; set; }
        }


    }
}