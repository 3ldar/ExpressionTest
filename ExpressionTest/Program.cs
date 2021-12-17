using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var k = new Kaz { X = 3 };
            var f = new Foo { Bar = 1, Baz = "a", FooBar = k };
            Express(f);
        }

        public static T Express<T>(T source)
        {
            var parameter = Expression.Parameter(typeof(T), "source");
            var type = typeof(T);
            var ctor = Expression
                        .New(type.GetConstructor(Type.EmptyTypes));
            var propertyInfos = type.GetProperties();
            var e = Expression
                   .Lambda<Func<T, T>>(
                Expression
                   .MemberInit(ctor, propertyInfos.Select(pi =>
                                                     Expression.Bind(pi, CanBeAssigned(pi.PropertyType)
                                                    ? (Expression)Expression.Property(parameter, pi.Name)
                                                    : Expression.Call(typeof(Program).GetMethod(nameof(Express)).MakeGenericMethod(pi.PropertyType),
                                                                        Expression.Property(parameter, pi.Name)
                                                                      ))
                                                 )),
                parameter
            );
            var x = e.Compile();
            var z = x(source);
            return z;
        }

        public static bool CanBeAssigned(Type t)
        {
            return t.IsValueType || t.Name == "String";
        }
    }

    class Foo
    {
        public int Bar { get; set; }

        public string Baz { get; set; }

        public Kaz FooBar { get; set; }
    }

    class Kaz
    {
        public int X { get; set; }
    }
}
