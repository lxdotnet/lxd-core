
using System;
using System.Xml;
using System.Linq;
using System.Reflection;

using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Operators.Models.Math;
using Lxdn.Core.Expressions.Operators.Models.Strings;

using Lxdn.Core._MSTests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lxdn.Core.Expressions._MSTests
{
    [TestClass]
    [DeploymentItem("Expressions\\ExpressionTests.xml", "")]
    public class ExpressionTests
    {
        private ExpressionTestsConfigs testXml;

        private ExecutionEngine engine;

        private Person person;

        [TestInitialize]
        public void Initialize()
        {
            testXml = new ExpressionTestsConfigs("ExpressionTests.xml");

            var model = new Model("person", typeof(Person));
            engine = new ExecutionEngine(model);
            engine.Operators.Models.Parse(Assembly.GetExecutingAssembly());

            person = new Person("Alexander", "Dolnik", new DateTime(1976, 9, 17));
            person.MaritalStatus = MaritalStatus.Married;
            person.Relatives.Add(new Person("Katya", new DateTime(1978, 6, 26)));
            person.Relatives.Add(new Person("Sofia", new DateTime(2013, 10, 26)));
        }

        [TestMethod]
        public void TestConst()
        {
            var xml = testXml.Of("TestConst");
            var func = engine.Create<string>(xml);
            string result = func.Evaluate(person);

            Assert.AreEqual(result, "Tuesday");
        }

        [TestMethod]
        public void TestProperty()
        {
            var xml = testXml.Of("TestProperty");
            var func = engine.Create<DateTime>(xml);
            DateTime dt = func.Evaluate(person);

            Assert.AreEqual(dt, new DateTime(1976, 9, 17));
        }

        [TestMethod]
        public void TestEquals()
        {
            var xml = testXml.Of("TestEquals");
            var condition = engine.Create<bool>(xml);
            bool equals = condition.Evaluate(person);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void TestBinaryOp()
        {
            var xml = testXml.Of("TestBinaryOp");
            var condition = engine.Create<bool>(xml);
            bool exists = condition.Evaluate(person);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void TestLambdaAny()
        {
            var xml = testXml.Of("TestLambdaAny");
            var condition = engine.Create<bool>(xml);
            bool any = condition.Evaluate(person);

            Assert.IsTrue(any);
        }

        [TestMethod]
        public void TestOptimisticLambda()
        {
            var xml = testXml.Of("TestOptimisticLambda");
            IEvaluator<Person> func = engine.Create<Person>(xml);
            Person relative = func.Evaluate(person);

            Assert.AreEqual("Katya", relative.Name);
        }

        [TestMethod]
        public void TestPredicateOnOperatorDeliveringString()
        {
            var xml = testXml.Of("TestPredicateOnOperatorDeliveringString");
            var condition = engine.Create<bool>(xml);
            bool b = condition.Evaluate(person);

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestPredicateOnOperatorDeliveringProperty()
        {
            var xml = testXml.Of("TestPredicateOnOperatorDeliveringProperty");
            var condition = engine.Create<bool>(xml);
            bool b = condition.Evaluate(person);

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestPredicateOnOperatorDeliveringNullable()
        {
            var xml = testXml.Of("TestPredicateOnOperatorDeliveringNullable");
            var condition = engine.Create<bool>(xml);
            bool b = condition.Evaluate(person);

            Assert.IsFalse(b);
        }

        [TestMethod]
        public void TestLambdaScalarCount()
        {
            var xml = testXml.Of("TestLambdaScalarCount");
            IEvaluator<int> func = engine.Create<int>(xml);
            int count = func.Evaluate(person);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestMathAdd()
        {
            var xml = testXml.Of("TestMathAdd");
            IEvaluator<int> func = engine.Create<int>(xml);
            int count = func.Evaluate(person);

            Assert.AreEqual(2000, count);
        }

        [TestMethod]
        public void TestMathModulo()
        {
            var xml = testXml.Of("TestMathModulo");
            IEvaluator<int> func = engine.Create<int>(xml);
            int rest = func.Evaluate(person);

            Assert.AreEqual(8, rest);
        }

        [TestMethod]
        public void TestBlockAndRegex()
        {
            var xml = testXml.Of("TestBlockAndRegex");
            IEvaluator<int> func = engine.Create<int>(xml);
            int count = func.Evaluate(person);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void TestCallOfInstanceMethod()
        {
            var xml = testXml.Of("TestCallOfInstanceMethod");
            IEvaluator<string> func = engine.Create<string>(xml);
            string relationship = func.Evaluate(person);

            Assert.AreEqual("Alexander + Katya", relationship);
        }

        [TestMethod]
        public void TestCallOfStaticMethod()
        {
            var xml = testXml.Of("TestCallOfStaticMethod");
            IEvaluator<string> func = engine.Create<string>(xml);
            string result = func.Evaluate(person);

            Assert.AreEqual("My name is Alexander", result);
        }

        [TestMethod]
        public void TestDirectCallOfRegex()
        {
            var xml = testXml.Of("TestDirectCallOfRegex");
            IEvaluator<int> func = engine.Create<int>(xml);
            int result = func.Evaluate(person);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestStringFormatWithSpecializedSyntax()
        {
            var xml = testXml.Of("TestStringFormatWithSpecializedSyntax");
            IEvaluator<string> func = engine.Create<string>(xml);
            string result = func.Evaluate(person);

            // todo: attention, the result is formatted in the context of current thread's culture!
            Assert.IsTrue(result.StartsWith("My name is Alexander and my birthday is on "));
            Assert.IsTrue(result.Contains("17"));
            Assert.IsTrue(result.Contains("1976"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void TestStringFormatInvalidCountOfArguments()
        {
            var xml = testXml.Of("TestStringFormatInvalidCountOfArguments");
            IEvaluator<string> func = engine.Create<string>(xml);
            string result = func.Evaluate(person);
        }

        [TestMethod]
        public void TestSwitch()
        {
            var xml = testXml.Of("TestSwitch");
            IEvaluator<string> func = engine.Create<string>(xml);
            string result = func.Evaluate(person);

            Assert.AreEqual("married", result);

            result = func.Evaluate(person.Relatives[0]);
            Assert.AreEqual("not married", result);

            try
            {
                Person widow = new Person("Anonymous", new DateTime(1900, 1, 1));
                widow.MaritalStatus = MaritalStatus.Widowed;
                func.Evaluate(widow);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.InnerException.Message.Contains("Widowed"));
                return;
            }

            Assert.Fail("Must throw an exception");
            // todo: write a test for testing default: in switch
        }

        [TestMethod]
        public void TestMatchOf()
        {
            var xml = testXml.Of("TestMatchOf");
            int c = engine.Create<int>(xml).Evaluate(person);
            Assert.AreEqual(c, 1);
        }

        [TestMethod]
        public void TestSequenceOfConditionsShouldNotFail()
        {
            var xml = testXml.Of("TestSequenceOfConditionsShouldNotFail");
            var b = (bool)engine.Create<object>(xml).Evaluate(person);
            Assert.AreEqual(b, false);
        }

        [TestMethod]
        public void TestEmptyStringOrNull()
        {
            var xml = testXml.Of("TestEmptyStringOrNull");
            bool result = engine.Create<bool>(xml).Evaluate(person);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestLinqWhere()
        {
            var xml = testXml.Of("TestLinqWhere");
            int countOfChildren = engine.Create<int>(xml).Evaluate(person);
            Assert.AreEqual(countOfChildren, 1);
        }

        [TestMethod]
        public void TestLinqWhereOnBooleanAndStringFormat()
        {
            var xml = testXml.Of("TestLinqWhereOnBooleanAndStringFormat");
            string format = engine.Create<string>(xml).Evaluate(person);
            Assert.AreEqual(format, "Count: 1");
        }

        [TestMethod]
        public void TestSwitchNullableHavingDefaultValue()
        {
            var xml = testXml.Of("TestSwitchNullable");
            string format = engine.Create<string>(xml).Evaluate(person);

            Assert.AreEqual("Unknown", format);
        }

        [TestMethod]
        public void TestTypedEvaluator()
        {
            var xml = testXml.Of("TestProperty");
            var evaluator = engine.Create<DateTime>(xml);
            var bd = evaluator.Evaluate(person);

            Assert.AreEqual(new DateTime(1976, 9, 17), bd);
        }

        // obsolete, delete later
        //[TestMethod]
        //public void TestAutoFormat2()
        //{
        //    var xml = this.testXml.LogicOf("TestAutoFormat2");
        //    var evaluator = engine.Create<string>(logic);
        //    var formatted = evaluator.Evaluate(person);

        //    Assert.AreEqual(formatted, "My name is Alexander");
        //}

        [TestMethod]
        public void TestModulo()
        {
            var xml = testXml.Of("TestModulo");
            var evaluator = engine.Create<int>(xml);
            var remainder = evaluator.Evaluate(person);

            Assert.AreEqual(4, remainder);
        }

        // obsolete, delete later
        //[TestMethod]
        //public void TestStringFormatEx()
        //{
        //    var xml = this.testXml.LogicOf("TestStringFormatEx");
        //    var evaluator = engine.Create<string>(logic);
        //    var formatted = evaluator.Evaluate(person);

        //    Assert.AreEqual("Alexander Dolnik: 17.9", formatted);
        //}

        [TestMethod]
        public void TestThrowSimple()
        {
            var xml = testXml.Of("TestThrowSimple");
            var evaluator = engine.Create<string>(xml);

            try
            {
                evaluator.Evaluate(person);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.InnerException.Message, "Error occurred");
                return;
            }

            throw new Exception("Must throw");
        }

        [TestMethod]
        public void TestThrowCustomException()
        {
            var xml = testXml.Of("TestThrowCustomException");
            var evaluator = engine.Create<string>(xml);

            try
            {
                evaluator.Evaluate(person);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.InnerException.Message, "September");
                return;
            }

            throw new Exception("Must throw");
        }

        [TestMethod]
        public void TestComparisonOfEquatables_Equal()
        {
            var xml = testXml.Of("TestComparisonOfEquatables");
            var condition = engine.Create<bool>(xml);
            bool equals = condition.Evaluate(person);

            Assert.AreEqual(true, equals); // mony should be compared as value objects since they implement IEquatable<>
        }

        [TestMethod]
        public void TestCountOfObjectHavingSomeConcreteAttributeValue()
        {
            var xml = testXml.Of("TestCountOfObjectHavingSomeConcreteAttributeValue");
            var evaluator = engine.Create<int>(xml);
            int count = evaluator.Evaluate(person);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestCountOfObjectHavingSomeConcreteAttributeValue2()
        {
            var xml = testXml.Of("TestCountOfObjectHavingSomeConcreteAttributeValue2");
            var evaluator = engine.Create<int>(xml);
            int count = evaluator.Evaluate(person);

            Assert.AreEqual(1, count);
        }

        //[TestMethod]
        //public void TestFormatting()
        //{
        //    var money = this.person.Money.Amount.TryFormat(Thread.CurrentThread.CurrentUICulture);
        //    var xml = this.testXml.LogicOf("TestFormatting");
        //    var op = engine.Create<>(xml);
        //    var evaluator = op.ToEvaluator<string>();
        //    var s = evaluator.Evaluate(person);

        //    Assert.AreEqual(s, "Here we are: 100");
        //}

        // obsolete, delete later
        //[TestMethod]
        //public void TestAutoFormatWithNull()
        //{
        //    this.person.Name = null;
        //    var xml = this.testXml.LogicOf("TestAutoFormatWithNull");
        //    var evaluator = engine.Create<string>(logic);
        //    var s = evaluator.Evaluate(person);

        //    Assert.AreEqual(s, "My name is (null) Dolnik");
        //}

        [TestMethod]
        public void Test1()
        {
            //var result = DateTime.Now - TimeSpan.FromMinutes(5);
            var xml = testXml.Of("TestTimespan");
            var evaluator = engine.Create<DateTime>(xml);
            var time = evaluator.Evaluate(person);

            Assert.AreEqual(new DateTime(1976, 9, 16, 23, 55, 0), time);
        }

        [TestMethod]
        public void TestThrowCustomWithArgs()
        {
            var xml = testXml.Of("TestThrowCustomWithArgs");
            var evaluator = engine.Create<string>(xml);

            try
            {
                evaluator.Evaluate(person);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.InnerException.Message, "September");
                return;
            }

            throw new Exception("Must throw");
        }

        [TestMethod]
        public void TestCompareWithString()
        {
            var xml = testXml.Of("TestCompareWithString");
            var op = engine.Create<object>(xml);
        }

        [TestMethod]
        public void TestCustomOperator()
        {
            var logic = testXml.Of("TestCustomOperator");
            var now = engine.Create<DateTime>(logic).Evaluate(person);

            var span = now - DateTime.Now;
            Assert.IsTrue(span.TotalMilliseconds < 100);
        }

        [TestMethod]
        public void TestDomainAssemblies()
        {
            var browser = new TypeBrowser(engine.Models);
            var n = 0;
            var known = browser.KnownTypes.Select(t => new { Value = t, Number = n++ })
                   .FirstOrDefault(t => typeof(Person) == t.Value);

            //browser.FriendlyAssemblies.ToList().ForEach(assembly => Debug.WriteLine(assembly.FullName));

            Assert.AreEqual(5, browser.FriendlyAssemblies.Count());
            Assert.IsTrue(known.Number < 250);
        }

        [TestMethod]
        public void TestCustomOperatorModelHierarchy()
        {
            var xml = testXml.Of("TestCustomOperatorModelHierarchy");
            var val = engine.Create<string>(xml).Evaluate(person);
            Assert.AreEqual("Test", val);
        }
        /*
        [TestMethod]
        public void TestDynamic()
        {
            dynamic instance = new DynamicModel();
            var model = new Model("person", instance.GetType());
            var val1 = instance.Name;

            IDynamicMetaObjectProvider provider = instance.GetType() as IDynamicMetaObjectProvider;

            var engine = new ExecutionEngine(model);
            var property = engine.Operators.CreateProperty("person.Name");            
            object val = property.ToAccessor().GetValue(instance);
        }*/

        [TestMethod]
        public void TestEmit()
        {
            var derived = OperatorModelEmitter.DeriveFrom(typeof(MathModel), "MathAddModel");
            Assert.AreEqual(2, derived.GetConstructors().Length);
            Assert.AreEqual(0, derived.GetConstructors()[0].GetParameters().Length);
            Assert.AreEqual(2, derived.GetConstructors()[1].GetParameters().Length);
            Assert.AreEqual(typeof(XmlNode), derived.GetConstructors()[1].GetParameters()[0].ParameterType);
            Assert.AreEqual(typeof(OperatorModelFactory), derived.GetConstructors()[1].GetParameters()[1].ParameterType);
            Assert.AreEqual(typeof(MathModel).Namespace, derived.Namespace);
        }

        [TestMethod]
        public void TestStringOccurenceUsingContainsPredicate()
        {
            var xml = testXml.Of("TestStringOccurenceUsingContainsPredicate");
            var predicate = engine.Create<bool>(xml);
            bool contains = predicate.Evaluate(person);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void TestRootNamespaceMapping()
        {
            var ns = engine.Operators.Models.Namespaces.Map(typeof(StringOccurenceModel));
            Assert.AreEqual("Core.Strings", ns);

            // also check for emitted models:
            var linq = engine.Operators.Models.Single(model => model.Id == "Linq.Min");
            ns = engine.Operators.Models.Namespaces.Map(linq.Type);
            Assert.AreEqual("Core.Linq", ns);
        }

        [TestMethod]
        public void TestRuntimeContext()
        {
            var xml = testXml.Of("TestRuntimeContext");
            var result = engine.Create<string>(xml).Evaluate(person);
            Assert.AreEqual("42", result);
        }

        //[TestMethod] // currently fails as we don't support indices since the last refactoring - may in the future
        public void TestPathsWithIndexers()
        {
            Person person = new Person("Alex", "Dolnik", new DateTime(1976, 9, 17));
            person.Relatives.Add(new Person("Katja", "Volkova", new DateTime(1978, 6, 26)));
            person.Relatives.Add(new Person("Sofia", "Dolnik", new DateTime(2013, 10, 26)));

            var model = new Model("person", typeof(Person));
            var engine = new ExecutionEngine(model);
            var prop = engine.Operators.CreateProperty("person.Relatives[1].Birthday");

            var bdOfSofia = prop.ToEvaluator<DateTime>(engine.Models).Evaluate(person);
            Assert.AreEqual(bdOfSofia, person.Relatives[1].Birthday);

            var prop2 = engine.Operators.CreateProperty("person.Relatives.Count");
            var count = prop2.ToEvaluator<int>(engine.Models).Evaluate(person);
            Assert.AreEqual(2, count);

            /*
            Persons persons = new Persons();
            persons.AddRange(person.Relatives);

            engine.Models.Add(new Model("persons", typeof(Persons)));
            Property prop3 = new Property("persons[0]", engine);
            var p = prop3.ToEvaluator<Person>().Run(persons);*/
            // does not work yet
        }
    }
}
