﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CobieLiteUk;
using Xbim.CobieLiteUk.Validation.Reporting;

namespace Tests
{
     [DeploymentItem(@"TestFiles\")]
    [TestClass]
    public class ConvertCobieLiteUkToValidationReportTests
    {
        [TestMethod]
        public void ConvertCobieLiteUkToValidationReportTest()
        {
            var facility = Facility.ReadJson("ValidationReport.json");
            var rep = new ExcelValidationReport();
            Assert.IsTrue(rep.Create(facility, "ValidationReport",
                ExcelValidationReport.SpreadSheetFormat.Xlsx));
        }
    }
}
