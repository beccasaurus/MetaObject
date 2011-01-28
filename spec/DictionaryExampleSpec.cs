using System;
using System.Dynamic;
using System.Collections.Generic;
using NUnit.Framework;

public class DynamicDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>, IDynamicMetaObjectProvider {

	public DynamicDictionary() : base() {}

	public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression e){ return new MetaObject(e, this); }

	public bool TryGetMember(GetMemberBinder binder, out object result) {
		var key = (TKey) ((object) binder.Name);
		result = ContainsKey(key) ? this[key] : default(TValue);
		return true;
	}

	public bool TrySetMember(SetMemberBinder binder, object value) {
		var key = (TKey) ((object) binder.Name);
		this[key] = (TValue) value;
		return true;
	}
}

// A dynamic dictionary is one of the most typical examples you see of dynamic C#
[TestFixture]
public class DictionaryExampleSpec {

	[Test]
	public void should_work() {
		var realDictionary = new DynamicDictionary<string, object>();
		dynamic dict       = realDictionary;

		realDictionary.Should(Be.Empty);

		dict.Foo = "bar";

		realDictionary.ShouldNot(Be.Empty);
		realDictionary["Foo"].ShouldEqual("bar");

		(dict.Foo as string).ShouldEqual("bar");
		(dict.Bar as string).Should(Be.Null);
	}
}
