using Cgtor.Lib.Controllers;
using Cgtor.Lib.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Cgtor.Lib.Test.Controllers
{
    [TestFixture(TestOf = typeof(ModelProperty)), Category("Unit")]
    public class ModelPropertyControllerTests
    {
        private ModelPropertyController _target;
        private List<ModelProperty> _listModelProperties;

        [SetUp]
        public void SetUp()
        {
            _target = new ModelPropertyController();

            _listModelProperties = new List<ModelProperty>
            {
                new ModelProperty
                {
                    PropertyName = "RATE_GROUP_CODE",
                    PropertyDataType = "int",
                    IsNullable = true
                },
                new ModelProperty
                {
                    PropertyName = "SERVICE_LEVEL_NO",
                    PropertyDataType = "int",
                    IsNullable = true
                },
            };
        }

        [Test]
        public void Test_GetNullableMarkIfApplicable()
        {
            const string expectedMark = "?";

            var result = _target.GetNullableMarkIfApplicable(_listModelProperties[0]);

            Assert.AreEqual(expectedMark, result);
        }
    }
}