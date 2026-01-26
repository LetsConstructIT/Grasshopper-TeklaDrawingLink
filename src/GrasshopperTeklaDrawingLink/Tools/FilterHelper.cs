using System;
using System.Text.RegularExpressions;
using Tekla.Structures.Filtering;
using Tekla.Structures.TeklaStructuresInternal.Filtering;

namespace GTDrawingLink.Tools
{
    internal class FilterHelper
    {
        public static FilterExpression? Parse(string filterExpressionString, FilterExpressionParserType? parserType, ref string errorMessage, bool tryFixIfFailed = true)
        {
            if (parserType == null)
                return null;

            try
            {
                return  FilterExpressionParserFactory.CreateParser(parserType.Value).ParseExpressionString(filterExpressionString, null);
            }
            catch (NotSupportedException ex)
            {
                if (ex.Message.Contains("TemplateCustomString"))
                    errorMessage = "The filter contains \"Template\" category properties that can't be parsed from the expression. Try using the SObjGrp syntax instead.";
                else
                    errorMessage = "The filter contains entries that can't be parsed, possibly due to API limitations in this Tekla version: " + ex.Message;

                return null;
            }
            catch (Exception generalException)
            {
                if (tryFixIfFailed)
                {
                    FilterExpressionParserType? filterExpressionParserType = parserType;
                    FilterExpressionParserType filterExpressionParserType2 = FilterExpressionParserType.TEKLA;
                    if (filterExpressionParserType.GetValueOrDefault() == filterExpressionParserType2 & filterExpressionParserType != null)
                    {
                        filterExpressionString = FilterHelper.AddFirstAndLastParentheses(filterExpressionString);
                        return FilterHelper.Parse(filterExpressionString, parserType, ref errorMessage, false);
                    }
                }

                if (generalException.GetType().Name == "FilterExpressionParserException")
                {
                    object value = generalException.GetType().GetProperty("FilterExpressionString").GetValue(generalException);
                    errorMessage = "FilterExpressionParserException: " + (value?.ToString());
                }
                else
                    errorMessage = "The filter could not be parsed: " + generalException.Message;

                return null;
            }
        }

        public static string CreateFilter(FilterExpression filterExpression, string targetFileNameNoExtension, ref string errorMessage)
        {
            try
            {
               return new Filter(filterExpression).CreateFile(FilterExpressionFileType.OBJECT_GROUP_SELECTION, targetFileNameNoExtension);
            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                if (exception.Message.Contains("TemplateCustomString"))
                    errorMessage = "The filter contains \"Template\" category properties. Use the SObjGrp syntax instead.";
                else
                    errorMessage = "The filter could not be created: " + exception.Message;

                return string.Empty;
            }
        }

        public static string AddFirstAndLastParentheses(string sObjGrpFilterText)
        {
            var text = sObjGrpFilterText;
            var matchCollection = new Regex("SECTION_OBJECT_GROUP\\s*\\{([^}]*)\\}", RegexOptions.Singleline).Matches(text);
            if (matchCollection.Count == 0)
                return text;

            var regex = new Regex("(\\d+)(?!.*\\d)", RegexOptions.Singleline);
            var index = matchCollection[matchCollection.Count - 1].Index;
            var length = matchCollection[matchCollection.Count - 1].Length;

            var text2 = text.Substring(index, length);
            text2 = regex.Replace(text2, (Match m) => (int.Parse(m.Groups[1].Value) + 1).ToString());
            text = text.Remove(index, length).Insert(index, text2);

            var value2 = matchCollection[0].Groups[1].Value;
            text = new Regex("^\\s*(\\d+)", RegexOptions.Multiline).Replace(text, (Match m) => (int.Parse(m.Groups[1].Value) + 1).ToString(), 1);
            return text;
        }
    }
}
