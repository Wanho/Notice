using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Notice.Data.Core
{
    public sealed class Protocol : Code<Protocol>
    {
        public static readonly Protocol Http;
        public static readonly Protocol Https;
    }

    public sealed class AuthType : Code<AuthType>
    {
        public static readonly AuthType None;
        public static readonly AuthType User;
        public static readonly AuthType Admin;
    }

    public sealed class SortOrder : Code<SortOrder>
    {
        [CodeName("DESC")]
        public static readonly SortOrder Desc;

        [CodeName("ASC")]
        public static readonly SortOrder Asc;
    }

    public sealed class IsYNType : Code<IsYNType>
    {
        [CodeName("Y")]
        public static readonly IsYNType True;

        [CodeName("N")]
        public static readonly IsYNType False;
    }

    public sealed class BoardSortType : Code<BoardSortType>
    {
        public static readonly BoardSortType RowNum;
        public static readonly BoardSortType Ref_Seq;
        public static readonly BoardSortType Ref_Step;
        public static readonly BoardSortType ItemID;
        public static readonly BoardSortType OrderNumber;
        public static readonly BoardSortType Title;
        public static readonly BoardSortType ViewCnt;
        public static readonly BoardSortType UserName;
        public static readonly BoardSortType FileOnly;
        public static readonly BoardSortType Importance;
        public static readonly BoardSortType AttachCNT;
        public static readonly BoardSortType BoardName;
        public static readonly BoardSortType Company;
        public static readonly BoardSortType CreateDate;
    }

    public sealed class FileExtentionType : Code<FileExtentionType>
    {
        [CodeName(".FRE")]
        public static readonly FileExtentionType FRE;
        [CodeName(".EMF")]
        public static readonly FileExtentionType EMF;
        [CodeName(".GIF")]
        public static readonly FileExtentionType GIF;
        [CodeName(".JPEG")]
        public static readonly FileExtentionType JPEG;
        [CodeName(".JPG")]
        public static readonly FileExtentionType JPG;
        [CodeName(".PNG")]
        public static readonly FileExtentionType PNG;
        [CodeName(".BMP")]
        public static readonly FileExtentionType BMP;
        [CodeName(".TIF")]
        public static readonly FileExtentionType TIF;
    }

    public sealed class LanguageType : Code<LanguageType>
    {
        public static readonly LanguageType Kor;
        public static readonly LanguageType Eng;
        public static readonly LanguageType Chn;
        public static readonly LanguageType Jpn;
        public static readonly LanguageType Etc;
    }

    //[FileRoot("FileRoot")]
    //public sealed class FileType : FileTypeCode<FileType>
    //{
    //    // 첨부파일 폴더
    //    [FileTypeCodeName("yyyyMMdd")]
    //    public static readonly FileType Attach;

    //    // 인라인 첨부파일 폴더
    //    [FileTypeCode("yyyyMMdd")]
    //    public static readonly FileType InlineAttach;

    //    // 인라인 임시폴더
    //    [FileTypeCode("yyyyMMdd")]
    //    public static readonly FileType TempInline;

    //    // 첨부파일 임시폴더
    //    [FileTypeCode("yyyyMMdd")]
    //    public static readonly FileType TempAttach;

    //    public static readonly FileType Log;
    //}

    public class Code<T> where T : Code<T>
    {
        /// <summary>
        /// 문자열에 해당하는 코드가 실제 코드로 존재하는지 판단
        /// </summary>
        /// <param name="code">코드</param>
        /// <returns>존재 여부</returns>
        public static bool Exist(string code)
        {

            Type type = typeof(T);

            var list = from prop in type.GetFields()
                       where prop.GetRawConstantValue().ToString().Equals(code, StringComparison.CurrentCultureIgnoreCase)
                       select prop;
            return list.Count() > 0;
        }

        /// <summary>
        /// 코드의 FieldInfo개체를 반환
        /// </summary>
        /// <param name="code">코드</param>
        /// <returns>FieldInfo개체</returns>
        public static FieldInfo GetField(string code)
        {
            Type type = typeof(T);

            var list = from prop in type.GetFields()
                       where prop.Name.Equals(code, StringComparison.CurrentCultureIgnoreCase)
                       select prop;
            if (list.Count() > 0)
            {
                return list.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 파스칼 케이로 입력된 코드를 실 코드로 반환
        /// </summary>
        /// <param name="code">파스칼 케이스 코드값</param>
        /// <returns>실 코드</returns>
        public static string GetCode(string code)
        {
            Type type = typeof(T);

            var list = from prop in type.GetFields()
                       where prop.Name.ToPascalCase().Equals(code, StringComparison.CurrentCultureIgnoreCase)
                       select prop;
            if (list.Count() > 0)
            {
                return list.First().Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 코드 순서
        /// </summary>
        /// <param name="code">코드</param>
        /// <returns>코스순서</returns>
        public static int GetOrder(string code)
        {

            Type type = typeof(T);

            var list = from prop in type.GetFields()
                       where prop.GetRawConstantValue().ToString().Equals(code, StringComparison.CurrentCultureIgnoreCase)
                       select prop;

            var attr = list.First().GetCustomAttributes(typeof(OrderAttribute), false);
            if (attr.Length == 0) return -1;
            else
            {
                return (attr[0] as OrderAttribute).Order;
            }
        }

        /// <summary>
        /// 코드의 명칭을 반환
        /// </summary>
        /// <param name="code">코드</param>
        /// <returns>코드명</returns>
        public static string GetName(string code)
        {

            Type type = typeof(T);

            var list = from prop in type.GetFields() where prop.GetRawConstantValue().ToString().Equals(code, StringComparison.CurrentCultureIgnoreCase)
                       select prop;

            var attr = list.First().GetCustomAttributes(typeof(CodeNameAttribute), false);
            if (attr.Length == 0) return string.Empty;
            else
            {
                return (attr[0] as CodeNameAttribute).CodeNm;
            }
        }

        public static Dictionary<string, string> GetCodeList(bool isValueKey = false, bool isNameValue = false)
        {
            Type type = typeof(T);

            var dic = new Dictionary<string, string>();


            FieldInfo field = null;
            foreach (var prop in type.GetFields())
            {
                int min = int.MaxValue;
                foreach (var attr in type.GetFields())
                {
                    int order = int.MinValue;
                    if (dic.ContainsKey(isValueKey ? attr.GetRawConstantValue() as string : attr.Name))
                    {
                        continue;
                    }

                    var list = attr.GetCustomAttributes(typeof(OrderAttribute), false);
                    if (list.Length > 0)
                    {
                        order = (list[0] as OrderAttribute).Order;
                    }

                    if (min > order)
                    {
                        field = attr;
                        min = order;
                    }
                }

                string name = field.GetRawConstantValue() as string;
                if (isNameValue)
                {
                    var list = field.GetCustomAttributes(typeof(CodeNameAttribute), false);
                    if (list.Length > 0)
                    {
                        name = (list[0] as CodeNameAttribute).CodeNm;
                    }
                }

                dic.Add(isValueKey ? field.GetRawConstantValue() as string : field.Name, name);
            }

            return dic;
        }
    }
}
