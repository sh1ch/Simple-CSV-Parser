using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Tests1
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
            var data = CsvParser.ParseFromText(text);

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
            var expect = CsvParser.ParseFieldsFromText(text);

            Assert.AreEqual(expect.Count(), count);
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
            var data = CsvParser.ParseFromText(text);
            var expect = data.FirstOrDefault();

            Assert.AreEqual(expect, actual);
        }

        [TestCase("sample.csv", 12)]
        public void Csv_�t�@�C���̓ǂݍ��݃e�X�g(string fileName, int count)
        {
            var path = System.AppDomain.CurrentDomain.BaseDirectory;
            var data = CsvParser.ParseFromFile(System.IO.Path.Combine(path, fileName));

            Assert.AreEqual(data?.Count() ?? 0, count);
        }

    }
}