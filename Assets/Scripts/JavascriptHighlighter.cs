using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InGameTextEditor.Format {
    public class CSharpSyntaxHighlighter : TextFormatter {
        public TextStyle textStyleComment = new TextStyle(new Color(0.5f, 0.5f, 0.5f));
        public TextStyle textStyleString = new TextStyle(new Color(0.9f, 0.4f, 0.1f));
        public TextStyle textStyleNumber = new TextStyle(new Color(0.2f, 0.4f, 0.6f));
        public TextStyle textStyleKeyword = new TextStyle(new Color(0.2f, 0.6f, 0.6f));

        readonly string[] keywords = {"abstract", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue", "debugger", "default", "delete", "do", "double", "else", "enum", "export", "extends", "false", "final", "finally", "float", "for", "function", "goto", "if", "implements", "import", "in", "instanceof", "int", "interface", "long", "native", "new", "null", "package", "private", "protected", "public", "return", "short", "static", "super", "switch", "synchronized", "this", "throw", "throws", "transient", "true", "try", "typeof", "var", "void", "volatile", "while", "with"};

        bool initialized = false;

        Regex regex = null;

        public override bool Initialized { get { return initialized; } }

        public override void Init() {
            string regexPattern = "";

            // matches the end of a multi-line comment: ...*/
            // use mark u001B + 0 to indicate that this line starts with an open multi-line comment)
            regexPattern += @"(?<multiLineCommentEnding>^\u001B0.*?\*/)";

            // matches the start of a multi-line comment: /*...
            regexPattern += @"|(?<multiLineCommentStarting>/\*((?!\*/).)*$)";

            // matches a comment with /* and */
            regexPattern += @"|(?<comment>/\*((?!\*/).)*\*/)";

            // matches a line comment: //...
            regexPattern += @"|(?<comment>//.*$?)";

            // matches a string within double quotes: "..."
            regexPattern += @"|(?<string>""(?:[^""\\]|\\.)*[""]?)";

            // matches a string within single quotes: '...'
            regexPattern += @"|(?<stringSingle>'(?:[^'\\]|\\.)*[']?)";

            // matches a template string: `...`
            regexPattern += @"|(?<stringTpl>`(?:[^`\\]|\\.)*[`]?)";

            // matches a floating point number
            regexPattern += @"|(?<floatImplicit>\b[0-9]*\.[0-9]+[dfDF]?)";

            // matches an integer number
            regexPattern += @"|(?<intImplicit>\b[0-9]+[lL]?)";

            // matches a JS keyword
            regexPattern += @"|(?<keyword>";
            for (int i = 0; i < keywords.Length; i++) {
                regexPattern += "\\b" + keywords[i] + "\\b" + (i < keywords.Length - 1 ? "|" : "");
            }
            regexPattern += ")";

            // create regex
            regex = new Regex(regexPattern);

            // initialization complete
            initialized = true;
        }

        public override void OnLineChanged(Line line) {
            List<TextFormatGroup> textFormatGroups = new List<TextFormatGroup>();

            bool lineStartsWithMultiLineComment = line.PreviousLine != null && line.PreviousLine.GetProperty<bool>("endsWithMultiLineComment", false);
            bool endsExistingMultiLineComment = false;
            bool startsNewMultiLineComment = false;

            if (line.Text.Length > 0) {
                MatchCollection matches = null;

                if (lineStartsWithMultiLineComment) {
                    // add special mark to indicate that the line starts with an open multi-line comment
                    // escape character (unicode 001B) followed by 0
                    matches = regex.Matches('\u001B' + "0" + line.Text);
                }
                else {
                    matches = regex.Matches(line.Text);
                }

                foreach (Match match in matches) {
                    int i = 0;
                    foreach (Group group in match.Groups) {
                        if (group.Success && i > 0) {
                            string groupName = regex.GroupNameFromNumber(i);
                            int tokenStartIndex = group.Index;
                            int tokenEndIndex = tokenStartIndex + group.Value.Length - 1;

                            // add corresponding text format group
                            switch (groupName)
                            {
                                case "multiLineCommentEnding":
                                    endsExistingMultiLineComment = true;
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex + 2, tokenEndIndex, textStyleComment));
                                    break;
                                case "comment":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleComment));
                                    break;
                                case "string":
                                case "stringSingle":
                                case "stringTpl":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleString));
                                    break;
                                case "floatImplicit":
                                case "intImplicit":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleNumber));
                                    break;
                                case "keyword":
                                    textFormatGroups.Add(new TextFormatGroup(tokenStartIndex, tokenEndIndex, textStyleKeyword));
                                    break;
                            }
                        }

                        i++;
                    }
                }

                // correct offset introduced by multiline escape sequence
                if (lineStartsWithMultiLineComment) {
                    foreach (TextFormatGroup textFormatGroup in textFormatGroups) {
                        textFormatGroup.startIndex -= 2;
                        textFormatGroup.endIndex -= 2;
                    }
                }
            }

            // set property indicating if the current line ends with a multi-line comment
            if (lineStartsWithMultiLineComment && !endsExistingMultiLineComment) {
                textFormatGroups.Clear();
                if (line.Text.Length > 0) {
                    textFormatGroups.Add(new TextFormatGroup(0, line.Text.Length - 1, textStyleComment));
                }
                line.SetProperty<bool>("endsWithMultiLineComment", true);
            }
            else {
                line.SetProperty<bool>("endsWithMultiLineComment", startsNewMultiLineComment);
            }

            // apply format to line
            line.ApplyTextFormat(textFormatGroups);
        }
    }
}
