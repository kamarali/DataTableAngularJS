using System;
using System.Reflection;
using System.Xml;

namespace Iata.IS.Core.ValidationErrorCorrection
{
    public static class ValidationErrorConfigReader
    {
        /// <summary>
        /// Path of Config file having Exception code data
        /// </summary>
        internal const string EmbeddedIdecResourcTypeFileName = "Iata.IS.Core.ValidationErrorCorrection.ValidationErrorType.xml";

        internal const string DocumentHeaderNode = "Exception";
        internal const string DocumentCodeNode = "Code";
        internal const string DocumentBillingCategory = "BillingCategory";
        internal const string DocumentTypeNode = "Type";
        internal const string DocumentRegularExpression = "RegularExpression";
        internal const string DocumentValidationTypeNode = "ValidationType";
        internal const string DocumentMasterTableNode = "MasterTable";
        internal const string DocumentMasterColumnName = "MasterColumnName";
        internal const string DocumentMasterGroupId = "MasterGroupId";
        internal const string DocumentMasterGroupColumnName = "MasterGroupColumnName";
        internal const string DocumentErrorLevelName = "Name";
        internal const string DocumentErrorLevelChildTableName = "ChildTableName";
        internal const string DocumentErrorLevelColumnName = "ColumnName";
        internal const string DocumentErrorLevelPkColumnName = "PrimaryColumnName";

        /// <summary>
        /// Returns the Exceptioncode configuration details
        /// </summary>
        /// <param name="exceptionCode"></param>
        /// <param name="billingCategoryId"></param>
        /// <returns></returns>
        public static ErrorCorrectionExceptionCode GetExceptionCodeDetails(string exceptionCode, int billingCategoryId)
        {
            try
            {
                var errorCorrectionExceptionCode = new ErrorCorrectionExceptionCode();
                var objXmlDocumt = new XmlDocument();

                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedIdecResourcTypeFileName);
                if (stream != null)
                {
                    objXmlDocumt.Load(stream);
                }

                var spath = string.Format("//{0}[@{1}='{2}']", DocumentHeaderNode, DocumentCodeNode, exceptionCode);

                var node = objXmlDocumt.SelectSingleNode(spath);

                if (node != null)
                {
                    //Loop through all the attributes of the node.
                    if (node.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.LocalName.CompareTo(DocumentCodeNode) == 0)
                                errorCorrectionExceptionCode.ExceptionCode = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentBillingCategory) == 0)
                                errorCorrectionExceptionCode.BillingCategory = Convert.ToInt32(attribute.Value);

                            if (attribute.LocalName.CompareTo(DocumentValidationTypeNode) == 0)
                                errorCorrectionExceptionCode.ValidationType = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentMasterTableNode) == 0)
                                errorCorrectionExceptionCode.MasterTableName = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentMasterColumnName) == 0)
                                errorCorrectionExceptionCode.MasterColumnName = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentMasterGroupId) == 0)
                                errorCorrectionExceptionCode.MasterGroupId = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentMasterGroupColumnName) == 0)
                                errorCorrectionExceptionCode.MasterGroupColumnName = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentRegularExpression) == 0)
                                errorCorrectionExceptionCode.RegularExpression = attribute.Value;

                            if (attribute.LocalName.CompareTo(DocumentTypeNode) == 0)
                                errorCorrectionExceptionCode.FieldType = attribute.Value;
                        }

                        //Loop throught all the Child nodes of the node 
                        foreach (XmlNode fieldNode in node)
                        {
                            if (fieldNode.Attributes != null)
                            {
                                var errorLevelModel = new ErrorLevelModel();

                                foreach (XmlAttribute attribute in fieldNode.Attributes)
                                {
                                    if (attribute.LocalName.CompareTo(DocumentErrorLevelName) == 0)
                                        errorLevelModel.ErrorLevelName = attribute.Value;

                                    if (attribute.LocalName.CompareTo(DocumentErrorLevelChildTableName) == 0)
                                        errorLevelModel.ChildTableName = attribute.Value;

                                    if (attribute.LocalName.CompareTo(DocumentErrorLevelColumnName) == 0)
                                        errorLevelModel.ColumnName = attribute.Value;

                                    if (attribute.LocalName.CompareTo(DocumentErrorLevelPkColumnName) == 0)
                                        errorLevelModel.PrimaryColumnName = attribute.Value;
                                }
                                errorCorrectionExceptionCode.ErrorLevels.Add(errorLevelModel);
                            }
                        }
                    }
                    return errorCorrectionExceptionCode;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
