
using System;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Operators.Models.Math;
using Lxdn.Core.Expressions.Operators.Models.Strings;

using Lxdn.Core._MSTests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lxdn.Core.Expressions.Utils;

namespace Lxdn.Core.Expressions._MSTests
{
    [TestClass]
    [DeploymentItem("Expressions\\ExpressionTests.xml", "")]
    public class Tests
    {
        private ExpressionTestsConfigs configs;

        private ExecutionEngine engine;

        private Person person;

        [TestInitialize]
        public void Initialize()
        {
            this.configs = new ExpressionTestsConfigs("ExpressionTests.xml");

            Model model = new Model("person", typeof(Person));
            this.engine = new ExecutionEngine(model);
            this.engine.Operators.Models.Parse(Assembly.GetExecutingAssembly());

            this.person = new Person("Alexander", "Dolnik", new DateTime(1976, 9, 17));
            this.person.MaritalStatus = MaritalStatus.Married;
            this.person.Relatives.Add(new Person("Katya", new DateTime(1978, 6, 26)));
            this.person.Relatives.Add(new Person("Sofia", new DateTime(2013, 10, 26)));
        }

        [TestMethod]
        public void TestConst()
        {
            var logic =this.configs.LogicOf("TestConst");
            var func = this.engine.Create<string>(logic);
            string result = func.From(this.person);

            Assert.AreEqual(result, "Tuesday");
        }

        [TestMethod]
        public void TestProperty()
        {
            var logic =this.configs.LogicOf("TestProperty");
            var func = this.engine.Create<DateTime>(logic);
            DateTime dt = func.From(this.person);

            Assert.AreEqual(dt, new DateTime(1976, 9, 17));
        }

