using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonLogicCSharp;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void OpTestDeepEqual()
        {
            Assert.IsFalse(JsonLogic.Apply("{\"===\" :[2, 2.0]}"));
            Assert.IsTrue(JsonLogic.Apply("{\"===\" :[2.0, 2.0]}"));
            Assert.IsTrue(JsonLogic.Apply("{\"===\" :[\"dog\", \"dog\"]}"));
        }

        [TestMethod]
        public void OpTestShallowEqual()
        {
            Assert.IsTrue(JsonLogic.Apply("{\"==\" :[2, 2.0]}"));
            Assert.IsTrue(JsonLogic.Apply("{\"==\" :[2.0, 2.0]}"));
            Assert.IsFalse(JsonLogic.Apply("{\"==\" :[2.0, 3.0]}"));
            Assert.IsTrue(JsonLogic.Apply("{\"==\" :[\"dog\", \"dog\"]}"));

            Assert.IsTrue(JsonLogic.Apply("{'==' :[2, 2.0]}"));
            Assert.IsTrue(JsonLogic.Apply("{'==' :[2.0, 2.0]}"));
            Assert.IsFalse(JsonLogic.Apply("{'==' :[2.0, 3.0]}"));
            Assert.IsTrue(JsonLogic.Apply("{'==' :['dog', 'dog']}"));
        }

        [TestMethod]
        public void OpTestNot01()
        {
            Assert.IsTrue(JsonLogic.Apply("{\"!\" :false}"));
            Assert.IsFalse(JsonLogic.Apply("{\"!\" :true}"));
            Assert.IsFalse(JsonLogic.Apply("{\"!!\" :false}"));
            Assert.IsTrue(JsonLogic.Apply("{\"!!\" :true}"));
        }

        [TestMethod]
        public void OpTestNot02()
        {
            Assert.IsTrue(JsonLogic.Apply("{\"!\" :[false]}"));
            Assert.IsFalse(JsonLogic.Apply("{\"!\" :[true]}"));
            Assert.IsFalse(JsonLogic.Apply("{\"!!\" :[false]}"));
            Assert.IsTrue(JsonLogic.Apply("{\"!!\" :[true]}"));
        }

        [TestMethod]
        public void OpTestAdd01()
        {
            Assert.AreEqual(5, JsonLogic.Apply("{\"+\" :[2, 3]}"));
            Assert.AreEqual(4.0, JsonLogic.Apply("{\"+\" :[2.0, 2.0]}"));
            Assert.AreEqual(8, JsonLogic.Apply("{\"+\" :[2, 2, 2, 2]}"));
            Assert.AreEqual(10, JsonLogic.Apply("{\"+\" :[10]}"));
            Assert.AreEqual(10, JsonLogic.Apply("{\"+\" :10}"));
        }

        [TestMethod]
        public void OpTestMult01()
        {
            Assert.AreEqual(6, JsonLogic.Apply("{'*' :[2, 3]}"));
            Assert.AreEqual(9.0, JsonLogic.Apply("{\"*\" :[3.0, 3.0]}"));
            Assert.AreEqual(16, JsonLogic.Apply("{\"*\" :[2, 2, 2, 2]}"));
            Assert.AreEqual(10, JsonLogic.Apply("{\"*\" :[10]}"));
            Assert.AreEqual(10, JsonLogic.Apply("{\"*\" :10}"));
        }

        [TestMethod]
        public void OpTestDiv01()
        {
            Assert.AreEqual(6, JsonLogic.Apply("{'/' :[12, 2]}"));
            Assert.AreEqual(2.5, JsonLogic.Apply("{'/' :[5.0, 2.0]}"));
            Assert.AreEqual(2, JsonLogic.Apply("{'/' :[5, 2]}"));
        }

        [TestMethod]
        public void OpTestLessThan01()
        {
            Assert.IsTrue(JsonLogic.Apply("{'<' :[2, 3]}"));
            Assert.IsFalse(JsonLogic.Apply("{'<' :[3, 2]}"));
            Assert.IsTrue(JsonLogic.Apply("{'<' :[1, 2, 3, 4, 5]}"));
            Assert.IsFalse(JsonLogic.Apply("{'<' :[2, 3, 4, 1]}"));
        }

        [TestMethod]
        public void OpTestGreaterThan01()
        {
            Assert.IsFalse(JsonLogic.Apply("{'>' :[2, 3]}"));
            Assert.IsTrue(JsonLogic.Apply("{'>' :[3, 2]}"));
            Assert.IsFalse(JsonLogic.Apply("{'>' :[1, 2, 3, 4, 5]}"));
            Assert.IsTrue(JsonLogic.Apply("{'>' :[5, 4, 3, 2, 1]}"));
        }

        [TestMethod]
        public void OpTestComplexAdd01()
        {
            Assert.AreEqual(27, JsonLogic.Apply(
                            @"{'+': 
                                [   { '*' : [3,2]},
                                    { '+' : [2,2,2,2,2]},
                                    { '+' : [2,2,2,2,2]},
                                    { '/' : [10,10]}
                                ]
                              }"));
        }

        [TestMethod]
        public void OpTestBoolean01()
        {
            Assert.IsFalse(JsonLogic.Apply("{'and': [true, true, true, false]}"));
            Assert.IsTrue(JsonLogic.Apply("{'and': [true, true, true, true]}"));
            Assert.IsTrue(JsonLogic.Apply("{'or': [false, false, false, true]}"));
            Assert.IsFalse(JsonLogic.Apply("{'or': [false, false, false, false]}"));
        }

        [TestMethod]
        public void OpTestMin01()
        {
            Assert.AreEqual(1, JsonLogic.Apply("{'min': [1,2,3]}"));
            Assert.AreEqual(8, JsonLogic.Apply("{'min': [10,9,8]}"));
            Assert.AreEqual(8.1, JsonLogic.Apply("{'min': [10.1,9.1,8.1]}"));
        }

        [TestMethod]

        public void OpTestMax01()
        {
            Assert.AreEqual(3, JsonLogic.Apply("{'max': [1,2,3]}"));
            Assert.AreEqual(10, JsonLogic.Apply("{'max': [10,9,8]}"));
            Assert.AreEqual(10.1, JsonLogic.Apply("{'max': [10.1,9.1,8.1]}"));
        }

        [TestMethod]
        public void OpTestIf01()
        {
            Assert.AreEqual("yes", JsonLogic.Apply("{'if' : [true, 'yes', 'no']}"));
            Assert.AreEqual("no", JsonLogic.Apply("{'if' : [false, 'yes', 'no']}"));
        }

        [TestMethod]
        public void OpTestIn01()
        {
            Assert.IsTrue(JsonLogic.Apply("{'in':[ 'Ringo', ['John', 'Paul', 'George', 'Ringo'] ]}"));
            Assert.IsFalse(JsonLogic.Apply("{'in':[ 'Ringo', ['John', 'Paul', 'George'] ]}"));
            Assert.IsTrue(JsonLogic.Apply("{'in':[ 1, [2, 3, 4, 1] ]}"));
            Assert.IsFalse(JsonLogic.Apply("{'in':[ 1, [2, 3, 4] ]}"));
        }

        [TestMethod]
        public void OpTestIn02()
        {
            Assert.IsTrue(JsonLogic.Apply("{'in':[ 'Spring', 'Springfield']}"));
            Assert.IsFalse(JsonLogic.Apply("{'in':[ 'Springx', 'Springfield']}"));
        }

        [TestMethod]
        public void OpTestBoolean02()
        {
            Assert.IsTrue(JsonLogic.Apply(
                            @"{'and': 
                                [   { '==' : [3,3]},
                                    { '==' : [2,2]},
                                    { '!=' : [4,2]}                                   
                                ]
                              }"));

            Assert.IsFalse(JsonLogic.Apply(
                           @"{'and': 
                                [   { '==' : [3,3]},
                                    { '==' : [2,2]},
                                    { '==' : [4,2]}                                   
                                ]
                              }"));
        }

        [TestMethod]
        public void OpTestVar01()
        {
            string rule = @"{'var': 'a'}";
            string data = @"{'a' : 1}";

            Assert.AreEqual(1, JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestVar02()
        {
            string rule = @"{'var': 'person1.age'}";
            string data = @"{'person1' : {'name': 'Mary', 'age': 10, },
                             'person2' : {'name': 'Joe', 'age': 15, }}";

            Assert.AreEqual(10, JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestVar03()
        {
            string rule = @"{'+': [1, {'var':'age'}, 2]}";
            string data = @"{'age' : 10, 'salary' : 150000}";

            Assert.AreEqual(13, JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestVar04()
        {
            string rule = @"{'>': [{'var':'age'}, 30]}";
            string data = @"{'age' : 25, 'salary' : 150000}";

            Assert.IsFalse(JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestVar05()
        {
            string rule = @"{'<': [{'var':'age'}, 30]}";
            string data = @"{'age' : 25, 'salary' : 150000}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestVar06()
        {
            string rule = @"{'var': 1}";
            string data = @"{'x': ['zero', 'one', 'two', 'three']}";

            Assert.AreEqual("one", JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestMissing01()
        {
            string rule = @"{'missing': ['a', 'b', 'c']}";
            string data = @"{'a':'ape', 'b':'boy'}";

            CollectionAssert.AreEqual(new string[] { "c" }, JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestMissing02()
        {
            string rule = @"{'missing': ['a', 'b', 'c']}";
            string data = @"{'a':'ape', 'b':'boy', 'c':'cat'}";

            CollectionAssert.AreEqual(new string[] { }, JsonLogic.Apply(rule, data));
        }



        [TestMethod]
        public void OpTestSomeMissing01()
        {
            string rule = @"{'missing_some': ['a', 'b', 'c']}";
            string data = @"{'a':'ape', 'b':'boy', 'c':'cat'}";

            Assert.AreEqual(false, JsonLogic.Apply(rule, data));

            rule = @"{'missing_some': ['a', 'b', 'c']}";
            data = @"{'a':'ape', 'b':'boy'}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));
        }

        [TestMethod]
        public void OpTestMerge01()
        {
            string rule = @"{'merge' :[ [1,2], [3,4] ]}";

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, JsonLogic.Apply(rule, null));
        }

        [TestMethod]
        public void OpTestMerge02()
        {
            string rule = @"{'merge' :[ ['a', 'b'], ['c','d'] ]}";

            CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d" }, JsonLogic.Apply(rule, null));
        }


        [TestMethod]
        public void OpTestCat01()
        {
            string rule = @"{'cat' : ['The ', 'cat ', 'in ', 'the ', 'hat.']}";

            Assert.AreEqual("The cat in the hat.", JsonLogic.Apply(rule, null));
        }

        [TestMethod]
        public void OpTestSome01()
        {
            string rule = @"{ 'some' : [ {'var':'pies'}, {'==':[{'var':'filling'}, 'apple']} ]}";
            string data = @"
                        {'pies':[
                                 { 'filling':'pumpkin','temp':110},
                                 { 'filling':'rhubarb','temp':210},
                                 { 'filling':'apple','temp':310}
                        ]}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));

            rule = @"{ 'some' : [ {'var':'pies'}, {'==':[{'var':'filling'}, 'peach']} ]}";

            Assert.IsFalse(JsonLogic.Apply(rule, data));

            rule = @"{ 'some' : [ {'var':'pies'}, {'==':[{'var':'temp'}, 210]} ]}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));


        }

        [TestMethod]
        public void OpTestNone01()
        {
            string rule = @"{ 'none' : [ {'var':'pies'}, {'==':[{'var':'filling'}, 'apple']} ]}";
            string data = @"
                        {'pies':[
                                 { 'filling':'pumpkin','temp':110},
                                 { 'filling':'rhubarb','temp':210},
                                 { 'filling':'apple','temp':310}
                        ]}";

            Assert.IsFalse(JsonLogic.Apply(rule, data));

            rule = @"{ 'none' : [ {'var':'pies'}, {'==':[{'var':'filling'}, 'peach']} ]}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));

            rule = @"{ 'none' : [ {'var':'pies'}, {'==':[{'var':'temp'}, 210]} ]}";

            Assert.IsFalse(JsonLogic.Apply(rule, data));


        }

        [TestMethod]
        public void OpTestAll01()
        {
            string rule = @"{ 'all' : [ {'var':'pies'}, {'==':[{'var':'filling'}, 'apple']} ]}";
            string data = @"
                        {'pies':[
                                 { 'filling':'apple','temp':110},
                                 { 'filling':'apple','temp':210},
                                 { 'filling':'apple','temp':310}
                        ]}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));
            

            rule = @"{ 'all' : [ {'var':'pies'}, {'==':[{'var':'filling'}, 'peach']} ]}";

            Assert.IsFalse(JsonLogic.Apply(rule, data));

            rule = @"{ 'all' : [ {'var':'pies'}, {'==':[{'var':'temp'}, 210]} ]}";

            Assert.IsFalse(JsonLogic.Apply(rule, data));

            data = @"
                        {'pies':[
                                 { 'filling':'peach','temp':300},
                                 { 'filling':'pear','temp':300},
                                 { 'filling':'apple','temp':300}
                        ]}";

            rule = @"{ 'all' : [ {'var':'pies'}, {'==':[{'var':'temp'}, 300]} ]}";

            Assert.IsTrue(JsonLogic.Apply(rule, data));
        }
    }
}
