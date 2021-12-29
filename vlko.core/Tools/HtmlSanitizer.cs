using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace vlko.core.Tools
{
    public static class HtmlSanitizer
    {
        private static readonly IDictionary<string, string[]> WhiteListExtended;
        private static readonly IDictionary<string, string[]> WhiteList;
        private static readonly string[] LineBreakNodes;
        private static Regex SecureSpecialEntitiesRegex = new Regex("&(nbsp|gt|lt|quot|#38|#62|#60|#34);", RegexOptions.Compiled);
        private static Regex RecoverSpecialEntitiesRegex = new Regex("#(nbsp|gt|lt|quot|#38|#62|#60|#34)#", RegexOptions.Compiled);

        static HtmlSanitizer()
        {
            // class attribute is enabled by default
            WhiteListExtended = new Dictionary<string, string[]> {
                { "a", new[] { "href" } },
                { "strong", null },
                { "em", null },
                { "blockquote", null },
                { "b", null},
                { "p", null},
                { "pre", null},
                { "span", null},
                { "br", null},
                { "ul", null},
                { "ol", null},
                { "li", null},
                { "div", new[] { "align" } },
                { "strike", null},
                { "u", null},
                { "sub", null},
                { "sup", null},
                { "table", null },
                { "tr", null },
                { "td", null },
                { "th", null }
                };
            WhiteList = new Dictionary<string, string[]> {
                { "p", null},
                { "pre", null},
                { "span", null},
                { "br", null},
                { "ul", null},
                { "ol", null},
                { "li", null},
                { "div", new[] { "align" } },
                { "table", null },
                { "tr", null },
                { "td", null },
                { "th", null }
                };
            LineBreakNodes = new[] { "p", "br" };
        }

        public static string Sanitize(string input, bool extendeHtml = false)
        {
            if (string.IsNullOrEmpty(input) || input.Trim().Length < 1)
                return string.Empty;
            var htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.OptionAutoCloseOnEnd = true;
            htmlDocument.OptionWriteEmptyNodes = true;
            htmlDocument.OptionDefaultStreamEncoding = Encoding.UTF8;
            htmlDocument.OptionFixNestedTags = true;

            htmlDocument.LoadHtml(input.Replace("\t", "  ").ReduceSpaces());
            RemoveEmptyTextNodes(htmlDocument);
            PrepareLineBreakNodes(htmlDocument);
            SanitizeNode(htmlDocument.DocumentNode, extendeHtml ? WhiteListExtended : WhiteList);
            StripHtml(htmlDocument, "//removeablenode");

            return
                RecoverSpecialEntitiesRegex.Replace(
                    HtmlEntity.DeEntitize(
                        SecureSpecialEntitiesRegex.Replace(htmlDocument.DocumentNode.OuterHtml, "#$1#")
                    ), "&$1;")
                .Trim();
        }

        private static void RemoveEmptyTextNodes(HtmlDocument document)
        {
            // Remove script & style nodes
            document.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style").ToList().ForEach(n => n.Remove());

            // remove empty text nodes
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//text()[normalize-space(.) = '']") ?? new HtmlNodeCollection(document.DocumentNode))
            {
                node.Remove();
            }

            // remove empty nodes at the end
            while (document.DocumentNode.LastChild != null && string.IsNullOrWhiteSpace(document.DocumentNode.LastChild.InnerText))
            {
                document.DocumentNode.LastChild.Remove();
            }
        }

        private static void PrepareLineBreakNodes(HtmlDocument document)
        {
            foreach (var lineBreakNode in LineBreakNodes)
            {
                var nodes = document.DocumentNode.SelectNodes("//" + lineBreakNode);
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        if (node.NextSibling != null)
                        {
                            node.ParentNode.InsertAfter(document.CreateTextNode("\r\n"), node);
                        }
                    }
                }
            }
        }
        private static void SanitizeChildren(HtmlNode parentNode, IDictionary<string, string[]> whiteList)
        {
            for (int i = parentNode.ChildNodes.Count - 1; i >= 0; i--)
            {
                SanitizeNode(parentNode.ChildNodes[i], whiteList);
            }
        }

        private static void SanitizeNode(HtmlNode node, IDictionary<string, string[]> whiteList)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                var textNode = node as HtmlTextNode;
                if (!string.IsNullOrWhiteSpace(textNode.Text))
                {
                    textNode.Text = textNode.Text.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
                    var specialCharacters = SecureSpecialEntitiesRegex.IsMatch(textNode.Text);
                    if (specialCharacters)
                    {
                        textNode.Text = RecoverSpecialEntitiesRegex.Replace(
                                HtmlEntity.DeEntitize(SecureSpecialEntitiesRegex.Replace(textNode.Text, "#$1#")
                            ), "&$1;");
                    }
                    else
                    {
                        textNode.Text = HtmlEntity.DeEntitize(textNode.Text);
                    }
                }
            }
            else if (node.NodeType == HtmlNodeType.Element)
            {
                if (!whiteList.ContainsKey(node.Name))
                {
                    node.Name = "removeablenode";
                    if (node.HasChildNodes)
                    {
                        SanitizeChildren(node, whiteList);
                    }

                    return;
                }

                if (node.HasAttributes)
                {
                    for (int i = node.Attributes.Count - 1; i >= 0; i--)
                    {
                        HtmlAttribute currentAttribute = node.Attributes[i];
                        if (currentAttribute.Name == "class" && !string.IsNullOrEmpty(currentAttribute.Value))
                        {
                            currentAttribute.Value = string.Join(" ", currentAttribute.Value.Split(new[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries).Select(x => "x_" + x));
                        }
                        else
                        {
                            string[] allowedAttributes = whiteList[node.Name];
                            if (allowedAttributes != null)
                            {
                                if (!allowedAttributes.Contains(currentAttribute.Name))
                                {
                                    node.Attributes.Remove(currentAttribute);
                                }
                            }
                            else
                            {
                                node.Attributes.Remove(currentAttribute);
                            }
                        }
                    }
                }
            }

            if (node.HasChildNodes)
            {
                SanitizeChildren(node, whiteList);
            }
        }

        private static void StripHtml(HtmlDocument htmlDoc, string xPath)
        {
            if (xPath.Length > 0)
            {
                HtmlNodeCollection invalidNodes = htmlDoc.DocumentNode.SelectNodes(@xPath) ?? new HtmlNodeCollection(null);
                foreach (HtmlNode node in invalidNodes)
                {
                    node.ParentNode.RemoveChild(node, true);
                }
            }
        }

    }
}
