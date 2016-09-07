﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MiniTrols;

namespace eFormResponse
{
    public class Response
    {
        #region con
        public Response()
        {
            Checks = new List<Check>();
        }

        public Response(ResponseTypes type, string value)
        {
            Type = type;
            Value = value;
            Checks = new List<Check>();
        }
        #endregion

        #region var
        private Tools tool = new Tools();

        public ResponseTypes Type { get; set; }
        public string Value { get; set; }
        public string UnitFetchedAt { get; set; }
        public string UnitId { get; set; }
        public List<Check> Checks{ get; set; }
        #endregion

        #region public
        public Response XmlToClass(string xmlStr)
        {
            try
            {
                ResponseTypes rType = ResponseTypes.Invalid;
                string value = "";

                #region value type
                if (xmlStr.Contains("<Value type="))
                {
                    string subXmlStr = tool.Locate(xmlStr, "<Response>", "</Response>").Trim();
                    string valueTypeLower = tool.Locate(xmlStr, "Value type=\"", "\"").Trim().ToLower(); //digs out value's type
                    value = tool.Locate(xmlStr, "\">", "</").Trim();

                    switch (valueTypeLower)
                    {
                        case "success":
                            rType = ResponseTypes.Success;
                            break;

                        case "error":
                            rType = ResponseTypes.Error;
                            break;

                        case "parsing":
                            rType = ResponseTypes.Parsing;
                            break;

                        case "received":
                            rType = ResponseTypes.Received;
                            break;

                        case "invalid":
                            rType = ResponseTypes.Invalid;
                            break;

                        default:
                            throw new IndexOutOfRangeException("'" + valueTypeLower + "' is not a know ResponseType");
                    }
                }
                #endregion

                Response resp = new Response(rType, value);

                #region Unit fetched
                if (xmlStr.Contains("<Unit fetched_at="))
                {
                    string subXmlStr = xmlStr.Substring(xmlStr.IndexOf("<Unit fetched_at=\"") + 18); // 18 magic int = "<Unit fetched_at=\"".Length;
                    string dateTimeStr = subXmlStr.Substring(0, subXmlStr.IndexOf("\"")); //digs out unit's dateTime
                    string idStr = subXmlStr.Substring(dateTimeStr.Length + 6, subXmlStr.IndexOf("\"/>") - dateTimeStr.Length - 6); //digs out value's text

                    resp.UnitFetchedAt = dateTimeStr;
                    resp.UnitId = idStr;
                }
                #endregion

                #region checks
                string checkXmlStr = xmlStr;
                while (checkXmlStr.Contains("<Check "))
                {
                    Check check = new Check();

                    check.UnitId = tool.Locate(checkXmlStr, " unit_id=\"",   "\"");
                    check.Date = tool.Locate(checkXmlStr, " date=\"",      "\"");
                    check.Worker = tool.Locate(checkXmlStr, " worker=\"",    "\"");
                    check.Id = tool.Locate(checkXmlStr, " id=\"",        "\"");
                    check.WorkerId = tool.Locate(checkXmlStr, " worker_id=\"", "\"");

                    while (checkXmlStr.Contains("<ElementList>"))
                    {
                        string inderXmlStr = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?><ElementList>" + tool.Locate(checkXmlStr, "<ElementList>", "</ElementList>") + "</ElementList>";
                        ElementList eResp = XmlToClassCheck(inderXmlStr);
                        check.ElementList.Add(eResp);

                        int index = checkXmlStr.IndexOf("</ElementList>");
                        checkXmlStr = checkXmlStr.Substring(index + 14); //removes extracted xml
                    }

                    resp.Checks.Add(check);
                }
                #endregion

                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Response failed to convert XML", ex);
            }
        }

        public string ClassToXml()
        {
            try
            {
                string      xmlStr  = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine +
                                      "<Response>" + Environment.NewLine +
                                      "<Value type=\"" + Type + "\">" + Value + "</Value>" + Environment.NewLine; 
                if (UnitId != null)
                            xmlStr +=     "<Unit fetched_at=\"" + UnitFetchedAt + "\" id=\"" + UnitId + "\"/>" + Environment.NewLine;
                if (Checks.Count > 0)
                {
                            xmlStr +=     "<Checks>" + Environment.NewLine;
                    foreach (Check chk in Checks) {
                            xmlStr +=         "<Check unit_id=\""+chk.UnitId+"\" date=\""+chk.Date+"\" worker=\""+chk.Worker+"\" id=\""+chk.Id+"\" worker_id=\""+chk.WorkerId+"\">";
                        foreach (ElementList elemLst in chk.ElementList) { 
                            xmlStr +=             PureXml(ClassToXmlCheck(elemLst)) + Environment.NewLine;
                        }
                            xmlStr +=         "</Check>" + Environment.NewLine;
                    }
                            xmlStr +=     "</Checks>" + Environment.NewLine;
                }
                            xmlStr += "</Response>";

                return xmlStr;
            }
            catch (Exception ex)
            {
                throw new Exception("Response failed to convert Class", ex);
            }
        }
        #endregion

        #region private
        private ElementList XmlToClassCheck(string xmlStr)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(xmlStr);
                XmlSerializer serializer = new XmlSerializer(typeof(ElementList));
                StreamReader reader = new StreamReader(new MemoryStream(byteArray));

                ElementList elementResp = null;
                elementResp = (ElementList)serializer.Deserialize(reader);
                reader.Close();

                return elementResp;
            }
            catch (Exception ex)
            {
                throw new Exception("Response failed to convert XML", ex);
            }
        }

        private string ClassToXmlCheck(ElementList elementList)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ElementList));
                string xmlStr;
                using (StringWriter writer = new Utf8StringWriter())
                {
                    serializer.Serialize(writer, elementList);
                    xmlStr = writer.ToString();
                }
                return xmlStr;
            }
            catch (Exception ex)
            {
                throw new Exception("Response failed to convert Class", ex);
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        private string PureXml(string xmlStr)
        {
            xmlStr = xmlStr.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            xmlStr = xmlStr.Replace("<ElementList xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<ElementList>");

            return xmlStr.Trim();
        }
        #endregion

        public enum ResponseTypes
        {
            Error,
            Received,
            Parsing,
            Success,
            Invalid
        }
    }
}