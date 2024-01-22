using System.Reflection;
using System.Reflection.Emit;

namespace LocalizationProject
{
    public static class UnknownClass
    {
        /// <summary>
        /// Создание динамического класса
        /// </summary>
        /// <param name="className">Наименование класса</param>
        /// <param name="properties">Свойства (название свойства, тип свойства)</param>
        /// <returns>object - созданный класс</returns>
        public static object BuildingClass(string className, List<(string property, Type type)> properties)
        {
            TypeBuilder dynamicClass = BuildingType(className);

            foreach (var (property, type) in properties)
                AddProperty(dynamicClass, property, type);

            return Activator.CreateInstance(dynamicClass.CreateType())!;
        }

        /// <summary>
        /// Получение значение свойства класса
        /// </summary>
        /// <param name="currentClass">Класс</param>
        /// <param name="propertyName">Наименование свойства</param>
        /// <returns></returns>
        public static object GetProperty(object currentClass, string propertyName)
        {
            try
            {
                return currentClass.GetType().GetProperty(propertyName)!.GetValue(currentClass)!;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Изменение значение свойства
        /// </summary>
        /// <typeparam name="T">Неизвестный тип нового значений свойства</typeparam>
        /// <param name="currentClass">Класс</param>
        /// <param name="propertyName">Наименование свойства</param>
        /// <param name="value">Значение свойства с неизвестным типом</param>
        /// <returns>True\Сообщение об ошибке</returns>
        public static object SetProperty<T>(object currentClass, string propertyName, T value)
        {
            try
            {
                currentClass.GetType().GetProperty(propertyName)!.SetValue(currentClass, value);

                return true;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Создание динамического класса
        /// </summary>
        /// <param name="className">Название будущего класса</param>
        /// <returns></returns>
        private static TypeBuilder BuildingType(string className)
        {
            // Создание динамической сборки
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule("DynamicModule").DefineType(className, TypeAttributes.Public);
        }

        /// <summary>
        /// Добавление свойства в класс
        /// </summary>
        /// <param name="typeBuilder">Динамический класс</param>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="propertyType">Значение свойства</param>
        private static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

            var getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            var getILGenerator = getMethodBuilder.GetILGenerator();
            getILGenerator.Emit(OpCodes.Ldarg_0);
            getILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getILGenerator.Emit(OpCodes.Ret);

            var setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
            var setILGenerator = setMethodBuilder.GetILGenerator();
            setILGenerator.Emit(OpCodes.Ldarg_0);
            setILGenerator.Emit(OpCodes.Ldarg_1);
            setILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setILGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}
