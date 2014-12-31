using System;
using System.Collections.Generic;
using System.Text;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.TranslationManager")]
    public class StringEntry {
        public string Comment { get; set; }
        public string Usage { get; set; }
        public string Scope { get; set; }
        public string Context { get; set; }
        public string Id { get; set; }
        public string Translation { get; set; }
        public bool Used { get; set; }

        public string UniqueKey {
            get {
                //(msgid,msgctx) is the unique key for the dictionary, 
                //if msgctx is missing you can consider it null
                //http://www.gnu.org/software/gettext/manual/gettext.html#PO-Files
                return (Id + "|" + (Context ?? string.Empty)).ToLowerInvariant(); //case insensitive
            }
        }

        public static StringEntry Parse(string s) {
            var lines = s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var stringEntry = new StringEntry
            {
                Id = @"msgid """"",
                Translation = @"msgstr """""
            };
            foreach (var currentLine in lines) {
                var line = currentLine.Trim();
                if (line.StartsWith("#: ")) {
                    stringEntry.Scope = line;
                }
                if (line.StartsWith("msgctxt ")) {
                    stringEntry.Context = line;
                    //more robust external editor support, editors may add extra " around context
                    stringEntry.Context = stringEntry.Context.Replace("\"", "");
                }
                else if (line.StartsWith("# ")) {
                    if (string.IsNullOrEmpty(stringEntry.Comment))
                        stringEntry.Comment = line;
                    else
                        stringEntry.Comment += Environment.NewLine + line;
                }
                else if (line.StartsWith("#. ")) {
                    if (string.IsNullOrEmpty(stringEntry.Usage))
                        stringEntry.Usage = line;
                    else
                        stringEntry.Usage += Environment.NewLine + line;
                }
                else if (line.StartsWith("msgid ")) {
                    stringEntry.Id = line;
                }
                else if (line.StartsWith("msgstr ")) {
                    stringEntry.Translation = line;
                }
                else if (line.StartsWith("\"") && line.EndsWith("\"")) {
                    //add support for parsing message on multiple lines
                    stringEntry.Translation = string.Format(@"{0}""{1}""", stringEntry.Translation.TrimEnd('\"'), line.TrimStart('\"').TrimEnd('\"'));
                }
            }
            return stringEntry;
        }

        public override string ToString() {
            if (Id == @"msgid """"")
                return string.Empty;

            var builder = new StringBuilder();

            var scope = Scope;
            if (string.IsNullOrEmpty(Scope) && !string.IsNullOrEmpty(Context))
                scope = Context.Replace("msgctxt ", "#: ");
            var context = Context;
            if (string.IsNullOrEmpty(Context) && !string.IsNullOrEmpty(Scope))
                context = Context.Replace("#: ", "msgctxt ");

            if (!string.IsNullOrEmpty(Comment))
                builder.AppendLine(Comment);
            builder.AppendLine(scope);
            if (!string.IsNullOrEmpty(Usage))
                builder.AppendLine(Usage);
            builder.AppendLine(context);
            builder.AppendLine(Id);
            builder.AppendLine(Translation);
            builder.AppendLine();

            return builder.ToString();
        }
    }

    [OrchardFeature("Vandelay.TranslationManager")]
    public class StringEntryEqualityComparer : IEqualityComparer<StringEntry> {
        public bool Equals(StringEntry x, StringEntry y) {
            if (x == null) return y == null;
            if (y == null) return false;
            return x.UniqueKey == y.UniqueKey;
        }

        public int GetHashCode(StringEntry obj) {
            return obj.UniqueKey.GetHashCode() ^ obj.UniqueKey.GetHashCode();
        }
    }

    [OrchardFeature("Vandelay.TranslationManager")]
    public class StringEntryBuilder : Dictionary<string, StringEntry> {
        public bool ContainsKey(StringEntry translation) {
            return ContainsKey(translation.UniqueKey);
        }

        public void Add(StringEntry translation) {
            Add(translation.UniqueKey, translation);
        }

        public override string ToString() {
            var builder = new StringBuilder();

            foreach (var value in Values) {
                builder.Append(value);
            }

            return builder.ToString();
        }
    }
}
