using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace PdfParser.FormatLinkedin
{
    public class FormatLinkedin
    {
        public string Contact { get; set; }
        public string UserEmail { get; set; }
        public string UserCode { get; set; }
        public List<string> ContactDetail { get; set; } = new List<string>();
        public string Skills { get; set; }
        public List<string> SkillsDetail { get; set; } = new List<string>();
        public string Certifications { get; set; }
        public List<string> CertificationsDetail { get; set; } = new List<string>();
        public string HonorsAwards { get; set; }
        public List<string> HonorsAwardsDetail { get; set; } = new List<string>();
        public string LanguagesAll { get; set; }
        public List<string> Languages { get; set; } = new List<string>();
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Introduction { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<EduUninvesityLinkedin> Educations { get; set; }
    }

    public class Format
    {
        public string Contact { get; set; }
        public string Skills { get; set; }
        public string Certifications { get; set; }
        public string HonorsAwards { get; set; }
        public string UserName { get; set; }
        public string Introduction { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<EduUninvesityLinkedin> Educations { get; set; }
    }

    public class Experience
    {
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string JobLocation { get; set; }
        public string DateDesc { get; set; }
        public DateDescDetail DateDescDetail { get; set; } = new DateDescDetail();
        public string Desc { get; set; }
        //public int MergeIndex { get; set; }
        public string TotalDateDesc { get; set; }
        public List<Experience> SubExperiences { get; set; }
        //public string ToString() {
        //    return $"{CompanyName}\n{JobTitle}\n{JobLocation}\n{DateDesc}\n{Desc}";
        //}
    }

    public class DateDescDetail
    {
        public string StartDate   { get; set; }
        public string EndDate     { get; set; }
        public string TotalYear   { get; set; }
    }

    public class EduUninvesityLinkedin
    {
        public string Uninvesity { get; set; }
        //public string College { get; set; }
        public string SDate { get; set; }
        public string EDate { get; set; }
        public string Degree { get; set; }
    }

    //public class EduUninvesityTest
    //{
    //    public string Uninvesity { get; set; }
    //    public string College { get; set; }
    //    public string SDate { get; set; }
    //    public string EDate { get; set; }
    //    public string MEMO { get; set; }
    //}

    class Body
    {
        public static FormatLinkedin Format(Stream pdfFileStream)
        {
            string splitInfo = "****splitInfo****";
            string splitCompId = "****splitCompId****";
            string splitEduId = "****splitEduId****";
            string splitColumn = "****splitColumn****";

            //第一階段 基本拆分
            string[][] keyWords = {
                new string[]{"聯絡", "聯絡資料", "联系方式", "Contact"},
                //new string[]{"熱門技能"},
                //new string[]{"Languages"},
                //new string[]{"Certifications"},
                //new string[]{"Honors-Awards"},
                //new string[]{"簡介"},
                new string[]{"經歷", "工作经历", "Experience"},
                new string[]{"學歷","Education" }
            };

            int keyWordsIndex = 0;
            double LastLeft = 0;
            FormatLinkedin Format = new FormatLinkedin();
            Dictionary<string, string> keyWordsMap = new Dictionary<string, string>();
            string stringEnd = "";
            string LastKeyString = keyWords[0][0];
            List<Letter> Letters = new List<Letter>();
            using (var document = PdfDocument.Open(pdfFileStream))
            {
                StringBuilder sb = new StringBuilder();

                for (var i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);

                    Letters.AddRange(page.Letters);

                    var words = page.GetWords(NearestNeighbourWordExtractor.Instance);
                    var blocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);

                    var unsupervisedReadingOrderDetector = new UnsupervisedReadingOrderDetector(10);
                    var orderedBlocks = unsupervisedReadingOrderDetector.Get(blocks);

                    for (int j = 0; j < blocks.Count; j++)
                    {
                        var block = blocks[j];
                        //Console.WriteLine(block);
                        //Console.WriteLine("--b--");
                        System.Diagnostics.Debug.WriteLine(block);
                        System.Diagnostics.Debug.WriteLine(block.BoundingBox.Left);
                        System.Diagnostics.Debug.WriteLine("--b--");

                        if (LastLeft > 0 && LastLeft != block.BoundingBox.Left)
                        {
                            sb.Append(splitColumn);
                        }

                        // Do something
                        foreach (var Line in block.TextLines)
                        {
                            if (Line.Text.StartsWith($"Page {i + 1} of {document.NumberOfPages}"))
                            {
                                //Page 3 of 4
                                continue;
                            }

                            if (keyWordsIndex < keyWords.Length)
                            {
                                sb.Append(Line + "\n");
                                //https://stackoverflow.com/questions/2796891/how-to-use-string-endswith-to-test-for-multiple-endings
                                //if (sb.ToString().EndsWith(keyWords[keyWordsIndex] + "\n"))
                                if (keyWords[keyWordsIndex].Any(x => sb.ToString().EndsWith(x + "\n")))
                                {
                                    //if (sb.ToString().EndsWith("簡介" + "\n"))
                                    //{
                                    //    int p = 0;
                                    //    keyWordsMap["基本資料"] = blocks[j - 1].Text;
                                    //    if (blocks[j - 1].TextLines.Count > 0)
                                    //    {
                                    //        Format.UserName = blocks[j - 1].TextLines[0].Text;
                                    //    }
                                    //}

                                    if (keyWordsIndex > 0)
                                    {
                                        //if (!keyWordsMap.ContainsKey(keyWords[keyWordsIndex - 1]))
                                        //{
                                        //    keyWordsMap[keyWords[keyWordsIndex - 1]] = sb.ToString().Substring(0, sb.ToString().Length - keyWords[keyWordsIndex].Length - 1);
                                        //}
                                        if (!keyWords[keyWordsIndex].Any(x => keyWordsMap.ContainsKey(x)))
                                        {
                                            //keyWordsMap[LastKeyString] = sb.ToString().Substring(0, sb.ToString().Length - LastKeyString.Length - 1);
                                            keyWordsMap[LastKeyString] = sb.ToString();
                                            foreach (var item in keyWords[keyWordsIndex])
                                            {
                                                if (keyWordsMap[LastKeyString].LastIndexOf(item) > 0)
                                                {
                                                    keyWordsMap[LastKeyString] = keyWordsMap[LastKeyString].Substring(0, keyWordsMap[LastKeyString].LastIndexOf(item));
                                                }
                                            }
                                            if (LastKeyString == "聯絡")
                                            {
                                                keyWordsMap[LastKeyString] = "聯絡\n" + keyWordsMap[LastKeyString];
                                            }
                                        }
                                        LastKeyString = keyWords[keyWordsIndex][0];
                                    }

                                    //sb.Append("\r\n");

                                    //Console.WriteLine("---"+ keyWords[(keyWordsIndex ==0?0: keyWordsIndex - 1)] + "---"+ keyWords[keyWordsIndex] + "---");
                                    //Console.WriteLine(sb.ToString());
                                    //Console.WriteLine("---");
                                    sb = new StringBuilder();
                                    keyWordsIndex++;
                                }
                            }
                            else
                            {
                                sb.Append(Line + "\n");
                                //stringEnd = sb.ToString();
                            }
                        }
                        if (keyWordsIndex < 2)//"基本資料"
                        {
                            sb.Append(splitInfo);
                        }
                        if (keyWordsIndex == 2)//"經歷"
                        {
                            sb.Append(splitCompId);
                        }
                        if (keyWordsIndex == 3)//"學歷"
                        {
                            sb.Append(splitEduId);
                        }

                        LastLeft = block.BoundingBox.Left;
                    }
                }

                //keyWordsMap[keyWords[keyWords.Length - 1][0]] = sb.ToString(); //stringEnd;
                keyWordsMap[LastKeyString] = sb.ToString(); //stringEnd;


                int a = 0;

                //第二階段 細部拆分
                Regex regNum = new Regex("^[0-9]");
                string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

                keyWordsIndex = 0;

                //keyWordsIndex++;
                string[] InfosAll = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(splitColumn);
                string[] Infos = InfosAll[0].Split(splitInfo);

                Dictionary<string, string> InfoWords =
                    new Dictionary<string, string>
                        {
                            { "聯絡", "聯絡" },
                            { "聯絡資料", "聯絡" },
                            { "联系方式", "聯絡" },
                            { "Contact", "聯絡" },

                            { "熱門技能", "熱門技能" },
                            { "Top Skills", "熱門技能" },
                            { "热门技能", "熱門技能" },

                            { "Languages", "Languages" },

                            { "Certifications", "Certifications" },

                            { "Honors-Awards", "Honors-Awards" }
                        };

                StringBuilder sbInfo = new StringBuilder();
                string LastInfoKeyString = "";
                string LastInfoKeyStringOrg = "";
                int LastInfoKeyIndex = 0;
                string LastSecondInfoKeyString = "";
                int LastSecondInfoKeyIndex = 0;
                bool LastInfoStringIsOneLine = false;
                for (int i = 0; i < Infos.Length; i++)
                {
                    //if(Infos[i].Trim().Contains("(LinkedIn)\n"))
                    //{
                    //    Infos[i] = Infos[i].Replace("\n","");
                    //}

                    string[] InfosArray = Infos[i].Split("\n");
                    if (InfosArray.Length >= 1 && InfoWords.ContainsKey(InfosArray[0].Trim()))
                    {
                        sbInfo = GetInfo(Format, sbInfo, LastInfoKeyString , LastInfoKeyStringOrg);
                        LastSecondInfoKeyString = LastInfoKeyString;
                        LastInfoKeyString = InfoWords[InfosArray[0].Trim()];
                        LastInfoKeyStringOrg = InfosArray[0].Trim();//InfoWords[InfosArray[0].Trim()];
                        LastSecondInfoKeyIndex = LastInfoKeyIndex;
                        LastInfoKeyIndex = i;
                        LastInfoStringIsOneLine = Infos[LastSecondInfoKeyIndex].Trim().Split("\n").Length <= 1 ? true : false;

                    }
                    sbInfo.Append(Infos[i]);
                }

                GetInfo(Format, sbInfo, LastInfoKeyString , LastInfoKeyStringOrg);

                //--------------
                //public string Contact { get; set; }
                //public List<string> ContactDetail { get; set; } = new List<string>();
                //public string Skills { get; set; }
                //public List<string> SkillsDetail { get; set; } = new List<string>();
                //public string Certifications { get; set; }
                //public List<string> CertificationsDetail { get; set; } = new List<string>();
                //public string HonorsAwards { get; set; }
                //public List<string> HonorsAwardsDetail { get; set; } = new List<string>();
                //public string Languages { get; set; }
                //public List<string> LanguagesDetail { get; set; } = new List<string>();
                if(!string.IsNullOrWhiteSpace(Format.Contact))
                {
                    string[] ContactD = Format.Contact.Split(")\n");
                    foreach (var item in ContactD)
                    {
                        if(string.IsNullOrWhiteSpace(item))
                        {
                            continue;
                        }
                        if (item.Contains("@"))
                        {
                            Format.ContactDetail.Add(item.Trim().Substring(0, item.Trim().IndexOf("\n")));
                            Format.ContactDetail.Add(item.Trim().Substring(item.Trim().IndexOf("\n")).Replace("\n", "").Trim() + ")");
                        }
                        else
                        {
                            Format.ContactDetail.Add(item.Replace("\n", "").Trim() + ")");
                        }
                    }
                }

                foreach (var item in Format.ContactDetail)
                {
                    if (item.Contains("@"))
                    {
                        Format.UserEmail = item.Trim();
                    }
                    if (item.Contains("(LinkedIn)"))
                    {
                        Format.UserCode = item.Replace("(LinkedIn)", "").Trim();
                    }
                }

                if (!string.IsNullOrWhiteSpace(Format.Skills))
                {
                    string[] SkillsD = Format.Skills.Split("\n");
                    foreach (var item in SkillsD)
                    {
                        if (string.IsNullOrWhiteSpace(item))
                        {
                            continue;
                        }
                        Format.SkillsDetail.Add(item.Trim());
                    }
                }

                if (!string.IsNullOrWhiteSpace(Format.Certifications))
                {
                    string[] CertificationsD = Format.Certifications.Split("\n");
                    foreach (var item in CertificationsD)
                    {
                        if (string.IsNullOrWhiteSpace(item))
                        {
                            continue;
                        }
                        Format.CertificationsDetail.Add(item.Trim());
                    }
                }

                if (!string.IsNullOrWhiteSpace(Format.HonorsAwards))
                {
                    string[] HonorsAwardsD = Format.HonorsAwards.Split("\n");
                    foreach (var item in HonorsAwardsD)
                    {
                        if (string.IsNullOrWhiteSpace(item))
                        {
                            continue;
                        }
                        Format.HonorsAwardsDetail.Add(item.Trim());
                    }
                }
                //string clean = Regex.Replace(Format.Languages, @"[^\w\s]", string.Empty);
                if (!string.IsNullOrWhiteSpace(Format.LanguagesAll))
                {
                    string[] LanguagesD = Format.LanguagesAll.Split(")\n");
                    foreach (var item in LanguagesD)
                    {
                        if (string.IsNullOrWhiteSpace(item))
                        {
                            continue;
                        }
                        if (item.Contains("("))
                        {
                            Format.Languages.Add(item.Replace("\n", " ").Trim() + ")");
                        }
                        else
                        {
                            string[] LanguagesDTemp = item.Split("\n");
                            foreach (var itemD in LanguagesDTemp)
                            {
                                Format.Languages.Add(itemD.Trim());
                            }
                        }
                    }
                }


                //--------------

                //InfosAll[1]
                string[] Introductions = { "簡介", "Summary" };
                Infos = InfosAll[1].Split(splitInfo);

                bool HaveIntroductions = false;
                int IntroductionIndex = 0;
                for (int i = 0; i < Infos.Length; i++)
                {
                    if (Introductions.Any(x => Infos[i].Trim().Equals(x)))
                    {
                        HaveIntroductions = true;
                        IntroductionIndex = i;
                    }
                }

                //
                //else

                Format.UserName = Infos[0];
                string[] temp = Format.UserName.Split("\n");
                if (temp.Length >= 3)
                {
                    Format.UserName = temp[0].Trim();
                    Format.Title = temp[1].Trim();
                    Format.Location = temp[2].Trim();
                }

                Format.UserName = Format.UserName?.Trim();
                Format.Title = Format.Title?.Trim();
                Format.Location = Format.Location?.Trim();

                if (HaveIntroductions)
                {
                    if (string.IsNullOrWhiteSpace(Format.Title))
                    {
                        Format.Title = Infos[IntroductionIndex - 1].Trim();
                        string[] tempA = Format.Title.Split("\n");
                        if (tempA.Length == 2)
                        {
                            Format.Title = tempA[0].Trim();
                            Format.Location = tempA[1].Trim();
                        }
                    }
                    Format.Introduction = Infos[IntroductionIndex+1].Trim();

                    string[] copy = new string[Infos.Length - IntroductionIndex - 1];
                    Array.Copy(Infos, IntroductionIndex + 1, copy, 0, copy.Length);
                    Format.Introduction = string.Join("\n", copy);
                }
                else
                {
                    string[] tempA = Infos[1].Trim().Split("\n");
                    if (tempA.Length == 2)
                    {
                        Format.Title = tempA[0].Trim();
                        Format.Location = tempA[1].Trim();
                    }
                    //else
                    //{
                    //    Format.Title = Infos[1].Trim();
                    //}
                }


                /*
                if (Infos.Length == 3 || Infos.Length == 4 || Infos.Length == 5)
                {
                    Format.UserName = Infos[0];
                    string[] temp = Format.UserName.Split("\n");
                    //if (temp.Length == 2)
                    //{
                    //    Format.UserName = temp[0].Trim();
                    //    Format.Title = temp[1].Trim();
                    //}
                    if (temp.Length >= 3)
                    {
                        Format.UserName = temp[0].Trim();
                        Format.Title = temp[1].Trim();
                        Format.Location = temp[2].Trim();
                    }

                    Format.UserName = Format.UserName?.Trim();
                    Format.Title = Format.Title?.Trim();
                    Format.Location = Format.Location?.Trim();

                    if (Introductions.Any(x => Infos[1].Trim().Equals(x)))
                    {
                        Format.Introduction = Infos[2].Trim();
                    }
                    else
                    {
                        string[] tempA = Infos[1].Trim().Split("\n");
                        if(tempA.Length == 2)
                        {
                            Format.Title = tempA[0].Trim();
                            Format.Location = tempA[1].Trim();
                        }   
                        else
                        {
                            Format.Title = Infos[1].Trim();
                        }
                        
                    }

                    if(Infos.Length >= 4)
                    {
                        if (Introductions.Any(x => Infos[2].Trim().Equals(x)))
                        {
                            Format.Introduction = Infos[3].Trim();
                        }
                    }
                    
                }
                else if (Infos.Length >= 7)
                {
                    Format.UserName = Infos[0].Trim();
                    Format.Title = Infos[1].Trim();
                    string[] tempA = Infos[1].Trim().Split("\n");
                    if (tempA.Length == 2)
                    {
                        Format.Title = tempA[0].Trim();
                        Format.Location = tempA[1].Trim();
                    }

                    if (Introductions.Any(x=> Infos[2].Trim().Equals(x)))
                    {
                        string[] copy = new string[Infos.Length - 3];
                        Array.Copy(Infos, 3, copy, 0, copy.Length);
                        Format.Introduction = string.Join("\n", copy);
                    }
                    //Format.Location = Infos[3].Trim();

                    //Format.Introduction = Infos[2].Trim();
                }
                */
                int z = 0;
                /*
                if(sbInfo.ToString().StartsWith("簡介"))
                {
                    if(LastInfoStringIsOneLine)
                    {
                        if (Infos.Length > LastSecondInfoKeyIndex + 2)
                        {
                            Format.UserName = Infos[LastSecondInfoKeyIndex + 2];
                        }
                        if (Infos.Length > LastSecondInfoKeyIndex + 3)
                        {
                            Format.Title = Infos[LastSecondInfoKeyIndex + 3];
                        }
                        if (Infos.Length > LastSecondInfoKeyIndex + 4)
                        {
                            if (!InfoWords.ContainsKey(Infos[LastSecondInfoKeyIndex + 4].Trim()))
                            {
                                Format.Location = Infos[LastSecondInfoKeyIndex + 4];
                            }
                        }
                    }
                    else
                    {
                        Format.UserName = Infos[LastInfoKeyIndex - 1];
                    }                    
                    Format.Introduction = sbInfo.ToString().Substring(LastInfoKeyString.Length);

                    switch (LastSecondInfoKeyString)
                    {
                        case "聯絡":
                            Format.Contact = Format.Contact.Substring(0, Format.Contact.LastIndexOf(Format.UserName));
                            break;
                        case "熱門技能":
                            Format.Skills = Format.Skills.Substring(0, Format.Skills.LastIndexOf(Format.UserName));
                            break;
                        case "Languages":
                            Format.Languages = Format.Languages.Substring(0, Format.Languages.LastIndexOf(Format.UserName));
                            break;
                        case "Certifications":
                            Format.Certifications = Format.Certifications.Substring(0, Format.Certifications.LastIndexOf(Format.UserName));
                            break;
                        case "Honors-Awards":
                            Format.HonorsAwards = Format.HonorsAwards.Substring(0, Format.HonorsAwards.LastIndexOf(Format.UserName));
                            break;
                        default:
                            break;
                    }

                    string[] temp = Format.UserName.Split("\n");
                    if(temp.Length >= 3)
                    {
                        Format.UserName = temp[0].Trim();
                        Format.Title = temp[1].Trim();
                        Format.Location = temp[2].Trim();
                    }

                    Format.UserName = Format.UserName?.Trim();
                    Format.Title = Format.Title?.Trim();
                    Format.Location = Format.Location?.Trim();

                    Format.Contact = Format.Contact?.Trim();
                    Format.Skills = Format.Skills?.Trim();
                    Format.Certifications = Format.Certifications?.Trim();
                    Format.HonorsAwards = Format.HonorsAwards?.Trim();
                    Format.Languages = Format.Languages?.Trim();
                    Format.Introduction = Format.Introduction?.Trim(); 
                }
                */
                z = 0;
                //    {
                //    new string[]{"聯絡", "聯絡資料", "联系方式", "Contact"},
                //    //new string[]{"熱門技能"},
                //    //new string[]{"Languages"},
                //    //new string[]{"Certifications"},
                //    //new string[]{"Honors-Awards"},
                //    //new string[]{"簡介"},
                //    new string[]{"經歷", "工作经历", "Experience"},
                //    new string[]{"學歷","Education" }
                //};

                //keyWordsIndex++;
                //keyWordsIndex++;
                /*
                //2.1 聯絡,熱門技能,Certifications,Honors-Awards
                Format.Contact = keyWordsMap[keyWords[keyWordsIndex++][0]];
                Format.Skills = keyWordsMap[keyWords[keyWordsIndex++][0]];//.Replace("\r\n\r\n", "");
                Format.Certifications = keyWordsMap[keyWords[keyWordsIndex++][0]];//.Replace("\r\n", " ");
                //Format.HonorsAwards = keyWordsMap[keyWords[keyWordsIndex++]].Replace("\r\n", " ");
                string[] aa = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(Format.UserName);
                Format.HonorsAwards = aa[0];

                Format.Introduction = keyWordsMap[keyWords[keyWordsIndex++][0]];
                */
                //經歷
                //Format.Experience = 
                if (keyWordsMap.ContainsKey("經歷"))
                {
                    keyWordsMap["經歷"] = keyWordsMap["經歷"].Replace(splitCompId + splitCompId, "");//修正換頁錯誤
                    keyWordsMap["經歷"] = keyWordsMap["經歷"].Replace(splitColumn, "");//修正換頁錯誤
                }
                string[] Exps = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(splitCompId);
                Format.Experiences = new List<Experience>();
                List<Experience> tempExperiences = new List<Experience>();
                //int MergeIndex = 0;
                for (int i = 0; i < Exps.Length; i++)
                {
                    string Exp = Exps[i];
                    if (string.IsNullOrWhiteSpace(Exp))
                    {
                        continue;
                    }
                    Experience Experience = new Experience();

                    string[] Exp0 = Exp.Split("\n");
                    if (Exp0 != null && Exp0.Length >= 2)
                    {
                        Experience.CompanyName = Exp0[0];
                        Experience.JobTitle = Exp0[1];
                        Regex regYear = new Regex("^20\\d{2}|^19\\d{2}");
                        Regex regNumCount = new Regex("^[0-9]\\.");
                        if (regNumCount.IsMatch(Experience.JobTitle))
                        {
                            //1. 2. 3. 等等
                            Experience.CompanyName = "";
                            Experience.JobTitle = "";
                            Experience.Desc = Exp;
                            tempExperiences.Add(Experience);
                        }
                        else if (regNum.IsMatch(Experience.JobTitle)
                             || months.Any(x => Experience.JobTitle.StartsWith(x)
                             )
                            )
                        {
                            if ((Experience.JobTitle.Contains("months") || Experience.JobTitle.Contains("個月")) && !Experience.JobTitle.Contains(")"))
                            {
                                //總經驗 暫時不寫入
                                Experience.TotalDateDesc = Experience.JobTitle;
                                Experience.JobTitle = "";
                                string[] copy = new string[Exp0.Length - 2];
                                Array.Copy(Exp0, 2, copy, 0, copy.Length);
                                Experience.Desc = string.Join("\n", copy);

                                tempExperiences.Add(Experience);
                                //MergeIndex = tempExperiences.Count() - 1;
                            }
                            else if (Experience.JobTitle.Contains(")"))
                            {
                                Experience.CompanyName = "";
                                Experience.JobTitle = "";
                                //Experience.DateDesc = "0";
                                //Experience.JobTitle = Exp0[2];
                                //Experience.DateDesc = Exp0[3];

                                string[] copy = new string[Exp0.Length];
                                Array.Copy(Exp0, 0, copy, 0, copy.Length);

                                Experience.Desc = string.Join("\n", copy);
                                //Experience.MergeIndex = MergeIndex;
                                tempExperiences.Add(Experience);
                                //tempExperiences[MergeIndex].Desc += Experience.Desc;
                            }
                            else
                            {
                                if (Exp0.Length > 5)
                                {
                                    Experience.JobTitle = Exp0[2];
                                    Experience.DateDesc = Exp0[3];
                                    Experience.JobLocation = Exp0[4];

                                    string[] copy = new string[Exp0.Length - 5];
                                    Array.Copy(Exp0, 5, copy, 0, copy.Length);

                                    Experience.Desc = string.Join("\n", copy);
                                }
                                //else
                                //{
                                //    Experience.DateDesc = "0";
                                //}
                                tempExperiences.Add(Experience);
                            }
                        }
                        else if (Exp0.Length > 2 && !string.IsNullOrWhiteSpace(Exp0[2]))
                        {
                            Experience.DateDesc = Exp0[2];

                            if (regYear.IsMatch(Experience.DateDesc) && !Experience.DateDesc.Contains("年"))
                            {  //這類都沒有地點
                                Experience.JobLocation = "";
                                string[] copy = new string[Exp0.Length - 3];
                                Array.Copy(Exp0, 3, copy, 0, copy.Length);

                                Experience.Desc = string.Join("\n", copy);
                                tempExperiences.Add(Experience);
                            }
                            else if (regNum.IsMatch(Experience.DateDesc)
                             || months.Any(x => Experience.DateDesc.StartsWith(x)
                             //|| regYear.IsMatch(Experience.DateDesc)
                             )
                            )
                            {
                                Experience.JobLocation = Exp0[3];
                                string[] spChar = { "•", "-" }; //不可能是地名開頭的
                                if (spChar.Any(x => Experience.JobLocation.StartsWith(x)))
                                {
                                    Experience.JobLocation = "";
                                    string[] copy = new string[Exp0.Length - 3];
                                    Array.Copy(Exp0, 3, copy, 0, copy.Length);

                                    Experience.Desc = string.Join("\n", copy);
                                    tempExperiences.Add(Experience);
                                }
                                else
                                {
                                    //-------------判斷 地點顏色是灰色
                                    //Page.
                                    bool IsJobLocation = false;
                                    StringBuilder sbLz = new StringBuilder();
                                    for (int Lz = 0; Lz < Letters.Count() ; Lz++)
                                    {
                                        sbLz.Append(Letters[Lz].Value);
                                        if(sbLz.ToString().EndsWith(Experience.JobLocation))
                                        {
                                            //測試是否是灰色                                            
                                            //System.Diagnostics.Debug.WriteLine(Letters[Lz].Color);
                                            var fontColor = Letters[Lz].Color.ToRGBValues();
                                            decimal dec = 0.6941M;
                                            //if ("((0.6941,0.6941,0.6941))" == Letters[Lz].Color.ToString())
                                            if (fontColor.r == dec && fontColor.g == dec && fontColor.b == dec)
                                            {
                                                IsJobLocation = true;
                                            }
                                        }
                                    }

                                    //-------------
                                    if (IsJobLocation)
                                    {
                                        string[] copy = new string[Exp0.Length - 4];
                                        Array.Copy(Exp0, 4, copy, 0, copy.Length);

                                        Experience.Desc = string.Join("\n", copy);
                                        tempExperiences.Add(Experience);
                                    }                                    
                                    else
                                    {
                                        Experience.JobLocation = "";
                                        string[] copy = new string[Exp0.Length - 3];
                                        Array.Copy(Exp0, 3, copy, 0, copy.Length);

                                        Experience.Desc = string.Join("\n", copy);
                                        tempExperiences.Add(Experience);
                                    }
                                }
                            }
                            else
                            {
                                Experience = new Experience();
                                Experience.Desc = Exp;
                                tempExperiences.Add(Experience);
                            }
                        }
                        else
                        {
                            tempExperiences.Add(Experience);
                        }
                    }
                }

                int LastOktempExperiencesIndex = 0;
                bool NeedInsert = false;
                for (int i = 0; i < tempExperiences.Count; i++)
                {
                    //DateDesc
                    //檢測 如果日期錯誤(不是數字開頭)  表示拆解錯誤  合併到上一筆
                    //Regex regNum = new Regex("^[0-9]");
                    //string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

                    if (!string.IsNullOrWhiteSpace(tempExperiences[i].TotalDateDesc))
                    {
                        LastOktempExperiencesIndex = i;
                        NeedInsert = true;
                    }

                    //if (tempExperiences[i].DateDesc == "0")
                    //{
                    //    //只放0 刻意拆解的
                    //    tempExperiences[LastOktempExperiencesIndex].Desc += tempExperiences[i].Desc;
                    //}
                    //else if (string.IsNullOrEmpty(tempExperiences[i].DateDesc))
                    //{
                    //    tempExperiences[LastOktempExperiencesIndex].Desc += tempExperiences[i].Desc;
                    //    LastOktempExperiencesIndex = i;
                    //}
                    //else if (string.IsNullOrWhiteSpace(tempExperiences[i].DateDesc))
                    //{
                    //    Format.Experiences.Add(tempExperiences[i]);
                    //    LastOktempExperiencesIndex = i;
                    //}
                    //else 
                    Regex regYear = new Regex("^20\\d{2}|19\\d{2}");
                    if (!string.IsNullOrEmpty(tempExperiences[i].DateDesc) &&
                        ((regNum.IsMatch(tempExperiences[i].DateDesc) && tempExperiences[i].DateDesc.Contains("月"))
                        || months.Any(x => tempExperiences[i].DateDesc.StartsWith(x))
                        || regYear.IsMatch(tempExperiences[i].DateDesc)
                        ))
                    {
                        if (NeedInsert)
                        {
                            Format.Experiences.Add(tempExperiences[LastOktempExperiencesIndex]);
                            NeedInsert = false;
                        }
                        Format.Experiences.Add(tempExperiences[i]);
                        if (!NeedInsert)
                        {
                            LastOktempExperiencesIndex = i;
                        }

                    }
                    else
                    {
                        //Experience.CompanyName = Exp0[0];
                        //Experience.JobTitle = Exp0[1];
                        //Experience.DateDesc
                        //Experience.Desc
                        StringBuilder tempSb = new StringBuilder("\n");
                        tempSb
                            .Append(tempExperiences[i].CompanyName + "\n")
                            .Append(tempExperiences[i].JobTitle + "\n")
                            .Append(tempExperiences[i].DateDesc + "\n")
                            .Append(tempExperiences[i].Desc + "\n");
                        if (LastOktempExperiencesIndex != i)
                        {
                            tempExperiences[LastOktempExperiencesIndex].Desc += tempSb.ToString();
                        }
                        //NeedInsert = true;
                    }
                }

                if (NeedInsert)
                {
                    Format.Experiences.Add(tempExperiences[LastOktempExperiencesIndex]);
                }

                if (tempExperiences.Count == 1 && Format.Experiences.Count == 0)
                {
                    Format.Experiences.Add(tempExperiences[0]);//特例 資料實在太少無法判斷
                }

                //string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                foreach (var Experience in Format.Experiences)
                {
                    if (!string.IsNullOrWhiteSpace(Experience.TotalDateDesc))
                    {
                        List<Experience> formatSubExperiences = new List<Experience>();
                        //Regex regNum = new Regex("^[0-9]");
                        string[] desc = Experience.Desc.Split("\n");
                        for (int i = 0; i < desc.Length; i++)
                        {
                            if ((regNum.IsMatch(desc[i]) && desc[i].Contains("月"))
                             || months.Any(x => desc[i].StartsWith(x)))
                            {
                                if (i > 0 && i + 1 < desc.Length)
                                {
                                    Experience Exp = new Experience();
                                    Exp.JobTitle = desc[i - 1];
                                    Exp.DateDesc = desc[i];
                                    Exp.JobLocation = desc[i + 1];
                                    formatSubExperiences.Add(Exp);
                                }
                            }
                        }
                        Experience.SubExperiences = formatSubExperiences;
                    }
                }

                foreach (var Experience in Format.Experiences)
                {
                    if (!string.IsNullOrWhiteSpace(Experience.DateDesc))
                    {
                        if (Experience.DateDesc.Contains("-"))
                        {
                            string[] DateDesc1 = Experience.DateDesc.Split("-");
                            if (DateDesc1.Length > 1)
                            {
                                Experience.DateDescDetail.StartDate = DateDesc1[0].Trim();
                                if (DateDesc1[1].Trim().Contains("("))
                                {
                                    string[] DateDesc2 = DateDesc1[1].Trim().Split("(");
                                    if (DateDesc2.Length > 1)
                                    {
                                        Experience.DateDescDetail.EndDate = DateDesc2[0].Trim();
                                        Experience.DateDescDetail.TotalYear = DateDesc2[1].Replace(")", "").Trim();
                                    }
                                }
                                else
                                {
                                    Experience.DateDescDetail.EndDate = DateDesc1[1].Trim();
                                }
                            }
                        }
                    }
                    if (Experience.SubExperiences !=null && Experience.SubExperiences.Count() > 0)
                    {
                        foreach (var SubExperience in Experience.SubExperiences)
                        {
                            if (!string.IsNullOrWhiteSpace(SubExperience.DateDesc))
                            {
                                if (SubExperience.DateDesc.Contains("-"))
                                {
                                    string[] DateDesc1 = SubExperience.DateDesc.Split("-");
                                    if (DateDesc1.Length > 1)
                                    {
                                        SubExperience.DateDescDetail.StartDate = DateDesc1[0].Trim();
                                        if (DateDesc1[1].Trim().Contains("("))
                                        {
                                            string[] DateDesc2 = DateDesc1[1].Trim().Split("(");
                                            if (DateDesc2.Length > 1)
                                            {
                                                SubExperience.DateDescDetail.EndDate = DateDesc2[0].Trim();
                                                SubExperience.DateDescDetail.TotalYear = DateDesc2[1].Replace(")", "").Trim();
                                            }
                                        }
                                        else
                                        {
                                            SubExperience.DateDescDetail.EndDate = DateDesc1[1].Trim();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //學歷"
                if (keyWordsMap.ContainsKey("學歷"))
                {
                    keyWordsMap["學歷"] = keyWordsMap["學歷"].Replace(splitEduId + splitEduId, "");//修正換頁錯誤
                    keyWordsMap["學歷"] = keyWordsMap["學歷"].Replace(")\n", ")" + splitEduId);
                    keyWordsMap["學歷"] = keyWordsMap["學歷"].Replace(splitColumn, "");
                }
                //Format.Educations = keyWordsMap[keyWords[keyWordsIndex++]];
                Format.Educations = new List<EduUninvesityLinkedin>();
                string eduKey = keyWords[keyWordsIndex++][0];
                /*
                if (keyWordsMap.ContainsKey(eduKey))
                {
                    string[] Edus = keyWordsMap[eduKey].Split(splitEduId);
                    for (int i = 0; i < Edus.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(Edus[i]))
                        {
                            continue;
                        }
                        EduUninvesityLinkedin EduUninvesity = new EduUninvesityLinkedin();
                        string[] edu0 = Edus[i].Split("\n");
                        EduUninvesity.Uninvesity = edu0[0].Trim();

                        string[] copy = new string[edu0.Length - 1];
                        Array.Copy(edu0, 1, copy, 0, copy.Length);

                        string[] edu1 = string.Join("\n", copy).Replace("\n", " ").Split(",");

                        if(edu1.Length == 1)
                        {
                            string[] edu11 = edu1[0].Trim().Split("·");
                            if(edu11.Length >=2)
                            {
                                EduUninvesity.College = edu11[0].Trim();
                                if (edu11[1].Contains("-"))
                                {
                                    string[] edu3 = edu11[1].Split("-");

                                    EduUninvesity.SDate = edu3[0].Replace("(", "").Trim();
                                    EduUninvesity.EDate = edu3[1].Replace(")", "").Trim();
                                }
                            }
                            else
                            {
                                EduUninvesity.MEMO = edu1[0].Trim();
                            }
                        }    
                        else if (edu1.Length >= 2 && edu1[1].Contains("·"))
                        {
                            EduUninvesity.MEMO = edu1[0].Trim();
                            string[] edu2 = edu1[1].Split("·");
                            EduUninvesity.College = edu2[0].Trim();

                            if (edu2.Length >= 2 && edu2[1].Contains("-"))
                            {
                                string[] edu3 = edu2[1].Split("-");

                                EduUninvesity.SDate = edu3[0].Replace("(", "").Trim();
                                EduUninvesity.EDate = edu3[1].Replace(")", "").Trim();
                            }
                        }
                        else
                        {
                            string eduZ = string.Join("\n", copy).Replace("\n", " ");
                            if (eduZ.Contains("·"))
                            {
                                string[] edu2 = eduZ.Split("·");
                                EduUninvesity.College = edu2[0].Trim();

                                if (edu2.Length >= 2 && edu2[1].Contains("-"))
                                {
                                    string[] edu3 = edu2[1].Split("-");

                                    EduUninvesity.SDate = edu3[0].Replace("(", "").Trim();
                                    EduUninvesity.EDate = edu3[1].Replace(")", "").Trim();
                                }
                            }
                            else
                            {
                                EduUninvesity.MEMO = string.Join("\n", copy).Replace("\n", " ");
                            }
                        }
                        Format.Educations.Add(EduUninvesity);
                    }
                }
                */
                if (keyWordsMap.ContainsKey(eduKey))
                {
                    string[] Edus = keyWordsMap[eduKey].Split(splitEduId);
                    for (int i = 0; i < Edus.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(Edus[i]))
                        {
                            continue;
                        }
                        EduUninvesityLinkedin EduUninvesity = new EduUninvesityLinkedin();
                        string[] edu0 = Edus[i].Split("\n");
                        EduUninvesity.Uninvesity = edu0[0].Trim();

                        string[] copy = new string[edu0.Length - 1];
                        Array.Copy(edu0, 1, copy, 0, copy.Length);

                        string[] edu1 = string.Join("\n", copy).Replace("\n", " ").Split("·");

                        if (edu1.Length == 1)
                        {
                            EduUninvesity.Degree = edu1[0].Trim();
                        }
                        else if (edu1.Length >= 2)
                        {
                            EduUninvesity.Degree = edu1[0].Trim();
                            if (edu1[1].Length >= 2 && edu1[1].Contains("-"))
                            {
                                string[] edu2 = edu1[1].Split("-");

                                EduUninvesity.SDate = edu2[0].Replace("(", "").Trim();
                                EduUninvesity.EDate = edu2[1].Replace(")", "").Trim();
                            }
                        }
                        else
                        {
                            EduUninvesity.Degree = string.Join("\n", copy).Replace("\n", " ");
                        }
                        Format.Educations.Add(EduUninvesity);
                    }
                }

                a = 0;

                return Format;
            }
        }

        private static StringBuilder GetInfo(FormatLinkedin Format, StringBuilder sbInfo, string LastInfoKeyString, string LastInfoKeyStringOrg)
        {
            switch (LastInfoKeyString)
            {
                case "聯絡":
                    Format.Contact = sbInfo.ToString().Substring(LastInfoKeyStringOrg.Length);
                    sbInfo = new StringBuilder();
                    break;
                case "熱門技能":
                    Format.Skills = sbInfo.ToString().Substring(LastInfoKeyStringOrg.Length);
                    sbInfo = new StringBuilder();
                    break;
                case "Languages":
                    Format.LanguagesAll = sbInfo.ToString().Substring(LastInfoKeyStringOrg.Length);
                    sbInfo = new StringBuilder();
                    break;
                case "Certifications":
                    Format.Certifications = sbInfo.ToString().Substring(LastInfoKeyStringOrg.Length);
                    sbInfo = new StringBuilder();
                    break;
                case "Honors-Awards":
                    Format.HonorsAwards = sbInfo.ToString().Substring(LastInfoKeyStringOrg.Length);
                    sbInfo = new StringBuilder();
                    break;
                //case "簡介":
                //    Format.UserName = Infos[i - 1];
                //    Format.Introduction = sbInfo.ToString().Substring(LastInfoKeyStringOrg.Length);
                //    sbInfo = new StringBuilder();
                //    break;
                default:
                    break;
            }

            return sbInfo;
        }
    }
}


