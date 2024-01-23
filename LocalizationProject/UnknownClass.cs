using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace LocalizationProject
{
    public static class UnknownClass
    {
        #region Get and Set
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
        public static void SetProperty<T>(object currentClass, string propertyName, T value)
        {
            try
            {
                currentClass.GetType().GetProperty(propertyName)!.SetValue(currentClass, value!.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Создание динамического класса
        /// </summary>
        /// <param name="className">Наименование класса</param>
        /// <param name="properties">Свойства (название свойства, тип свойства)</param>
        /// <returns>object - созданный класс</returns>
        /// 
        public static object BuildingClass(string className, List<(string property, Type type)> properties)
        {
            TypeBuilder dynamicClass = BuildingType(className);

            foreach (var (property, type) in properties)
                AddProperty(dynamicClass, property, type);

            dynamicClass = Inheritance(dynamicClass, typeof(IEditableCollectionView)); // наследование класса

            return Activator.CreateInstance(dynamicClass.CreateType())!;
        }

        private static TypeBuilder Inheritance(TypeBuilder dynamicClass, Type type)
        { 

            dynamicClass.AddInterfaceImplementation(type);

            CreatingPropertyFurtherBinding(dynamicClass, "NewItemPlaceholderPosition",
                (typeof(NewItemPlaceholderPosition), Type.EmptyTypes), (null, new[] { typeof(NewItemPlaceholderPosition) })); // Создание свойства и привязка методов для него

            // реализация свойств
            PropertyRelease(dynamicClass);

            //Реализация методов
            MethodRelease(dynamicClass);

            return dynamicClass;
        }

        private static void MethodRelease(TypeBuilder dynamicClass)
        {
            // Создание методов и привязка их к классу
            CreatingNewMethod(dynamicClass, "AddNew", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), new Type[] { });
            CreatingNewMethod(dynamicClass, "CancelEdit", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { });
            CreatingNewMethod(dynamicClass, "CancelNew", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { });
            CreatingNewMethod(dynamicClass, "CommitEdit", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { });
            CreatingNewMethod(dynamicClass, "CommitNew", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { });
            CreatingNewMethod(dynamicClass, "EditItem", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { typeof(object) });
            CreatingNewMethod(dynamicClass, "Remove", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { typeof(object) });
            CreatingNewMethod(dynamicClass, "RemoveAt", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { typeof(int) });
        }

        private static void PropertyRelease(TypeBuilder dynamicClass)
        {
            // Создание свойства и привязка методов для него
            CreatingPropertyFurtherBinding(dynamicClass, "CanAddNew", (typeof(bool), Type.EmptyTypes));
            CreatingPropertyFurtherBinding(dynamicClass, "CanCancelEdit", (typeof(bool), Type.EmptyTypes));
            CreatingPropertyFurtherBinding(dynamicClass, "CanRemove", (typeof(bool), Type.EmptyTypes));
            CreatingPropertyFurtherBinding(dynamicClass, "CurrentAddItem", (typeof(object), Type.EmptyTypes));
            CreatingPropertyFurtherBinding(dynamicClass, "CurrentEditItem", (typeof(object), Type.EmptyTypes));
            CreatingPropertyFurtherBinding(dynamicClass, "IsAddingNew", (typeof(bool), Type.EmptyTypes));
            CreatingPropertyFurtherBinding(dynamicClass, "IsEditingItem", (typeof(bool), Type.EmptyTypes));
        }

        //Создание и привязка методов
        private static void CreatingNewMethod(TypeBuilder dynamicClass, string name, MethodAttributes attributes, Type? returnType, Type[] parameterTypes)
        {
            // Реализация метода AddNew
            MethodBuilder addNewMethodBuilder = dynamicClass.DefineMethod(name,
                attributes,
                returnType,
                parameterTypes);

            ILGenerator ilGenerator = addNewMethodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes)!);
            ilGenerator.Emit(OpCodes.Throw);
            dynamicClass.DefineMethodOverride(addNewMethodBuilder, typeof(IEditableCollectionView).GetMethod(name)!);
        }

        // Создание и привязка свойств
        private static void CreatingPropertyFurtherBinding(TypeBuilder dynamicClass, string name,
            (Type? GetReturnType, Type[]? GetparameterTypes) getTypes = default, (Type? SetReturnType, Type[]? SetparameterTypes) setTypes = default)
        {
            // Преобразование название свойства
            (string privateName, string publicName, string getName, string setName) = ConvertPropertyName(name);

            // Создание приватного поле для хранения значения свойства
            // пример: _newItemPlaceholderPosition
            var fieldBuilder = dynamicClass.DefineField(privateName, getTypes.GetReturnType!, FieldAttributes.Private);

            // Создание свойства
            // пример: NewItemPlaceholderPosition
            var propertyBuilder = dynamicClass.DefineProperty(publicName, PropertyAttributes.None, getTypes.GetReturnType!, getTypes.GetparameterTypes);

            // Указание, что методы get и set являются геттером и сеттером свойства NewItemPlaceholderPosition

            // get_NewItemPlaceholderPosition
            if (getTypes != default)
                propertyBuilder.SetGetMethod(GetBuilderOnClass(dynamicClass, fieldBuilder, getName, getTypes.GetReturnType, getTypes.GetparameterTypes!));
            //set_NewItemPlaceholderPosition
            if (setTypes != default)
                propertyBuilder.SetSetMethod(SetBuilderOnClass(dynamicClass, fieldBuilder, setName, setTypes.SetReturnType, setTypes.SetparameterTypes!));
        }

        /// <summary>
        /// Преобразование наименования свойства
        /// </summary>
        /// <param name="name">Наименование свойства</param>
        /// <returns>Список из преобразованных наименований свойств</returns>
        private static (string privateName, string publicName, string getName, string setName) ConvertPropertyName(string name) =>
            ($"_{name[0].ToString().ToLower() + name.Substring(1)}", name, $"get_{name}", $"set_{name}");

        #region Creating and setting methods for a property

        /// <summary>
        /// Привязка Get Метода к свойству класса
        /// </summary>
        /// <param name="dynamicClass">Класс</param>
        /// <param name="fieldBuilder">Приватное поле</param>
        /// <param name="title">Название свойства</param>
        /// <param name="returnType"></param>
        /// <param name="parameterTypes">Типы параметров</param>
        /// <returns>метод для привязка</returns>
        private static MethodBuilder GetBuilderOnClass(TypeBuilder dynamicClass, FieldBuilder fieldBuilder, string title, Type? returnType, Type[] parameterTypes)
        {
            // Создание метода get

            var getMethodBuilder = dynamicClass.DefineMethod(title,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                returnType,
                parameterTypes);

            var ilGeneratorGet = getMethodBuilder.GetILGenerator();
            ilGeneratorGet.Emit(OpCodes.Ldarg_0); // Загрузка значения "this" на стек
            ilGeneratorGet.Emit(OpCodes.Ldfld, fieldBuilder); // Загрузка значения поля на стек
            ilGeneratorGet.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        /// <summary>
        /// Привязка Set Метода к свойству класса
        /// </summary>
        /// <param name="dynamicClass">Класс</param>
        /// <param name="fieldBuilder">Приватное поле</param>
        /// <param name="title">Название свойства</param>
        /// <param name="returnType"></param>
        /// <param name="parameterTypes">Типы параметров</param>
        /// <returns>метод для привязка</returns>
        private static MethodBuilder SetBuilderOnClass(TypeBuilder dynamicClass, FieldBuilder fieldBuilder, string title, Type? returnType, Type[] parameterTypes)
        {
            // Создание метода set

            var setMethodBuilder = dynamicClass.DefineMethod(title,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                returnType,
                parameterTypes);

            var ilGeneratorSet = setMethodBuilder.GetILGenerator();
            ilGeneratorSet.Emit(OpCodes.Ldarg_0); // Загрузка значения "this" на стек
            ilGeneratorSet.Emit(OpCodes.Ldarg_1); // Загрузка значения аргумента на стек
            ilGeneratorSet.Emit(OpCodes.Stfld, fieldBuilder); // Присваивание значения полю
            ilGeneratorSet.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }
        #endregion

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
