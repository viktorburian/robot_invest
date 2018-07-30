using System;
using System.IO;
using System.Collections.Generic;

namespace RobotInvest.Model
{
    static class HelperClass
    {
        public static string homeDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support", "RobotInvest");

        public static readonly string[] fileNames = { "DJIA", "MZM", "MZMYTD", "VIX", "LOANSYTD", "TIGHTENING", "YIELDSPREAD", "PROFITS" };
        private static string[] fredWebsiteParts = {"https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd=",
                                                    "&coed=" };
                                                     
        public static string GetURL(DateTime startDateDT, DateTime endDateDT, string fileName)
        {
            string startDate = string.Concat(startDateDT.Year.ToString(), "-", startDateDT.Month.ToString(), "-", startDateDT.Day.ToString());
            string endDate = string.Concat(endDateDT.Year.ToString(), "-", endDateDT.Month.ToString(), "-", endDateDT.Day.ToString());
            string urlPart = "";
            switch (fileName)
            {
                case "DJIA":
                    urlPart = "&height=450&stacking=&range=Custom&mode=fred&id=DJIA&transformation=lin&nd=2008-03-28&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "MZM":
                    urlPart = "&height=450&stacking=&range=&mode=fred&id=MZMSL&transformation=lin&nd=1959-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "MZMYTD":
                    urlPart = "&height=450&stacking=&range=Custom&mode=fred&id=MZMSL&transformation=pc1&nd=1959-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "VIX":
                    urlPart = "&height=450&stacking=&range=Custom&mode=fred&id=VIXCLS&transformation=lin&nd=1990-01-02&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily%2C+Close&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "LOANSYTD":
                    urlPart = "&height=450&stacking=&range=Custom&mode=fred&id=BUSLOANS&transformation=pc1&nd=1947-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "TIGHTENING":
                    urlPart = "&height=450&stacking=&range=&mode=fred&id=DRTSCILM&transformation=lin&nd=1990-04-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Quarterly%2C+End+of+Period&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "YIELDSPREAD":
                    urlPart = "&height=450&stacking=&range=Custom&mode=fred&id=T10Y2Y&transformation=lin&nd=1976-06-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                case "PROFITS":
                    urlPart = "&height=450&stacking=&range=Custom&mode=fred&id=CPATAX&transformation=lin&nd=1947-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Quarterly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168";
                    break;
                default:
                    break;
            }
            return string.Concat(fredWebsiteParts[0],startDate,fredWebsiteParts[1],endDate,urlPart);
        }
    }
}