using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fluent.Zip;
using Orchard;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;
using Path = Fluent.IO.Path;

namespace Vandelay.Industries.Services {
    public interface ILocalizationManagementService : IDependency {
        void InstallTranslation(byte[] zippedTranslation, string sitePath);
        byte[] PackageTranslations(string cultureCode, string sitePath);
        byte[] PackageTranslations(string cultureCode, string sitePath, IEnumerable<string> extensionNames);
        byte[] ExtractDefaultTranslation(string sitePath);
        byte[] ExtractDefaultTranslation(string sitePath, IEnumerable<string> extensionNames);
        void SyncTranslation(string sitePath, string cultureCode);
    }

    [OrchardFeature("Vandelay.TranslationManager")]
    public class LocalizationManagementService : ILocalizationManagementService {
        private static readonly Regex NamespaceExpression =
            new Regex(
                @"namespace ([^\s]*)\s*{",
                RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex ClassExpression =
            new Regex(
                @"\s?class ([^\s:]*)[\s:]",
                RegexOptions.Multiline | RegexOptions.Compiled);

        public void InstallTranslation(byte[] zippedTranslation, string sitePath) {
            var siteRoot = Path.Get(sitePath);
            ZipExtensions.Unzip(zippedTranslation, 
                (path, contents) => {
                    if (path.Extension == ".po") {
                        var tokens = path.Tokens;
                        var destPath = siteRoot.Combine(tokens);
                        // If a translation file is for a module or a theme, only install it
                        // if said module or theme exists already. Otherwise, skip.
                        if ((!tokens[0].Equals("modules", StringComparison.OrdinalIgnoreCase) &&
                             !tokens[0].Equals("themes", StringComparison.OrdinalIgnoreCase)) ||
                              siteRoot.Combine(tokens[0], tokens[1]).IsDirectory) {
                            destPath.Write(contents);
                        }
                    }
                });
        }

        public byte[] PackageTranslations(string cultureCode, string sitePath) {
            var site = Path.Get(sitePath);
            var translationFiles = site
                .Files("orchard.*.po", true)
                .Where(p => p.Parent().FileName.Equals(cultureCode, StringComparison.OrdinalIgnoreCase))
                .MakeRelativeTo(site);
            return ZipExtensions.Zip(
                translationFiles,
                p => site.Combine(p).ReadBytes());
        }

        public byte[] PackageTranslations(string cultureCode, string sitePath, IEnumerable<string> extensionNames) {
            var site = Path.Get(sitePath);
            var translationFiles = site
                .Files("orchard.*.po", true)
                .Where(p =>
                       extensionNames != null &&
                       (p.Parent().FileName.Equals(cultureCode, StringComparison.OrdinalIgnoreCase) &&
                        (extensionNames.Contains(p.MakeRelativeTo(site.Combine("Modules")).Tokens[0], StringComparer.OrdinalIgnoreCase) ||
                         extensionNames.Contains(p.MakeRelativeTo(site.Combine("Themes")).Tokens[0], StringComparer.OrdinalIgnoreCase))))
                .MakeRelativeTo(site);
            return ZipExtensions.Zip(
                translationFiles,
                p => site.Combine(p).ReadBytes());
        }

        public byte[] ExtractDefaultTranslation(string sitePath, IEnumerable<string> extensionNames) {
            if (extensionNames == null || !extensionNames.Any()) {
                return ExtractDefaultTranslation(sitePath);
            }
            var site = Path.Get(sitePath);
            var zipFiles = new Dictionary<Path, StringEntryBuilder>();
            // Extract resources for module manifests
            site.Files("module.txt", true)
                .Where(p => extensionNames.Contains(p.Parent().FileName, StringComparer.OrdinalIgnoreCase))
                .Read((content, path) => {
                    var moduleName = path.Parent().FileName;
                    var poPath = GetModuleLocalizationPath(site, moduleName);
                    ExtractPoFromManifest(zipFiles, poPath, content, path, site);
                });
            // Extract resources for theme manifests
            site.Files("theme.txt", true)
                .Where(p => extensionNames.Contains(p.Parent().FileName, StringComparer.OrdinalIgnoreCase))
                .Read((content, path) => {
                    var themeName = path.Parent().FileName;
                    var poPath = GetThemeLocalizationPath(site, themeName);
                    ExtractPoFromManifest(zipFiles, poPath, content, path, site);
                });
            // Extract resources from views and cs files
            site.Files("*", true)
                .WhereExtensionIs(".cshtml", ".aspx", ".ascx", ".cs")
                .Where(p => {
                           var tokens = p.MakeRelativeTo(site).Tokens;
                           return new[] {"themes", "modules"}.Contains(tokens[0], StringComparer.OrdinalIgnoreCase) &&
                                  extensionNames.Contains(tokens[1], StringComparer.OrdinalIgnoreCase);
                       })
                .Read((contents, path) => ExtractResourcesFromCode(contents, null, null, site, path, zipFiles));
            return ZipExtensions.Zip(
                new Path(zipFiles.Keys.Select(p => p.MakeRelativeTo(site))),
                p => Encoding.UTF8.GetBytes(zipFiles[site.Combine(p)].ToString()));
        }

        private void ExtractResourcesFromCode(string contents, Path corePoPath, Path rootPoPath, Path site, Path path, Dictionary<Path, StringEntryBuilder> zipFiles) {
            foreach (var str in FindLocalizedStrings(contents)) {
                DispatchResourceString(zipFiles, corePoPath, rootPoPath, site, path, site, contents, '"' + str + '"');
            }
            foreach (var str in FindLocalizedStrings(contents, "T.Plural(", true)) {
                DispatchResourceString(zipFiles, corePoPath, rootPoPath, site, path, site, contents, '"' + str + '"');
            }
        }

        public byte[] ExtractDefaultTranslation(string sitePath) {
            var site = Path.Get(sitePath);
            var corePoPath = site.Combine(
                "Core", "App_Data",
                "Localization", "en-US",
                "orchard.core.po");
            var rootPoPath = site.Combine(
                "App_Data", "Localization", "en-US",
                "orchard.root.po");
            var zipFiles = new Dictionary<Path, StringEntryBuilder>();
            // Extract resources for module manifests
            site.Files("module.txt", true)
                .Read((content, path) => {
                    var moduleName = path.Parent().FileName;
                    var poPath =
                        path.MakeRelativeTo(sitePath).Tokens[0].Equals("core", StringComparison.OrdinalIgnoreCase) ?
                             corePoPath : GetModuleLocalizationPath(site, moduleName);
                    ExtractPoFromManifest(zipFiles, poPath, content, path, site);
                });
            // Extract resources for theme manifests
            site.Files("theme.txt", true)
                .Read((content, path) => {
                    var themeName = path.Parent().FileName;
                    var poPath = GetThemeLocalizationPath(site, themeName);
                    ExtractPoFromManifest(zipFiles, poPath, content, path, site);
                });
            // Extract resources from views and cs files, for the web site
            // as well as for the framework and Azure projects.
            site/*.Add(site.Parent().Combine("Orchard"))
                .Add(site.Parent().Combine("Orchard.Azure"))*/
                .ForEach(p =>
                         p.Files("*", true)
                             .WhereExtensionIs(".cshtml", ".aspx", ".ascx", ".cs")
                             .Read((contents, path) => ExtractResourcesFromCode(contents, corePoPath, rootPoPath, site, path, zipFiles)));
            return ZipExtensions.Zip(
                new Path(zipFiles.Keys.Select(p => p.MakeRelativeTo(site))),
                p => Encoding.UTF8.GetBytes(zipFiles[site.Combine(p)].ToString()));
        }

        public void SyncTranslation(string sitePath, string cultureCode) {
            Path.Get(sitePath)
                .Files("orchard.*.po", true)
                .Where(p => p.Parent().FileName.Equals("en-US", StringComparison.OrdinalIgnoreCase))
                .ForEach(baselinePath => {
                             var path = baselinePath.Parent().Parent().Combine(cultureCode, baselinePath.FileName);
                             var translations = new List<StringEntry>();
                             if (path.Exists) {
                                 path.Open(inStream => ReadTranslations(inStream, translations), FileMode.Open, FileAccess.Read, FileShare.Read);
                             }
                             path.Parent().CreateDirectory();
                             path.Open(outStream => {
                                           var englishTranslations = new List<StringEntry>();
                                           baselinePath.Open(baselineStream => ReadTranslations(baselineStream, englishTranslations), FileMode.Open, FileAccess.Read, FileShare.Read);
                                           using (var writer = new StreamWriter(outStream)) {
                                               foreach (var englishTranslation in englishTranslations) {
                                                   var entry = englishTranslation;
                                var translation = translations.FirstOrDefault(t => t.UniqueKey == entry.UniqueKey);
                                if (translation == null) {
                                    translation = StringEntry.Parse(entry.ToString());
                                    translation.Translation = @"msgstr """"";
                                                   }
                                                   else {
                                    if (translation.Comment != null && translation.Comment.Contains("# Untranslated string"))
                                        translation.Comment = translation.Comment.Replace("# Untranslated string", string.Empty); //remove previous untraslated comment if any
                                    translation.Used = true;
                                }
                                translation.Scope = entry.Scope; //fix case
                                translation.Context = entry.Context; //fix case
                                if (translation.Translation.Equals(@"msgstr """"")) {
                                    var findAny = translations.Where(t => t.Id == entry.Id);
                                    var find = findAny.FirstOrDefault(any => any.Translation != @"msgstr """"" && any.Translation != translation.Translation);
                                    if (find != null) {
                                        //picks new scopes from translated one and states it on comment
                                        translation.Translation = find.Translation;
                                        translation.Comment = "# Picked from: " + find.Scope;
                                                   }
                                    else
                                        translation.Comment = "# Untranslated string";
                                }
                                writer.WriteLine(translation);
                                               }
                                               foreach (var translation in translations.Where(t => !t.Used)) {
                                translation.Comment = "# Obsolete translation";
                                writer.WriteLine(translation);
                                               }
                                           }
                                       }, FileMode.Create, FileAccess.Write, FileShare.None);
                         });
        }

        private static void ReadTranslations(FileStream inStream, List<StringEntry> translations) {
            var currentEntry = new StringBuilder();
            var comparer = new StringEntryEqualityComparer();
            using (var reader = new StreamReader(inStream)) {
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line)) {
                        currentEntry.AppendLine(line);
                        }
                    else {
                        var translation = StringEntry.Parse(currentEntry.ToString());
                            if (!translations.Contains(translation, comparer)) {
                                translations.Add(translation);
                            }
                        currentEntry = new StringBuilder();
                    }
                }
            }
        }

        private static StringEntryBuilder GetBuilder(IDictionary<Path, StringEntryBuilder> fileCatalog, Path path) {
            StringEntryBuilder entry;
            if (!fileCatalog.ContainsKey(path)) {
                entry = new StringEntryBuilder();
                fileCatalog.Add(path, entry);
            }
            else {
                entry = fileCatalog[path];
            }
            return entry;
        }

        private static void DispatchResourceString(
            IDictionary<Path, StringEntryBuilder> fileCatalog,
            Path corePoPath,
            Path rootPoPath,
            Path sitePath,
            Path path,
            Path currentInputPath,
            string contents, string str) {

            var current = "~/" + path.MakeRelativeTo(currentInputPath).ToString().Replace('\\', '/');
            var context = current;
            if (path.Extension == ".cs") {
                var ns = NamespaceExpression.Match(contents).Groups[1].ToString();
                var type = ClassExpression.Match(contents).Groups[1].ToString();
                context = ns + "." + type;
            }
            Path targetPath;
            if (current.StartsWith("~/core/", StringComparison.OrdinalIgnoreCase)) {
                targetPath = corePoPath;
            }
            else if (current.StartsWith("~/themes/", StringComparison.OrdinalIgnoreCase)) {
                targetPath = GetThemeLocalizationPath(sitePath, current.Substring(9, current.IndexOf('/', 9) - 9));
            }
            else if (current.StartsWith("~/modules/", StringComparison.OrdinalIgnoreCase)) {
                targetPath = GetModuleLocalizationPath(sitePath, current.Substring(10, current.IndexOf('/', 10) - 10));
            }
            else {
                targetPath = rootPoPath;
            }
            WriteResourceString(GetBuilder(fileCatalog, targetPath), context, str);
        }

        private static readonly Regex FeatureNameExpression = new Regex(@"^\s+([^\s:]+):\s*$");

        private static void ExtractPoFromManifest(
            IDictionary<Path, StringEntryBuilder> fileCatalog,
            Path poPath,
            string manifest,
            Path manifestPath,
            Path rootPath) {

            var context = "~/" + manifestPath.MakeRelativeTo(rootPath).ToString()
                                     .Replace('\\', '/');
            var reader = new StringReader(manifest);
            var builder = GetBuilder(fileCatalog, poPath);
            string line;
            while ((line = reader.ReadLine()) != null) {
                var split = line.Split(new[] {':'}, 2, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToArray();
                if (split.Length == 2) {
                    var comment = split[0];
                    if (new[] { "Name", "Description", "Author", "Website", "Tags" }.Contains(comment)) {
                        var value = split[1];
                        WriteResourceString(
                            builder,
                            context,
                            '"' + comment + '"',
                            '"' + value + '"');
                    }
                }
                if (line.StartsWith("Features:")) {
                    var feature = "";
                    while ((line = reader.ReadLine()) != null) {
                        var match = FeatureNameExpression.Match(line);
                        if (match.Success) {
                            feature = match.Groups[1].Value;
                            continue;
                        }
                        split = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim()).ToArray();
                        if (split.Length != 2) continue;
                        var comment = split[0];
                        if (new[] { "Name", "Description", "Category" }.Contains(comment)) {
                            var value = split[1];
                            WriteResourceString(
                                builder,
                                context,
                                '"' + feature + '.' + comment + "\"",
                                '"' + value + '"');
                        }
                    }
                }
            }
        }

        private static Path GetThemeLocalizationPath(Path siteRoot, string themeName) {
            return siteRoot.Combine(
                "Themes", themeName, "App_Data",
                "Localization", "en-US",
                "orchard.theme.po");
        }

        private static Path GetModuleLocalizationPath(Path siteRoot, string moduleName) {
            return siteRoot.Combine(
                "Modules", moduleName, "App_Data",
                "Localization", "en-US",
                "orchard.module.po");
        }

        private static bool WriteResourceString(StringEntryBuilder builder, string context, string value) {
            return WriteResourceString(builder, context, value, value);
        }

        private static bool WriteResourceString(StringEntryBuilder builder, string context, string comment, string value) {
            var translation = new StringEntry()
            {
                Usage = !string.IsNullOrEmpty(comment) && comment != value ?
                    "#. " + comment : string.Empty,
                Context = "msgctxt " + context,
                Id = "msgid " + value,
                Translation = "msgstr " + value
            };

            if (!builder.ContainsKey(translation)) {
                builder.Add(translation);
                return true;
            }
            translation = builder[translation.UniqueKey];
            var newComment = "#. " + comment;
            if (!translation.Usage.Contains(newComment))
                translation.Usage += "\r\n" + newComment;
            return false;
        }

        public static IEnumerable<string> FindLocalizedStrings(string csharp, string prefix = "T(", bool fetchTwoStrings = false) {
            // This will fail on some carefully crafted comments such as /* T("ha ha! */. Well, yeah.
            var current = 0;
            var gotOne = false;
            while (current >=0 && current < csharp.Length) {
                var nextT = current;
                if (!fetchTwoStrings || !gotOne) {
                    // Look for next prefix.
                    nextT = csharp.IndexOf(prefix, current);
                    if (nextT == -1) yield break; // prefix not found, we're done.
                    nextT += prefix.Length; // skip prefix.
                }
                var quote = csharp.IndexOf('"', nextT); // Where's the next quote?
                if (quote == -1) yield break; // None found, we're done.
                var between = csharp.Substring(nextT, quote - nextT).Replace(" ", "");
                if (between != "" && between != "@" && !(fetchTwoStrings && gotOne && (between == "," || between == ",@"))) {
                    // stuff found between prefix and string, skip this to next prefix.
                    current = nextT + 1;
                    continue;
                }
                nextT = quote;
                // We should be in position to find the string, and on the opening quote.
                if (csharp[nextT - 1] == '@'
                    && csharp.Length > nextT + 1) { // @"..." string.
                    var nextQuote = nextT + 1;
                    while (nextQuote >= 0 && nextQuote < csharp.Length) {
                        nextQuote = csharp.IndexOf('"', nextQuote);
                        if (nextQuote != -1
                            && csharp.Length > nextQuote + 1
                            && csharp[nextQuote + 1] == '"') { // This is a double-quote, don't stop here.
                            nextQuote += 2; // Skip that double-quote.
                            continue;
                        }
                        if (nextQuote != -1) { // Found the end of the string.
                            yield return csharp.Substring(nextT + 1, nextQuote - nextT - 1);
                            current = nextQuote + 1; // prepare to find next string.
                            gotOne = !gotOne; // If dealing with plural we need to prepare to find the second string, or switch back to finding the prefix.
                            break;
                        }
                        yield break;
                    }
                } else {
                    var nextQuote = nextT + 1;
                    while (nextQuote >= 0 && nextQuote < csharp.Length) {
                        nextQuote = csharp.IndexOf('"', nextQuote);
                        if (nextQuote != -1
                            && csharp[nextQuote - 1] == '\\') { // This is an escaped string, don't stop there.
                            nextQuote++;
                            continue;
                        }
                        if (nextQuote != -1) { // Found the end of the string.
                            yield return csharp.Substring(nextT + 1, nextQuote - nextT - 1);
                            current = nextQuote + 1; // prepare to find next string.
                            gotOne = !gotOne; // If dealing with plural we need to prepare to find the second string, or switch back to finding the prefix.
                            break;
                        }
                        yield break;
                    }
                }
                if (nextT == -1) yield break;
            }
        }
    }
}