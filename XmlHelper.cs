using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Collections;

class XmlHelper
{
    public static string readFromXml(string xmlFilePath, string attributeName)
    {
        if (File.Exists(xmlFilePath))
        {
            try
                {
                    XmlTextReader reader = new XmlTextReader(xmlFilePath);
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == attributeName)
                        {
                            reader.Read();
                            return reader.Value;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!String.IsNullOrWhiteSpace(e.Message) && e.Message.StartsWith("Invalid character in the given encoding"))
                    {
                        XmlTextReader reader = new XmlTextReader(xmlFilePath);
                        System.Text.Encoding.GetEncoding(1252).GetString(reader.Encoding.GetBytes(reader.Value));
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == attributeName)
                            {
                                reader.Read();
                                return reader.Value;
                            }
                        }
                    }
                    else
                        throw (e);
                }
        }
        else
        {
            throw new Exception("File not found");
        }
        throw new Exception("Attribute not found");
    }

    public static string[] getElements(string xmlFile, string[] attributePath)
    {
        if (!File.Exists(xmlFile))
            throw new Exception("File not found");

        int attributePathLength = attributePath.Length;
        List<string> results = new List<string>();
        bool[] checkedAttributes = new bool[attributePathLength];

        XmlTextReader reader = new XmlTextReader(xmlFile);
        while (reader.Read())
        {
            if ((reader.NodeType != XmlNodeType.Element)
                || (reader.Depth >= attributePathLength))
                continue;
            for (int i = reader.Depth + 1; i < checkedAttributes.Length; i++)
                checkedAttributes[i] = false;
            checkedAttributes[reader.Depth] = (attributePath[reader.Depth].Equals(reader.Name));
            for (int i = 0; i < checkedAttributes.Length; i++)
                if (!(checkedAttributes[i]))
                    goto xmlReaderLoopEnd;
            reader.Read();
            results.Add(reader.Value);
            xmlReaderLoopEnd: { };
        }
        return results.ToArray<string>();
    }

    public static Hashtable[] getElementLists(string xmlFile, string[] attributePath)
    {
        if (!File.Exists(xmlFile))
            throw new Exception("File not found");

        int attributePathLength = attributePath.Length;
        bool[] checkedAttributes = new bool[attributePathLength];
        List<Hashtable> results = new List<Hashtable>();
        int resultsCurrentIndex = 0;

        XmlTextReader reader = new XmlTextReader(xmlFile);
        while (reader.Read())
        {
            if ((reader.NodeType != XmlNodeType.Element) || (reader.Depth > attributePathLength))
                continue;
            if (reader.Depth == attributePathLength)
            {
                for (int i = 0; i < checkedAttributes.Length; i++)
                    if (!(checkedAttributes[i]))
                        goto xmlReaderLoopEnd;
                if (results.Count <= resultsCurrentIndex)
                    results.Add(new Hashtable());
                string tagName = reader.Name.ToString();
                reader.Read();
                results[resultsCurrentIndex].Add(tagName, reader.Value.ToString());
                continue;
            }
            for (int i = reader.Depth + 1; i < checkedAttributes.Length; i++)
                checkedAttributes[i] = false;
            checkedAttributes[reader.Depth] = (attributePath[reader.Depth].Equals(reader.Name));
            for (int i = 0; i < checkedAttributes.Length; i++)
                if (!(checkedAttributes[i]))
                    goto xmlReaderLoopEnd;
            resultsCurrentIndex = results.Count;
            xmlReaderLoopEnd: { };
        }
        return results.ToArray<Hashtable>();
    }
}

