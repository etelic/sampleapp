# region Includes...
using System;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Threading;
using System.Collections.Generic;
# endregion // Includes...
/// <summary>
/// Summary description for Export
/// </summary>
public class DataToExcel
{
    public enum ExportFormat : int { CSV = 1, Excel = 2 };
    System.Web.HttpResponse response;
    private string appType;
    public DataToExcel()
    {
        appType = "Web";
        response = System.Web.HttpContext.Current.Response;
    }
    public DataToExcel(string ApplicationType)
    {
        appType = ApplicationType;
        if (appType == "Web") response = System.Web.HttpContext.Current.Response;
    }
    #region ExportDetails
    public void ExportDetails(DataTable DetailsTable, string FormatType, string FileName, List<String> errorList)
    {
        try
        {
            if (DetailsTable.Rows.Count == 0)
                throw new Exception("There are no details to export.");
            if (errorList.Count > 0)
                DetailsTable.Columns.Add("Import Status").SetOrdinal(0);
            for (int i = 0; i < DetailsTable.Rows.Count; i++)
            {
                if (errorList.Contains((i + 1).ToString()))
                    DetailsTable.Rows[i]["Import Status"] = "Error";
                else
                    DetailsTable.Rows[i]["Import Status"] = "Success";
            }
            DataSet dsExport = new DataSet("Export");
            DataTable dtExport = DetailsTable.Copy();
            dtExport.TableName = "Values";
            dsExport.Tables.Add(dtExport);
            string[] sHeaders = new string[dtExport.Columns.Count];
            string[] sFileds = new string[dtExport.Columns.Count];
            for (int i = 0; i < dtExport.Columns.Count; i++)
            {
                sHeaders[i] = dtExport.Columns[i].ColumnName;
                sFileds[i] = ReplaceSpclChars(dtExport.Columns[i].ColumnName);
            }
            if (appType == "Web")
                ExportwithXSLT(dsExport, sHeaders, sFileds, FormatType, FileName);
        }
        catch (Exception Ex)
        {
            throw Ex;
        }
    }
    #endregion
    #region Export with XSLT 
    private void ExportwithXSLT(DataSet dsExport, string[] sHeaders, string[] sFileds, string FormatType, string FileName)
    {
        try
        {
            response.Clear();
            response.Buffer = true;
            if (FormatType == ExportFormat.CSV.ToString())
            {
                response.ContentType = "text/csv";
                response.AppendHeader("content-disposition", "attachment; filename=" + FileName);
            }
            else
            {
                response.ContentType = "application/vnd.ms-excel";
                response.AppendHeader("content-disposition", "attachment; filename=" + FileName);
            }
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            CreateStylesheet(writer, sHeaders, sFileds, FormatType);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            XmlDataDocument xmlDoc = new XmlDataDocument(dsExport);
            XslTransform xslTran = new XslTransform();
            xslTran.Load(new XmlTextReader(stream), null, null);
            System.IO.StringWriter sw = new System.IO.StringWriter();
            xslTran.Transform(xmlDoc, null, sw, null);
            response.Write(sw.ToString());
            sw.Close();
            writer.Close();
            stream.Close();
            response.End();
        }
        catch (ThreadAbortException Ex)
        {
            string ErrMsg = Ex.Message;
        }
        catch (Exception Ex)
        {
            throw Ex;
        }
    }
    #endregion
    #region CreateStylesheet
    private void CreateStylesheet(XmlTextWriter writer, string[] sHeaders, string[] sFileds, string FormatType)
    {
        try
        {
            string ns = "http://www.w3.org/1999/XSL/Transform";
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("xsl", "stylesheet", ns);
            writer.WriteAttributeString("version", "1.0");
            writer.WriteStartElement("xsl:output");
            writer.WriteAttributeString("method", "text");
            writer.WriteAttributeString("version", "4.0");
            writer.WriteEndElement();
            writer.WriteStartElement("xsl:template");
            writer.WriteAttributeString("match", "/");
            for (int i = 0; i < sHeaders.Length; i++)
            {
                writer.WriteString("\"");
                writer.WriteStartElement("xsl:value-of");
                writer.WriteAttributeString("select", "'" + sHeaders[i] + "'");
                writer.WriteEndElement();
                writer.WriteString("\"");
                if (i != sFileds.Length - 1) writer.WriteString((FormatType == ExportFormat.CSV.ToString()) ? "," : "	");
            }
            writer.WriteStartElement("xsl:for-each");
            writer.WriteAttributeString("select", "Export/Values");
            writer.WriteString("\r\n");
            for (int i = 0; i < sFileds.Length; i++)
            {
                writer.WriteString("\"");
                writer.WriteStartElement("xsl:value-of");
                writer.WriteAttributeString("select", sFileds[i]);
                writer.WriteEndElement(); 
                writer.WriteString("\"");
                if (i != sFileds.Length - 1) writer.WriteString((FormatType == ExportFormat.CSV.ToString()) ? "," : "	");
            }
            writer.WriteEndElement();
            writer.WriteEndElement(); 
            writer.WriteEndElement(); 
            writer.WriteEndDocument();
        }
        catch (Exception Ex)
        {
            throw Ex;
        }
    }
    #endregion 
    #region ReplaceSpclChars
    private string ReplaceSpclChars(string fieldName)
    {
        //			space 	-> 	_x0020_
        //			%		-> 	_x0025_
        //			#		->	_x0023_
        //			&		->	_x0026_
        //			/		->	_x002F_
        fieldName = fieldName.Replace(" ", "_x0020_");
        fieldName = fieldName.Replace("%", "_x0025_");
        fieldName = fieldName.Replace("#", "_x0023_");
        fieldName = fieldName.Replace("&", "_x0026_");
        fieldName = fieldName.Replace("/", "_x002F_");
        return fieldName;
    }
    #endregion 
}