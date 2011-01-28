MetaObject
==========

Background
----------

C# 4.0's new [dynamic][] keyword brings a realm of new possibilities to C#.

If you inherit your class from [DynamicObject][], it becomes trivial to add support for 
dynamic methods, properties, indexes, and more.

As soon as you want to inherit your class from something else, however, it becomes a small 
nightmare ...

 - for your class to support dynamic functionality, it needs to implement [IDynamicMetaObjectProvider][]
 - to implement [IDynamicMetaObjectProvider], you need to have a [GetMetaObject][] method that returns a [DynamicMetaObject][]
 - to return a [DynamicMetaObject][] you need to be *INTIMATELY* familiar with pretty much everything in the [System.Linq.Expressions][] namespace

As much as I'd love to learn how to create expression trees in C#, what I *really* want to do is add dynamic functionality to my classes!

I started digging around and discovered that [DynamicObject][] is basically an empty class that has a rock solid 
[DynamicMetaObject][] implementation that delegates dynamic calls to methods on the [DynamicObject][], eg. [TryInvokeMember][]

Unfortunately, this [DynamicMetaObject][] implementation is private and sealed!

Thankfully, the [DLR][] (which is where this class comes from) is open-source, released under the [Apache 2.0 license][apache]!

I took the source code, dealt with some dependency issues, removed the dependency on a [DynamicObject][] and ended up with **MetaObject**

Usage
-----

Assuming you have a reference to MetaObject.dll or you included MetaObject.cs in your project:

    using System;
    using System.Dynamic;

    public class MyClass : Whatever, IDynamicMetaObjectProvider {

        // This 1 line is *ALL* you need to add support for all of the DynamicObject methods
        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression e){ return new MetaObject(e, this); }

        // Now, if you want to handle dynamic method calls, you can implement TryInvokeMember, just like you would in DynamicObject!
        public bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            if (binder.Name.Contains("Cool")) {
                result = "You called a method with Cool in the name!";
                return true;
            } else {
                result = null;
                return false;
            }
        }
    }

Now, if you want to invoke your dynamic methods:

    // This will give you a compiler error because MyClass doesn't have a method called CoolMethod
    var instance = new MyClass();
    instance.CoolMethod();

    // This will work and return "You called a method with Cool in the name!"
    dynamic instance = new MyClass();
    instance.CoolMethod();

All of the Try* methods that [DynamicObject][] supports are supported by MetaObject:

 - TryBinaryOperation
 - TryConvert
 - TryGetIndex
 - TryGetMember
 - TryInvoke
 - TryInvokeMember
 - TrySetIndex
 - TrySetMember
 - TryUnaryOperation

[dynamic]:                    http://msdn.microsoft.com/en-us/library/dd264741.aspx
[DynamicObject]:              http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.aspx
[IDynamicMetaObjectProvider]: http://msdn.microsoft.com/en-us/library/system.dynamic.idynamicmetaobjectprovider.aspx
[GetMetaObject]:              http://msdn.microsoft.com/en-us/library/system.dynamic.idynamicmetaobjectprovider.getmetaobject.aspx
[DynamicMetaObject]:          http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.aspx
[System.Linq.Expressions]:    http://msdn.microsoft.com/en-us/library/system.linq.expressions.aspx
[TryInvokeMember]:            http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.tryinvokemember.aspx
[DLR]:                        http://dlr.codeplex.com/
[apache]:                     http://www.apache.org/licenses/LICENSE-2.0.html
