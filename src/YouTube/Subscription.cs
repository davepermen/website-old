using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace YouTube
{
    class Subscription
    {
        internal static Xml.feedEntry[] ReadFeed(Stream contentStream)
        {
            var feed = (Xml.feed)new XmlSerializer(typeof(Xml.feed)).Deserialize(contentStream);
            return feed.Items.OfType<Xml.feedEntry>().ToArray();
        }
    }

    namespace Xml
    {

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
        [XmlRoot(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
        public partial class feed
        {
            [XmlElement("author", typeof(feedAuthor))]
            [XmlElement("entry", typeof(feedEntry))]
            [XmlElement("id", typeof(string))]
            [XmlElement("link", typeof(feedLink))]
            [XmlElement("published", typeof(DateTime))]
            [XmlElement("title", typeof(string))]
            [XmlElement("channelId", typeof(string), Namespace = "http://www.youtube.com/xml/schemas/2015")]
            [XmlChoiceIdentifier("ItemsElementName")]
            public object[] Items { get; set; }

            [XmlElement("ItemsElementName")]
            [XmlIgnore()]
            public ItemsChoiceType[] ItemsElementName { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
        public partial class feedAuthor
        {
            public string name { get; set; }
            public string uri { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
        public partial class feedEntry
        {
            public string id { get; set; }

            [XmlElement(Namespace = "http://www.youtube.com/xml/schemas/2015")]
            public string videoId { get; set; }

            [XmlElement(Namespace = "http://www.youtube.com/xml/schemas/2015")]
            public string channelId { get; set; }
            public string title { get; set; }
            public feedEntryLink link { get; set; }
            public feedEntryAuthor author { get; set; }
            public DateTime published { get; set; }
            public DateTime updated { get; set; }

            [XmlElement(Namespace = "http://search.yahoo.com/mrss/")]
            public group group { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
        public partial class feedEntryLink
        {
            [XmlAttribute()]
            public string rel { get; set; }

            [XmlAttribute()]
            public string href { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
        public partial class feedEntryAuthor
        {
            public string name { get; set; }

            public string uri { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://search.yahoo.com/mrss/")]
        [XmlRoot(Namespace = "http://search.yahoo.com/mrss/", IsNullable = false)]
        public partial class group
        {
            public string title { get; set; }
            public groupContent content { get; set; }
            public groupThumbnail thumbnail { get; set; }
            public string description { get; set; }
            public groupCommunity community { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://search.yahoo.com/mrss/")]
        public partial class groupContent
        {
            [XmlAttribute()]
            public string url { get; set; }

            [XmlAttribute()]
            public string type { get; set; }

            [XmlAttribute()]
            public uint width { get; set; }

            [XmlAttribute()]
            public uint height { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://search.yahoo.com/mrss/")]
        public partial class groupThumbnail
        {
            [XmlAttribute()]
            public string url { get; set; }

            [XmlAttribute()]
            public uint width { get; set; }

            [XmlAttribute()]
            public uint height { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://search.yahoo.com/mrss/")]
        public partial class groupCommunity
        {
            public groupCommunityStarRating starRating { get; set; }
            public groupCommunityStatistics statistics { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://search.yahoo.com/mrss/")]
        public partial class groupCommunityStarRating
        {
            [XmlAttribute()]
            public uint count { get; set; }

            [XmlAttribute()]
            public decimal average { get; set; }

            [XmlAttribute()]
            public byte min { get; set; }

            [XmlAttribute()]
            public byte max { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://search.yahoo.com/mrss/")]
        public partial class groupCommunityStatistics
        {
            [XmlAttribute()]
            public uint views { get; set; }
        }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
        public partial class feedLink
        {
            [XmlAttribute()]
            public string rel { get; set; }

            [XmlAttribute()]
            public string href { get; set; }
        }

        [Serializable()]
        [XmlType(Namespace = "http://www.w3.org/2005/Atom", IncludeInSchema = false)]
        public enum ItemsChoiceType
        {
            author,
            entry,
            id,
            link,
            published,
            title,
            [XmlEnum("http://www.youtube.com/xml/schemas/2015:channelId")]
            channelId,
        }


    }
}