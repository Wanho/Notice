using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

public sealed class AnonymousTypeBuilder
{
    private readonly static CustomAttributeBuilder compilerGeneratedAttributeBuilder;

    private readonly static CustomAttributeBuilder debuggerBrowsableAttributeBuilder;

    private readonly static CustomAttributeBuilder debuggerHiddenAttributeBuilder;

    private readonly static ConstructorInfo objectCtor;

    private readonly static MethodInfo objectToString;

    private readonly static ConstructorInfo stringBuilderCtor;

    private readonly static MethodInfo stringBuilderAppendString;

    private readonly static MethodInfo stringBuilderAppendObject;

    private readonly static Type equalityComparer;

    private readonly static Type equalityComparerGenericArgument;

    private readonly static MethodInfo equalityComparerDefault;

    private readonly static MethodInfo equalityComparerEquals;

    private readonly static MethodInfo equalityComparerGetHashCode;

    private readonly ConcurrentDictionary<string, Type> generatedTypes = new ConcurrentDictionary<string, Type>();

    private AssemblyBuilder assemblyBuilder;

    private ModuleBuilder moduleBuilder;

    private int Index = -1;

    static AnonymousTypeBuilder()
    {
        AnonymousTypeBuilder.compilerGeneratedAttributeBuilder = new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
        AnonymousTypeBuilder.debuggerBrowsableAttributeBuilder = new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new Type[] { typeof(DebuggerBrowsableState) }), new object[] { DebuggerBrowsableState.Never });
        AnonymousTypeBuilder.debuggerHiddenAttributeBuilder = new CustomAttributeBuilder(typeof(DebuggerHiddenAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
        AnonymousTypeBuilder.objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
        AnonymousTypeBuilder.objectToString = typeof(object).GetMethod("ToString", Type.EmptyTypes);
        AnonymousTypeBuilder.stringBuilderCtor = typeof(StringBuilder).GetConstructor(Type.EmptyTypes);
        AnonymousTypeBuilder.stringBuilderAppendString = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) });
        AnonymousTypeBuilder.stringBuilderAppendObject = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(object) });
        AnonymousTypeBuilder.equalityComparer = typeof(EqualityComparer<>);
        AnonymousTypeBuilder.equalityComparerGenericArgument = AnonymousTypeBuilder.equalityComparer.GetGenericArguments()[0];
        AnonymousTypeBuilder.equalityComparerDefault = AnonymousTypeBuilder.equalityComparer.GetMethod("get_Default", Type.EmptyTypes);
        AnonymousTypeBuilder.equalityComparerEquals = AnonymousTypeBuilder.equalityComparer.GetMethod("Equals", new Type[] { AnonymousTypeBuilder.equalityComparerGenericArgument, AnonymousTypeBuilder.equalityComparerGenericArgument });
        AnonymousTypeBuilder.equalityComparerGetHashCode = AnonymousTypeBuilder.equalityComparer.GetMethod("GetHashCode", new Type[] { AnonymousTypeBuilder.equalityComparerGenericArgument });
    }

    public AnonymousTypeBuilder(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
    {
        this.assemblyBuilder = assemblyBuilder;
        this.moduleBuilder = moduleBuilder;
    }

    public Type CreateType(Type[] types, string[] names)
    {
        Type orAdd;
        string str;
        string str1 = string.Join("|",
            from x in names
            select AnonymousTypeBuilder.Escape(x));
        if (!this.generatedTypes.TryGetValue(str1, out orAdd))
        {
            lock (this.generatedTypes)
            {
                if (!this.generatedTypes.TryGetValue(str1, out orAdd))
                {
                    int num = Interlocked.Increment(ref this.Index);
                    str = (names.Length != 0 ? string.Format("<T>AnonymousType{0}`{1}", num, (int)names.Length) : string.Format("<T>AnonymousType{0}", num));
                    TypeBuilder typeBuilder = this.moduleBuilder.DefineType(str, TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
                    typeBuilder.SetCustomAttribute(AnonymousTypeBuilder.compilerGeneratedAttributeBuilder);
                    ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, types);
                    constructorBuilder.SetCustomAttribute(AnonymousTypeBuilder.debuggerHiddenAttributeBuilder);
                    ILGenerator lGenerator = constructorBuilder.GetILGenerator();
                    lGenerator.Emit(OpCodes.Ldarg_0);
                    lGenerator.Emit(OpCodes.Call, AnonymousTypeBuilder.objectCtor);
                    FieldBuilder[] fieldBuilderArray = new FieldBuilder[(int)names.Length];
                    for (int i = 0; i < (int)names.Length; i++)
                    {
                        fieldBuilderArray[i] = typeBuilder.DefineField(string.Format("<{0}>i__Field", names[i]), types[i], FieldAttributes.Private | FieldAttributes.InitOnly);
                        fieldBuilderArray[i].SetCustomAttribute(AnonymousTypeBuilder.debuggerBrowsableAttributeBuilder);
                        constructorBuilder.DefineParameter(i + 1, ParameterAttributes.None, names[i]);
                        lGenerator.Emit(OpCodes.Ldarg_0);
                        if (i == 0)
                        {
                            lGenerator.Emit(OpCodes.Ldarg_1);
                        }
                        else if (i == 1)
                        {
                            lGenerator.Emit(OpCodes.Ldarg_2);
                        }
                        else if (i == 2)
                        {
                            lGenerator.Emit(OpCodes.Ldarg_3);
                        }
                        else if (i >= 255)
                        {
                            lGenerator.Emit(OpCodes.Ldarg, (short)(i + 1));
                        }
                        else
                        {
                            lGenerator.Emit(OpCodes.Ldarg_S, (byte)(i + 1));
                        }
                        lGenerator.Emit(OpCodes.Stfld, fieldBuilderArray[i]);
                        MethodBuilder methodBuilder = typeBuilder.DefineMethod(string.Format("get_{0}", names[i]), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, types[i], Type.EmptyTypes);
                        ILGenerator lGenerator1 = methodBuilder.GetILGenerator();
                        lGenerator1.Emit(OpCodes.Ldarg_0);
                        lGenerator1.Emit(OpCodes.Ldfld, fieldBuilderArray[i]);
                        lGenerator1.Emit(OpCodes.Ret);
                        typeBuilder.DefineProperty(names[i], PropertyAttributes.None, CallingConventions.HasThis, types[i], Type.EmptyTypes).SetGetMethod(methodBuilder);
                    }
                    MethodBuilder methodBuilder1 = typeBuilder.DefineMethod("ToString", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(string), Type.EmptyTypes);
                    methodBuilder1.SetCustomAttribute(AnonymousTypeBuilder.debuggerHiddenAttributeBuilder);
                    ILGenerator lGenerator2 = methodBuilder1.GetILGenerator();
                    lGenerator2.DeclareLocal(typeof(StringBuilder));
                    lGenerator2.Emit(OpCodes.Newobj, AnonymousTypeBuilder.stringBuilderCtor);
                    lGenerator2.Emit(OpCodes.Stloc_0);
                    lGenerator.Emit(OpCodes.Ret);
                    lGenerator2.Emit(OpCodes.Ldloc_0);
                    lGenerator2.Emit(OpCodes.Ldstr, (names.Length == 0 ? "{ }" : " }"));
                    lGenerator2.Emit(OpCodes.Callvirt, AnonymousTypeBuilder.stringBuilderAppendString);
                    lGenerator2.Emit(OpCodes.Pop);
                    lGenerator2.Emit(OpCodes.Ldloc_0);
                    lGenerator2.Emit(OpCodes.Callvirt, AnonymousTypeBuilder.objectToString);
                    lGenerator2.Emit(OpCodes.Ret);
                    orAdd = typeBuilder.CreateTypeInfo().AsType();
                    orAdd = this.generatedTypes.GetOrAdd(str1, orAdd);
                }
            }
        }
        return orAdd;
    }

    private static string Escape(string str)
    {
        str = str.Replace("\\", "\\\\");
        str = str.Replace("|", "\\|");
        return str;
    }
}