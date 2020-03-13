using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>
    /// <see cref="CsvParser"/> クラスは、CSV データをパースする機能をシンプルに提供するクラスです。
    /// <para>
    /// RFC 4180 準拠。レコードは、ダブルクォートに対応。（ダブルクォート内のカンマ・ダブルクォート・改行）
    /// ヘッダーを通常のレコードと同じように処理します。
    /// </para>
    /// </summary>
    public class CsvParser
    {
        private const char DoubleQuote = '"';
        private const char CarriageReturn = '\r';
        private const char LineFeed = '\n';

        public enum Delimiter : int
        {
            Comma = ',',
            Tab = '\t',
            Semicolon = ';',
        }

        private enum QuoteState
        {
            Normal,
            Quoted,
        }

        #region Initializes

        /// <summary>
        /// <see cref="CsvParser"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CsvParser() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// 指定したフォーマットに従って、CSV ファイルを CSV レコードに分解します。
        /// </summary>
        /// <param name="filePath">ファイル名。</param>
        /// <param name="delimiter">区切り文字。</param>
        /// <param name="encoding">ファイルの文字エンコーディング。</param>
        /// <returns>CSV レコードのコレクション。</returns>
        public static IEnumerable<IEnumerable<string>> ParseFromFile(string filePath, Delimiter delimiter = Delimiter.Comma, Encoding encoding = null)
        {
            var text = File.ReadAllText(filePath, encoding ?? Encoding.UTF8);

            UnifyCrlf(ref text);

            return ParseRecords(text, delimiter).Select(p => ParseFieldsFromText(p, Delimiter.Comma));
        }

        /// <summary>
        /// 指定したフォーマットに従って、テキストを CSV レコードに分解します。
        /// </summary>
        /// <param name="text">CSV フォーマットのテキスト。</param>
        /// <param name="delimiter">区切り文字。</param>
        /// <returns>CSV レコードのコレクション。</returns>
        public static IEnumerable<IEnumerable<string>> ParseFromText(string text, Delimiter delimiter = Delimiter.Comma)
        {
            UnifyCrlf(ref text);

            return ParseRecords(text, delimiter).Select(p => ParseFieldsFromText(p, Delimiter.Comma));
        }

        /// <summary>
        /// 指定したフォーマットに従って、CSV レコード形式のテキストを CSV フィールドに分解します。
        /// </summary>
        /// <param name="text">CSV レコード形式のテキスト。</param>
        /// <param name="delimiter">区切り文字。</param>
        /// <returns>CSV フィールドのコレクション。</returns>
        public static IEnumerable<string> ParseFieldsFromText(string text, Delimiter delimiter = Delimiter.Comma)
        {
            UnifyCrlf(ref text);

            return ParseFields(text, delimiter);
        }

        #endregion

        #region Private Methods

        private static void UnifyCrlf(ref string text)
        {
            text = Regex.Replace(text, @"\r\n|\r|\n", "\r\n");
        }

        private static IEnumerable<string> ParseRecords(string text, Delimiter delimiter)
        {
            var records = new List<string>();
            var recordText = new StringBuilder();

            var state = QuoteState.Normal;
            var delimiterChar = (char)delimiter;
            var isBuild = false;
            var isInline = false;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var isAppend = true;

                if (c == DoubleQuote)
                {
                    if (state == QuoteState.Normal)
                    {
                        state = QuoteState.Quoted;
                    }
                    else if (state == QuoteState.Quoted)
                    {
                        if (!isInline && i + 1 < text.Length && text[i + 1] == DoubleQuote)
                        {
                            // ダブルクォートをダブルクォート内で利用
                            isInline = true;
                        }
                        else
                        {
                            if (!isInline)
                            {
                                // ステートの終了
                                state = QuoteState.Normal;
                            }
                            else
                            {
                                // ダブルクォート内のダブルクォートとしてスキップ
                                isInline = false;
                            }
                        }
                    }
                }
                else if (c == CarriageReturn)
                {
                    if (state == QuoteState.Normal)
                    {
                        isAppend = false;
                    }
                }
                else if (c == LineFeed)
                {
                    if (state == QuoteState.Normal)
                    {
                        isAppend = false;
                        isBuild = true;
                    }
                }

                if (isAppend)
                {
                    recordText.Append(c);
                }

                // ダブルクォート状態で EOF に到達したとき
                if (state == QuoteState.Quoted && i + 1 >= text.Length)
                {
                    throw new FormatException("ダブルクォートを閉じない状態でテキストの最後に到達しました。");
                }

                // \r\n で閉じるか EOF に到達したとき
                if (isBuild || (state == QuoteState.Normal && i + 1 >= text.Length))
                {
                    isBuild = false;

                    records.Add(recordText.ToString());
                    recordText.Clear();
                }
            }

            return records;
        }

        private static IEnumerable<string> ParseFields(string text, Delimiter delimiter)
        {
            var fields = new List<string>();
            var fieldText = new StringBuilder();

            var state = QuoteState.Normal;
            var delimiterChar = (char)delimiter;
            var isBuild = false;
            var hasQuote = false;
            var hasOmitted = false;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var isAppend = true;

                if (c == DoubleQuote)
                {
                    if (state == QuoteState.Normal)
                    {
                        hasQuote = true;
                        state = QuoteState.Quoted;
                    }
                    else if (state == QuoteState.Quoted)
                    {
                        if (i + 1 < text.Length && text[i + 1] == DoubleQuote)
                        {
                            // ダブルクォートをインラインで利用
                            i += 1;
                        }
                        else
                        {
                            // ステートの終了
                            state = QuoteState.Normal;
                        }
                    }
                }
                else if (c == CarriageReturn || c == LineFeed)
                {
                    if (state == QuoteState.Normal)
                    {
                        isAppend = false;
                    }
                }
                else if (c == delimiterChar)
                {
                    if (state == QuoteState.Normal)
                    {
                        isAppend = false;
                        isBuild = true;

                        // 最後の文字が , で終わっているためデータを１つ省略表記している
                        if (i + 1 >= text.Length)
                        {
                            hasOmitted = true;
                        }
                    }
                }

                if (isAppend)
                {
                    fieldText.Append(c);
                }

                // ダブルクォート状態で EOF に到達したとき
                if (state == QuoteState.Quoted && i + 1 >= text.Length)
                {
                    throw new FormatException("ダブルクォートを閉じない状態でテキストの最後に到達しました。");
                }

                // 区切り文字を検出したか EOF に到達したとき
                if (isBuild || (state == QuoteState.Normal && i + 1 >= text.Length))
                {
                    isBuild = false;

                    if (fieldText.Length > 0)
                    {
                        
                        var newField = fieldText.ToString();

                        if (hasQuote && newField.Count(co => co == DoubleQuote) >= 2)
                        {
                            // ダブルクォートで区切られるときは、前後の空白文字は削除する
                            newField = newField.Trim(' ');

                            // ダブルクォートの削除
                            newField = newField.Substring(1, newField.Length - 2);
                        }

                        fields.Add(newField);
                    }
                    else
                    {
                        fields.Add("");
                    }

                    if (hasOmitted)
                    {
                        // 空白のデータ
                        hasOmitted = false;
                        fields.Add("");
                    }

                    fieldText.Clear();
                }
            }

            return fields;
        }

        #endregion
    }
}
