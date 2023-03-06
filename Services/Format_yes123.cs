using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace PdfParser.Formatyes123
{
    public class Format_yes123
    {
        public Title Title { get; set; } = new Title();
        public UserInfo UserInfo { get; set; } = new UserInfo();
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
        public Education_yes123 Education_yes123 { get; set; } = new Education_yes123();
        public WorkExperience WorkExperience { get; set; } = new WorkExperience();
        public Skills Skills { get; set; } = new Skills();
        public Autobiography Autobiography { get; set; } = new Autobiography();
        public HopeJobCondition HopeJobCondition { get; set; } = new HopeJobCondition();
    }
    public class Title
    {
        public string UserApplyDateTime { get; set; }
        public string UserApplyJob { get; set; }
        public string SelfRecommand { get; set; }
    }
    public class UserInfo
    {
        public string UserCode { get; set; }
        public string LastLoginDate { get; set; }
        public string UpdateDateTime { get; set; }
        public string UserName { get; set; }
        public string UserEngName { get; set; }
        public string UserGender { get; set; }
        public string UserAge { get; set; }
        public string UserAddress { get; set; }
        public string EmploymentStatement { get; set; }
        public string IdentityCategory { get; set; }
        public string StartToWork { get; set; }
        public string Transportations { get; set; }
        public string HopeSalary { get; set; }
        public string UserHeight { get; set; }
        public string UserWeight { get; set; }
        public string UserMarriage { get; set; }
        public string UserChildren { get; set; }
    }
    public class ContactInfo
    {
        public string UserEmail { get; set; }
        public string UserMobilePhone { get; set; }
        public string ContactTime { get; set; }
        public string ContactWay { get; set; }
    }
    public class Education_yes123
    {
        public string TopDegree { get; set; }
        public List<EducationDetail> EducationDetails { get; set; } = new List<EducationDetail>();
        public string Researchs { get; set; } 
        public string Competitions { get; set; } 
        public string Monographs { get; set; } 
        public string RefresherCourses { get; set; } 
    }
    public class EducationDetail
    {
        public string CollegeName { get; set; }
        public string StudyPeriod { get; set; }
    }
    public class WorkExperience
    {
        public string TotalJobTenure { get; set; }
        public List<string> IndividualJobTenures { get; set; }
        public List<IndividualWorkExp> IndividualWorkExps { get; set; } = new List<IndividualWorkExp>();
    }
    public class IndividualWorkExp
    {
        public string JobTitleJobPeriod { get; set; }
        public string JobCategory { get; set; }
        public string ScaleOfEnterprise { get; set; }
        public string JobNature { get; set; }
        public string Salary { get; set; }
        public string JobContent { get; set; }
        public string ManagePeople { get; set; }
    }
    public class Skills
    {
        public string Languages { get; set; }
        public List<string> ComputerSkills { get; set; }
        public List<string> SkillsLicenses { get; set; }
        public string DrivingLicenses { get; set; }
        public string OtherSkills { get; set; }
    }
    public class Autobiography
    {
        public string autobiography { get; set; }
        public string attachment { get; set; }
        public string recommender { get; set; }
    }
    public class HopeJobCondition
    {
        public List<string> JobTitles { get; set; }
        public string JobDescribe { get; set; }
        public List<string> JobIndustry { get; set; }
        public List<string> JobLocations { get; set; }
        public string JobNature { get; set; }
        public string HolidaySystem { get; set; }
        public string WorkPeriod { get; set; }
    }
    public class Format
    {
        public static Format_yes123 ParsePDF(Stream pdfFilestream)
        {
            var result = new Format_yes123();
            string fileAllText = "";
            string paragraphTitle = "";
            string[] keyWordsArr = { "汎亞國際⼈事顧問股份有限公司", "姓　　名：", "會員編號", "連絡資料", "連絡資料", "學歷背景", "學歷背景", "⼯作經歷", "⼯作經歷", "技能專⻑", "技能專⻑", "⾃　　傳", "⾃　　傳", "應徵職務：", "應徵職務：", "★提醒您個資法規定：" };
            using(var document = PdfDocument.Open(pdfFilestream)) {
                for(int i = 1; i <= document.NumberOfPages; i++) {
                    var page = document.GetPage(i);
                    fileAllText += page.Text;
                }
                for(int i = 0; i < keyWordsArr.Length; i += 2) {
                    var keyWordIndex = fileAllText.IndexOf(keyWordsArr[i]);
                    var keyWordIndex2 = fileAllText.IndexOf(keyWordsArr[i + 1]);
                    switch(i) {
                        case 0:
                            paragraphTitle = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            var ky1 = paragraphTitle.IndexOf("【應徵時間】");
                            var ky2 = paragraphTitle.IndexOf("【應徵職務】");
                            var ky3 = paragraphTitle.IndexOf("【⾃我推薦】");
                            if(ky1 != -1 && ky2 != -1)
                                result.Title.UserApplyDateTime = paragraphTitle.Substring(ky1, ky2 - ky1);
                            if(ky2 != -1 && ky3 != -1)
                                result.Title.UserApplyJob = paragraphTitle.Substring(ky2, ky3 - ky2);
                            if(ky3 != -1)
                                result.Title.SelfRecommand = paragraphTitle.Substring(ky3, paragraphTitle.Length - ky3);
                            break;
                        case 2:
                            string paragraphUserInfo = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            paragraphUserInfo = paragraphUserInfo.Replace(paragraphTitle, "").Replace("　", "");
                            string[] kyArr_userinfo = { "會員編號", "最後登入", "履歷更新", "姓名", "英文名字", "性別", "年齡", "居住地區", "就業狀態", "⾝分類別", "可上班⽇", "⾃備交通", "希望薪資", "⾝⾼", "體重", "婚姻", "⼦女" };
                            result.UserInfo = HandleUserInfo(paragraphUserInfo, kyArr_userinfo);
                            break;
                        case 4:
                            string paragraphContactInfo = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            paragraphContactInfo = paragraphContactInfo.Replace("　", "");
                            string[] kyArr_conttactinfo = { "Email：", "⼿機", "連絡時間", "連絡⽅式" };
                            result.ContactInfo = HandleContactInfo(paragraphContactInfo, kyArr_conttactinfo);
                            break;
                        case 6:
                            string paragraphEducation = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            string[] kyArr_education = { "教育程度", "學歷資料", "研究專⻑", "參加競賽", "專題論文", "進修課程" };
                            result.Education_yes123 = HandleEduInfo(paragraphEducation, kyArr_education);
                            break;
                        case 8:
                            string paragraphWorkExp = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            string[] kyArr_workexp = { "總年資", "相關年資", "⼯作經歷" };
                            result.WorkExperience = HandleWorkExp(paragraphWorkExp, kyArr_workexp);
                            break;
                        case 10:
                            string paragraphSkills = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            paragraphSkills = paragraphSkills.Replace("　", "");
                            string[] kyArr_skills = { "語⾔能⼒", "電腦技能", "證照：", "駕照：", "其他技能：" };
                            result.Skills = HandleSkills(paragraphSkills, kyArr_skills);
                            break;
                        case 12:
                            string paragraphAutobiography = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            string[] kyArr_Autobiography = { "⾃　　傳　", "附件：", "推薦⼈：" };
                            result.Autobiography = HandleAutobiography(paragraphAutobiography, kyArr_Autobiography);
                            break;
                        case 14:
                            string paragraphHopeJob = fileAllText.Substring(keyWordIndex, keyWordIndex2 - keyWordIndex);
                            string[] kyArr_hopejob = { "應徵職務：", "職務描述：", "應徵⾏業：", "⼯作地點：", "⼯作性質：", "休假制度：", "上班時段：" };
                            result.HopeJobCondition = HandleHopeJob(paragraphHopeJob, kyArr_hopejob);
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static UserInfo HandleUserInfo(string paragraphUserInfo, string[] kyArr)
        {
            var result = new UserInfo();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphUserInfo, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;
                    switch(kyString) {
                        case "會員編號":
                            result.UserCode = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "最後登入":
                            result.LastLoginDate = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "履歷更新":
                            result.UpdateDateTime = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "姓名":
                            result.UserName = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "英文名字":
                            result.UserEngName = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "性別":
                            result.UserGender = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "年齡":
                            result.UserAge = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "居住地區":
                            result.UserAddress = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "就業狀態":
                            result.EmploymentStatement = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "⾝分類別":
                            result.IdentityCategory = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "可上班⽇":
                            result.StartToWork = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "⾃備交通":
                            result.Transportations = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "希望薪資":
                            result.HopeSalary = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "⾝⾼":
                            result.UserHeight = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "體重":
                            result.UserWeight = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "婚姻":
                            result.UserMarriage = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "⼦女":
                            result.UserChildren = paragraphUserInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static ContactInfo HandleContactInfo(string paragraphContactInfo, string[] kyArr)
        {
            var result = new ContactInfo();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphContactInfo, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;
                    switch(kyString) {
                        case "Email：":
                            result.UserEmail = paragraphContactInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "⼿機":
                            result.UserMobilePhone = paragraphContactInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "連絡時間":
                            result.ContactTime = paragraphContactInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "連絡⽅式":
                            result.ContactWay = paragraphContactInfo.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static Education_yes123 HandleEduInfo(string paragraphEducation, string[] kyArr)
        {
            var result = new Education_yes123();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphEducation, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;
                    switch(kyString) {
                        case "教育程度":
                            result.TopDegree = paragraphEducation.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "學歷資料":
                            List<EducationDetail> eduDetails = new List<EducationDetail>();
                            var vs = Exten(paragraphEducation, kyIndex, kyIndex2, false,false);
                            foreach(var item in vs) {
                                var eduDetail = new EducationDetail();
                                eduDetail.CollegeName = item.Split("就學期間")[0];
                                eduDetail.StudyPeriod = item.Split("就學期間")[1].Split("：")[1];
                                eduDetails.Add(eduDetail);
                            }
                            result.EducationDetails = eduDetails;
                            break;
                        case "研究專⻑":
                            result.Researchs = paragraphEducation.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "參加競賽":
                            result.Competitions = paragraphEducation.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "專題論文":
                            result.Monographs = paragraphEducation.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "進修課程":
                            result.RefresherCourses = paragraphEducation.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static WorkExperience HandleWorkExp(string paragraphWorkExp, string[] kyArr)
        {
            var result = new WorkExperience();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphWorkExp, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;
                    switch(kyString) {
                        case "總年資":
                            result.TotalJobTenure = paragraphWorkExp.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "相關年資":
                            result.IndividualJobTenures = Exten(paragraphWorkExp, kyIndex, kyIndex2, false,false);
                            break;
                        case "⼯作經歷":
                            var vs = Exten(paragraphWorkExp, kyIndex, kyIndex2, false,true);
                            result.IndividualWorkExps = IndiviWorkExtention(vs);
                            //string[] key = { "）", "企業規模", "性質", "薪資", "⼯作內容" };
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static Skills HandleSkills(string paragraphSkills, string[] kyArr)
        {
            var result = new Skills();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphSkills, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;
                    switch(kyString) {
                        case "語⾔能⼒":
                            result.Languages = paragraphSkills.Substring(kyIndex + kyString.Length + 1, kyIndex2 - (kyIndex + kyString.Length + 1));
                            break;
                        case "電腦技能":
                            result.ComputerSkills = Exten(paragraphSkills, kyIndex, kyIndex2, false,false);
                            break;
                        case "證照：":
                            result.SkillsLicenses = Exten(paragraphSkills, kyIndex, kyIndex2, false,false);
                            break;
                        case "駕照：":
                            result.DrivingLicenses = paragraphSkills.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        case "其他技能：":
                            result.OtherSkills = paragraphSkills.Substring(kyIndex, kyIndex2 - kyIndex).Split("：")[1];
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static Autobiography HandleAutobiography(string paragraphAutobiography, string[] kyArr)
        {
            var result = new Autobiography();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphAutobiography, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;

                    switch(kyString) {
                        case "⾃　　傳　":
                            result.autobiography = paragraphAutobiography.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length));
                            if(result.autobiography.Contains("希望⼯作　")) {
                                result.autobiography = result.autobiography.Remove(result.autobiography.IndexOf("希望⼯作　"));
                            }
                            break;
                        case "附件：":
                            result.attachment = paragraphAutobiography.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length));
                            if (result.autobiography.Contains("希望⼯作　")) {
                                result.autobiography = result.autobiography.Remove(result.autobiography.IndexOf("希望⼯作　"));
                            }
                            break;
                        case "推薦⼈：":
                            result.recommender = paragraphAutobiography.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length + 5));
                            if(result.autobiography.Contains("希望⼯作　")) {
                                result.autobiography = result.autobiography.Remove(result.autobiography.IndexOf("希望⼯作　"));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static HopeJobCondition HandleHopeJob(string paragraphHopeJob, string[] kyArr)
        {
            var result = new HopeJobCondition();
            string kyString;
            int kyIndex;
            int kyIndex2;
            for(int i1 = 0; i1 < kyArr.Length; i1++) {
                var tup = GetIndex1AndIndex2(paragraphHopeJob, kyArr, i1);
                if(tup != null) {
                    kyString = tup.Item1;
                    kyIndex = tup.Item2;
                    kyIndex2 = tup.Item3;

                    switch(kyString) {
                        case "應徵職務：":
                            result.JobTitles = Exten(paragraphHopeJob, kyIndex, kyIndex2, true);
                            break;
                        case "職務描述：":
                            result.JobDescribe = paragraphHopeJob.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length));
                            break;
                        case "應徵⾏業：":
                            result.JobIndustry = Exten(paragraphHopeJob, kyIndex, kyIndex2, true);
                            break;
                        case "⼯作地點：":
                            result.JobLocations = Exten(paragraphHopeJob, kyIndex, kyIndex2, true);
                            break;
                        case "⼯作性質：":
                            result.JobNature = paragraphHopeJob.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length));
                            break;
                        case "休假制度：":
                            result.HolidaySystem = paragraphHopeJob.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length));
                            break;
                        case "上班時段：":
                            result.WorkPeriod = paragraphHopeJob.Substring(kyIndex + kyString.Length, kyIndex2 - (kyIndex + kyString.Length));
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public static List<string> Exten(string paragraphSource, int kyI1, int kyI2, bool isHopeJob,bool? isworkExp=null)
        {
            //if(isHopeJob) {
            //    Regex regex = new Regex(@"[0-9]{1}\.");
            //} else {
            //    Regex regex = new Regex(@"[0-9]{1}\. ");
            //}
            Regex regex = new Regex(isHopeJob == true ? @"[0-9]{1,}\." : isworkExp==true? @"[0-9]{1,}\. \D{1,}（": @"[0-9]{1,}\. \D{1,}");
            List<int> indexs = new List<int>();
            List<string> vs = new List<string>();
            var tempString = paragraphSource.Substring(kyI1, kyI2 - kyI1);
            var res = regex.Matches(tempString);
            foreach(var item in res) {
                indexs.Add(tempString.IndexOf(item.ToString()));
            }
            for(int i = 0; i < indexs.Count; i++) {
                if(i == indexs.Count - 1) {
                    vs.Add(tempString.Substring(indexs[i], tempString.Length - indexs[i]));
                } else {
                    vs.Add(tempString.Substring(indexs[i], indexs[i + 1] - indexs[i]));
                }
            }
            return vs;
        }
        public static List<IndividualWorkExp> IndiviWorkExtention(List<string> sourceListText)
        {
            string[] key = { "）", "企業規模", "性質", "薪資", "⼯作內容", "管理⼈數" };
            var result = new List<IndividualWorkExp>();
            string kyString;
            int kyIndex;
            int kyIndex2;
            foreach(var item in sourceListText) {
                var individualWorkExp = new IndividualWorkExp();
                for(int i1 = 0; i1 < key.Length; i1++) {
                    var tup = GetIndex1AndIndex2(item, key, i1);
                    if(tup != null) {
                        kyString = tup.Item1;
                        kyIndex = tup.Item2;
                        kyIndex2 = tup.Item3;
                        switch(kyString) {
                            case "）":
                                individualWorkExp.JobCategory = item.Substring(kyIndex + 1, kyIndex2 - kyIndex - 1);
                                individualWorkExp.JobTitleJobPeriod = item.Substring(0, kyIndex + 1);
                                break;
                            case "企業規模":
                                individualWorkExp.ScaleOfEnterprise = item.Substring(kyIndex, kyIndex2 - kyIndex);
                                break;
                            case "性質":
                                individualWorkExp.JobNature = item.Substring(kyIndex, kyIndex2 - kyIndex);
                                break;
                            case "薪資":
                                individualWorkExp.Salary = item.Substring(kyIndex, kyIndex2 - kyIndex);
                                break;
                            case "⼯作內容":
                                individualWorkExp.JobContent = item.Substring(kyIndex, kyIndex2 - kyIndex);
                                break;
                            case "管理⼈數":
                                individualWorkExp.ManagePeople = item.Substring(kyIndex, kyIndex2 - kyIndex);
                                break;
                            default:
                                break;
                        }
                    }
                }
                result.Add(individualWorkExp);
            }
            return result;
        }
        public static Tuple<string, int, int> GetIndex1AndIndex2(string sourceText, string[] kyArr, int indexI)
        {
            string kyString = kyArr[indexI];
            List<int> keyWordsIndex = new List<int>();
            int kyIndex;
            if(sourceText.Contains("⼯作經歷")) {
                kyIndex = sourceText.Substring(4).IndexOf(kyArr[indexI]);
            } else {
                kyIndex = sourceText.IndexOf(kyArr[indexI]);
            }

            if(kyIndex != -1) {
                string[] tempkyArr = kyArr.Where(x => x != kyString).ToArray();
                foreach(var item in tempkyArr) {
                    keyWordsIndex.Add(sourceText.IndexOf(item, kyIndex + 1));
                    keyWordsIndex = keyWordsIndex.Where(x => x != -1).ToList();
                }
                int kyIndex2;
                if(keyWordsIndex.Count == 0) {
                    kyIndex2 = sourceText.Length;
                    return new Tuple<string, int, int>(kyString, kyIndex, kyIndex2);
                } else {
                    kyIndex2 = keyWordsIndex.Min();
                    return new Tuple<string, int, int>(kyString, kyIndex, kyIndex2);
                }
            }
            return null;
        }
    }
}
