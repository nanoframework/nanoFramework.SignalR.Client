using System;
using System.Runtime;
using System.Collections;
using System.Reflection;

namespace nanoFramework.SignalR.Client.json
{
    internal static class JsonReflectionDecode
    {
        internal static object JsonDecode(string json, Type type)
        {
            var hashTableResult = JsonParser.JsonDecode(json) as Hashtable;
            if (hashTableResult.Count > 0)
            {
                return CastObject(hashTableResult, type);
            }
            else
            {
                return null;
            }
        }

        private static object CastObject(Hashtable objectContent, Type objectType)
        { 
            object objectInstance = CreateInstance(objectType);

            Hashtable matchingProperties = new Hashtable();
            foreach (var key in objectContent.Keys)
            {
                MethodInfo memberPropGetMethod = objectType.GetMethod("get_" + key);
                if (memberPropGetMethod != null) {
                    matchingProperties.Add(key, memberPropGetMethod.ReturnType);
                 }
            }

            foreach(var key in matchingProperties.Keys)
            {
                MethodInfo memberPropSetMethod = objectType.GetMethod("set_" + key);
                if (memberPropSetMethod != null)
                {
                    var type = (Type)matchingProperties[key];

                    object castObject = TryCastObjectType(objectContent[key], type);
                    if (castObject != null)
                    {
                        memberPropSetMethod.Invoke(objectInstance, new object[] { castObject });
                    }
                }
            }

            return objectInstance;
        }

        internal static object TryCastObjectType(object objectContent ,Type returnType)
        {
            if(objectContent == null)
            {
                return null;
            }
            var currentType = objectContent.GetType();
           
            if (returnType == currentType || returnType == typeof(object))
            {
                return objectContent;
            }
            else
            {
                if (returnType.IsValueType) 
                {
                    if (returnType.Name == "DateTime" && currentType.Name == "String") //datetime
                    {
                        return DateTimeExtensions.FromIso8601((string)objectContent);
                    }
                    else
                    {
                        var memberPropConstructor = returnType.GetConstructor(new Type[] { currentType });
                        if (memberPropConstructor != null)
                        {
                            return  memberPropConstructor.Invoke(new object[] { objectContent });
                        }
                        else if (IsNumber(returnType) && IsNumber(currentType))
                        {
                            return ParseNumber(objectContent, returnType);
                        }
                        else if(returnType.IsEnum && IsNumber(currentType))
                        {
                            Console.WriteLine("cant't cast ENUMS");
                                
                        }
                        else
                        {
                            Console.WriteLine("unsuported value type");
                        }
                        
                    }
                    //try parse the value
                }
                else if (currentType.Name == "Hashtable")
                {
                    return CastObject((Hashtable)objectContent, returnType);
                }
                else if (currentType.Name == "ArrayList")
                {
                    if (returnType.BaseType.Name == "Array")
                    {
                        var arrayType = Type.GetType(returnType.FullName.Substring(0, returnType.FullName.Length - 2));
                        ArrayList arrayObjects = (ArrayList)objectContent;
                        ArrayList tempArray = new ArrayList(/*arrayObjects.Count*/);
                        int count = 0;
                        foreach(var arrayObject in arrayObjects)
                        {
                            object castObject = TryCastObjectType(arrayObject, arrayType);
                            if (castObject != null) {
                                tempArray.Add(castObject);
                                count++;
                            }
                        }
                        var resultArray = Array.CreateInstance(arrayType, count);
                        Array.Copy(tempArray.ToArray(), resultArray, count);

                        return resultArray;

                        
                    }
                }
                else
                {
                    Console.WriteLine($"can't cast {returnType.Name} to {currentType.Name}");
                }
            }
            return null;
        }


        private static object ParseNumber(object number, Type type)
        {
            Double doubleNumber = 0;
            Int64 int64Number= 0;

            bool isDouble = number.GetType().FullName == "System.Double";

            if (isDouble) doubleNumber = (double)number;
            else if(number.GetType().FullName == "System.Int64") int64Number = (Int64)number;
            else throw new Exception("CastNumbers can only parse numbers");


            switch (type.FullName)
            {
                case "System.Byte":
                    return isDouble ? (byte)doubleNumber : (byte)int64Number;
                case "System.SByte":
                    return isDouble ? (sbyte)doubleNumber : (sbyte)int64Number;
                case "System.UInt16":
                    return isDouble ? (UInt16)doubleNumber : (UInt16)int64Number;
                case "System.UInt32":
                    return isDouble ? (UInt32)doubleNumber : (UInt32)int64Number;
                case "System.UInt64":
                    return isDouble ? (UInt64)doubleNumber : (UInt64)int64Number;
                case "System.Int16":
                    return isDouble ? (Int16)doubleNumber : (Int16)int64Number;
                case "System.Int32":
                    return isDouble ? (Int32)doubleNumber : (Int32)int64Number;
                case "System.Int64":
                    return isDouble ? (Int64)doubleNumber : (Int64)int64Number;
                //No decimal in NF so use Double
                case "System.Decimal":
                    return isDouble ? (Double)doubleNumber : (Double)int64Number;
                //return isDouble ? (Decimal)doubleNumber : (Decimal)int64Number;
                case "System.Double":
                    return isDouble ? (Double)doubleNumber : (Double)int64Number;
                case "System.Single":
                    return isDouble ? (Single)doubleNumber : (Single)int64Number;

                default:
                    throw new Exception("CastNumbers can only parse numbers");
            }
        }

        private static object CreateInstance(Type type)
        {
           return  type.GetConstructor(new Type[] { }).Invoke(null);
        }

        private static bool IsNumber(Type type)
        {
            switch (type.FullName)
            {
                case "System.Byte":
                case "System.SByte":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                    return true;
                default:
                    return false;
            }
        }
    }
}
