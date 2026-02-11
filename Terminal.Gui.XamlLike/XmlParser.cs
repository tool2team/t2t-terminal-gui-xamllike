using System;
using System.Collections.Generic;
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
                var doc = new XmlDocument();
                doc.LoadXml(content);

                if (doc.DocumentElement == null)
                {
                    return ParseResult<XamlDocument>.CreateError(
                        TuiDiagnostics.InvalidXml,
                        filePath,
                        "No root element found");
                }

                var rootElement = ParseElement(doc.DocumentElement);
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
        /// Parses an XML element into a XamlElement
        /// </summary>
        private static XamlElement ParseElement(XmlElement xmlElement)
        {
            var attributes = new Dictionary<string, string>();
            var children = new List<XamlElement>();

            // Parse attributes
            foreach (XmlAttribute attr in xmlElement.Attributes)
            {
                attributes[attr.Name] = attr.Value;
            }

            // Parse child elements
            foreach (XmlNode child in xmlElement.ChildNodes)
            {
                if (child is XmlElement childElement)
                {
                    children.Add(ParseElement(childElement));
                }
            }

            return new XamlElement(xmlElement.Name, attributes, children);
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
            var controlType = Mappings.GetControlTypeName(element.Name);
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
        private static bool IsKnownControlType(string controlName) => controlName switch
        {
            "Window" => true,
            "Label" => true,
            "Button" => true,
            "TextField" => true,
            "TextView" => true,
            "CheckBox" => true,
            "OptionSelector" => true,
            "ListView" => true,
            "FrameView" => true,
            "ScrollView" => true,
            "TabView" => true,
            "MenuBar" => true,
            "StatusBar" => true,
            _ => false
        };

        /// <summary>
        /// Checks if an event name is known for a control type
        /// </summary>
        private static bool IsKnownEventName(string controlName, string eventName) => controlName switch
        {
            "Button" => eventName == "Accepting",
            "TextField" => eventName == "TextChanged" || eventName == "Accept",
            "CheckBox" => eventName == "Toggled",
            "OptionSelector" => eventName == "SelectedItemChanged",
            "ListView" => eventName == "SelectedItemChanged" || eventName == "OpenSelectedItem",
            "Window" => eventName == "Loaded" || eventName == "Closing",
            _ => false
        };
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

        public static ParseResult<T> Success(T value) => new ParseResult<T>
        {
            IsSuccess = true,
            Value = value
        };

        public static ParseResult<T> CreateError(DiagnosticDescriptor descriptor, string filePath, string? message = null) => new ParseResult<T>
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
            new TuiDiagnostic(descriptor, filePath, message);

        public Diagnostic ToDiagnostic() =>
            Diagnostic.Create(Descriptor, Location.None, Message ?? Descriptor.MessageFormat);
    }
}