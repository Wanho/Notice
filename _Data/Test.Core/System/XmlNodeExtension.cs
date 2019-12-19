using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Test.Core;

namespace System.Xml
{
    public static class XmlNodeExtension
    {
        public static T ToVo<T>(this XmlNode node) where T : ValueObject
        {
            return node.ToVo<T>(default(T));
        }

        public static T ToVo<T>(this XmlNode node, T vo) where T : ValueObject
        {
            object name;
            object innerText;
            if (node == null)
            {
                if (vo != null)
                {
                    return vo;
                }
                return default(T);
            }
            vo = (vo == null ? Activator.CreateInstance<T>() : vo);
            PropertyInfo[] properties = vo.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                MethodInfo setMethod = propertyInfo.GetSetMethod();
                if (!(setMethod == null) && (setMethod.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
                {
                    NameAttribute customAttribute = propertyInfo.GetCustomAttribute<NameAttribute>();
                    if (customAttribute != null)
                    {
                        name = customAttribute.Name;
                    }
                    else
                    {
                        name = null;
                    }
                    if (name == null)
                    {
                        name = propertyInfo.Name;
                    }
                    string str = (string)name;
                    string value = null;
                    if (str != "Text")
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (!attribute.Name.MatchDataName(str))
                            {
                                continue;
                            }
                            value = attribute.Value;
                            goto Label0;
                        }
                    }
                    else
                    {
                        XmlNode itemOf = node.ChildNodes[0];
                        if (itemOf != null)
                        {
                            innerText = itemOf.InnerText.Trim();
                        }
                        else
                        {
                            innerText = null;
                        }
                        if (innerText == null)
                        {
                            innerText = node.InnerText;
                        }
                        value = (string)innerText;
                    }
                    Label0:
                    vo.Invoke(value, propertyInfo);
                }
            }
            return vo;
        }
    }
}