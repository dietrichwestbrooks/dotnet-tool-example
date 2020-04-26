namespace Monaco.Tools.Serialization
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Aristocrat.G2S.Emdi.Protocol.v21ext1b1;

    public class MessageSerializer
    {
        private const string DefaultNamespace = "http://mediaDisplay.igt.com";
        // private const string DefaultNamespace = "";

        private readonly XmlSerializerNamespaces _namespaces = new XmlSerializerNamespaces();
        private readonly XmlSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSerializer"/> class.
        /// </summary>
        public MessageSerializer()
        {
            _namespaces.Add("md", "http://mediaDisplay.igt.com");
            _namespaces.Add("cpc", "http://www.gamingstandards.com/emdi/schemas/v1b/CPC");
            _namespaces.Add("hci", "http://www.gamingstandards.com/emdi/schemas/v1b/HCI");
            _namespaces.Add("plc", "http://www.gamingstandards.com/emdi/schemas/v1b/PLC");
            _namespaces.Add("cci", "http://www.gamingstandards.com/emdi/schemas/v1b/CCI");
            _namespaces.Add("rcl", "http://www.gamingstandards.com/emdi/schemas/v1b/RCL");
            _namespaces.Add("hst", "http://www.aristocrat.com/emdi/schemas/v1b/HST");

            _serializer = CreateSerializer();
        }

        public bool TrySerialize(mdMsg message, out string xml)
        {
            xml = null;

            var result = true;

            try
            {
                xml = Serialize(message);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "memory stream disposal is ok")]
        public string Serialize(mdMsg message)
        {
            if (message == null)
            {
                return null;
            }

            string xml;

            using (var memorySteam = new MemoryStream())
            using (var writer = new XmlTextWriter(memorySteam, Encoding.UTF8))
            {
                _serializer.Serialize(writer, message, _namespaces);
                memorySteam.Position = 0;
                using (var reader = new StreamReader(memorySteam))
                {
                    xml = reader.ReadToEnd();
                }
            }

            return xml;
        }

        public bool TryDeserialize(string xml, out mdMsg message)
        {
            message = null;

            var result = true;

            try
            {
                message = Deserialize(xml);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public mdMsg Deserialize(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                throw new ArgumentNullException(nameof(xml));
            }

            object result;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                result = _serializer.Deserialize(ms);
            }

            return (mdMsg)result;
        }

        private XmlSerializer CreateSerializer()
        {
            var overrides = new XmlAttributeOverrides();

            overrides.Add(typeof(mdCabinet), "Item", GetCabinetOverrides());
            overrides.Add(typeof(c_mdMsgBase), "Item", GetFunctionalGroupsOverrides());
            overrides.Add(typeof(c_eventReportEventItem), "Item", GetEventReportOverrides());

            return new XmlSerializer(typeof(mdMsg), overrides, new Type[] { }, null, DefaultNamespace);
        }

        private XmlAttributes GetCabinetOverrides()
        {
            var attributes = new XmlAttributes();

            foreach (var attr in typeof(mdCabinet).GetProperty("Item")?.GetCustomAttributes(false).OfType<XmlElementAttribute>() ??
                                 new XmlElementAttribute[] { })
            {
                attributes.XmlElements.Add(attr);
            }

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "contentToHostMessage",
                Type = typeof(contentToHostMessage),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/HCI"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "contentToHostMessageAck",
                Type = typeof(contentToHostMessageAck),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/HCI"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "getCabinetStatus",
                Type = typeof(getCabinetStatus),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/PLC"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "cabinetStatus",
                Type = typeof(cabinetStatus),
                Namespace = "http://mediaDisplay.igt.com"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "setCardRemoved",
                Type = typeof(setCardRemoved),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/CPC"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "hostToContentMessage",
                Type = typeof(hostToContentMessage),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/HCI"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "hostToContentMessageAck",
                Type = typeof(hostToContentMessageAck),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/HCI"
            });

            return attributes;
        }

        private XmlAttributes GetFunctionalGroupsOverrides()
        {
            var attributes = new XmlAttributes();

            foreach (var attr in typeof(mdMsg).GetProperty("Item")?.GetCustomAttributes(true).OfType<XmlElementAttribute>() ??
                                 new XmlElementAttribute[] { })
            {
                attributes.XmlElements.Add(attr);
            }

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "mdContentToContent",
                Type = typeof(mdContentToContent),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/CCI"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "mdHost",
                Type = typeof(mdHost),
                Namespace = "http://www.aristocrat.com/emdi/schemas/v1b/HST"
            });

            return attributes;
        }

        private XmlAttributes GetEventReportOverrides()
        {
            var attributes = new XmlAttributes();

            foreach (var attr in typeof(c_eventReportEventItem).GetProperty("Item")?.GetCustomAttributes(false).OfType<XmlElementAttribute>() ??
                                 new XmlElementAttribute[] { })
            {
                attributes.XmlElements.Add(attr);
            }

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "recallLog",
                Type = typeof(recallLog),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/RCL"
            });

            attributes.XmlElements.Add(new XmlElementAttribute
            {
                ElementName = "activeContent",
                Type = typeof(activeContent),
                Namespace = "http://www.gamingstandards.com/emdi/schemas/v1b/CCI"
            });

            return attributes;
        }
    }
}
