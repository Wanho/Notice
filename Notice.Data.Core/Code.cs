using System;

namespace Notice.Data.Core
{
    public sealed class Config : Code<Config>
    {
        static Config()
        {
            setting.CodeLength = 15;
        }

        public static void SetSuffix(string suffix)
        {
            setting.Suffix = suffix;
        }


        [Custom_Code("Root")]
        public static readonly Config RootName;

        [Custom_Code("domain.com")]
        public static readonly Config Domain;

        [Custom_Code("test")]
        public static readonly Config AdminId;

        [Custom_Code("password")]
        public static readonly Config AdminPwd;

        [Custom_Code("Notice")]
        public static readonly Config AppName;

        [Custom_Code("2b97726c2d374ade")]
        public static readonly Config ASEKey;

        [Custom_Code("test.domain.com")]
        public static readonly Config CookieDomain;
    }

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
        [Custom_Code("DESC")]
        public static readonly SortOrder Desc;

        [Custom_DefaultCode]
        [Custom_Code("ASC")]
        public static readonly SortOrder Asc;
    }

    public sealed class IsYNType : Code<IsYNType>
    {
        [Custom_Code("Y")]
        public static readonly IsYNType True;

        [Custom_Code("N")]
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
        public static readonly BoardSortType SortCreateDate;
            }

    public sealed class FileExtentionType : Code<FileExtentionType>
    {
        [Custom_Code(".FRE")]
        public static readonly FileExtentionType FRE;
        [Custom_Code(".EMF")]
        public static readonly FileExtentionType EMF;
        [Custom_Code(".GIF")]
        public static readonly FileExtentionType GIF;
        [Custom_Code(".JPEG")]
        public static readonly FileExtentionType JPEG;
        [Custom_Code(".JPG")]
        public static readonly FileExtentionType JPG;
        [Custom_Code(".PNG")]
        public static readonly FileExtentionType PNG;
        [Custom_Code(".BMP")]
        public static readonly FileExtentionType BMP;
        [Custom_Code(".TIF")]
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

    [Custom_FileRoot("Custom_FileRoot")]
    public sealed class FileType : CodeFileType<FileType>
    {
        // 첨부파일 폴더
        [Custom_CodeFileType("yyyyMMdd")]
        public static readonly FileType Attach;

        // 인라인 첨부파일 폴더
        [Custom_CodeFileType("yyyyMMdd")]
        public static readonly FileType InlineAttach;

        // 인라인 임시폴더
        [Custom_CodeFileType("yyyyMMdd")]
        public static readonly FileType TempInline;

        // 첨부파일 임시폴더
        [Custom_CodeFileType("yyyyMMdd")]
        public static readonly FileType TempAttach;

        public static readonly FileType Log;
    }


}
