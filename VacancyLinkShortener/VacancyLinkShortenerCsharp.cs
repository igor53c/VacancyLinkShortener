using System;
using System.Linq;
using System.Globalization;
using System.Text;

namespace VacancyLinkShortenerCsharpImplementation
{
    public static class VacancyLinkShortenerCsharp
    {
        public class SiteContent
        {
            public readonly string Url;
            public readonly string Title;

            public SiteContent(string url, string title)
            {
                Url = url;
                Title = title;
            }
        }

        public enum Platform
        {
            STELLANCHA_OS,
            JOBS_MIT_BIZ,
            JOB_DEALER
        }

        public class Vacancy
        {
            private int _id;
            private string _title;
            private string _company;
            private string[] _regions;
            private Platform _platform;

            public Vacancy(int id, string title, string company, string[] regions, Platform platform)
            {
                _id = id;
                _title = title;
                _company = company;
                _regions = regions;
                _platform = platform;
            }

            private string CleanAndFormatForUrl(string s, Platform platform, bool toLowerCase = true, bool capitalizeFirstLetter = false, bool replaceAmpersand = false, string replaceWith = "")
            {
                string replacementChar;
                if (platform == Platform.JOBS_MIT_BIZ || platform == Platform.STELLANCHA_OS)
                {
                    replacementChar = "-";
                    s = s.Replace(" - ", " ").Replace(" -", " ").Replace("- ", " ");
                    s = s.Replace("m/w/d", "m-w-d");
                }
                else
                {
                    replacementChar = "_";
                }

                s = s.Replace(" ", replacementChar);

                while (s.Contains("--"))
                {
                    s = s.Replace("--", "-");
                }

                var result = s.Replace(" - ", replacementChar).Replace(" -", replacementChar).Replace("- ", replacementChar);

                if (replaceAmpersand)
                {
                    switch (platform)
                    {
                        case Platform.STELLANCHA_OS:
                            // Keep the ampersand as it is.
                            break;
                        default:
                            result = result.Replace("&", replaceWith);
                            break;
                    }
                }

                result = result.Replace("&", "").Replace(".", "").Replace("(", "").Replace(")", "").Replace("%", "").Replace("/", "");

                if (toLowerCase)
                {
                    result = result.ToLower();
                }

                if (capitalizeFirstLetter && result.Length > 0)
                {
                    result = result.Substring(0, 1).ToUpper() + result.Substring(1);
                }

                while (result.Contains("--"))
                {
                    result = result.Replace("--", "-");
                }

                while (result.Contains("__"))
                {
                    result = result.Replace("__", "_");
                }

                while (result.EndsWith("!"))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                return result;
            }

            private string FormatRegions(string[] regions, Platform platform)
            {
                string conjunction;
                if (platform == Platform.JOBS_MIT_BIZ)
                    conjunction = " und ";
                else if (platform == Platform.STELLANCHA_OS)
                    conjunction = ", ";
                else // platform == Platform.JOBS_DEALER
                    conjunction = " oder ";

                if (regions.Length == 1)
                {
                    return regions[0];
                }
                else if (regions.Length == 2)
                {
                    return regions[0] + conjunction + regions[1];
                }
                else
                {
                    if (platform == Platform.JOBS_MIT_BIZ)
                    {
                        if (regions.Length > 4)
                        {
                            return regions[0] + ", " + regions[1] + conjunction + (regions.Length - 2) + " weiteren Regionen";
                        }
                        else
                        {
                            return string.Join(", ", regions.Take(regions.Length - 1)) + conjunction + regions.Last();
                        }
                    }
                    else if (platform == Platform.JOB_DEALER)
                    {
                        if (regions.Length > 4)
                        {
                            return regions[0] + ", " + regions[1] + conjunction + "einer von " + (regions.Length - 2) + " weiteren Regionen";
                        }
                        else
                        {
                            return string.Join(", ", regions.Take(regions.Length - 1)) + conjunction + regions.Last();
                        }
                    }
                    else
                    {
                        var allButLastTwo = string.Join(", ", regions.Take(regions.Length - 2));
                        var lastTwo = string.Join(conjunction, regions.Skip(regions.Length - 2));
                        return allButLastTwo + ", " + lastTwo;
                    }
                }
            }

            public static string RemoveDiacritics(string text, Platform platform)
            {
                // Dodajemo posebne zamene za JOBS_MIT_BIZ platformu.
                if (platform == Platform.JOBS_MIT_BIZ)
                {
                    text = text.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue")
                        .Replace("Ä", "Ae").Replace("Ö", "Oe").Replace("Ü", "Ue")
                        .Replace("ß", "ss");
                }
                // Originalne zamene za ostale platforme.
                else if (platform == Platform.JOB_DEALER)
                {
                    text = text.Replace("ä", "a").Replace("ö", "o").Replace("ü", "u")
                        .Replace("Ä", "A").Replace("Ö", "O").Replace("Ü", "U")
                        .Replace("ß", "ss");
                }
                else if (platform == Platform.STELLANCHA_OS)
                {
                    text = text.Replace("ä", "a").Replace("ö", "o").Replace("ü", "u")
                        .Replace("Ä", "A").Replace("Ö", "O").Replace("Ü", "U")
                        .Replace("ß", "s");
                }

                var normalizedString = text.Normalize(NormalizationForm.FormD);
                var stringBuilder = new StringBuilder();

                foreach (var c in normalizedString)
                {
                    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    {
                        stringBuilder.Append(c);
                    }
                }

                return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            }

            public SiteContent GetSiteContent()
            {
                var url = "";
                var title = "";
                var regionForTitle = FormatRegions(_regions, _platform);
                var regionForUrl = string.Join("-", _regions.Select(r => CleanAndFormatForUrl(RemoveDiacritics(r, _platform), _platform, false)));

                switch (_platform)
                {
                    case Platform.STELLANCHA_OS:
                        Console.WriteLine($"Title: {_title}, Company: {_company}, ID: {_id}"); // Dodaj ovo
                        url = $"https://www.stellencha.os/stellen/{CleanAndFormatForUrl(RemoveDiacritics(_company, _platform), _platform, replaceAmpersand: true)}/{_id}/{CleanAndFormatForUrl(RemoveDiacritics(_title, _platform), _platform, replaceAmpersand: true)}/";
                        title = $"{_title} | {_company} | {regionForTitle} | stellencha.os";
                        break;
                    case Platform.JOBS_MIT_BIZ:
                        url = $"https://www.jobs-mit.biz/Jobs/{regionForUrl}/{_id}/{CleanAndFormatForUrl(RemoveDiacritics(_title, _platform), _platform, false, true)}/";
                        title = $"Jetzt Bewerben! - {_title} @ {regionForTitle}";
                        break;
                    case Platform.JOB_DEALER:
                        var formattedCompany = RemoveDiacritics(CleanAndFormatForUrl(_company, _platform).Replace("-", "_"), _platform);
                        var titleForUrl = RemoveDiacritics(_title, _platform).Replace("-", "_").Replace("(m/w/d)", "_m_w_d");
                        titleForUrl = CleanAndFormatForUrl(titleForUrl, _platform);
                        var formattedRegion = RemoveDiacritics(regionForUrl.Replace("-", "_"), _platform).ToLower();
                        url = $"https://www.job.dealer/job/{_id}_{titleForUrl}_bei_{formattedCompany}_in_{formattedRegion}";
                        title = $"{_title} bei {_company} in {regionForTitle} von deinem job.dealer";
                        break;

                }

                return new SiteContent(url, title);
            }
        }
    }
}
