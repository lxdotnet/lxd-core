
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core._MSTests.Domain;
using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Operators.Models;
using Lxdn.Core.Expressions.Operators.Models.Validation;
using Lxdn.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lxdn.Core.Expressions.Validation.Tests
{
    [TestClass]
    public class ValidationTests
    {
        private OperatorModelValidator validator;

        private AndModel model;

        [TestInitialize]
        public void TestInitialize()
        {
            var person = new Model("person", typeof(Person));
            var engine = new ExecutionEngine(person);
            this.validator = engine.Operators.Models.Validator;

            this.model = new AndModel
            {
                Operands = new[] {
                    new BinaryComparerModel {
                        Left = new PropertyModel { Path = "person.Relatives.Count" },
                        Right = new ConstModel { Value = "2" },
                        Operation = ExpressionType.Equal
                    },
                    new BinaryComparerModel {
                        Left = new PropertyModel { Path = "person.Birthday" },
                        Right = new ConstModel { Value = "1976-09-17" },
                        Operation = ExpressionType.Equal
                    },
                },
            };
        }

        [TestMethod]
        public void TestValid()
        {
            Assert.AreEqual(0, validator.Validate(this.model).Count());
        }

        [TestMethod]
        public void TestValidateMissingOperator()
        {
            ((BinaryComparerModel)this.model.Operands.First()).Right = null;
            var results = this.validator.Validate(this.model).ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(results[0].Id, "MissingValue"); // todo: 'MissingValue' should not be a magic constant here
            Assert.AreEqual(".Operands[0].Right", results[0].Path.Aggregate("", (path, step) => path += step.ToString()));
        }

        [TestMethod]
        public void TestValidateTooFewMembersOfLogicalOperator()
        {
            var operands = new List<OperatorModel>(this.model.Operands);
            operands.RemoveAt(1);
            this.model.Operands = operands;
            var results = this.validator.Validate(this.model).ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(results[0].Id, GenericMessages.LogicalOperatorHasTooFewMembers.ToString());
        }

        internal class WithNullables
        {
            public ExpressionType? MayBeType { get; set; }
        }

        [TestMethod]
        public void TestNullables()
        {
            var obj = new WithNullables();
            var results = this.validator.Validate(obj).ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("MissingValue", results[0].Id);
            Assert.AreEqual("MayBeType", results[0].Path.Single().Property.Name);
            obj.MayBeType = ExpressionType.Add;
            results = this.validator.Validate(obj).ToList();
            Assert.AreEqual(0, results.Count);
        }

        // test optional
        // test custom validator
    }
}
