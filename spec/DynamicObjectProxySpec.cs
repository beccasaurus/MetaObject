using System;
using System.Dynamic;
using NUnit.Framework;
using Microsoft.CSharp.RuntimeBinder;

public class HasDynamicStuff : IDynamicMetaObjectProvider {

	#region DynamicObjectProxy
	public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression e){ return new MetaObject(e, this); }
	#endregion

	#region Properties for testing
	public string MyPropertyValue;
	#endregion

	public string RealMethod() { return "hi from a real, non-dynamic method"; }

	#region DynamicObject method implementations ... these are all optional
	public bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
		if (binder.Name == "MyMethod") {
			result = "Result of MyMethod"; return true;
		} else {
			result = null; return false;
		}
	}

	public bool TryGetMember(GetMemberBinder binder, out object result) {
		if (binder.Name == "MyProperty") {
			result = "Result of MyProperty"; return true;
		} else {
			result = null; return false;
		}
	}

	public bool TrySetMember(SetMemberBinder binder, object value) {
		if (binder.Name == "MyProperty") {
			MyPropertyValue = value.ToString(); return true;
		} else {
			return false;
		}
	}
	#endregion
}

[TestFixture]
public class DynamicObjectProxySpec {

	dynamic Magical;

	[SetUp]
	public void Before() { Magical = new HasDynamicStuff(); }

    [Test][Ignore]
    public void can_implement_TryBinaryOperation() {
	}

    [Test][Ignore]
    public void can_implement_TryConvert() {
	}

    [Test][Ignore]
    public void can_implement_TryCreateInstance() {
	}

    [Test][Ignore]
    public void can_implement_TryDeleteIndex() {
	}

    [Test][Ignore]
    public void can_implement_TryDeleteMember() {
	}

    [Test][Ignore]
    public void can_implement_TryGetIndex() {
	}

    [Test]
    public void can_implement_TryGetMember() {
		AssertThrows<RuntimeBinderException>("`HasDynamicStuff' does not contain a definition for `IDontExist'", () => {
			var x = Magical.IDontExist;
		});

		(Magical.MyProperty as string).ShouldEqual("Result of MyProperty");
	}

    [Test][Ignore]
    public void can_implement_TryInvoke() {
	}

    [Test]
    public void can_implement_TryInvokeMember() {
		(Magical.RealMethod() as string).ShouldEqual("hi from a real, non-dynamic method");
		(Magical.MyMethod()   as string).ShouldEqual("Result of MyMethod");

		AssertThrows<RuntimeBinderException>("`HasDynamicStuff' does not contain a definition for `IDontExist'", () => {
			Magical.IDontExist();
		});
    }

    [Test][Ignore]
    public void can_implement_TrySetIndex() {
	}

    [Test]
    public void can_implement_TrySetMember() {
		AssertThrows<RuntimeBinderException>("`HasDynamicStuff' does not contain a definition for `IDontExist'", () => {
			Magical.IDontExist = "hi";
		});

		(Magical.MyPropertyValue as string).Should(Be.Null);
		Magical.MyProperty = "hello world";
		(Magical.MyPropertyValue as string).ShouldEqual("hello world");
	}

    [Test][Ignore]
    public void can_implement_TryUnaryOperation() {
	}

	#region AssertThrows
    // AssertThrows<SpecialException>(() => { ... })
    public static void AssertThrows<T>(Action action) {
        AssertThrows<T>(action);
    }

    // AssertThrows<SpecialException>("BOOM!", () => { ... })
    public static void AssertThrows<T>(string messagePart, Action action) {
        AssertThrows(action, messagePart, typeof(T));
    }

    // AssertThrows("BOOM!", () => { ... })
    public static void AssertThrows(string messagePart, Action action) {
        AssertThrows(action, messagePart);
    }

    // AssertThrows(() => { ... })
    // AssertThrows(() => { ... }, "BOOM!")                           // <--- AssertThrows(Message, Action) is preferred
    // AssertThrows(() => { ... }, "BOOM!", typeof(SpecialException)) // <--- AssertThrows<T>(Message) is preferred
    public static void AssertThrows(Action action, string messagePart = null, Type exceptionType = null) {
        try {
            action.Invoke();
            Assert.Fail("Expected Exception to be thrown, but none was.");
        } catch (Exception ex) {
            // NOTE: Sometimes, this might be a TargetInvocationException, in which case 
            //       the *actual* exception thrown will be ex.InnerException.
            //       If I run into that circumstance again, I'll update the code to reflect this.

            // check exception type, if provided
            if (exceptionType != null)
                if (!exceptionType.IsAssignableFrom(ex.GetType()))
                    Assert.Fail("Expected Exception of type {0} to be thrown, but got an Exception of type {1}", exceptionType, ex.GetType());

            // check exception message part, if provided
            if (messagePart != null)
                if (! ex.Message.Contains(messagePart))
                    Assert.Fail("Expected {0} Exception to be thrown with a message containing {1}, but message was: {2}",
                        exceptionType, messagePart, ex.Message);
        }
    }
	#endregion
}
