using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Tests2
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("", 0)]
        [TestCase(" ", 1)]
        [TestCase("���{��,����,127767944 \r\n�A�����J���O��, ���V���g��, 300007997 \r\n", 2)]
        [TestCase("���{��,����, \r\n", 1)]
        [TestCase("\"���{ \r\n��\",\"\"\"����\"\"\",\"127,767,944\"", 1)]
        [TestCase("\"���{ \r\n��\",\"\"\"����\"\"\",\"127,767,944\" \r\n", 1)]
        [TestCase("\"���{ \r\n��\",\"\"\"����\"\"\",\"127,767,944\" \r\n\r\n", 2)]
        [TestCase("\"���{ \r\n��\",\"\"\"����\"\"\",\"127,767,944\" \r\n\n", 2)]
        [TestCase("\"aaa\",\"bbb\",\"ccc\" \r\nzzz, yyy, xxx", 2)]
        [TestCase("\"aaa\",\"b \r\nbb\",\"ccc\" \r\nzzz, yyy, xxx", 2)]
        [TestCase("\"aaa\",\"b\"\"bb\",\"ccc\"", 1)]
        public void CSV_���R�[�h�̍s���e�X�g(string text, int count)
        {
            var data = YutokunCSVParser.LoadFromString(text);

            Assert.AreEqual(data.Count(), count);
        }

        [TestCase("", 0)]
        [TestCase(" ", 1)]
        [TestCase("���{��,����,127767944 ", 3)]
        [TestCase("���{��,����, ", 3)]
        [TestCase("\"���{ \r\n��\",\"\"\"����\"\"\",\"127,767,944\"", 3)]
        [TestCase("\"aaa\",\"bbb\",\"ccc\"", 3)]
        [TestCase("\"aaa\",\"b \r\nbb\",\"ccc\"", 3)]
        [TestCase("\"\",\"b\"\"bb\",\"\"", 3)]
        public void CSV_�s�̃t�B�[���h���e�X�g(string text, int count)
        {
            var data = YutokunCSVParser.LoadFromString(text);
            var expect = data.FirstOrDefault();

            Assert.AreEqual(expect?.Count() ?? 0, count);
        }

        [TestCase("", null)]
        [TestCase(" ", new string[] { " " })]
        [TestCase("���{��,����,127767944 ", new string[] { "���{��", "����", "127767944 " })]
        [TestCase("���{��,����, ", new string[] { "���{��", "����", " " })]
        [TestCase("���{��,����,", new string[] { "���{��", "����", "" })]
        [TestCase("\"���{ \r\n��\",\"\"\"����\"\"\",\"127,767,944\"", new string[] { "���{ \r\n��", "\"����\"", "127,767,944" })]
        [TestCase("\"aaa\",\"bbb\",\"ccc\"", new string[] { "aaa", "bbb", "ccc" })]
        [TestCase("\"aaa\",\"b \r\nbb\",\"ccc\"", new string[] { "aaa", "b \r\nbb", "ccc" })]
        [TestCase("\"\",\"b\"\"bb\",\"\"", new string[] { "", "b\"bb", "" })]
        [TestCase("AAA, \"BBB\"", new string[] { "AAA", "BBB" })]
        public void CSV_�s�̃t�B�[���h�f�[�^�̈�v�e�X�g(string text, IEnumerable<string> actual)
        {
            var data = YutokunCSVParser.LoadFromString(text);
            var expect = data.FirstOrDefault();

            Assert.AreEqual(expect, actual);
        }



    }
}