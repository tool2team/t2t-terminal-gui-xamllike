using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace Terminal.Gui.XamlLike
{
    /// <summary>
    /// Parses .tui.xaml files into XamlDocument objects
    /// </summary>
    public static class XamlParser
    {
        /// <summary>
        /// Parses a .tui.xaml file content into a XamlDocument
        /// </summary>
        public static ParseResult<XamlDocument> Parse(string content, string filePath)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return ParseResult<XamlDocument>.CreateError(
                    TuiDiagnostics.EmptyXamlFile,
                    filePath);
            }

            try
            {
                // Use XmlReader to preserve line information
                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = false,
                    IgnoreComments = true
                };

                using var stringReader = new StringReader(content);
                using var xmlReader = XmlReader.Create(stringReader, settings);

                // Move to the root element
                xmlReader.MoveToContent();

                if (xmlReader.NodeType != XmlNodeType.Element)
                {
                    return ParseResult<XamlDocument>.CreateError(
                        TuiDiagnostics.InvalidXml,
                        filePath,
                        "No root element found");
                }

                var rootElement = ParseElementWithReader(xmlReader);
                var xamlDoc = new XamlDocument(rootElement, filePath);

                // Validate required x:Class attribute
                if (string.IsNullOrWhiteSpace(xamlDoc.ClassName))
                {
                    return ParseResult<XamlDocument>.CreateError(
                        TuiDiagnostics.MissingXClass,
                        filePath);
                }

                return ParseResult<XamlDocument>.Success(xamlDoc);
            }
            catch (XmlException ex)
            {
                return ParseResult<XamlDocument>.CreateError(
                    TuiDiagnostics.InvalidXml,
                    filePath,
                    ex.Message);
            }
            catch (Exception ex)
            {
                return ParseResult<XamlDocument>.CreateError(
                    TuiDiagnostics.ParseError,
                    filePath,
                    ex.Message);
            }
        }

        /// <summary>
        /// Parses an XML element using XmlReader (preserves line info)
        /// </summary>
        private static XamlElement ParseElementWithReader(XmlReader reader)
        {
            var lineInfo = reader as IXmlLineInfo;
            int lineNumber = lineInfo?.HasLineInfo() == true ? lineInfo.LineNumber : 0;
            int linePosition = lineInfo?.HasLineInfo() == true ? lineInfo.LinePosition : 0;

            var elementName = reader.LocalName;
            var attributes = new Dictionary<string, string>();
            var children = new List<XamlElement>();

            // Read attributes
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    attributes[reader.Name] = reader.Value;
                }
                reader.MoveToElement();
            }

            // Read child elements
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        children.Add(ParseElementWithReader(reader));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                reader.Read(); // Read the end element
            }
            else
            {
                reader.Read(); // Move past the empty element
            }

            return new XamlElement(elementName, attributes, children, lineNumber, linePosition);
        }

        /// <summary>
        /// Validates a XamlDocument and returns any validation errors
        /// </summary>
        public static List<TuiDiagnostic> Validate(XamlDocument document)
        {
            var diagnostics = new List<TuiDiagnostic>();

            ValidateElement(document.RootElement, document.SourceFilePath, diagnostics, document.DataType);

            return diagnostics;
        }

        /// <summary>
        /// Validates a single element and its children
        /// </summary>
        private static void ValidateElement(XamlElement element, string filePath, List<TuiDiagnostic> diagnostics, string? dataType = null)
        {
            // Validate control type exists
            var controlType = Mappings.GetFullTypeName(element.Name);
            if (!IsKnownControlType(element.Name))
            {
                diagnostics.Add(TuiDiagnostics.UnknownControlType.Create(filePath, element.Name));
            }

            // Validate bindings
            foreach (var kvp in element.PropertyAttributes)
            {
                var propName = kvp.Key;
                var value = kvp.Value;

                if (IsBindingExpression(value))
                {
                    var binding = BindingExpression.Parse(value, dataType);
                    if (binding == null)
                    {
                        diagnostics.Add(TuiDiagnostics.InvalidBinding.Create(filePath, value));
                        continue;
                    }

                    // Validate TwoWay binding is only used on supported controls/properties
                    if (binding.Mode == BindingMode.TwoWay)
                    {
                        if (!Mappings.SupportsTwoWayBinding(element.Name, propName))
                        {
                            diagnostics.Add(TuiDiagnostics.UnsupportedTwoWayBinding.Create(
                                filePath, element.Name, propName));
                        }
                    }
                }
            }

            // Validate event handlers
            foreach (var kvp in element.EventAttributes)
            {
                var eventName = kvp.Key;
                var handlerName = kvp.Value;

                if (!IsKnownEventName(element.Name, eventName))
                {
                    diagnostics.Add(TuiDiagnostics.UnknownEvent.Create(filePath, element.Name, eventName));
                }
                // Note: Obsolete event diagnostic is emitted during code generation in Generator.cs

                if (string.IsNullOrWhiteSpace(handlerName))
                {
                    diagnostics.Add(TuiDiagnostics.EmptyEventHandler.Create(filePath, eventName));
                }
            }

            // Recursively validate children
            foreach (var child in element.Children)
            {
                ValidateElement(child, filePath, diagnostics, dataType);
            }
        }

        /// <summary>
        /// Checks if a value is a binding expression
        /// </summary>
        private static bool IsBindingExpression(string value)
        {
            var trimmed = value.Trim();
            return trimmed.StartsWith("{Bind ") && trimmed.EndsWith("}");
        }

        /// <summary>
        /// Checks if a control type is known/supported
        /// </summary>
        private static bool IsKnownControlType(string controlName) =>
            Mappings.ControlMappings.ContainsKey(controlName);

        /// <summary>
        /// Checks if an event name is known for a control type
        /// </summary>
        private static bool IsKnownEventName(string controlName, string eventName) =>
            Mappings.EventMappings.TryGetValue(controlName, out var events) && 
            events.ContainsKey(eventName);
    }

    /// <summary>
    /// Result of a parsing operation
    /// </summary>
    public class ParseResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public TuiDiagnostic? Error { get; private set; }

        private ParseResult() { }

        public static ParseResult<T> Success(T value) => new()
        {
            IsSuccess = true,
            Value = value
        };

        public static ParseResult<T> CreateError(DiagnosticDescriptor descriptor, string filePath, string? message = null) => new()
        {
            IsSuccess = false,
            Error = TuiDiagnostic.Create(descriptor, filePath, message)
        };
    }

    /// <summary>
    /// Represents a diagnostic message from the parser
    /// </summary>
    public class TuiDiagnostic
    {
        public DiagnosticDescriptor Descriptor { get; }
        public string FilePath { get; }
        public string? Message { get; }

        public TuiDiagnostic(DiagnosticDescriptor descriptor, string filePath, string? message = null)
        {
            Descriptor = descriptor;
            FilePath = filePath;
            Message = message;
        }

        public static TuiDiagnostic Create(DiagnosticDescriptor descriptor, string filePath, string? message = null) =>
            new(descriptor, filePath, message);

        public Diagnostic ToDiagnostic() =>
            Diagnostic.Create(Descriptor, Location.None, Message ?? Descriptor.MessageFormat);
    }
}