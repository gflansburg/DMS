using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Gafware.Modules.DMS.Xml
{
    /// <summary>
    /// XML serialization helper class
    /// </summary>
    /// <remarks>XML serialization helper class.</remarks>
    public class SerializerHelper
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>Default constructor</remarks>
        public SerializerHelper()
        {
        }

        /// <summary>
        /// Deserialize a XML encoded string into an object.
        /// </summary>
        /// <param name="xmldata">XML encoded string</param>
        /// <param name="objectType">Type of the object</param>
        /// <returns>A <see cref="System.Object"/></returns>
        /// <remarks>Use this method to deserailize a XML encoded string into an object of the specified type.</remarks>
        public static object FromXml(string xmldata, Type objectType)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);
            StringReader loginDataString = new StringReader(xmldata);
            return serializer.Deserialize(loginDataString);
        }

        /// <summary>
        /// Serialize an object into a XML encoded string
        /// </summary>
        /// <param name="obj">A <see cref="System.Object"/></param>
        /// <returns>A XML encoded string</returns>
        /// <remarks>Use this method to serialize an object into a XML encoded string.</remarks>
        public static string ToXml(object obj)
        {
            //StringWriter sw = null;
            System.Xml.XmlTextWriter xtWriter = null;
            System.IO.StreamReader reader = null;
            string xmlData = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                // create a MemoryStream here, we are just working
                // exclusively in memory
                System.IO.Stream stream = new System.IO.MemoryStream();
                // The XmlTextWriter takes a stream and encoding
                // as one of its constructors
                xtWriter = new System.Xml.XmlTextWriter(stream, Encoding.UTF8);
                serializer.Serialize(xtWriter, obj);
                xtWriter.Flush();
                // go back to the beginning of the Stream to read its contents
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                // read back the contents of the stream and supply the encoding
                reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                xmlData = reader.ReadToEnd();

                //StringBuilder sb = new StringBuilder();
                //sw = new StringWriter(sb);
                //XmlSerializer serializer = new XmlSerializer(obj.GetType());
                //serializer.Serialize(sw, obj);
                //xmlData = sb.ToString();
            }
            catch (Exception)
            {
                xmlData = null;
            }
            finally
            {
                //if(sw != null)
                //	sw.Close();
                if (reader != null)
                    reader.Close();
                if (xtWriter != null)
                    xtWriter.Close();
            }
            return xmlData;
        }

        /// <summary>
        /// Remove XML header from a XML encoded string
        /// </summary>
        /// <param name="xmlstring">Original XML string</param>
        /// <returns>A new XML encoded string with out headers</returns>
        /// <remarks>Use this method to remove XML headers.</remarks>
        public static string RemoveHeader(string xmlstring)
        {
            string newXml = "";
            string restXml = "";
            int ipos = xmlstring.IndexOf("\n");
            int xpos = xmlstring.IndexOf("xmlns:xsd=");
            int ypos = xmlstring.IndexOf("XMLSchema-instance");
            if (ipos > 0 && ypos < xmlstring.Length)
            {
                newXml = xmlstring.Substring(ipos + 1, xpos - (ipos + 1));
                restXml = xmlstring.Substring(ypos + 20, xmlstring.Length - (ypos + 20));
                newXml += restXml;
            }
            return newXml;
        }
    }
}