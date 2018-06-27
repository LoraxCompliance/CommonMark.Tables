using CommonMark.Syntax;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonMark.Tables
{
    public class TableFormatter : Formatters.HtmlFormatter
    {
        // 
        private readonly Regex tableRowRegex = new Regex(@"([^\|#]*)(\|[^\|#]*)+");
        private readonly Regex tableDividerRegex = new Regex(@"-+\|-+");


        // State detection
        private BlockTag? LastSeenBlockTag = null;
        private bool tableStarted = false;
        private bool tableDividerPresent = false;
        private bool tableEnded = false;


        public TableFormatter(TextWriter target, CommonMarkSettings settings) : base(target, settings) { }


        protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            LastSeenBlockTag = block.Tag;
            base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            if (LastSeenBlockTag != BlockTag.Paragraph)
            {
                base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
                return;
            }

            if (inline.Tag == InlineTag.String)
            {
                string str = inline.LiteralContent?.Trim();
                ignoreChildNodes = false;

                if (!tableStarted && tableRowRegex.IsMatch(str))
                {
                    // Table header
                    tableStarted = true;
                    tableDividerPresent = false;
                    tableEnded = false;
                    Write("<table>");

                    Write($"<tr>{string.Join("\n", str.Split('|').Select(s => $"<th>{s}</th>"))}</tr>");
                    return;
                }
                else if (!tableDividerPresent && tableDividerRegex.IsMatch(str))
                {
                    // Table divider
                    tableDividerPresent = true;
                    return;
                }
                else if (!tableEnded && tableRowRegex.IsMatch(str))
                {
                    // Table data rows
                    Write($"<tr>{string.Join("\n", str.Split('|').Select(s => $"<td>{s}</td>"))}</tr>");

                    if (inline.NextSibling == null)
                    {
                        tableStarted = false;
                        tableEnded = true;
                        Write("</table>");
                    }

                    return;
                }
            }

            // Fall through: defer to regular formatter
            base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
        }
    }
}
