using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncTemplatesMgr;
using EncTemplatesMgr.Common;
using EncTemplatesMgr.Models;

namespace EncTemplatesMgr_Tests
{
    [TestFixture]
    internal class FilterTests
    {
        private CustomDataTemplate _customDataTemplate;
        private CustomClosingCostTemplate _customClosingCostTemplate;

        [SetUp]
        public void SetUp()
        {
            _customDataTemplate = new CustomDataTemplate()
            {
                Name = "NewTestDataTemplate",
                FilePath = @"\\New\Test\Path",
                FieldIDsAndValues = new Dictionary<string, string>()
                {
                    { "CX.TEST1", "One" },
                    { "CX.TEST2", "Two" },
                    { "CX.TEST3", "Three" }
                }
            };

            _customClosingCostTemplate = new CustomClosingCostTemplate()
            {
                Name = "NewTestClosingCostTemplate",
                FilePath = @"\\New\Test\Path",
                FieldIDsAndValues = new Dictionary<string, string>()
                {
                    { "CX.TEST1", "One" },
                    { "CX.TEST2", "Two" },
                    { "CX.TEST3", "Three" }
                }
            };
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TemplateInFilter_DataTemplate_WhenCalled_ReturnBoolIfInFilter(string filePathContains, string templateNameContains, Dictionary<string, string> fieldValuesContains, bool expectedResult)
        {
            var filter = new Filter()
            {
                FilterFilePath = filePathContains,
                FilterTemplateName = templateNameContains,
                FilterFieldValues = fieldValuesContains
            };

            var result = filter.TemplateInFilter(_customDataTemplate);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TemplateInFilter_ClosingCostTemplate_WhenCalled_ReturnBoolIfInFilter(string filePathContains, string templateNameContains, Dictionary<string, string> fieldValuesContains, bool expectedResult)
        {
            var filter = new Filter()
            {
                FilterFilePath = filePathContains,
                FilterTemplateName = templateNameContains,
                FilterFieldValues = fieldValuesContains
            };

            var result = filter.TemplateInFilter(_customClosingCostTemplate);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        static object[] TestCases =
        {
            new object[] { "*", "", null, true },
            new object[] { "XYZ", "*", null, true },
            new object[] { "Test", "", null, true },
            new object[] { "", "Test", null, true },
            new object[] { "Test", "XYZ", null, true },
            new object[] { "XYZ", "Test", null, true },
            new object[] { "Tester", "", null, false },
            new object[] { "", "Testing", null, false },
            new object[] { "XYZ", "Testing", null, false },

            new object[]
            {
                "",
                "",
                new Dictionary<string, string>()
                {
                    { "CX.TEST1", "One"}
                },
                true
            },

            new object[]
            {
                "",
                "",
                new Dictionary<string, string>()
                {
                    { "CX.TEST1", "One"},
                    { "CX.TEST2", "Two"}
                },
                true
            },

            new object[]
            {
                "",
                "",
                new Dictionary<string, string>()
                {
                    { "CX.TEST2", "Two"},
                    { "CX.TEST3", "Four"}
                },
                false
            }
        };
    }
}
