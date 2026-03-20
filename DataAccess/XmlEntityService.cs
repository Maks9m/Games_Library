using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace GameLibrary.DataAccess
{
    // Reusable generic service for reading/writing one entity type as an XML section.
    // Knows nothing about domain types — all specifics are injected via delegates.
    public class XmlEntityService<T>
    {
        private readonly string _sectionElement;   // e.g. "Developers"
        private readonly string _itemElement;       // e.g. "Developer"
        private readonly Action<XmlWriter, T> _serialize;
        private readonly Func<XElement, T> _deserialize;

        public XmlEntityService(
            string sectionElement,
            string itemElement,
            Action<XmlWriter, T> serialize,
            Func<XElement, T> deserialize)
        {
            _sectionElement = sectionElement;
            _itemElement = itemElement;
            _serialize = serialize;
            _deserialize = deserialize;
        }

        // Writes <SectionElement><ItemElement>...</ItemElement></SectionElement>
        public void WriteSection(XmlWriter writer, IEnumerable<T> items)
        {
            writer.WriteStartElement(_sectionElement);
            foreach (var item in items)
            {
                writer.WriteStartElement(_itemElement);
                _serialize(writer, item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        // Reads all <ItemElement> descendants from the document
        public List<T> ReadSection(XDocument doc)
        {
            var result = new List<T>();
            foreach (var el in doc.Descendants(_itemElement))
                result.Add(_deserialize(el));
            return result;
        }
    }
}
