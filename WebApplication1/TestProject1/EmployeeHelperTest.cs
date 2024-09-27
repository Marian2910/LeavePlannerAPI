using Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestFixture]
   public class EmployeeHelperTest
    {
        [Test]
        public void CalculateLeaveDays()
        {
            //arrange
            DateTime employeeDate = new DateTime(2018, 6, 15);
            int leaveDays = 0;

            //act
            int result = EmployeeHelper.CalculateLeaveDays(employeeDate, leaveDays);

            //assert
            Assert.That(result, Is.EqualTo(6));
        }

        [Test]
        public void CalculateLeaveDays_ReturnZero()
        {
            //arrange 
            DateTime employeeDate = DateTime.Now;
            int leaveDays = 0;

            //act
            int result = EmployeeHelper.CalculateLeaveDays(employeeDate, leaveDays);

            //assert
            Assert.That(result, Is.EqualTo(0));
        }

    }
}