        [TestMethod]
        public void TestEquals()
        {
            var logic =this.configs.LogicOf("TestEquals");
            var condition = this.engine.Create<bool>(logic);
            bool equals = condition.From(this.person);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void TestBinaryOp()
        {
            var logic =this.configs.LogicOf("TestBinaryOp");
            var condition = this.engine.Create<bool>(logic);
            bool exists = condition.From(this.person);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void TestLambdaAny()
        {
            var logic =this.configs.LogicOf("TestLambdaAny");
            var condition = this.engine.Create<bool>(logic);
            bool any = condition.From(this.person);

            Assert.IsTrue(any);
        }

        [TestMethod]
        public void TestOptimisticLambda()
        {
            var logic =this.configs.LogicOf("TestOptimisticLambda");
            IEvaluator<Person> func = this.engine.Create<Person>(logic);
            Person relative = func.From(this.person);

            Assert.AreEqual("Katya", relative.Name);
        }

        [TestMethod]
        public void TestPredicateOnOperatorDeliveringString()
        {
            var logic =this.configs.LogicOf("TestPredicateOnOperatorDeliveringString");
            var condition = this.engine.Create<bool>(logic);
            bool b = condition.From(this.person);

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestPredicateOnOperatorDeliveringProperty()
        {
            var logic =this.configs.LogicOf("TestPredicateOnOperatorDeliveringProperty");
            var condition = this.engine.Create<bool>(logic);
            bool b = condition.From(this.person);

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestPredicateOnOperatorDeliveringNullable()
        {
            var logic =this.configs.LogicOf("TestPredicateOnOperatorDeliveringNullable");
            var condition = this.engine.Create<bool>(logic);
            bool b = condition.From(this.person);

            Assert.IsFalse(b);
        }

        [TestMethod]
        public void TestLambdaScalarCount()
        {
            var logic =this.configs.LogicOf("TestLambdaScalarCount");
            IEvaluator<int> func = this.engine.Create<int>(logic);
            int count = func.From(this.person);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestMathAdd()
        {
            var logic =this.configs.LogicOf("TestMathAdd");
            IEvaluator<int> func = this.engine.Create<int>(logic);
            int count = func.From(this.person);

            Assert.AreEqual(2000, count);
        }

        [TestMethod]
        public void TestMathModulo()
        {
            var logic =this.configs.LogicOf("TestMathModulo");
            IEvaluator<int> func = this.engine.Create<int>(logic);
            int rest = func.From(this.person);

            Assert.AreEqual(8, rest);
        }

        [TestMethod]
        public void TestBlockAndRegex()
        {
            var logic =this.configs.LogicOf("TestBlockAndRegex");
            IEvaluator<int> func = this.engine.Create<int>(logic);
            int count = func.From(this.person);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void TestCallOfInstanceMethod()
        {
            var logic =this.configs.LogicOf("TestCallOfInstanceMethod");
            IEvaluator<string> func = this.engine.Create<string>(logic);
            string relationship = func.From(this.person);

            Assert.AreEqual("Alexander + Katya", relationship);
        }

        [TestMethod]
        public void TestCallOfStaticMethod()
        {
            var logic =this.configs.LogicOf("TestCallOfStaticMethod");
            IEvaluator<string> func = this.engine.Create<string>(logic);
            string result = func.From(this.person);

            Assert.AreEqual("My name is Alexander", result);
        }

        [TestMethod]
        public void TestDirectCallOfRegex()
        {
            var logic =this.configs.LogicOf("TestDirectCallOfRegex");
            IEvaluator<int> func = this.engine.Create<int>(logic);
            int result = func.From(this.person);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestStringFormatWithSpecializedSyntax()
        {
            var logic =this.configs.LogicOf("TestStringFormatWithSpecializedSyntax");
            IEvaluator<string> func = this.engine.Create<string>(logic);
            string result = func.From(this.person);

            // todo: attention, the result is formatted in the context of current thread's culture!
            Assert.IsTrue(result.StartsWith("My name is Alexander and my birthday is on "));
            Assert.IsTrue(result.Contains("17"));
            Assert.IsTrue(result.Contains("1976"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void TestStringFormatInvalidCountOfArguments()
        {
            var logic =this.configs.LogicOf("TestStringFormatInvalidCountOfArguments");
            IEvaluator<string> func = this.engine.Create<string>(logic);
            string result = func.From(this.person);
        }

        [TestMethod]
        public void TestSwitch()
        {
            var logic =this.configs.LogicOf("TestSwitch");
            IEvaluator<string> func = this.engine.Create<string>(logic);
            string result = func.From(this.person);

            Assert.AreEqual("married", result);

            result = func.From(this.person.Relatives[0]);
            Assert.AreEqual("not married", result);

            try
            {
                Person widow = new Person("Anonymous", new DateTime(1900, 1, 1));
                widow.MaritalStatus = MaritalStatus.Widowed;
                func.From(widow);
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
            var logic =this.configs.LogicOf("TestMatchOf");
            int c = this.engine.Create<int>(logic).From(this.person);
            Assert.AreEqual(c, 1);
        }

        [TestMethod]
        public void TestSequenceOfConditionsShouldNotFail()
        {
            var logic =this.configs.LogicOf("TestSequenceOfConditionsShouldNotFail");
            var b = (bool)this.engine.Create<object>(logic).From(this.person);
            Assert.AreEqual(b, false);
        }

        [TestMethod]
        public void TestEmptyStringOrNull()
        {
            var logic =this.configs.LogicOf("TestEmptyStringOrNull");
            bool result = this.engine.Create<bool>(logic).From(this.person);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestLinqWhere()
        {
            var logic =this.configs.LogicOf("TestLinqWhere");
            int countOfChildren = this.engine.Create<int>(logic).From(this.person);
            Assert.AreEqual(countOfChildren, 1);
        }

        [TestMethod]
        public void TestLinqWhereOnBooleanAndStringFormat()
        {
            var logic =this.configs.LogicOf("TestLinqWhereOnBooleanAndStringFormat");
            string format = this.engine.Create<string>(logic).From(this.person);
            Assert.AreEqual(format, "Count: 1");
        }

        [TestMethod]
        public void TestSwitchNullableHavingDefaultValue()
        {
            var logic =this.configs.LogicOf("TestSwitchNullable");
            string format = this.engine.Create<string>(logic).From(this.person);

            Assert.AreEqual("Unknown", format);
        }

        [TestMethod]
        public void TestTypedEvaluator()
        {
            var logic =this.configs.LogicOf("TestProperty");
            var evaluator = this.engine.Create<DateTime>(logic);
            var bd = evaluator.From(this.person);

            Assert.AreEqual(new DateTime(1976, 9, 17), bd);
        }

        // obsolete, delete later
        //[TestMethod]
        //public void TestAutoFormat2()
        //{
        //    var logic =this.configs.LogicOf("TestAutoFormat2");
        //    var evaluator = this.engine.Create<string>(logic);
        //    var formatted = evaluator.From(this.person);

        //    Assert.AreEqual(formatted, "My name is Alexander");
        //}

        [TestMethod]
        public void TestModulo()
        {
            var logic =this.configs.LogicOf("TestModulo");
            var evaluator = this.engine.Create<int>(logic);
            var remainder = evaluator.From(this.person);

            Assert.AreEqual(4, remainder);
        }

        // obsolete, delete later
        //[TestMethod]
        //public void TestStringFormatEx()
        //{
        //    var logic =this.configs.LogicOf("TestStringFormatEx");
        //    var evaluator = this.engine.Create<string>(logic);
        //    var formatted = evaluator.From(this.person);

        //    Assert.AreEqual("Alexander Dolnik: 17.9", formatted);
        //}

        [TestMethod]
        public void TestThrowSimple()
        {
            var logic =this.configs.LogicOf("TestThrowSimple");
            var evaluator = this.engine.Create<string>(logic);

            try
            {
                evaluator.From(this.person);
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
            var logic =this.configs.LogicOf("TestThrowCustomException");
            var evaluator = this.engine.Create<string>(logic);

            try
            {
                evaluator.From(this.person);
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
            var logic =this.configs.LogicOf("TestComparisonOfEquatables");
            var condition = this.engine.Create<bool>(logic);
            bool equals = condition.From(this.person);

            Assert.AreEqual(true, equals); // mony should be compared as value objects since they implement IEquatable<>
        }

        [TestMethod]
        public void TestCountOfObjectHavingSomeConcreteAttributeValue()
        {
            var logic =this.configs.LogicOf("TestCountOfObjectHavingSomeConcreteAttributeValue");
            var evaluator = this.engine.Create<int>(logic);
            int count = evaluator.From(this.person);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestCountOfObjectHavingSomeConcreteAttributeValue2()
        {
            var logic =this.configs.LogicOf("TestCountOfObjectHavingSomeConcreteAttributeValue2");
            var evaluator = this.engine.Create<int>(logic);
            int count = evaluator.From(this.person);

            Assert.AreEqual(1, count);
        }

        //[TestMethod]
        //public void TestFormatting()
        //{
        //    var money = this.person.Money.Amount.TryFormat(Thread.CurrentThread.CurrentUICulture);
        //    var logic =this.configs.LogicOf("TestFormatting");
        //    var op = this.engine.Create<>(xml);
        //    var evaluator = op.ToEvaluator<string>();
        //    var s = evaluator.From(this.person);

        //    Assert.AreEqual(s, "Here we are: 100");
        //}

        // obsolete, delete later
        //[TestMethod]
        //public void TestAutoFormatWithNull()
        //{
        //    this.person.Name = null;
        //    var logic =this.configs.LogicOf("TestAutoFormatWithNull");
        //    var evaluator = this.engine.Create<string>(logic);
        //    var s = evaluator.From(this.person);

        //    Assert.AreEqual(s, "My name is (null) Dolnik");
        //}

        [TestMethod]
        public void Test1()
        {
            //var result = DateTime.Now - TimeSpan.FromMinutes(5);
            var logic =this.configs.LogicOf("TestTimespan");
            var evaluator = this.engine.Create<DateTime>(logic);
            var time = evaluator.From(this.person);

            Assert.AreEqual(new DateTime(1976, 9, 16, 23, 55, 0), time);
        }

        [TestMethod]
        public void TestThrowCustomWithArgs()
        {
            var logic =this.configs.LogicOf("TestThrowCustomWithArgs");
            var evaluator = this.engine.Create<string>(logic);

            try
            {
                evaluator.From(this.person);
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
            var logic =this.configs.LogicOf("TestCompareWithString");
            var op = this.engine.Create<object>(logic);
        }

        [TestMethod]
        public void TestCustomOperator()
        {
            var logic =this.configs.LogicOf("TestCustomOperator");
            var now = this.engine.Create<DateTime>(logic).From(this.person);

            var span = now - DateTime.Now;
            Assert.IsTrue(span.TotalMilliseconds < 100);
        }

        [TestMethod]
        public void TestPropertyEvaluationFromPath()
        {
            var relatives = (IEnumerable<Person>)this.engine.Operators.CreateProperty("person.Relatives").Accessor.GetValue(person);
            Assert.AreEqual(relatives.Count(), 2);
        }

        [TestMethod]
        public void TestDomainAssemblies()
        {
            var browser = new TypeBrowser(this.engine.Models);
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
            var logic =this.configs.LogicOf("TestCustomOperatorModelHierarchy");
            var val = this.engine.Create<string>(logic).From(this.person);
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
            var logic =this.configs.LogicOf("TestStringOccurenceUsingContainsPredicate");
            var predicate = this.engine.Create<bool>(logic);
            bool contains = predicate.From(this.person);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void TestRootNamespaceMapping()
        {
            var ns = this.engine.Operators.Models.Namespaces.Map(typeof(StringOccurenceModel));
            Assert.AreEqual("Core.Strings", ns);

            // also check for emitted models:
            var linq = this.engine.Operators.Models.Single(model => model.Id == "Linq.Min");
            ns = this.engine.Operators.Models.Namespaces.Map(linq.Type);
            Assert.AreEqual("Core.Linq", ns);
        }

        [TestMethod]
        public void TestRuntimeContext()
        {
            var logic =this.configs.LogicOf("TestRuntimeContext");
            var result = this.engine.Create<string>(logic).From(this.person);
            Assert.AreEqual("42", result);
        }

        [TestMethod]
        public void TestPathsWithIndexers()
        {
            Person person = new Person("Alex", "Dolnik", new DateTime(1976, 9, 17));
            person.Relatives.Add(new Person("Katja", "Volkova", new DateTime(1978, 6, 26)));
            person.Relatives.Add(new Person("Sofia", "Dolnik", new DateTime(2013, 10, 26)));

            var model = new Model("person", typeof(Person));
            var engine = new ExecutionEngine(model);
            var prop = engine.Operators.CreateProperty("person.Relatives[1].Birthday");

            var bdOfSofia = prop.ToEvaluator<DateTime>(engine.Models).From(person);
            Assert.AreEqual(bdOfSofia, person.Relatives[1].Birthday);

            var prop2 = engine.Operators.CreateProperty("person.Relatives.Count");
            var count = prop2.ToEvaluator<int>(engine.Models).From(person);
            Assert.AreEqual(2, count);

            /*
            Persons persons = new Persons();
            persons.AddRange(person.Relatives);

            engine.Models.Add(new Model("persons", typeof(Persons)));
            Property prop3 = new Property("persons[0]", engine);
            var p = prop3.ToEvaluator<Person>().Run(persons);*/
            // does not work yet
        }

        [TestMethod]
        public void TestPropertyAccessor()
        {
            var person = new Person("Manuel", new DateTime(1991, 4, 13));
            var resolver = new PropertyResolver(typeof(Person));
            var property = resolver.Resolve("person.Birthday");
            var accessor = property.CreateAccessor(person);
            var birthday = accessor.Value;

            Assert.AreEqual(birthday, person.Birthday);

            var person2 = (Person)resolver.Resolve("person").CreateAccessor(person).Value;
            Assert.AreEqual(person2.Birthday, person.Birthday);
        }
    }
}
