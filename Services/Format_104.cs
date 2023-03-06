using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace PdfParser.Format104
{
    public class Format
    {
        public string UserName { get; set; }
        public string UserAge { get; set; }
        public string UserGender { get; set; }
        public string UserCode { get; set; }
        public string UserHighestEducation { get; set; }
        public string UserHopeJobTitle { get; set; }
        public string UserYearsOfTotalExperience { get; set; }
        public string UserLastJob { get; set; }
        public string UserResidence { get; set; }
        public string UserEmail { get; set; }
        public string UserTEL { get; set; }
        public string UserContactInformation { get; set; }
        public string UserUpdateTime { get; set; }
        public string UserApplyTime { get; set; }
        public string UserGetTime { get; set; }
        public UserApplyInfo UserApplyInfo { get; set; }
        public List<Apply> Applys { get; set; }
        //public WorkExpSummery WorkExpSummery { get; set; }
        public string WorkTotalExps { get; set; }
        public List<WorkExp> WorkExps { get; set; }
        public Education Education { get; set; }
        public PersonInformation PersonInformation { get; set; }
        public JobCondition JobCondition { get; set; }
        public List<string> Languages { get; set; }
        public string SkillLicenses { get; set; }
        public SkillLicenseDetail SkillLicenseDetail { get; set; }
        public string Autobiography { get; set; }
        public string Attachments { get; set; }
    }

    public class SkillLicenseDetail
    {
        public List<string> Tools { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Licenses { get; set; }
    }

    public class UserApplyInfo
    {
        public string ApplyJob { get; set; }
        public string SelfDesc { get; set; }
    }

    public class Apply
    {
        public string ApplyTime { get; set; }
        public string ApplyJob { get; set; }
    }

    //public class WorkExpSummery
    //{
    //    public string WorkYear { get; set; }
    //    public string WorkSummery { get; set; }
    //}

    public class WorkExp
    {
        public string WorkTitle { get; set; }
        public WorkTitleDetail WorkTitleDetail { get; set; } = new WorkTitleDetail();
        public string WorkIndustryCategory { get; set; }
        public string WorkJobCategory { get; set; }
        public string ScaleOfEnterprise { get; set; }
        public string ManagementResponsibility { get; set; }
        public string Salary { get; set; }
        public string WorkContent { get; set; }
        public string WorkSkill { get; set; }
    }

    public class WorkTitleDetail
    {
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TotalYear { get; set; }
    }

    public class Education
    {
        public string Degree { get; set; }
        public string Top { get; set; }
        public string Second { get; set; }
        public string Other { get; set; }
        public List<EduUninvesity> EduUninvesity { get; set; }
    }

    public class EduUninvesity
    {
        public string UninvesityCollege { get; set; }
        public UninvesityCollegeDetail UninvesityCollegeDetail { get; set; } = new UninvesityCollegeDetail();
        //public string College { get; set; }
        public string SDate { get; set; }
        public string EDate { get; set; }
        public string MEMO { get; set; }
    }

    public class UninvesityCollegeDetail
    {
        public string Uninvesity { get; set; }
        public string major { get; set; }
    }
    public class PersonInformation
    {
        public string EmployedStatus { get; set; }
        public string UserEngName { get; set; }
        public string Birthdate { get; set; }
        public string MarriageStatus { get; set; }
        public string BodyWeight { get; set; }
        public string BodyHeight { get; set; }
        public string VeteranStatus { get; set; }
        public List<string> DriverLicences { get; set; }
        public List<string> Transportations { get; set; }
    }

    public class JobCondition
    {
        public string JobFullPartTime { get; set; }
        public string JobTitle { get; set; }
        public string JobContent { get; set; }
        public string JobCategory { get; set; }
        public string JobIndustry { get; set; }
        public List<string> JobCounties { get; set; }
        public string JobPay { get; set; }
        public string AvailableDate { get; set; }
        public string JobSchedule { get; set; }
    }

    class Body
    {
        public static Format Format(Stream pdfFileStream)
        {
            //第一階段 基本拆分
            //string[] keyWords = {
            //    //"，以避免觸犯個資法。",
            //    //"歲",
            //    //"代碼",
            //    //"最高學歷",
            //    //"希望職稱",
            //    //"總年資",
            //    //"最近工作",
            //    //"居住地",
            //    //"E-mail",
            //    //"聯絡電話",
            //    //"聯絡方式",
            //    //"更新日",
            //    //"應徵",
            //    "工作經歷",
            //    "教育背景",
            //    "個人資料",
            //    "求職條件",
            //    "語文能力",
            //    "技能專長",
            //    "自傳",
            //    "附件"
            //};

            string[][] keyWords = {
                new string[]{ "工作經歷", "⼯作經歷"}, //兩個工 ⼯ utf8 編碼不同
                new string[]{ "教育背景"},
                new string[]{ "個人資料","個⼈資料"},  //兩個人 ⼈ utf8 編碼不同
                new string[]{ "求職條件"},
                new string[]{ "語文能力","語文能⼒"},//兩個力 ⼒ utf8 編碼不同
                new string[]{ "技能專長","技能專⻑"}, //兩個長 ⻑ utf8 編碼不同
                new string[]{ "自傳","⾃傳"},//兩個自 ⾃ utf8 編碼不同
                new string[]{ "附件" }
            };

            int keyWordsIndex = 1;

            Dictionary<string, string> keyWordsMap = new Dictionary<string, string>();
            //string stringEnd = "";
            string LastKeyString = keyWords[0][0];

            string FirstHalfText = "";
            string testUserKey = "";
            string useCompany = "";
            var is104Ext=false;
            using (var document = PdfDocument.Open(pdfFileStream)) {
                StringBuilder sb = new StringBuilder();

                for (var i = 0; i < document.NumberOfPages; i++) {
                    var page = document.GetPage(i + 1);

                    //string pagetext = page.Text;
                    //https://github.com/UglyToad/PdfPig/blob/master/examples/ExtractTextWithNewlines.cs
                    string pagetext = ContentOrderTextExtractor.GetText(page, true);
                    //Console.WriteLine(pagetext);
                    //Console.WriteLine("---");
                    pagetext = pagetext.Replace(@$"https://pro.104.com.tw/vip/ResumeTools/resumePreview {i + 1}/{document.NumberOfPages}", "");
                    //https://pro.104.com.tw/vip/ResumeTools/resumePreview 2/3

                    //https://pro.104.com.tw/vip/ResumeTools/resumePreview?pageSource=hunter&searchEngineIdNos=&snapshotIds=547343&jobNo=&ec=11 2/4
                    //pagetext = Regex.Replace(pagetext, @$"https://pro.104.com.tw/vip/ResumeTools/resumePreview?pageSource=hunter&searchEngineIdNos=&snapshotIds=[^0-9]&jobNo=&ec=[^0-9] {i + 1}/{document.NumberOfPages}", String.Empty);
                    //https://pro.104.com.tw/vip/ResumeTools/resumePreview?pageSource=hunter&searchEngineIdNos=&snapshotIds=549118&jobNo=&ec=11
                    pagetext = Regex.Replace(pagetext, @$"https://pro.104.com.tw/vip/ResumeTools/resumePreview\?pageSource=hunter&searchEngineIdNos=&snapshotIds=[0-9]*&jobNo=&ec=[0-9]* {i + 1}/{document.NumberOfPages}", String.Empty);

                    if (Regex.IsMatch(pagetext, @$"履歷使⽤公司:")) {
                        string[] split = Regex.Split(pagetext.Substring(pagetext.IndexOf("履歷使⽤公司:") + 7), "使⽤⼈員:");
                        if (split != null && split.Length >= 1) {
                            useCompany = split[0].Trim();
                        }
                    }

                    pagetext = pagetext.Replace("\r\n" + useCompany, "");

                    if (pagetext.Contains("列印預覽| 104 招募管理")) {
                        string printDate = pagetext.Substring(0, pagetext.IndexOf("列印預覽| 104 招募管理")).Trim();
                        if (printDate?.Length <= 10) {
                            pagetext = pagetext.Replace(printDate, "");
                        }
                        pagetext = pagetext.Replace("列印預覽| 104 招募管理", "");
                    }
                    if (pagetext.Contains("列印預覽 | 104招募管理")) {
                        string printDate = pagetext.Substring(0, pagetext.IndexOf("列印預覽 | 104招募管理")).Trim();
                        if (printDate?.Length <= 18) {
                            pagetext = pagetext.Replace(printDate, "");
                        }
                        pagetext = pagetext.Replace("列印預覽 | 104招募管理", "");
                    }
                    if (pagetext.Contains("本系統提供之履歷僅供徵才⽬的使⽤，請勿違法蒐集或利⽤，以免觸犯個⼈資料保護法")) {
                        pagetext = pagetext.Substring(0, pagetext.LastIndexOf("本系統提供之履歷僅供徵才⽬的使⽤，請勿違法蒐集或利⽤，以免觸犯個⼈資料保護法"));
                    }
                    if (pagetext.Contains("符合分析")) {
                        var kw = pagetext.IndexOf("符合分析");
                        var s = pagetext.IndexOf("代碼:");
                        var e = pagetext.IndexOf("聯絡⽅式");

                        ///0412
                        //if (e==-1) {
                        //    e = pagetext.IndexOf("聯絡電話");
                        //}


                        var userinfotext = pagetext.Substring(s - 10, e - (s - 10));
                        var except = pagetext.Remove(kw, pagetext.Length - kw);
                        var secpagetext = ContentOrderTextExtractor.GetText(document.GetPage(2), true);
                        pagetext = except + secpagetext + "\r\n\r\n" + userinfotext;
                        is104Ext = true;
                    }
                    //pagetext = pagetext.Replace("104⼈⼒銀⾏ © 2020", "");


                    //本系統提供之履歷僅供徵才⽬的使⽤，請勿違法蒐集或利⽤，以免觸犯個⼈資料保護法  104⼈⼒銀⾏ © 2020

                    //if (pagetext.Contains("根據此份履歷的學歷、經歷、技能等指標，與您的職務所需條件進⾏比對分析結果。")) {
                    //   new Format_104_Ext().ParsePDF(document);
                    //}

                    char[] arrayOfCharacters = pagetext.ToCharArray();

                    if (i == 0) {

                        //pagetext.IndexOf("⼯作經歷")
                        //var utf8 = Encoding.UTF8;
                        //byte[] utfBytes1 = Encoding.UTF8.GetBytes("工");
                        //byte[] utfBytes2 = Encoding.UTF8.GetBytes("⼯");
                        int IndexOf = 0;
                        foreach (var item in keyWords[0]) {
                            int tempIndex = pagetext.IndexOf(item);
                            if (tempIndex >= 0) {
                                IndexOf = tempIndex;
                            }
                        }
                        FirstHalfText = pagetext.Substring(0, IndexOf - 1);
                        string SecondHalfText = pagetext.Substring(IndexOf);
                        arrayOfCharacters = SecondHalfText.ToCharArray();

                    }


                    foreach (var onechar in arrayOfCharacters) {
                        sb.Append(onechar);
                        if (keyWordsIndex < keyWords.Length) {
                            //if (keyWordsIndex == 13)
                            //{
                            //    int p = 0;
                            //}

                            //if (keyWordsIndex == 14)
                            //{
                            //    Console.WriteLine(sb.ToString());
                            //}

                            /*
                            //if (sb.ToString().EndsWith(keyWords[keyWordsIndex]))
                            if (keyWords[keyWordsIndex].Any(x => sb.ToString().EndsWith(x)))
                            {
                                if (keyWordsIndex > 0)
                                {
                                    if (!keyWordsMap.ContainsKey(keyWords[keyWordsIndex - 1]))
                                    {
                                        keyWordsMap[keyWords[keyWordsIndex - 1]] = sb.ToString().Substring(0, sb.ToString().Length - keyWords[keyWordsIndex].Length);
                                    }
                                }

                                //Console.WriteLine("---"+ keyWords[(keyWordsIndex ==0?0: keyWordsIndex - 1)] + "---"+ keyWords[keyWordsIndex] + "---");
                                //Console.WriteLine(sb.ToString());
                                //Console.WriteLine("---");
                                sb = new StringBuilder();
                                keyWordsIndex++;
                            }
                            */
                            //if (keyWordsIndex > 0)
                            //{
                            if (!keyWords[keyWordsIndex].Any(x => keyWordsMap.ContainsKey(x))) {
                                //keyWordsMap[LastKeyString] = sb.ToString().Substring(0, sb.ToString().Length - LastKeyString.Length - 1);
                                if (!string.IsNullOrWhiteSpace(sb.ToString())) {
                                    keyWordsMap[LastKeyString] = sb.ToString();
                                    foreach (var item in keyWords[keyWordsIndex]) {
                                        if (keyWordsMap[LastKeyString].LastIndexOf(item) > 0) {
                                            keyWordsMap[LastKeyString] = keyWordsMap[LastKeyString].Substring(0, keyWordsMap[LastKeyString].LastIndexOf(item));
                                            if (keyWordsMap[LastKeyString].Contains("聯絡電話")) {
                                                testUserKey = LastKeyString;
                                            }
                                            sb = new StringBuilder();
                                            LastKeyString = keyWords[keyWordsIndex][0];
                                            keyWordsIndex++;
                                            break;
                                        }
                                        if (is104Ext&&i==0) {
                                            i = 1;
                                        }
                                    }
                                }

                            }
                            //}  
                        }
                        //else
                        //{
                        //    stringEnd = sb.ToString();
                        //}
                    }
                }

                keyWordsMap[LastKeyString] = sb.ToString();

                int a = 0;

                //第二階段 細部拆分
                keyWordsIndex = 0;
                Format Format = new Format();

                Dictionary<string, string> InfoWords =
                    new Dictionary<string, string>
                        {
                            //{ "以避免觸犯個資法。", "以避免觸犯個資法。" },
                            { "歲", "歲" },
                            { "代碼", "代碼" },
                            { "最高學歷", "最高學歷" },//高 ⾼ 不同字
                            { "最⾼學歷", "最高學歷" },
                            { "希望職稱", "希望職稱" },
                            { "總年資", "總年資" },
                            { "最近工作", "最近工作" },
                            { "最近⼯作", "最近工作" },//工 ⼯ 不同字
                            { "居住地", "居住地" },
                            { "E-mail", "E-mail" },
                            { "聯絡電話", "聯絡電話" },
                            { "聯絡方式", "聯絡方式" },//方 ⽅ 不同字
                            { "聯絡⽅式", "聯絡方式" },
                            { "更新日", "更新日" },
                            { "取得日期", "取得日期" },//日 ⽇ 不同字
                            { "取得⽇期", "取得日期" },
                            { "應徵日期", "應徵日期" },//日 ⽇ 不同字
                            { "應徵⽇期", "應徵日期" },
                        };

                string[] Infos = FirstHalfText.Split("應徵資訊");
                string[] InfosA = Infos[0].Split(Environment.NewLine);
                if (Infos.Length > 1) {
                    Format.UserApplyInfo = new UserApplyInfo();
                    foreach (var item in Infos[1].Split(Environment.NewLine)) {
                        if (string.IsNullOrWhiteSpace(item)) {
                            continue;
                        }

                        if (item.IndexOf(" ") < 0) {
                            Format.UserApplyInfo.SelfDesc += Environment.NewLine + item;
                            continue;
                        }

                        string InfosADesc1 = item.Substring(0, item.IndexOf(" "));
                        string InfosADesc2 = item.Substring(item.IndexOf(" ")).Trim();

                        switch (InfosADesc1) {
                            case "應徵職務":
                                Format.UserApplyInfo.ApplyJob = InfosADesc2;
                                break;
                            case "⾃我推薦":
                            case "自我推薦":
                                Format.UserApplyInfo.SelfDesc = InfosADesc2;
                                break;
                            default:
                                Format.UserApplyInfo.SelfDesc += Environment.NewLine + item;
                                break;
                        }
                    }
                }

                bool keepFlag = false;
                string keepString = "";

                for (int i = 0; i < InfosA.Length; i++) {
                    string InfosADesc = InfosA[i]?.Trim();

                    if (InfoWords.Keys.Any(x => InfosADesc.Contains(x))) {
                        string InfosADesc1 = InfosADesc.Substring(0, InfosADesc.IndexOf(" "));
                        string InfosADesc2 = InfosADesc.Substring(InfosADesc.IndexOf(" ")).Trim();

                        switch (InfosADesc1) {
                            //case "歲":
                            //    Format.UserName = InfosADesc2;
                            //    break;
                            case "代碼":
                                //case "代碼：":
                                Format.UserCode = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "最高學歷":
                            case "最⾼學歷":
                                Format.UserHighestEducation = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "希望職稱":
                                Format.UserHopeJobTitle = InfosADesc2;
                                keepFlag = true;
                                keepString = "";
                                break;
                            case "總年資":
                                Format.UserYearsOfTotalExperience = InfosADesc2;
                                if (keepFlag) {
                                    Format.UserHopeJobTitle += keepString; //如果順序一定是接續 希望職稱
                                }
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "最近工作":
                            case "最近⼯作":
                                Format.UserLastJob = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "居住地":
                                Format.UserResidence = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "E-mail":
                                Format.UserEmail = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "聯絡電話":
                                Format.UserTEL = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "聯絡方式":
                            case "聯絡⽅式":
                                Format.UserContactInformation = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "更新日":
                                Format.UserUpdateTime = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "取得⽇期：":
                            case "取得日期":
                                Format.UserGetTime = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            default:
                                keepFlag = false;
                                keepString = "";
                                break;
                        }

                        if (InfosADesc.Contains("歲")) {
                            string[] InfosADesc3 = InfosADesc.Split(" ");
                            Format.UserName = InfosADesc3[0];
                            Format.UserAge = InfosADesc3[1];
                            Format.UserGender = InfosADesc3[2];
                            if (InfosADesc3.Length == 4 && InfosADesc3[3].StartsWith("代碼")) {
                                Format.UserCode = InfosADesc3[3].Split("：")[1];
                            }
                        }

                        if (InfosADesc.StartsWith("更新日：")) {
                            string[] InfosADesc3 = InfosADesc.Substring(InfosADesc.IndexOf("更新日：") + 4).Split(Environment.NewLine);
                            Format.UserUpdateTime = InfosADesc3[0]?.Trim();
                        }

                        if (InfosADesc.StartsWith("取得⽇期：")) {
                            string[] InfosADesc3 = InfosADesc.Substring(InfosADesc.IndexOf("取得⽇期：") + 5).Split(Environment.NewLine);
                            Format.UserGetTime = InfosADesc3[0]?.Trim();
                        }

                        if (InfosADesc.StartsWith("應徵⽇期：")) {
                            string[] InfosADesc3 = InfosADesc.Substring(InfosADesc.IndexOf("應徵⽇期：") + 5).Split(Environment.NewLine);
                            Format.UserApplyTime = InfosADesc3[0]?.Trim();
                        }

                        if (InfosADesc.StartsWith("代碼：")) {
                            string[] InfosADesc3 = InfosADesc.Substring(InfosADesc.IndexOf("代碼：") + 3).Split("應徵⽇期：");
                            Format.UserCode = InfosADesc3[0]?.Trim();
                            if (InfosADesc3.Length > 1) {
                                //應徵日期
                                Format.UserApplyTime = InfosADesc3[1]?.Trim();
                            }
                            InfosADesc3 = InfosADesc.Substring(InfosADesc.IndexOf("代碼：") + 3).Split("更新⽇：");//日是特別字
                            if (InfosADesc3.Length > 1) {
                                Format.UserCode = InfosADesc3[0]?.Trim();
                                //更新日
                                Format.UserUpdateTime = InfosADesc3[1]?.Trim();
                            }
                        }
                    } else {
                        if (keepFlag) {
                            keepString += InfosADesc;
                        }
                    }


                }

                /*
                //2.1 姓名 歲 代碼 最高學歷 希望職稱 總年資 最近工作 居住地 E-mail 聯絡電話 聯絡方式 更新日
                string[] userName = keyWordsMap[keyWords[keyWordsIndex++]].Split(' ');
                Format.UserName = userName[0].Trim();
                Format.UserAge = userName[1].Trim();
                Format.UserGender = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserCode = keyWordsMap[keyWords[keyWordsIndex++]].Trim().Replace("：", "");
                Format.UserHighestEducation = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserHopeJobTitle = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserYearsOfTotalExperience = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserLastJob = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserResidence = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserEmail = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserTEL = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserContactInformation = keyWordsMap[keyWords[keyWordsIndex++]].Trim();
                Format.UserUpdateTime = keyWordsMap[keyWords[keyWordsIndex++]].Trim().Replace("：", "");
                string temp = "";
                if (Format.UserUpdateTime.Length > 16)
                {
                    temp = Format.UserUpdateTime.Substring(16);
                    Format.UserUpdateTime = Format.UserUpdateTime.Substring(0, 16);
                }
                

                //2.2 應徵
                
                string[] Applys = keyWordsMap[keyWords[keyWordsIndex++]].Split("應徵");
                Format.Applys = new List<Apply>();
                for (int i = 0; i < Applys.Length; i++)
                {
                    Apply apply = new Apply();
                    apply.ApplyTime = temp.Trim();
                    apply.ApplyJob = Applys[i].Trim();

                    if (i == 0)
                    {
                        if (Applys[0].Length >= 17)
                        {
                            temp = Applys[0].Substring(Applys[0].Length - 17).Trim();
                            apply.ApplyJob = Applys[i].Substring(0, Applys[i].Length - 17).Trim();
                        }
                    }
                    else
                    {

                        if (Applys[i - 1].Length >= 17)
                        {
                            apply.ApplyTime = Applys[i - 1].Substring(Applys[i - 1].Length - 17).Trim();
                            apply.ApplyJob = Applys[i].Substring(0, Applys[i].Length - 17).Trim();
                        }

                    }
                    Format.Applys.Add(apply);
                }
                */

                //2.3 工作經歷
                keyWordsMap[keyWords[keyWordsIndex][0]] = keyWordsMap[keyWords[keyWordsIndex][0]].Replace("⼯", "工").Replace("⽉", "月");
                string[] WorkExps = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(Environment.NewLine + Environment.NewLine);

                Dictionary<string, string> WorkWords =
                    new Dictionary<string, string>
                        {
                            { "總年資", "總年資" },
                            { "年)", "年)" },//年)
                            { "個月", "個月" },//個⽉
                            { "仍在職", "仍在職" },
                            { "產業類別", "產業類別" },
                            { "職務類別", "職務類別" },
                            { "公司規模 ", "公司規模 " },
                            { "管理責任", "管理責任" },
                            { "工作待遇", "工作待遇" },
                            { "工作內容 ", "工作內容 " },
                            { "工作技能", "工作技能" },
                        };

                List<string> newWorkExpsList = new List<string>();
                List<string> newSumList = new List<string>();
                int testIndex = 0;
                for (int i = 0; i < WorkExps.Length; i++) {
                    if (string.IsNullOrWhiteSpace(WorkExps[i])) {
                        continue;
                    }
                    if (WorkExps[i] == "最⾼學歷") {
                        testIndex = i;
                    }
                }

                for (int i = 0; i < WorkExps.Length; i++) {
                    if (string.IsNullOrWhiteSpace(WorkExps[i])) {
                        continue;
                    }

                    if (i >= 9 && i >= (testIndex - 9) && i <= (testIndex + 7)) {
                        if (i >= (testIndex - 9)) {
                            newSumList.Add(WorkExps[i]);
                        }
                    } else {
                        newWorkExpsList.Add(WorkExps[i]);
                    }

                }
                /*
                for (int i = 0; i < newSumList.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            //Format.UserHighestEducation = newSumList[i];
                            string[] splits = newSumList[i].Split(" ");
                            if (splits != null && splits.Length >= 3)
                            {
                                Format.UserName = splits[0];
                                Format.UserAge = splits[1];
                                Format.UserGender = splits[2];
                            }
                            break;
                        case 1:
                            Format.UserHighestEducation = newSumList[i];
                            break;
                        case 2:
                            Format.UserHopeJobTitle = newSumList[i];
                            break;
                        case 3:
                            Format.UserYearsOfTotalExperience = newSumList[i];
                            break;
                        case 4:
                            Format.UserLastJob = newSumList[i];
                            break;
                        case 5:
                            Format.UserResidence = newSumList[i];
                            break;
                        case 6:
                            Format.UserEmail = newSumList[i];
                            break;
                        case 7:
                            Format.UserTEL = newSumList[i];
                            break;
                        case 8:
                            Format.UserContactInformation = newSumList[i];
                            if (Format.UserContactInformation.Contains("\r\n"))
                            {
                                splits = Format.UserContactInformation.Split("\r\n");
                                if (splits != null && splits.Length == 2)
                                {
                                    Format.UserContactInformation = splits[0];
                                    Format.UserCode = splits[1];
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                */
                WorkExps = newWorkExpsList.ToArray();

                Format.WorkExps = new List<WorkExp>();
                WorkExp workExp = new WorkExp();
                bool SubEnd = false;
                for (int i = 0; i < WorkExps.Length; i++) {
                    if (string.IsNullOrWhiteSpace(WorkExps[i])) {
                        continue;
                    }

                    if (WorkExps[i].StartsWith("https://pro.104.com.tw/vip/ResumeTools/resumePreview?pageSource=hunter&searchEngineIdNos")) {
                        if (WorkExps[i].Contains("\r\n")) {
                            WorkExps[i] = WorkExps[i].Substring(WorkExps[i].IndexOf("\n") + 1);
                        }
                    }

                    string InfosADesc = WorkExps[i]?.Trim();

                    if (WorkWords.Keys.Any(x => InfosADesc.Contains(x))) {
                        string InfosADesc1 = InfosADesc.Substring(0, InfosADesc.IndexOf(" "));
                        string InfosADesc2 = InfosADesc.Substring(InfosADesc.IndexOf(" ")).Trim();

                        switch (InfosADesc1) {
                            case "總年資":
                                Format.WorkTotalExps = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "產業類別":
                                workExp.WorkIndustryCategory = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                SubEnd = false;
                                break;
                            case "職務類別":
                                workExp.WorkJobCategory = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "公司規模":
                                workExp.ScaleOfEnterprise = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                break;
                            case "管理責任":
                                workExp.ManagementResponsibility = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                //SubEnd = true;
                                break;
                            case "工作待遇":
                                workExp.Salary = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                //SubEnd = true;
                                break;
                            case "工作技能":
                                workExp.WorkSkill = InfosADesc2;
                                keepFlag = false;
                                keepString = "";
                                //SubEnd = true;
                                break;
                            case "工作內容":
                                workExp.WorkContent = InfosADesc2;
                                keepFlag = true;
                                keepString = "";
                                //SubEnd = true;
                                break;
                            default:
                                keepFlag = false;
                                workExp.WorkContent += keepString;
                                keepString = "";
                                break;
                        }

                        workExp.WorkContent += keepString;

                        Regex regYear = new Regex("20\\d{2}|19\\d{2}");
                        if ((InfosADesc.Contains("個月") || InfosADesc.Contains("仍在職") || InfosADesc.Contains("年)")) && regYear.IsMatch(InfosADesc)) {
                            if (!string.IsNullOrWhiteSpace(workExp.WorkTitle)) {
                                Format.WorkExps.Add(workExp);
                                workExp = new WorkExp();
                            }

                            workExp.WorkTitle = InfosADesc;
                            //string[] InfosADesc3 = InfosADesc.Split(" ");
                            //Format.UserName = InfosADesc3[0];
                            //Format.UserAge = InfosADesc3[1];
                            //Format.UserGender = InfosADesc3[2];
                            //if (InfosADesc3.Length == 4 && InfosADesc3[3].StartsWith("代碼"))
                            //{
                            //    Format.UserCode = InfosADesc3[3].Split("：")[1];
                            //}
                        }

                    } else {
                        if (keepFlag) {
                            keepString += InfosADesc;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(workExp.WorkTitle)) {
                    Format.WorkExps.Add(workExp);
                }
                //int aaaa = 0;

                foreach (var item in Format.WorkExps) {
                    if (!string.IsNullOrWhiteSpace(item.WorkTitle)) {
                        string[] split = item.WorkTitle.Split("\r\n");
                        if (split != null && split.Length >= 2) {
                            item.WorkTitle = split[0];
                            if (split[1] != null && split[1].Contains(")")) {
                                item.WorkTitle = split[0] + split[1];
                            }
                        }

                        string[] temp = item.WorkTitle.Split(" ");
                        if (temp.Length == 4) {
                            item.WorkTitleDetail.CompanyName = temp[0].Trim();
                            item.WorkTitleDetail.JobTitle = temp[1].Trim();
                            if (temp[2].Trim().Contains("~")) {
                                string[] temp2 = temp[2].Trim().Split("~");
                                if (temp2.Length == 2) {
                                    item.WorkTitleDetail.StartDate = temp2[0].Trim();
                                    item.WorkTitleDetail.EndDate = temp2[1].Trim();
                                }
                            } else {
                                item.WorkTitleDetail.StartDate = temp[2].Trim();
                            }
                            item.WorkTitleDetail.TotalYear = temp[3].Replace("(", "").Replace(")", "").Trim();
                        }
                        if (temp.Length == 3) {
                            item.WorkTitleDetail.CompanyName = temp[0].Trim();
                            item.WorkTitleDetail.JobTitle = temp[1].Trim();
                            if (temp[2].Trim().Contains("~")) {
                                string[] temp2 = temp[2].Trim().Split("~");
                                if (temp2.Length == 2) {
                                    item.WorkTitleDetail.StartDate = temp2[0].Trim();
                                    item.WorkTitleDetail.EndDate = temp2[1].Trim();
                                }
                            } else {
                                item.WorkTitleDetail.StartDate = temp[2].Trim();
                            }
                            //item.WorkTitleDetail.TotalYear = temp[3].Replace("(", "").Replace(")", "").Trim();
                        }
                        if (temp.Length >= 5) {
                            item.WorkTitleDetail.CompanyName = temp[0].Trim();
                            //item.WorkTitleDetail.JobTitle = temp[1].Trim();
                            if ((temp.Length - 2) > 0) {
                                int plusIndex = 1;
                                while (plusIndex < temp.Length && temp[plusIndex].Trim() != null && !temp[plusIndex].Trim().StartsWith("20")) {
                                    item.WorkTitleDetail.JobTitle += temp[plusIndex];
                                    plusIndex++;
                                }
                            }

                            if (temp[temp.Length - 1].Contains("~")) {
                                string[] temp2 = temp[temp.Length - 1].Trim().Split("~");
                                if (temp2.Length == 2) {
                                    item.WorkTitleDetail.StartDate = temp2[0].Trim();
                                    item.WorkTitleDetail.EndDate = temp2[1].Trim();
                                }
                            } else if (temp[temp.Length - 2].Contains("~")) {
                                string[] temp2 = temp[temp.Length - 2].Trim().Split("~");
                                if (temp2.Length == 2) {
                                    item.WorkTitleDetail.StartDate = temp2[0].Trim();
                                    item.WorkTitleDetail.EndDate = temp2[1].Trim();
                                }
                                item.WorkTitleDetail.TotalYear = temp[temp.Length - 1].Replace("(", "").Replace(")", "").Trim();
                            }
                        }
                    }

                    if (item.WorkTitleDetail.TotalYear != null && item.WorkTitleDetail.TotalYear.Contains("\r\n")) {
                        string[] split = item.WorkTitleDetail.TotalYear.Split("\r\n");
                        if (split != null && split.Length >= 2) {
                            item.WorkTitleDetail.TotalYear = split[0];
                        }
                    }
                }



                /*
                string[] WorkExps = keyWordsMap[keyWords[keyWordsIndex++][0]].Split("產業類別");
                Format.WorkExps = new List<WorkExp>();
                WorkExp workExp = new WorkExp();
                string WorkTitle = "";
                for (int i = 0; i < WorkExps.Length; i++)
                {
                    workExp = new WorkExp();
                    if (i == 0)
                    {
                        string[] work0 = WorkExps[0].Split(Environment.NewLine + Environment.NewLine);
                        if (work0.Length >= 4)
                        {
                            WorkTitle = work0[work0.Length - 2].Trim();
                        }
                    }
                    else
                    {
                        //public string WorkIndustryCategory { get; set; }
                        //public string WorkJobCategory { get; set; }
                        //public string ScaleOfEnterprise { get; set; }
                        //public string WorkContent { get; set; }

                        workExp.WorkTitle = WorkTitle.Trim();
                        WorkTitle = "";
                        string[] work0 = WorkExps[i].Split(Environment.NewLine + Environment.NewLine);
                        if (work0.Length >= 4)
                        {
                            WorkTitle = work0[work0.Length - 2];
                        }

                        string[] work1 = WorkExps[i].Split("職務類別");
                        if (work1.Length >= 2)
                        {
                            workExp.WorkIndustryCategory = work1[0].Trim();
                            string[] work2 = work1[1].Split("公司規模");
                            workExp.WorkJobCategory = work2[0].Trim();
                            string[] work3 = work2[1].Split("工作內容");//⼯作內容 工不一樣
                            workExp.ScaleOfEnterprise = work3[0].Trim();
                            if (work3.Length > 1)
                            {
                                workExp.WorkContent = work3[1].Trim();
                            }
                            if (workExp.ScaleOfEnterprise.Contains("⼯作內容"))
                            {
                                work3 = work2[1].Split("⼯作內容");
                                workExp.ScaleOfEnterprise = work3[0].Trim();
                                if (work3.Length > 1)
                                {
                                    workExp.WorkContent = work3[1].Trim();
                                }
                            }

                            string[] work4 = workExp.WorkContent?.Split(Environment.NewLine + Environment.NewLine);
                            if (work4?.Length > 2)
                            {
                                workExp.WorkContent = workExp.WorkContent.Substring(0, workExp.WorkContent.LastIndexOf(work4[work4.Length - 2])).Trim();
                            }
                        }

                        Format.WorkExps.Add(workExp);
                    }


                }
                */
                //2.4 教育背景
                keyWordsMap[keyWords[keyWordsIndex][0]] = keyWordsMap[keyWords[keyWordsIndex][0]].Replace("⾼", "高");
                string[] Educations = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(Environment.NewLine + Environment.NewLine);
                Format.Education = new Education();
                List<string> Unvs = new List<string>();
                for (int i = 0; i < Educations.Length; i++) {
                    if (string.IsNullOrWhiteSpace(Educations[i])) {
                        continue;
                    } else if (Educations[i].Trim().StartsWith("最高學歷")) {
                        string[] edu0 = Educations[i].Split(" ");
                        if (edu0.Length >= 2) {
                            Format.Education.Degree = edu0[1].Trim();
                        }
                    } else if (Educations[i].Trim().StartsWith("最　　高")) {
                        string[] edu0 = Educations[i].Split("最　　高");
                        if (edu0.Length >= 2) {
                            Format.Education.Top = edu0[1].Trim();
                            Unvs.Add(Format.Education.Top);
                        }
                    } else if (Educations[i].Trim().StartsWith("次　　高")) {
                        string[] edu0 = Educations[i].Split("次　　高");
                        if (edu0.Length >= 2) {
                            Format.Education.Second = edu0[1].Trim();
                            Unvs.Add(Format.Education.Second);
                        }
                    } else if (Educations[i].Trim().StartsWith("其　　他")) {
                        string[] edu0 = Educations[i].Split("其　　他");
                        if (edu0.Length >= 2) {
                            Format.Education.Other = edu0[1].Trim();
                            Unvs.Add(Format.Education.Other);
                        }
                    } else if (Educations[i].Trim().StartsWith("其他")) {
                        string[] edu0 = Educations[i].Trim().Split("其他");
                        if (edu0.Length >= 2) {
                            Format.Education.Other = edu0[1].Trim();
                            Unvs.Add(Format.Education.Other);
                        }
                    }
                    Format.Education.EduUninvesity = new List<EduUninvesity>();
                    foreach (var Unv in Unvs) {
                        EduUninvesity EduUninvesity = new EduUninvesity();

                        EduUninvesity.UninvesityCollege = Unv.Substring(0, Unv.LastIndexOf("(")).Trim();
                        string[] eduDate = Unv.Substring(Unv.LastIndexOf("(")).Split("~");
                        if (eduDate.Length >= 2) {
                            EduUninvesity.SDate = eduDate[0].Replace("(", "");
                            EduUninvesity.EDate = eduDate[1].Replace(")", "");
                            string[] edu0 = EduUninvesity.EDate.Trim().Split(" ");
                            if (edu0.Length >= 2) {
                                EduUninvesity.EDate = edu0[0].Trim();
                                EduUninvesity.MEMO = edu0[1].Replace(")", "");
                            } else {
                                if (EduUninvesity.EDate.Contains("畢業")) {
                                    EduUninvesity.EDate = EduUninvesity.EDate.Substring(0, EduUninvesity.EDate.IndexOf("畢業")).Trim();
                                    EduUninvesity.MEMO = "畢業";
                                }
                                if (EduUninvesity.EDate.Contains("肄業")) {
                                    EduUninvesity.EDate = EduUninvesity.EDate.Substring(0, EduUninvesity.EDate.IndexOf("肄業")).Trim();
                                    EduUninvesity.MEMO = "肄業";
                                }
                            }
                        }

                        Format.Education.EduUninvesity.Add(EduUninvesity);
                    }
                }

                foreach (var item in Format.Education.EduUninvesity) {
                    if (!string.IsNullOrWhiteSpace(item.UninvesityCollege)) {
                        string[] temp = item.UninvesityCollege.Split(" ");
                        if (temp.Length == 2) {
                            item.UninvesityCollegeDetail.Uninvesity = temp[0].Trim();
                            item.UninvesityCollegeDetail.major = temp[1].Trim();
                        }
                        if (temp.Length == 3) {
                            item.UninvesityCollegeDetail.Uninvesity = temp[0].Trim();
                            item.UninvesityCollegeDetail.major = temp[1].Trim();
                        }
                    }
                }

                //2.5 個人資料
                keyWordsMap[keyWords[keyWordsIndex][0]] = keyWordsMap[keyWords[keyWordsIndex][0]].Replace("⾝⾼", "身高").Replace("⾃", "自").Replace("⾞", "車");//⾃備⾞輛
                string[] PersonInformation = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(Environment.NewLine + Environment.NewLine);
                Format.PersonInformation = new PersonInformation();
                for (int i = 0; i < PersonInformation.Length; i++) {
                    if (string.IsNullOrWhiteSpace(PersonInformation[i])) {
                        continue;
                    } else if (PersonInformation[i].StartsWith("履歷類別")) {
                        string[] pe0 = PersonInformation[i].Split("履歷類別");
                        if (pe0.Length >= 2) {
                            if (Format.JobCondition == null) {
                                Format.JobCondition = new JobCondition();
                            }
                            Format.JobCondition.JobFullPartTime = pe0[1].Trim();
                            //Format.PersonInformation.EmployedStatus = pe0[1].Trim();
                        }
                    } else if (PersonInformation[i].StartsWith("就業狀態")) {
                        string[] pe0 = PersonInformation[i].Split("就業狀態");
                        if (pe0.Length >= 2) {
                            Format.PersonInformation.EmployedStatus = pe0[1].Trim();
                        }
                    } else if (PersonInformation[i].StartsWith("英文姓名")) {
                        string[] pe0 = PersonInformation[i].Split("英文姓名");
                        if (pe0.Length >= 2) {
                            Format.PersonInformation.UserEngName = pe0[1].Trim();
                        }
                    } else if (PersonInformation[i].StartsWith("基本資料")) {
                        string[] pe0 = PersonInformation[i].Split("基本資料");
                        if (pe0.Length >= 2) {
                            string[] pe1 = pe0[1].Split(" ");
                            if (pe1.Length >= 3) {
                                Format.PersonInformation.Birthdate = pe1[1].Trim();
                                Format.PersonInformation.MarriageStatus = pe1[2].Trim();
                            }
                            if (pe1.Length >= 2) {
                                Format.PersonInformation.Birthdate = pe1[1].Trim();
                                //Format.PersonInformation.MarriageStatus = pe1[2].Trim();
                            }
                        }
                    } else if (PersonInformation[i].StartsWith("兵役狀況")) {
                        string[] pe0 = PersonInformation[i].Split("兵役狀況");
                        if (pe0.Length >= 2) {
                            Format.PersonInformation.VeteranStatus = pe0[1].Trim();
                        }
                    } else if (PersonInformation[i].StartsWith("身高體重")) {
                        string[] pe0 = PersonInformation[i].Split("身高體重");
                        if (pe0.Length >= 2) {
                            string[] pe1 = pe0[1].Split("　");
                            if (pe1.Length >= 2) {
                                Format.PersonInformation.BodyHeight = pe1[0].Trim();
                                Format.PersonInformation.BodyWeight = pe1[1].Trim();
                            }
                        }
                    } else if (PersonInformation[i].StartsWith("持有駕照")) {
                        string[] pe0 = PersonInformation[i].Split("持有駕照");
                        if (pe0.Length >= 2) {
                            Format.PersonInformation.DriverLicences = new List<string>();
                            string[] pe1 = pe0[1].Split("、");
                            foreach (var pe in pe1) {
                                if (!string.IsNullOrWhiteSpace(pe)) {
                                    Format.PersonInformation.DriverLicences.Add(pe.Trim());
                                }
                            }
                        }
                    } else if (PersonInformation[i].StartsWith("自備車輛"))//⾃備⾞輛
                      {
                        string[] pe0 = PersonInformation[i].Split("自備車輛");
                        if (pe0.Length >= 2) {
                            Format.PersonInformation.Transportations = new List<string>();
                            string[] pe1 = pe0[1].Split("、");
                            foreach (var pe in pe1) {
                                if (!string.IsNullOrWhiteSpace(pe)) {
                                    Format.PersonInformation.Transportations.Add(pe.Trim());
                                }
                            }
                        }
                    }

                }



                //2.6 求職條件              
                keyWordsMap[keyWords[keyWordsIndex][0]] = keyWordsMap[keyWords[keyWordsIndex][0]].Replace("⼯", "工").Replace("⽇", "日");
                string[] JobCondition = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(Environment.NewLine + Environment.NewLine);
                if (Format.JobCondition == null) {
                    Format.JobCondition = new JobCondition();
                }
                string tempJobContent = "";
                for (int i = 0; i < JobCondition.Length; i++) {
                    if (!string.IsNullOrWhiteSpace(JobCondition[i]) && JobCondition[i].StartsWith("https://pro.104.com.tw/vip/ResumeTools/resumePreview?pageSource=hunter&searchEngineIdNos")) {
                        if (JobCondition[i].Contains("\r\n")) {
                            JobCondition[i] = JobCondition[i].Substring(JobCondition[i].IndexOf("\n") + 1);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(JobCondition[i])) {
                        continue;
                    } else if (JobCondition[i].Trim().StartsWith("工作性質")) {
                        string[] pe0 = JobCondition[i].Split("工作性質");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobFullPartTime = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("希望職稱")) {
                        string[] pe0 = JobCondition[i].Split("希望職稱");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobTitle = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("希望內容")) {
                        string[] pe0 = JobCondition[i].Split("希望內容");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobContent = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("希望職類")) {
                        if (!string.IsNullOrWhiteSpace(tempJobContent)) {
                            Format.JobCondition.JobContent += tempJobContent;
                            tempJobContent = "";
                        }

                        string[] pe0 = JobCondition[i].Split("希望職類");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobCategory = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("希望產業")) {
                        string[] pe0 = JobCondition[i].Split("希望產業");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobIndustry = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("希望地點")) {
                        string[] pe0 = JobCondition[i].Split("希望地點");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobCounties = new List<string>();
                            string[] pe1 = pe0[1].Split("、");
                            foreach (var pe in pe1) {
                                if (!string.IsNullOrWhiteSpace(pe)) {
                                    Format.JobCondition.JobCounties.Add(pe.Trim());
                                }
                            }
                        }
                    } else if (JobCondition[i].Trim().StartsWith("希望待遇")) {
                        string[] pe0 = JobCondition[i].Split("希望待遇");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobPay = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("可上班日"))//可上班⽇
                      {
                        string[] pe0 = JobCondition[i].Split("可上班日");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.AvailableDate = pe0[1].Trim();
                        }
                    } else if (JobCondition[i].Trim().StartsWith("上班時段")) {
                        string[] pe0 = JobCondition[i].Split("上班時段");
                        if (pe0.Length >= 2) {
                            Format.JobCondition.JobSchedule = pe0[1].Trim();
                        }
                    } else {
                        tempJobContent += JobCondition[i];
                    }
                }

                //2.7 語文能力2
                if (keyWordsMap.ContainsKey(keyWords[keyWordsIndex][0]) || keyWordsMap.ContainsKey(keyWords[keyWordsIndex][1])) {
                    string[] Languages = keyWordsMap[keyWords[keyWordsIndex++][0]].Split(Environment.NewLine + Environment.NewLine);
                    Format.Languages = new List<string>();
                    foreach (var Language in Languages) {
                        if (string.IsNullOrWhiteSpace(Language)) {
                            continue;
                        } else {
                            Format.Languages.Add(Language.Trim());
                            //測試comiyu
                        }
                    }
                }


                //2.8 技能專長
                if (keyWordsMap.ContainsKey(keyWords[keyWordsIndex][0])) {
                    keyWordsMap[keyWords[keyWordsIndex][0]] = keyWordsMap[keyWords[keyWordsIndex][0]].Replace("⼯", "工").Replace("⽇", "日").Replace("⻑", "長");
                    string[] SkillLicenses = keyWordsMap[keyWords[keyWordsIndex++][0]].Trim().Split(Environment.NewLine + Environment.NewLine);
                    if (SkillLicenses == null || SkillLicenses.Length == 1) {
                        Format.SkillLicenses = keyWordsMap[keyWords[keyWordsIndex - 1][0]].Trim();
                    } else {
                        Format.SkillLicenses = keyWordsMap[keyWords[keyWordsIndex - 1][0]].Trim();

                        Dictionary<string, string> SkillWords =
                            new Dictionary<string, string>
                                {
                                    //{ "專長", "專長" },
                                    { "擅長工具", "擅長工具" },
                                    { "工作技能", "工作技能" },
                                    { "認證資格", "認證資格" }
                                };

                        Format.SkillLicenseDetail = new SkillLicenseDetail();
                        Format.SkillLicenseDetail.Licenses = new List<string>();
                        Format.SkillLicenseDetail.Tools = new List<string>();
                        Format.SkillLicenseDetail.Skills = new List<string>();
                        SubEnd = false;
                        string LastSkillLicense = "";
                        for (int i = 0; i < SkillLicenses.Length; i++) {
                            if (string.IsNullOrWhiteSpace(SkillLicenses[i])) {
                                continue;
                            }

                            string InfosADesc = SkillLicenses[i]?.Trim();

                            if (SkillWords.Keys.Any(x => InfosADesc.Contains(x))) {
                                SetSkillLicenseDetail(Format, keepString, LastSkillLicense);

                                string InfosADesc1 = "";
                                string InfosADesc2 = "";
                                if (InfosADesc.IndexOf(" ") > 0) {
                                    InfosADesc1 = InfosADesc.Substring(0, InfosADesc.IndexOf(" "));
                                    InfosADesc2 = InfosADesc.Substring(InfosADesc.IndexOf(" ")).Trim();
                                } else if (InfosADesc.IndexOf(Environment.NewLine) > 0) {
                                    InfosADesc1 = InfosADesc.Substring(0, InfosADesc.IndexOf(Environment.NewLine));
                                    InfosADesc2 = InfosADesc.Substring(InfosADesc.IndexOf(Environment.NewLine)).Trim();
                                }

                                switch (InfosADesc1) {

                                    case "專長":
                                        LastSkillLicense = "專長";
                                        //Format.SkillLicenseDetail.Tools.Add(InfosADesc2);
                                        string[] Tools = InfosADesc.Split("擅長工具");
                                        if (Tools.Length > 1) {
                                            LastSkillLicense = "擅長工具";
                                            Format.SkillLicenseDetail.Tools.Add(Tools[1]);
                                        }
                                        keepFlag = true;
                                        keepString = "";
                                        break;
                                    case "擅長工具":
                                        LastSkillLicense = "擅長工具";
                                        Format.SkillLicenseDetail.Tools.Add(InfosADesc2);
                                        keepFlag = true;
                                        keepString = "";
                                        break;
                                    case "工作技能":
                                        LastSkillLicense = "工作技能";
                                        Format.SkillLicenseDetail.Skills.Add(InfosADesc2);
                                        keepFlag = true;
                                        keepString = "";
                                        break;
                                    case "認證資格":
                                        LastSkillLicense = "認證資格";
                                        Format.SkillLicenseDetail.Licenses.Add(InfosADesc2);
                                        keepFlag = true;
                                        keepString = "";
                                        break;

                                    default:
                                        keepFlag = false;
                                        keepString = "";
                                        break;
                                }

                                workExp.WorkContent += keepString;

                                //Regex regYear = new Regex("20\\d{2}|19\\d{2}");
                                //if ((InfosADesc.Contains("個月") || InfosADesc.Contains("仍在職") || InfosADesc.Contains("年)")) && regYear.IsMatch(InfosADesc))
                                //{
                                //    if (!string.IsNullOrWhiteSpace(workExp.WorkTitle))
                                //    {
                                //        Format.WorkExps.Add(workExp);
                                //        workExp = new WorkExp();
                                //    }

                                //    workExp.WorkTitle = InfosADesc;
                                //}

                            } else {
                                if (keepFlag) {
                                    keepString += InfosADesc;
                                }
                            }
                        }

                        SetSkillLicenseDetail(Format, keepString, LastSkillLicense);

                        //if (!string.IsNullOrWhiteSpace(workExp.WorkTitle))
                        //{
                        //    Format.WorkExps.Add(workExp);
                        //}

                    }

                }
                //2.9 自傳
                if (keyWordsMap.ContainsKey(keyWords[keyWordsIndex][0])) {
                    Format.Autobiography = keyWordsMap[keyWords[keyWordsIndex++][0]].Trim();
                }
                //2.10 附件
                if (keyWordsMap.ContainsKey(keyWords[keyWordsIndex][0])) {
                    Format.Attachments = keyWordsMap[keyWords[keyWordsIndex++][0]].Trim();
                }


                //******************
                newSumList = new List<string>();
                string[] UserData = keyWordsMap[testUserKey].Split(Environment.NewLine + Environment.NewLine);
                //keyWordsMap[testUserKey]
                for (int i = 0; i < UserData.Length; i++) {
                    if (string.IsNullOrWhiteSpace(UserData[i])) {
                        continue;
                    }
                    if (UserData[i] == "最⾼學歷" || UserData[i] == "最高學歷") {
                        testIndex = i;
                    }
                }

                for (int i = 0; i < UserData.Length; i++) {
                    if (string.IsNullOrWhiteSpace(UserData[i])) {
                        continue;
                    }

                    if (testIndex >= 9 && i >= (testIndex - 10 ) && i <= (testIndex + 7)) {
                        if (i >= (testIndex - 10)) {
                            newSumList.Add(UserData[i]);
                        }
                    }
                }
                for (int i = 0; i < newSumList.Count(); i++) {
                    if (!newSumList[i].Contains("歲")) {
                        newSumList.RemoveAt(0);
                        i--;
                    } else {
                        break;
                    }
                }
                ////if (!newSumList[0].Contains("歲")) {
                //    newSumList.RemoveAt(0);
                //}
                for (int i = 0; i < newSumList.Count; i++) {
                    switch (i) {
                        case 0:
                            //Format.UserHighestEducation = newSumList[i];
                            string[] splits = newSumList[i].Split(" ");
                            if (splits != null && splits.Length >= 3) {
                                Format.UserName = splits[0];
                                Format.UserAge = splits[1];
                                Format.UserGender = splits[2];
                            }
                            break;
                        case 1:
                            Format.UserHighestEducation = newSumList[i];
                            break;
                        case 2:
                            Format.UserHopeJobTitle = newSumList[i];
                            break;
                        case 3:
                            Format.UserYearsOfTotalExperience = newSumList[i];
                            break;
                        case 4:
                            Format.UserLastJob = newSumList[i];
                            break;
                        case 5:
                            Format.UserResidence = newSumList[i];
                            break;
                        case 6:
                            Format.UserEmail = newSumList[i];
                            break;
                        case 7:
                            Format.UserTEL = newSumList[i];
                            break;
                        case 8:
                            Format.UserContactInformation = newSumList[i];
                            if (Format.UserContactInformation.Contains("\r\n")) {
                                splits = Format.UserContactInformation.Split("\r\n");
                                if (splits != null && splits.Length == 2) {
                                    Format.UserContactInformation = splits[0];
                                    Format.UserCode = splits[1];
                                }
                            } else {
                                if (newSumList[i+1].Contains("\r\n")) {
                                    splits = newSumList[i + 1].Split("\r\n");
                                    if (splits != null && splits.Length == 2) {
                                        if (splits[0].Contains("應徵⽇期:")) {
                                            Format.UserApplyTime = splits[0].Substring(6,16);
                                        }
                                        Format.UserCode = splits[1];
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                //******************

                a = 0;
                return Format;
            }
        }

        private static void SetSkillLicenseDetail(Format Format, string keepString, string LastSkillLicense)
        {
            if (!string.IsNullOrWhiteSpace(keepString)) {
                switch (LastSkillLicense) {
                    case "擅長工具":
                        Format.SkillLicenseDetail.Tools.Add(keepString);
                        break;
                    case "工作技能":
                        Format.SkillLicenseDetail.Skills.Add(keepString);
                        break;
                    case "認證資格":
                        Format.SkillLicenseDetail.Licenses.Add(keepString);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}

